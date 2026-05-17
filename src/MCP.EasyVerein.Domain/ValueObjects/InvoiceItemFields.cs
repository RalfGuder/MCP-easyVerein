namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Invoice Item API field names used in JSON serialization and query building.</summary>
internal static class InvoiceItemFields
{
    /// <summary>API field name for the unique invoice item identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the organization URL reference.</summary>
    internal const string Org = "org";

    /// <summary>API field name for the related invoice URL reference.</summary>
    internal const string RelatedInvoice = "relatedInvoice";

    /// <summary>API field name for the billing account URL reference.</summary>
    internal const string BillingAccount = "billingAccount";

    /// <summary>API field name for the total price of the invoice item.</summary>
    internal const string TotalPrice = "totalPrice";

    /// <summary>API field name for the article object URL reference.</summary>
    internal const string ArticleObject = "articleObject";

    /// <summary>API field name for the quantity of the invoice item.</summary>
    internal const string Quantity = "quantity";

    /// <summary>API field name for the unit price of the invoice item.</summary>
    internal const string UnitPrice = "unitPrice";

    /// <summary>API field name for the invoice item title.</summary>
    internal const string Title = "title";

    /// <summary>API field name for the invoice item description.</summary>
    internal const string Description = "description";

    /// <summary>API field name for the tax rate applied to the invoice item.</summary>
    internal const string TaxRate = "taxRate";

    /// <summary>API field name for the gross flag indicating whether the unit price already includes tax.</summary>
    internal const string Gross = "gross";

    /// <summary>API field name for the tax name (label) of the invoice item.</summary>
    internal const string TaxName = "taxName";

    /// <summary>API field name for the accounting sphere (Sphäre) of the invoice item.</summary>
    internal const string Sphere = "sphere";

    /// <summary>API field name for the cost centre (Kostenstelle) of the invoice item.</summary>
    internal const string CostCentre = "costCentre";

    /// <summary>API field name for the flag indicating whether an existing balance was deducted.</summary>
    internal const string DeductedExistingBalance = "deductedExistingBalance";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering invoice items by their related invoice.</summary>
    internal const string RelatedInvoiceFilter = "relatedInvoice";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
