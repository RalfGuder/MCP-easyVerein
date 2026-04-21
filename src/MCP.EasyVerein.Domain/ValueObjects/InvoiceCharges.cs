using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Payment-processor charge summary returned by the easyVerein API.</summary>
public class InvoiceCharges
{
    /// <summary>Gets or sets the charge amount. Maps to API field <c>charge</c>.</summary>
    [JsonPropertyName("charge")] public decimal Charge { get; set; }

    /// <summary>Gets or sets the charge-back amount. Maps to API field <c>chargeBack</c>.</summary>
    [JsonPropertyName("chargeBack")] public decimal ChargeBack { get; set; }

    /// <summary>Gets or sets the total amount (after charges). Maps to API field <c>total</c>.</summary>
    [JsonPropertyName("total")] public decimal Total { get; set; }
}
