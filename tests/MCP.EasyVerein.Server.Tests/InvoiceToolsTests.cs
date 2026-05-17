using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="InvoiceTools"/> MCP tool wrapper.</summary>
public class InvoiceToolsTests
{
    /// <summary>Creates a configuration with API URL <c>https://easyverein.com/api</c> and version <c>v2.0</c>.</summary>
    private static EasyVereinConfiguration CreateConfig() => new()
    {
        ApiKey = "test",
        ApiUrl = "https://easyverein.com/api",
        ApiVersion = "v2.0"
    };

    /// <summary>
    /// Verifies that <c>create_invoice</c> forwards every new parameter
    /// (refNumber, paymentInformation, actualCallStateName, callStateDelayDays,
    /// accnumber, guid, mode, offerStatus) verbatim onto the <see cref="Invoice"/>
    /// instance passed to the API client.
    /// </summary>
    [Fact]
    public async Task CreateInvoice_PassesAllNewParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Invoice? captured = null;
        mock.Setup(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Invoice, CancellationToken>((inv, _) => captured = inv)
            .ReturnsAsync(new Invoice { Id = 42 });

        var tools = new InvoiceTools(mock.Object, CreateConfig());

        await tools.CreateInvoice(
            invoiceNumber: "R-001",
            totalPrice: 123.45m,
            description: "Test payment",
            kind: "Revenue",
            refNumber: "REF-42",
            paymentInformation: "Account",
            actualCallStateName: "Reminder-1",
            callStateDelayDays: 14,
            accnumber: 4400,
            guid: "datev-guid-xyz",
            mode: "invoice",
            offerStatus: "accepted",
            receiver: null,
            relatedAddressId: null,
            relatedBookingIds: null,
            isReceipt: null,
            isDraft: null,
            ct: CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("R-001", captured!.InvoiceNumber);
        Assert.Equal(123.45m, captured.TotalPrice);
        Assert.Equal("Test payment", captured.Description);
        Assert.Equal("Revenue", captured.Kind);
        Assert.Equal("REF-42", captured.RefNumber);
        Assert.Equal("Account", captured.PaymentInformation);
        Assert.Equal("Reminder-1", captured.ActualCallStateName);
        Assert.Equal(14, captured.CallStateDelayDays);
        Assert.Equal(4400, captured.AccountNumber);
        Assert.Equal("datev-guid-xyz", captured.Guid);
        Assert.Equal("invoice", captured.Mode);
        Assert.Equal("accepted", captured.OfferStatus);
    }

    /// <summary>
    /// Verifies that <c>create_invoice</c> leaves every optional new field as <c>null</c>
    /// when the caller does not supply it, so the JSON serializer can omit them.
    /// </summary>
    [Fact]
    public async Task CreateInvoice_OptionalParametersOmitted_PropertiesAreNull()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Invoice? captured = null;
        mock.Setup(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Invoice, CancellationToken>((inv, _) => captured = inv)
            .ReturnsAsync(new Invoice { Id = 7 });

        var tools = new InvoiceTools(mock.Object, CreateConfig());

        await tools.CreateInvoice(
            invoiceNumber: null,
            totalPrice: 10m,
            description: null,
            kind: null,
            refNumber: null,
            paymentInformation: null,
            actualCallStateName: null,
            callStateDelayDays: null,
            accnumber: null,
            guid: null,
            mode: null,
            offerStatus: null,
            receiver: null,
            relatedAddressId: null,
            relatedBookingIds: null,
            isReceipt: null,
            isDraft: null,
            ct: CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Null(captured!.RefNumber);
        Assert.Null(captured.PaymentInformation);
        Assert.Null(captured.ActualCallStateName);
        Assert.Null(captured.CallStateDelayDays);
        Assert.Null(captured.AccountNumber);
        Assert.Null(captured.Guid);
        Assert.Null(captured.Mode);
        Assert.Null(captured.OfferStatus);
    }

    /// <summary>
    /// Verifies that <c>create_invoice</c> with <c>receiver</c> and <c>isDraft=true</c>
    /// produces an <see cref="Invoice"/> with the receiver text set, IsDraft=true,
    /// IsReceipt=false (default), and RelatedBookings=null (unset).
    /// </summary>
    [Fact]
    public async Task CreateInvoice_WithDraftAndReceiver_PassesAllFieldsToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Invoice? captured = null;
        mock.Setup(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Invoice, CancellationToken>((inv, _) => captured = inv)
            .ReturnsAsync((Invoice inv, CancellationToken _) => inv);

        var tools = new InvoiceTools(mock.Object, CreateConfig());

        await tools.CreateInvoice(
            invoiceNumber: null,
            totalPrice: 50m,
            description: "Beleg",
            kind: "Expense",
            refNumber: null,
            paymentInformation: null,
            actualCallStateName: null,
            callStateDelayDays: null,
            accnumber: null,
            guid: null,
            mode: null,
            offerStatus: null,
            receiver: "Test",
            relatedAddressId: null,
            relatedBookingIds: null,
            isReceipt: null,
            isDraft: true,
            ct: CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Test", captured!.Receiver);
        Assert.True(captured.IsDraft);
        Assert.False(captured.IsReceipt);
        Assert.Null(captured.RelatedBookings);
        Assert.Null(captured.RelatedAddress);
    }

    /// <summary>
    /// Verifies that <c>create_invoice</c> with <c>relatedBookingIds</c> converts every
    /// numeric ID into a fully qualified easyVerein resource URL using the configured base.
    /// </summary>
    [Fact]
    public async Task CreateInvoice_WithRelatedBookingIds_ConvertsToUrlList()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Invoice? captured = null;
        mock.Setup(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Invoice, CancellationToken>((inv, _) => captured = inv)
            .ReturnsAsync((Invoice inv, CancellationToken _) => inv);

        var tools = new InvoiceTools(mock.Object, CreateConfig());

        await tools.CreateInvoice(
            invoiceNumber: null,
            totalPrice: 100m,
            description: null,
            kind: null,
            refNumber: null,
            paymentInformation: null,
            actualCallStateName: null,
            callStateDelayDays: null,
            accnumber: null,
            guid: null,
            mode: null,
            offerStatus: null,
            receiver: null,
            relatedAddressId: null,
            relatedBookingIds: new long[] { 100, 200 },
            isReceipt: null,
            isDraft: null,
            ct: CancellationToken.None);

        Assert.NotNull(captured);
        Assert.NotNull(captured!.RelatedBookings);
        Assert.Equal(new List<string>
        {
            "https://easyverein.com/api/v2.0/booking/100",
            "https://easyverein.com/api/v2.0/booking/200"
        }, captured.RelatedBookings);
    }

    /// <summary>
    /// Verifies that <c>create_receipt</c> orchestrates all three client calls in the correct
    /// order (GetBooking - CreateInvoice - CreateInvoiceItem - UpdateInvoice), with the
    /// correct field values derived from the booking (kind=expense for negative amount,
    /// totalPrice=abs(amount), receiver fallback to booking.Receiver) and that the final
    /// PATCH carries isDraft=false plus the booking URL in relatedBookings.
    /// </summary>
    [Fact]
    public async Task CreateReceipt_OrchestratesAllThreeCalls_AndReturnsFinalizedInvoice()
    {
        // Booking has -2.90 (expense), Receiver "Sparkasse IBAN"
        var booking = new Booking
        {
            Id = 240005841,
            Amount = -2.90m,
            Date = new DateTime(2026, 5, 4),
            Receiver = "DE47..., WELADED1PMB",
            Description = "Entgeltabrechnung"
        };
        var draftCreated = new Invoice { Id = 469293618, IsDraft = true };
        var itemCreated = new InvoiceItem { Id = 469293630 };
        var finalized = new Invoice { Id = 469293618, IsDraft = false, IsReceipt = true };

        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetBookingAsync(240005841L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        Invoice? capturedDraft = null;
        mock.Setup(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Invoice, CancellationToken>((inv, _) => capturedDraft = inv)
            .ReturnsAsync(draftCreated);
        InvoiceItem? capturedItem = null;
        mock.Setup(c => c.CreateInvoiceItemAsync(It.IsAny<InvoiceItem>(), It.IsAny<CancellationToken>()))
            .Callback<InvoiceItem, CancellationToken>((it, _) => capturedItem = it)
            .ReturnsAsync(itemCreated);
        object? capturedPatch = null;
        mock.Setup(c => c.UpdateInvoiceAsync(469293618L, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, p, _) => capturedPatch = p)
            .ReturnsAsync(finalized);

        var tools = new InvoiceTools(mock.Object, CreateConfig());
        var result = await tools.CreateReceipt(
            bookingId: 240005841,
            billingAccountId: 58811,
            sphere: 2,
            costCentre: "2901",
            title: null,
            description: null,
            receiver: null,
            ct: CancellationToken.None);

        Assert.NotNull(capturedDraft);
        Assert.True(capturedDraft!.IsDraft);
        Assert.True(capturedDraft.IsReceipt);
        Assert.Equal("expense", capturedDraft.Kind);
        Assert.Equal(2.90m, capturedDraft.TotalPrice);

        Assert.NotNull(capturedItem);
        Assert.Equal("https://easyverein.com/api/v2.0/billing-account/58811", capturedItem!.BillingAccount);
        Assert.Equal(2, capturedItem.Sphere);
        Assert.Equal("2901", capturedItem.CostCentre);
        Assert.Equal(2.90m, capturedItem.UnitPrice);
        Assert.Equal("https://easyverein.com/api/v2.0/invoice/469293618", capturedItem.RelatedInvoice);

        Assert.NotNull(capturedPatch);
        var patchJson = JsonSerializer.Serialize(capturedPatch);
        Assert.Contains("isDraft", patchJson);
        Assert.Contains("relatedBookings", patchJson);
        Assert.Contains("booking/240005841", patchJson);

        Assert.Contains("\"id\": 469293618", result);
    }
}
