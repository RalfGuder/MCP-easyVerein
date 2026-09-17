using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an inventory object of the organization in the easyVerein API.
/// </summary>
public class InventoryObject : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the owning organization URL reference (read-only). Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Org)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the ID of the member responsible for lending. Maps to API field '<c>lendingResponsible</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.LendingResponsible)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? LendingResponsibleId { get; set; }

    /// <summary>Gets or sets the assigned inventory object group URLs (read-only). Maps to API field '<c>inventoryObjectGroups</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.InventoryObjectGroups)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? InventoryObjectGroups { get; set; }

    /// <summary>Gets or sets the picture URL (read-only in this client; upload needs multipart). Maps to API field '<c>picture</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Picture)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Picture { get; set; }

    /// <summary>Gets or sets the number of pieces currently lent (read-only). Maps to API field '<c>currentlyLend</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.CurrentlyLend)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CurrentlyLend { get; set; }

    /// <summary>Gets or sets the lending URLs (read-only). Maps to API field '<c>lendings</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Lendings)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Lendings { get; set; }

    /// <summary>Gets or sets the custom field assignment URLs (read-only). Maps to API field '<c>customFields</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.CustomFields)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? CustomFields { get; set; }

    /// <summary>Gets or sets the ID of the assigned location (read-only). Maps to API field '<c>locationObject</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.LocationObject)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? LocationObjectId { get; set; }

    /// <summary>Gets or sets the date of the latest lending (read-only). Maps to API field '<c>lastLendDate</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.LastLendDate)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? LastLendDate { get; set; }

    /// <summary>Gets or sets the date of the latest return (read-only). Maps to API field '<c>lastReturnDate</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.LastReturnDate)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? LastReturnDate { get; set; }

    /// <summary>Gets or sets the creation timestamp (read-only). Maps to API field '<c>created_at</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.CreatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Gets or sets the last-update timestamp (read-only). Maps to API field '<c>updated_at</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.UpdatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Gets or sets the date the object is removed from the wastebasket (read-only). Maps to API field '<c>_deleteAfterDate</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.DeleteAfterDate)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? DeleteAfterDate { get; set; }

    /// <summary>Gets or sets the name of the user who deleted the object (read-only). Maps to API field '<c>_deletedBy</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.DeletedBy)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DeletedBy { get; set; }

    /// <summary>Gets or sets the object name (max 500 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Name)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the article number (max 500 chars). Maps to API field '<c>identifier</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Identifier)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Identifier { get; set; }

    /// <summary>Gets or sets the description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Description)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the number of pieces. Maps to API field '<c>pieces</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Pieces)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Pieces { get; set; }

    /// <summary>Gets or sets the purchase price. Maps to API field '<c>price</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.Price)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; set; }

    /// <summary>Gets or sets the purchase date. Maps to API field '<c>purchaseDate</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.PurchaseDate)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? PurchaseDate { get; set; }

    /// <summary>Gets or sets the free-text location name (max 500 chars). Maps to API field '<c>locationName</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.LocationName)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LocationName { get; set; }

    /// <summary>Gets or sets whether the object can be lent. Maps to API field '<c>lendingAvailable</c>'.</summary>
    [JsonPropertyName(InventoryObjectFields.LendingAvailable)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LendingAvailable { get; set; }
}
