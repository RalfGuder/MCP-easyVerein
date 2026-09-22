using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing the organization's inventory object groups via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class InventoryObjectGroupTools(IEasyVereinApiClient client)
{
    /// <summary>Maximum length of a group name accepted by the API.</summary>
    private const int MaxNameLength = 200;

    /// <summary>Maximum length of a hex color accepted by the API.</summary>
    private const int MaxColorLength = 7;

    /// <summary>Maximum length of a short label accepted by the API.</summary>
    private const int MaxShortLength = 4;

    /// <summary>Lists inventory object groups with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_inventory_object_groups"), Description("List the organization's inventory object groups")]
    public async Task<string> ListInventoryObjectGroups(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Group name filter")] string? name,
        [Description("Hex color filter (e.g. '#ff8800')")] string? color,
        [Description("Short-label filter (max 4 chars)")] string? @short,
        [Description("Soft-deleted filter (true = only groups in the wastebasket)")] bool? deleted,
        [Description("Ordering (e.g. 'name' or '-short')")] string? ordering,
        [Description("Search terms (searches name, short, color)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var groups = await client.ListInventoryObjectGroupsAsync(
                idIn, name, color, @short, deleted, ordering, search, ct);
            return JsonSerializer.Serialize(groups, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single inventory object group by its unique identifier.</summary>
    [McpServerTool(Name = "get_inventory_object_group"), Description("Retrieve an inventory object group by its ID")]
    public async Task<string> GetInventoryObjectGroup(
        [Description("The ID of the inventory object group")] long id,
        CancellationToken ct)
    {
        try
        {
            var group = await client.GetInventoryObjectGroupAsync(id, ct);
            return group != null
                ? JsonSerializer.Serialize(group, new JsonSerializerOptions { WriteIndented = true })
                : $"Inventory object group with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new inventory object group in easyVerein.</summary>
    [McpServerTool(Name = "create_inventory_object_group"), Description("Create a new inventory object group")]
    public async Task<string> CreateInventoryObjectGroup(
        [Description("The group name (required, max 200 chars)")] string? name,
        [Description("Hex color (required, max 7 chars, e.g. '#ff8800')")] string? color,
        [Description("Short label (required, max 4 chars)")] string? @short,
        CancellationToken ct)
    {
        try
        {
            if (!HasValue(name) || string.IsNullOrWhiteSpace(name))
                return "ERROR: The group name (name) is required.";
            if (!HasValue(color) || string.IsNullOrWhiteSpace(color))
                return "ERROR: The hex color (color) is required.";
            if (!HasValue(@short) || string.IsNullOrWhiteSpace(@short))
                return "ERROR: The short label (short) is required.";
            var error = ValidateLengths(name, color, @short);
            if (error != null) return error;

            var group = new InventoryObjectGroup { Name = name, Color = color, Short = @short };
            var created = await client.CreateInventoryObjectGroupAsync(group, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an inventory object group (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_inventory_object_group"), Description("Update an inventory object group (only provided fields are changed)")]
    public async Task<string> UpdateInventoryObjectGroup(
        [Description("The ID of the group to update")] long id,
        [Description("New group name (max 200 chars)")] string? name,
        [Description("New hex color (max 7 chars)")] string? color,
        [Description("New short label (max 4 chars)")] string? @short,
        CancellationToken ct)
    {
        try
        {
            var error = ValidateLengths(name, color, @short);
            if (error != null) return error;

            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[InventoryObjectGroupFields.Name] = name!;
            if (HasValue(color)) patch[InventoryObjectGroupFields.Color] = color!;
            if (HasValue(@short)) patch[InventoryObjectGroupFields.Short] = @short!;

            var updated = await client.UpdateInventoryObjectGroupAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes an inventory object group by its unique identifier (moves it to the wastebasket).</summary>
    [McpServerTool(Name = "delete_inventory_object_group"), Description("Delete an inventory object group (moves it to the easyVerein wastebasket). Only authorized users are able to perform this action!")]
    public async Task<string> DeleteInventoryObjectGroup(
        [Description("The ID of the group to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteInventoryObjectGroupAsync(id, ct);
            return $"Inventory object group with ID {id} has been moved to the wastebasket.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks the length limits of the name, color and short label.</summary>
    /// <returns>An error message, or <c>null</c> if all values are within the limits.</returns>
    private static string? ValidateLengths(string? name, string? color, string? @short)
    {
        if (HasValue(name) && name!.Length > MaxNameLength)
            return $"ERROR: The group name must not exceed {MaxNameLength} characters.";
        if (HasValue(color) && color!.Length > MaxColorLength)
            return $"ERROR: The color must not exceed {MaxColorLength} characters.";
        if (HasValue(@short) && @short!.Length > MaxShortLength)
            return $"ERROR: The short label must not exceed {MaxShortLength} characters.";
        return null;
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
