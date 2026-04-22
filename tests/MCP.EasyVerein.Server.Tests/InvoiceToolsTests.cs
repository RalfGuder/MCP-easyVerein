using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="InvoiceTools"/> MCP tool wrapper.</summary>
public class InvoiceToolsTests
{
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

        var tools = new InvoiceTools(mock.Object);

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

        var tools = new InvoiceTools(mock.Object);

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
}
