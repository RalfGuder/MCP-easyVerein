using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "invNumber": "RE-2024-001",
                "totalPrice": 119.00,
                "date": "2024-01-15T00:00:00",
                "dateItHappend": "2024-02-15T00:00:00",
                "dateSent": "2024-01-16T00:00:00",
                "kind": "invoice",
                "description": "Jahresbeitrag",
                "receiver": "Max Mustermann",
                "relatedAddress": 42,
                "relatedBookings": [1, 2, 3],
                "payedFromUser": 7,
                "approvedFromAdmin": 3,
                "canceledInvoice": 5,
                "bankAccount": 11,
                "gross": true,
                "cancellationDescription": "Storno wegen Fehler",
                "templateName": "Standard",
                "refNumber": "REF-001",
                "isDraft": false,
                "isTemplate": false,
                "creationDateForRecurringInvoices": "2024-01-01T00:00:00",
                "recurringInvoicesInterval": 12,
                "paymentInformation": "Bitte bis 15.02. überweisen",
                "isRequest": false,
                "taxRate": 19.00,
                "taxName": "MwSt.",
                "actualCallStateName": "Erste Mahnung",
                "callStateDelayDays": 14,
                "accnumber": 4711,
                "guid": "abc-123-def-456",
                "selectionAcc": 8,
                "removeFileOnDelete": true,
                "customPaymentMethod": 2,
                "isReceipt": false,
                "mode": "default",
                "offerStatus": "accepted",
                "offerValidUntil": "2024-03-01T00:00:00",
                "offerNumber": "ANG-2024-001",
                "relatedOffer": 55,
                "closingDescription": "Mit freundlichen Grüßen",
                "useAddressBalance": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var invoice = JsonSerializer.Deserialize<Invoice>(json, options);

        Assert.NotNull(invoice);
        Assert.Equal(99, invoice.Id);
        Assert.Equal("RE-2024-001", invoice.InvoiceNumber);
        Assert.Equal(119.00m, invoice.TotalPrice);
        Assert.Equal(new DateTime(2024, 1, 15), invoice.Date);
        Assert.Equal(new DateTime(2024, 2, 15), invoice.DueDate);
        Assert.Equal(new DateTime(2024, 1, 16), invoice.DateSent);
        Assert.Equal("invoice", invoice.Kind);
        Assert.Equal("Jahresbeitrag", invoice.Description);
        Assert.Equal("Max Mustermann", invoice.Receiver);
        Assert.Equal(42L, invoice.RelatedAddress);
        Assert.Equal([1L, 2L, 3L], invoice.RelatedBookings);
        Assert.Equal(7L, invoice.PayedFromUser);
        Assert.Equal(3L, invoice.ApprovedFromAdmin);
        Assert.Equal(5L, invoice.CanceledInvoice);
        Assert.Equal(11L, invoice.BankAccount);
        Assert.True(invoice.Gross);
        Assert.Equal("Storno wegen Fehler", invoice.CancellationDescription);
        Assert.Equal("Standard", invoice.TemplateName);
        Assert.Equal("REF-001", invoice.RefNumber);
        Assert.False(invoice.IsDraft);
        Assert.False(invoice.IsTemplate);
        Assert.Equal(new DateTime(2024, 1, 1), invoice.CreationDateForRecurringInvoices);
        Assert.Equal(12, invoice.RecurringInvoicesInterval);
        Assert.Equal("Bitte bis 15.02. überweisen", invoice.PaymentInformation);
        Assert.False(invoice.IsRequest);
        Assert.Equal(19.00m, invoice.TaxRate);
        Assert.Equal("MwSt.", invoice.TaxName);
        Assert.Equal("Erste Mahnung", invoice.ActualCallStateName);
        Assert.Equal(14, invoice.CallStateDelayDays);
        Assert.Equal(4711, invoice.AccountNumber);
        Assert.Equal("abc-123-def-456", invoice.Guid);
        Assert.Equal(8, invoice.SelectionAccount);
        Assert.True(invoice.RemoveFileOnDelete);
        Assert.Equal(2, invoice.CustomPaymentMethod);
        Assert.False(invoice.IsReceipt);
        Assert.Equal("default", invoice.Mode);
        Assert.Equal("accepted", invoice.OfferStatus);
        Assert.Equal(new DateTime(2024, 3, 1), invoice.OfferValidUntil);
        Assert.Equal("ANG-2024-001", invoice.OfferNumber);
        Assert.Equal(55L, invoice.RelatedOffer);
        Assert.Equal("Mit freundlichen Grüßen", invoice.ClosingDescription);
        Assert.False(invoice.UseAddressBalance);
    }
}
