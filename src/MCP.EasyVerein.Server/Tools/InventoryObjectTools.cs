using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing the organization's inventory objects via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class InventoryObjectTools(IEasyVereinApiClient client)
{
    /// <summary>Maximum length of the name, article number and location name accepted by the API.</summary>
    private const int MaxTextLength = 500;

    /// <summary>Lists inventory objects with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_inventory_objects"), Description("List the organization's inventory objects")]
    public async Task<string> ListInventoryObjects(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Object name filter")] string? name,
        [Description("Article number filter")] string? identifier,
        [Description("Only objects that can (true) or cannot (false) be lent")] bool? lendingAvailable,
        [Description("Soft-deleted filter (true = only objects in the wastebasket)")] bool? deleted,
        [Description("Location ID filter")] long? locationObject,
        [Description("Location ID to exclude")] long? locationObjectNot,
        [Description("Comma-separated list of inventory object group IDs")] string? inventoryObjectGroups,
        [Description("Comma-separated list of inventory object group IDs to exclude")] string? inventoryObjectGroupsNot,
        [Description("Lending state filter")] string? lendingState,
        [Description("Ordering (e.g. 'name' or '-purchaseDate')")] string? ordering,
        [Description("Search terms (searches name, description, locationName)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var items = await client.ListInventoryObjectsAsync(
                idIn, name, identifier, lendingAvailable, deleted, locationObject, locationObjectNot,
                inventoryObjectGroups, inventoryObjectGroupsNot, lendingState, ordering, search, ct);
            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single inventory object by its unique identifier.</summary>
    [McpServerTool(Name = "get_inventory_object"), Description("Retrieve an inventory object by its ID")]
    public async Task<string> GetInventoryObject(
        [Description("The ID of the inventory object")] long id,
        CancellationToken ct)
    {
        try
        {
            var item = await client.GetInventoryObjectAsync(id, ct);
            return item != null
                ? JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true })
                : $"Inventory object with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new inventory object in easyVerein.</summary>
    [McpServerTool(Name = "create_inventory_object"), Description("Create a new inventory object")]
    public async Task<string> CreateInventoryObject(
        [Description("The object name (max 500 chars)")] string? name,
        [Description("Article number (max 500 chars)")] string? identifier,
        [Description("Description")] string? description,
        [Description("Number of pieces")] int? pieces,
        [Description("Purchase price")] decimal? price,
        [Description("Purchase date (YYYY-MM-DD or ISO 8601)")] string? purchaseDate,
        [Description("Free-text location name (max 500 chars)")] string? locationName,
        [Description("Whether the object can be lent")] bool? lendingAvailable,
        [Description("Member ID of the person responsible for lending")] long? lendingResponsible,
        CancellationToken ct)
    {
        try
        {
            var error = ValidateText(name, identifier, locationName);
            if (error != null) return error;

            DateTimeOffset? parsedDate = null;
            if (HasValue(purchaseDate))
            {
                if (!TryParseDate(purchaseDate!, out var date))
                    return $"ERROR: Invalid purchase date '{purchaseDate}'. Use YYYY-MM-DD or ISO 8601.";
                parsedDate = date;
            }

            var item = new InventoryObject
            {
                Name = HasValue(name) ? name : null,
                Identifier = HasValue(identifier) ? identifier : null,
                Description = HasValue(description) ? description : null,
                Pieces = pieces,
                Price = price,
                PurchaseDate = parsedDate,
                LocationName = HasValue(locationName) ? locationName : null,
                LendingAvailable = lendingAvailable,
                LendingResponsibleId = lendingResponsible
            };
            var created = await client.CreateInventoryObjectAsync(item, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an inventory object (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_inventory_object"), Description("Update an inventory object (only provided fields are changed)")]
    public async Task<string> UpdateInventoryObject(
        [Description("The ID of the inventory object to update")] long id,
        [Description("New object name (max 500 chars)")] string? name,
        [Description("New article number (max 500 chars)")] string? identifier,
        [Description("New description")] string? description,
        [Description("New number of pieces")] int? pieces,
        [Description("New purchase price")] decimal? price,
        [Description("New purchase date (YYYY-MM-DD or ISO 8601)")] string? purchaseDate,
        [Description("New free-text location name (max 500 chars)")] string? locationName,
        [Description("Whether the object can be lent")] bool? lendingAvailable,
        [Description("Member ID of the person responsible for lending")] long? lendingResponsible,
        CancellationToken ct)
    {
        try
        {
            var error = ValidateText(name, identifier, locationName);
            if (error != null) return error;
            if (HasValue(purchaseDate) && !TryParseDate(purchaseDate!, out _))
                return $"ERROR: Invalid purchase date '{purchaseDate}'. Use YYYY-MM-DD or ISO 8601.";

            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[InventoryObjectFields.Name] = name!;
            if (HasValue(identifier)) patch[InventoryObjectFields.Identifier] = identifier!;
            if (HasValue(description)) patch[InventoryObjectFields.Description] = description!;
            if (pieces.HasValue) patch[InventoryObjectFields.Pieces] = pieces.Value;
            if (price.HasValue) patch[InventoryObjectFields.Price] = price.Value;
            if (HasValue(purchaseDate)) patch[InventoryObjectFields.PurchaseDate] = purchaseDate!;
            if (HasValue(locationName)) patch[InventoryObjectFields.LocationName] = locationName!;
            if (lendingAvailable.HasValue) patch[InventoryObjectFields.LendingAvailable] = lendingAvailable.Value;
            if (lendingResponsible.HasValue) patch[InventoryObjectFields.LendingResponsible] = lendingResponsible.Value;

            var updated = await client.UpdateInventoryObjectAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes an inventory object by its unique identifier (moves it to the wastebasket).</summary>
    [McpServerTool(Name = "delete_inventory_object"), Description("Delete an inventory object (moves it to the easyVerein wastebasket). Only authorized users are able to perform this action!")]
    public async Task<string> DeleteInventoryObject(
        [Description("The ID of the inventory object to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteInventoryObjectAsync(id, ct);
            return $"Inventory object with ID {id} has been moved to the wastebasket.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks the length limits of the name, article number and location name.</summary>
    /// <returns>An error message, or <c>null</c> if all values are within the limits.</returns>
    private static string? ValidateText(string? name, string? identifier, string? locationName)
    {
        if (HasValue(name) && name!.Length > MaxTextLength)
            return $"ERROR: The name must not exceed {MaxTextLength} characters.";
        if (HasValue(identifier) && identifier!.Length > MaxTextLength)
            return $"ERROR: The article number must not exceed {MaxTextLength} characters.";
        if (HasValue(locationName) && locationName!.Length > MaxTextLength)
            return $"ERROR: The location name must not exceed {MaxTextLength} characters.";
        return null;
    }

    /// <summary>Parses a date-only or ISO 8601 date string using the invariant culture.</summary>
    private static bool TryParseDate(string value, out DateTimeOffset date) =>
        DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date);

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
