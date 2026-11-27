using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using System.ComponentModel;
using System.Reflection;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>
/// Tests for the payment check of <c>update_invoice</c> after linking bookings (US-0066).
/// Linking a booking usually settles the payment, also for finalized invoices; the tool warns
/// only if a payment difference remains or the settlement could not be checked.
/// </summary>
public class UpdateInvoicePaymentTests
{
    /// <summary>The ID of the invoice used in the tests.</summary>
    private const long InvoiceId = 459262173;

    /// <summary>Creates a v2.0 configuration.</summary>
    /// <returns>The configuration.</returns>
    private static EasyVereinConfiguration CreateConfig() => new()
    {
        ApiKey = "test",
        ApiUrl = "https://easyverein.com/api",
        ApiVersion = "v2.0"
    };

    /// <summary>Creates a mock whose update returns <paramref name="updated"/> and whose re-read returns <paramref name="reread"/>.</summary>
    /// <param name="updated">The invoice returned by the PATCH.</param>
    /// <param name="reread">The invoice returned by the subsequent GET.</param>
    /// <returns>The configured mock.</returns>
    private static Mock<IEasyVereinApiClient> CreateMock(Invoice updated, Invoice? reread)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.UpdateInvoiceAsync(InvoiceId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);
        mock.Setup(c => c.GetInvoiceAsync(InvoiceId, It.IsAny<CancellationToken>())).ReturnsAsync(reread);
        return mock;
    }

    /// <summary>Calls <c>update_invoice</c> with the given related bookings and draft flag.</summary>
    /// <param name="mock">The mock client.</param>
    /// <param name="bookingIds">The related booking IDs, or <c>null</c>.</param>
    /// <param name="isDraft">The draft flag argument.</param>
    /// <returns>The tool result.</returns>
    private static Task<string> Run(Mock<IEasyVereinApiClient> mock, long[]? bookingIds, bool? isDraft = null) =>
        new InvoiceTools(mock.Object, CreateConfig()).UpdateInvoice(
            InvoiceId, null, null, null, null, bookingIds, null, null, null, isDraft, CancellationToken.None);

    /// <summary>Verifies that an open payment difference after linking is reported with a hint to the UI.</summary>
    [Fact]
    public async Task UpdateInvoice_LinkedButNotSettled_ReturnsWarning()
    {
        var invoice = new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 60m };
        var mock = CreateMock(invoice, invoice);

        var result = await Run(mock, new[] { 247581501L });

        Assert.Contains("\"id\":459262173", result);
        Assert.Contains("WARNING", result);
        Assert.Contains("60", result);
        Assert.Contains("easyVerein", result);
    }

    /// <summary>Verifies that a settled invoice gives no warning.</summary>
    [Fact]
    public async Task UpdateInvoice_LinkedAndSettled_NoWarning()
    {
        var invoice = new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 0m };
        var mock = CreateMock(invoice, invoice);

        var result = await Run(mock, new[] { 247581501L });

        Assert.DoesNotContain("WARNING", result);
    }

    /// <summary>Verifies that the check uses the re-read invoice, not the PATCH response.</summary>
    [Fact]
    public async Task UpdateInvoice_UsesRereadPaymentDifference()
    {
        var mock = CreateMock(
            new Invoice { Id = InvoiceId, IsDraft = false },
            new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 60m });

        var result = await Run(mock, new[] { 247581501L });

        Assert.Contains("WARNING", result);
    }

    /// <summary>Verifies that without related bookings the invoice is not re-read.</summary>
    [Fact]
    public async Task UpdateInvoice_WithoutBookings_DoesNotReread()
    {
        var mock = CreateMock(new Invoice { Id = InvoiceId, PaymentDifference = 60m }, null);

        var result = await Run(mock, null);

        Assert.DoesNotContain("WARNING", result);
        mock.Verify(c => c.GetInvoiceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that a draft gives no warning, because finalizing it settles the payment.</summary>
    [Fact]
    public async Task UpdateInvoice_DraftLinked_NoWarning()
    {
        var invoice = new Invoice { Id = InvoiceId, IsDraft = true, PaymentDifference = 60m };
        var mock = CreateMock(invoice, invoice);

        var result = await Run(mock, new[] { 247581501L }, isDraft: true);

        Assert.DoesNotContain("WARNING", result);
    }

    /// <summary>Verifies that the warning names the open amount, the possible reasons and the UI check (AK-3).</summary>
    [Fact]
    public async Task UpdateInvoice_NotSettled_WarningIsFactual()
    {
        var invoice = new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 60m };
        var mock = CreateMock(invoice, invoice);

        var result = await Run(mock, new[] { 247581501L });

        var lastLine = result.Split('\n')[^1];
        Assert.StartsWith("WARNING:", lastLine);
        Assert.Contains("paymentDifference 60", lastLine);
        Assert.Contains("not fully settled", lastLine);
        Assert.Contains("partial payment", lastLine);
        Assert.Contains("different amount", lastLine);
        Assert.Contains("easyVerein web interface", lastLine);
        Assert.DoesNotContain("does not settle", result);
    }

    /// <summary>Verifies that a negative payment difference is reported with the read amount (EC-1).</summary>
    [Fact]
    public async Task UpdateInvoice_NegativeDifference_ReturnsWarningWithAmount()
    {
        var invoice = new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = -12.5m };
        var mock = CreateMock(invoice, invoice);

        var result = await Run(mock, new[] { 247581501L });

        var lastLine = result.Split('\n')[^1];
        Assert.StartsWith("WARNING:", lastLine);
        Assert.Contains("paymentDifference -12.5", lastLine);
    }

    /// <summary>Verifies that the re-read value wins over a settled PATCH response (EC-2).</summary>
    [Fact]
    public async Task UpdateInvoice_PatchSettledButRereadOpen_ReturnsWarning()
    {
        var mock = CreateMock(
            new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 0m },
            new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 60m });

        var result = await Run(mock, new[] { 247581501L });

        Assert.Contains("WARNING:", result);
        Assert.Contains("60", result.Split('\n')[^1]);
    }

    /// <summary>Verifies that an empty booking list does not trigger a re-read (EC-3).</summary>
    [Fact]
    public async Task UpdateInvoice_EmptyBookingList_DoesNotReread()
    {
        var invoice = new Invoice { Id = InvoiceId, IsDraft = false, PaymentDifference = 60m };
        var mock = CreateMock(invoice, invoice);

        var result = await Run(mock, Array.Empty<long>());

        Assert.DoesNotContain("WARNING", result);
        mock.Verify(c => c.GetInvoiceAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that a failing re-read still returns the update result plus a warning (AK-5).</summary>
    [Fact]
    public async Task UpdateInvoice_RereadThrows_ReturnsUpdateWithWarning()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.UpdateInvoiceAsync(InvoiceId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = InvoiceId, IsDraft = false });
        mock.Setup(c => c.GetInvoiceAsync(InvoiceId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

        var result = await Run(mock, new[] { 247581501L });

        Assert.DoesNotContain("ERROR", result);
        Assert.StartsWith("{", result);
        Assert.Contains("\"id\":459262173", result);
        var lastLine = result.Split('\n')[^1];
        Assert.StartsWith("WARNING:", lastLine);
        Assert.Contains("could not be checked", lastLine);
        Assert.Contains("503", lastLine);
    }

    /// <summary>Verifies that a re-read without result is treated like a failed re-read (AK-5).</summary>
    [Fact]
    public async Task UpdateInvoice_RereadReturnsNull_ReturnsUpdateWithWarning()
    {
        var mock = CreateMock(new Invoice { Id = InvoiceId, IsDraft = false }, null);

        var result = await Run(mock, new[] { 247581501L });

        Assert.Contains("\"id\":459262173", result);
        var lastLine = result.Split('\n')[^1];
        Assert.StartsWith("WARNING:", lastLine);
        Assert.Contains("could not be checked", lastLine);
    }

    /// <summary>Verifies that a failing PATCH is still reported as error.</summary>
    [Fact]
    public async Task UpdateInvoice_PatchThrows_ReturnsError()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.UpdateInvoiceAsync(InvoiceId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("400 Bad Request"));

        var result = await Run(mock, new[] { 247581501L });

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>Verifies that the tool description reflects the actual settlement behaviour (AK-1).</summary>
    [Fact]
    public void UpdateInvoice_Description_DescribesSettlement()
    {
        var method = typeof(InvoiceTools).GetMethod(nameof(InvoiceTools.UpdateInvoice))!;
        var description = method.GetCustomAttribute<DescriptionAttribute>()!.Description;

        Assert.Contains("usually settles", description);
        Assert.Contains("also for finalized invoices", description);
        Assert.Contains("WARNING", description);
        Assert.Contains("paymentDifference", description);
        Assert.DoesNotContain("does not settle", description);
    }
}
