using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an invoice item (Rechnungsposition) from the easyVerein API. Each invoice can
/// carry multiple invoice items describing a single line: quantity, unit price, total price,
/// tax handling and the accounting attribution (sphere / cost centre).
/// </summary>
public class InvoiceItem : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the organization URL reference. Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Org)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the related invoice URL reference. Maps to API field '<c>relatedInvoice</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.RelatedInvoice)]
    public string? RelatedInvoice { get; set; }

    /// <summary>Gets or sets the billing account URL reference. Maps to API field '<c>billingAccount</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.BillingAccount)]
    public string? BillingAccount { get; set; }

    /// <summary>Gets or sets the total price for the line. Maps to API field '<c>totalPrice</c>'. Accepts numeric or string-encoded values (v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.TotalPrice)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? TotalPrice { get; set; }

    /// <summary>Gets or sets the article object URL reference. Maps to API field '<c>articleObject</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.ArticleObject)]
    public string? ArticleObject { get; set; }

    /// <summary>Gets or sets the quantity. Maps to API field '<c>quantity</c>'. Accepts numeric or string-encoded values (v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.Quantity)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? Quantity { get; set; }

    /// <summary>Gets or sets the unit price. Maps to API field '<c>unitPrice</c>'. Accepts numeric or string-encoded values (v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.UnitPrice)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? UnitPrice { get; set; }

    /// <summary>Gets or sets the invoice item title. Maps to API field '<c>title</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Title)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the invoice item description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Description)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the tax rate applied to the line. Maps to API field '<c>taxRate</c>'. Accepts numeric or string-encoded values (v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.TaxRate)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? TaxRate { get; set; }

    /// <summary>Gets or sets the gross flag indicating whether the unit price already includes tax. Maps to API field '<c>gross</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Gross)]
    public bool? Gross { get; set; }

    /// <summary>Gets or sets the tax name (label) of the invoice item. Maps to API field '<c>taxName</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.TaxName)]
    public string? TaxName { get; set; }

    /// <summary>Gets or sets the accounting sphere (Sphäre). Maps to API field '<c>sphere</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Sphere)]
    public int? Sphere { get; set; }

    /// <summary>Gets or sets the cost centre (Kostenstelle). Maps to API field '<c>costCentre</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.CostCentre)]
    public string? CostCentre { get; set; }

    /// <summary>Gets or sets the flag indicating whether an existing balance was deducted. Maps to API field '<c>deductedExistingBalance</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.DeductedExistingBalance)]
    public bool? DeductedExistingBalance { get; set; }
}
