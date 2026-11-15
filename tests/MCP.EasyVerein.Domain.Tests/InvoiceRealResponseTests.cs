using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceRealResponseTests
{
    private static JsonSerializerOptions Options() => new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = false
    };

    [Fact]
    public void Deserialize_RealResponse_Succeeds()
    {
        var json = File.ReadAllText(Path.Combine("Fixtures", "invoice-real-response.json"));
        var invoice = JsonSerializer.Deserialize<Invoice>(json, Options());

        Assert.NotNull(invoice);
        Assert.Equal(348583191L, invoice!.Id);
        Assert.Equal("https://easyverein.com/api/v1.7/contact-details/345175845", invoice.RelatedAddress);
        Assert.Equal(345175845L, invoice.RelatedAddressId);
        Assert.NotNull(invoice.RelatedBookings);
        Assert.Single(invoice.RelatedBookings!);
        Assert.Equal(234717573L, UrlReference.ExtractId(invoice.RelatedBookings![0]));
        Assert.Equal(18.88m, invoice.TotalPrice);
        Assert.Equal(new DateTime(2025, 3, 26), invoice.Date);
        Assert.Equal(new DateTime(2025, 3, 26), invoice.DueDate);
        Assert.Null(invoice.DateSent);
        Assert.Equal("1", invoice.InvoiceNumber);
        Assert.Equal("expense", invoice.Kind);
        Assert.Equal("94KW7ABAR", invoice.RefNumber);
        Assert.True(invoice.IsReceipt);
    }

    /// <summary>
    /// Verifies that an invoice whose boolean flags are JSON null deserializes with <c>false</c>.
    /// easyVerein returns <c>_isTaxRatePerInvoiceItem: null</c> for receipts created via the API (2026-11-15).
    /// </summary>
    [Theory]
    [InlineData("gross")]
    [InlineData("isDraft")]
    [InlineData("isTemplate")]
    [InlineData("isRequest")]
    [InlineData("removeFileOnDelete")]
    [InlineData("isReceipt")]
    [InlineData("_isTaxRatePerInvoiceItem")]
    [InlineData("_isSubjectToTax")]
    [InlineData("useAddressBalance")]
    public void Deserialize_NullBooleanFlag_ReadsAsFalse(string field)
    {
        var json = $$"""{ "id": 607991811, "{{field}}": null }""";

        var invoice = JsonSerializer.Deserialize<Invoice>(json, Options());

        Assert.NotNull(invoice);
        Assert.Equal(607991811L, invoice!.Id);
    }
}
