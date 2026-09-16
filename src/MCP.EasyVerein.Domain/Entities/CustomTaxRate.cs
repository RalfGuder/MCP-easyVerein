using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a tax rate from the easyVerein API — either a standard rate (no organization)
/// or an organization-specific custom rate.
/// </summary>
public class CustomTaxRate : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(CustomTaxRateFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the tax-rate label (required, max 600 chars). Maps to API field '<c>taxName</c>'.</summary>
    [JsonPropertyName(CustomTaxRateFields.TaxName)]
    public string? TaxName { get; set; }

    /// <summary>
    /// Gets or sets the tax-rate percentage (required, 0–100). Maps to API field '<c>customTaxRate</c>'.
    /// Accepts numeric or string-encoded values (v2.0). Named with a <c>Value</c> suffix because a member
    /// cannot share the name of its enclosing type.
    /// </summary>
    [JsonPropertyName(CustomTaxRateFields.CustomTaxRate)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? CustomTaxRateValue { get; set; }

    /// <summary>Gets or sets the country code of a standard rate (read-only). Maps to API field '<c>countryCode</c>'.</summary>
    [JsonPropertyName(CustomTaxRateFields.CountryCode)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CountryCode { get; set; }

    /// <summary>Gets or sets the owning organization URL reference (read-only; null for standard rates). Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(CustomTaxRateFields.Org)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the creation timestamp (read-only, set by the API). Maps to API field '<c>created_at</c>'.</summary>
    [JsonPropertyName(CustomTaxRateFields.CreatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Gets or sets the last-update timestamp (read-only, set by the API). Maps to API field '<c>updated_at</c>'.</summary>
    [JsonPropertyName(CustomTaxRateFields.UpdatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; set; }
}
