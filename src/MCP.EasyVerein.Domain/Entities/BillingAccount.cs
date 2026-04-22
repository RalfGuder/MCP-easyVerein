using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a billing account (Buchungskonto) from the easyVerein API.
/// </summary>
public class BillingAccount : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the billing account name. Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the SKR account number. Maps to API field '<c>number</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.Number)]
    public int? Number { get; set; }

    /// <summary>Gets or sets the default SKR 42 sphere used for bookings. Maps to API field '<c>defaultSphere</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.DefaultSphere)]
    public int? DefaultSphere { get; set; }

    /// <summary>Gets or sets whether the account is excluded in EUR reporting. Maps to API field '<c>excludeInEur</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.ExcludeInEur)]
    public bool? ExcludeInEur { get; set; }

    /// <summary>Gets or sets the SKR chart identifier. Maps to API field '<c>skr</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.Skr)]
    public string? Skr { get; set; }

    /// <summary>Gets or sets the soft-delete flag. Maps to API field '<c>deleted</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.Deleted)]
    public bool? Deleted { get; set; }

    /// <summary>Gets or sets the virtual count of bookings linked to this billing account (GET only). Maps to API field '<c>linkedBookings</c>'.</summary>
    [JsonPropertyName(BillingAccountFields.LinkedBookings)]
    public int? LinkedBookings { get; set; }
}
