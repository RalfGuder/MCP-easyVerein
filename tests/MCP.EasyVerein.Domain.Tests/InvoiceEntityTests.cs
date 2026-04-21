using System.Text.Json;
using System.Text.Json.Serialization;
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
                "totalPrice": "119.00",
                "date": "2024-01-15",
                "dateItHappend": "2024-02-15",
                "dateSent": "2024-01-16",
                "kind": "invoice",
                "description": "Jahresbeitrag",
                "receiver": "Max Mustermann",
                "relatedAddress": "https://easyverein.com/api/v1.7/contact-details/42",
                "relatedBookings": [
                    "https://easyverein.com/api/v1.7/booking/1",
                    "https://easyverein.com/api/v1.7/booking/2",
                    "https://easyverein.com/api/v1.7/booking/3"
                ],
                "payedFromUser": "https://easyverein.com/api/v1.7/user/7",
                "approvedFromAdmin": "https://easyverein.com/api/v1.7/user/3",
                "canceledInvoice": "https://easyverein.com/api/v1.7/invoice/5",
                "bankAccount": "https://easyverein.com/api/v1.7/bank-account/11",
                "org": "https://easyverein.com/api/v1.7/organization/30189",
                "path": "https://easyverein.com/app/file?category=invoice&path=2024/01/test.pdf",
                "invoiceItems": ["https://easyverein.com/api/v1.7/invoice-item/100"],
                "charges": { "charge": 1.0, "chargeBack": 0.5, "total": 119.00 },
                "tax": "19.00",
                "paymentDifference": "0.00",
                "gross": true,
                "cancellationDescription": "Storno wegen Fehler",
                "templateName": "Standard",
                "refNumber": "REF-001",
                "isDraft": false,
                "isTemplate": false,
                "creationDateForRecurringInvoices": "2024-01-01",
                "recurringInvoicesInterval": 12,
                "paymentInformation": "Bitte bis 15.02. überweisen",
                "isRequest": false,
                "taxRate": "19.00",
                "taxName": "MwSt.",
                "actualCallStateName": "Erste Mahnung",
                "callStateDelayDays": 14,
                "accnumber": 4711,
                "guid": "abc-123-def-456",
                "selectionAcc": 8,
                "removeFileOnDelete": true,
                "customPaymentMethod": 2,
                "isReceipt": false,
                "_isTaxRatePerInvoiceItem": true,
                "_isSubjectToTax": true,
                "mode": "default",
                "offerStatus": "accepted",
                "offerValidUntil": "2024-03-01",
                "offerNumber": "ANG-2024-001",
                "relatedOffer": "https://easyverein.com/api/v1.7/invoice/55",
                "closingDescription": "Mit freundlichen Grüßen",
                "useAddressBalance": false,
                "_deleteAfterDate": null,
                "_deletedBy": null
            }
            """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        var invoice = JsonSerializer.Deserialize<Invoice>(json, options);

        Assert.NotNull(invoice);
        Assert.Equal(99, invoice!.Id);
        Assert.Equal("RE-2024-001", invoice.InvoiceNumber);
        Assert.Equal(119.00m, invoice.TotalPrice);
        Assert.Equal(new DateTime(2024, 1, 15), invoice.Date);
        Assert.Equal(new DateTime(2024, 2, 15), invoice.DueDate);
        Assert.Equal(new DateTime(2024, 1, 16), invoice.DateSent);
        Assert.Equal("invoice", invoice.Kind);
        Assert.Equal("Jahresbeitrag", invoice.Description);
        Assert.Equal("Max Mustermann", invoice.Receiver);
        Assert.Equal("https://easyverein.com/api/v1.7/contact-details/42", invoice.RelatedAddress);
        Assert.Equal(42L, invoice.RelatedAddressId);
        Assert.NotNull(invoice.RelatedBookings);
        Assert.Equal(3, invoice.RelatedBookings!.Count);
        Assert.Equal("https://easyverein.com/api/v1.7/user/7", invoice.PayedFromUser);
        Assert.Equal(7L, invoice.PayedFromUserId);
        Assert.Equal(3L, invoice.ApprovedFromAdminId);
        Assert.Equal(5L, invoice.CanceledInvoiceId);
        Assert.Equal(11L, invoice.BankAccountId);
        Assert.Equal(30189L, invoice.OrgId);
        Assert.Equal("https://easyverein.com/app/file?category=invoice&path=2024/01/test.pdf", invoice.Path);
        Assert.Single(invoice.InvoiceItems!);
        Assert.NotNull(invoice.Charges);
        Assert.Equal(119.00m, invoice.Charges!.Total);
        Assert.Equal(19.00m, invoice.Tax);
        Assert.Equal(0m, invoice.PaymentDifference);
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
        Assert.True(invoice.IsTaxRatePerInvoiceItem);
        Assert.True(invoice.IsSubjectToTax);
        Assert.Equal("default", invoice.Mode);
        Assert.Equal("accepted", invoice.OfferStatus);
        Assert.Equal(new DateTime(2024, 3, 1), invoice.OfferValidUntil);
        Assert.Equal("ANG-2024-001", invoice.OfferNumber);
        Assert.Equal(55L, invoice.RelatedOfferId);
        Assert.Equal("Mit freundlichen Grüßen", invoice.ClosingDescription);
        Assert.False(invoice.UseAddressBalance);
        Assert.Null(invoice.DeleteAfterDate);
        Assert.Null(invoice.DeletedBy);
    }
}
