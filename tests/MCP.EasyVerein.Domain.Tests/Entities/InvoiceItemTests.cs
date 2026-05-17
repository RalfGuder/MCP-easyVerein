using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Tests.Entities;

public class InvoiceItemFieldsTests
{
    [Fact]
    public void Constants_ExposeExpectedApiFieldNames()
    {
        Assert.Equal("id", InvoiceItemFields.Id);
        Assert.Equal("org", InvoiceItemFields.Org);
        Assert.Equal("relatedInvoice", InvoiceItemFields.RelatedInvoice);
        Assert.Equal("billingAccount", InvoiceItemFields.BillingAccount);
        Assert.Equal("totalPrice", InvoiceItemFields.TotalPrice);
        Assert.Equal("articleObject", InvoiceItemFields.ArticleObject);
        Assert.Equal("quantity", InvoiceItemFields.Quantity);
        Assert.Equal("unitPrice", InvoiceItemFields.UnitPrice);
        Assert.Equal("title", InvoiceItemFields.Title);
        Assert.Equal("description", InvoiceItemFields.Description);
        Assert.Equal("taxRate", InvoiceItemFields.TaxRate);
        Assert.Equal("gross", InvoiceItemFields.Gross);
        Assert.Equal("taxName", InvoiceItemFields.TaxName);
        Assert.Equal("sphere", InvoiceItemFields.Sphere);
        Assert.Equal("costCentre", InvoiceItemFields.CostCentre);
        Assert.Equal("deductedExistingBalance", InvoiceItemFields.DeductedExistingBalance);
    }

    [Fact]
    public void Filter_Constants_ExposeExpectedQueryParameters()
    {
        Assert.Equal("id__in", InvoiceItemFields.IdIn);
        Assert.Equal("relatedInvoice", InvoiceItemFields.RelatedInvoiceFilter);
        Assert.Equal("ordering", InvoiceItemFields.Ordering);
        Assert.Equal("search", InvoiceItemFields.Search);
    }
}

public class InvoiceItemEntityTests
{
    [Fact]
    public void Roundtrip_PreservesAllFields()
    {
        var item = new InvoiceItem
        {
            Id = 469271652,
            Org = "https://easyverein.com/api/v2.0/organization/30189",
            RelatedInvoice = "https://easyverein.com/api/v2.0/invoice/469271649",
            BillingAccount = "https://easyverein.com/api/v2.0/billing-account/58811",
            TotalPrice = 3.2m,
            Quantity = 1.00m,
            UnitPrice = 3.20m,
            Title = "Test",
            Description = "Desc",
            TaxRate = 0.00m,
            Gross = false,
            TaxName = " ",
            Sphere = 2,
            CostCentre = "2901",
            DeductedExistingBalance = false
        };

        var json = JsonSerializer.Serialize(item);
        var roundtrip = JsonSerializer.Deserialize<InvoiceItem>(json)!;

        Assert.Equal(item.Id, roundtrip.Id);
        Assert.Equal(item.Sphere, roundtrip.Sphere);
        Assert.Equal(item.CostCentre, roundtrip.CostCentre);
        Assert.Equal(item.BillingAccount, roundtrip.BillingAccount);
        Assert.Equal(item.RelatedInvoice, roundtrip.RelatedInvoice);
    }

    [Fact]
    public void Deserialize_AcceptsStringNumericFields()
    {
        // easyVerein v2.0 returns quantity/unitPrice/taxRate as strings.
        var json = "{\"id\":1,\"quantity\":\"1.00\",\"unitPrice\":\"3.20\",\"taxRate\":\"0.00\",\"totalPrice\":3.2}";
        var item = JsonSerializer.Deserialize<InvoiceItem>(json)!;
        Assert.Equal(1m, item.Quantity);
        Assert.Equal(3.20m, item.UnitPrice);
        Assert.Equal(0.00m, item.TaxRate);
        Assert.Equal(3.2m, item.TotalPrice);
    }
}
