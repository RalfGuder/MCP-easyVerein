using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities
{
    /// <summary>
    /// The accounting plan of an organization: the set of billing accounts enabled for bookings
    /// and receipts (US-0035). easyVerein keeps exactly one plan per organization.
    /// </summary>
    public class AccountingPlan : IHasId
    {
        /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
        [JsonPropertyName(AccountingPlanFields.Id)] public long Id { get; set; }

        /// <summary>Gets or sets the owning organization URL reference (read-only). Maps to API field '<c>org</c>'.</summary>
        [JsonPropertyName(AccountingPlanFields.Org)] public string? Org { get; set; }

        /// <summary>
        /// Gets or sets the enabled billing accounts. Maps to API field '<c>billingAccounts</c>'. Elements
        /// arrive as URL references or, with a nested field selection, as objects with ID, number and name.
        /// </summary>
        [JsonPropertyName(AccountingPlanFields.BillingAccounts)]
        [JsonConverter(typeof(FlexibleReferenceListConverter<BillingAccount>))]
        public List<BillingAccount>? BillingAccounts { get; set; }

        /// <summary>Gets or sets the creation timestamp. Maps to API field '<c>created_at</c>'.</summary>
        [JsonPropertyName(AccountingPlanFields.CreatedAt)]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the last update timestamp. Maps to API field '<c>updated_at</c>'.</summary>
        [JsonPropertyName(AccountingPlanFields.UpdatedAt)]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime? UpdatedAt { get; set; }
    }
}
