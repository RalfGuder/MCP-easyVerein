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
