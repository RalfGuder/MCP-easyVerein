using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>
/// Tests for the payment check of <c>update_invoice</c> after linking bookings (US-0066).
/// Linking a booking to an already finalized invoice does not settle the payment in easyVerein;
/// the tool must say so instead of reporting a silent success.
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
}
