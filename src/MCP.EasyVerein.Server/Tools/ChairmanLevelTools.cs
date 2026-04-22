using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing chairman levels (Vorstandsebenen) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class ChairmanLevelTools(IEasyVereinApiClient client)
{
    /// <summary>Lists chairman levels with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_chairman_levels"), Description("List all chairman levels")]
    public async Task<string> ListChairmanLevels(
        [Description("Exact name filter")] string? name,
        [Description("Exact short-label filter (max 4 chars)")] string? @short,
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Ordering (e.g. 'name' or '-name')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var levels = await client.ListChairmanLevelsAsync(
                name, @short, idIn, ordering, search, ct);
            return JsonSerializer.Serialize(levels, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single chairman level by its unique identifier.</summary>
    [McpServerTool(Name = "get_chairman_level"), Description("Retrieve a chairman level by its ID")]
    public async Task<string> GetChairmanLevel(
        [Description("The ID of the chairman level")] long id,
        CancellationToken ct)
    {
        try
        {
            var level = await client.GetChairmanLevelAsync(id, ct);
            return level != null
                ? JsonSerializer.Serialize(level, new JsonSerializerOptions { WriteIndented = true })
                : $"Chairman level with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new chairman level in easyVerein.</summary>
    [McpServerTool(Name = "create_chairman_level"), Description("Create a new chairman level")]
    public async Task<string> CreateChairmanLevel(
        [Description("The chairman level name (required, max 200 chars)")] string name,
        [Description("Hex color (max 7 chars, e.g. '#ff8800') - only required for groups")] string? color,
        [Description("Short label (max 4 chars) - only required for groups")] string? @short,
        [Description("Members-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleMembers,
        [Description("Events-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleEvents,
        [Description("Protocols-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleProtocols,
        [Description("Addresses-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleAddresses,
        [Description("Bookings-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleBookings,
        [Description("Inventory-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleInventory,
        [Description("Files-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleFiles,
        [Description("Account-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleAccount,
        [Description("Todo-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleTodo,
        [Description("Votings-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleVotings,
        [Description("Forum-module permission ('R', 'W' or 'N'; default 'W')")] string? moduleForum,
        CancellationToken ct)
    {
        try
        {
            var level = new ChairmanLevel { Name = name };
            if (HasValue(color)) level.Color = color;
            if (HasValue(@short)) level.Short = @short;
            if (HasValue(moduleMembers)) level.ModuleMembers = moduleMembers;
            if (HasValue(moduleEvents)) level.ModuleEvents = moduleEvents;
            if (HasValue(moduleProtocols)) level.ModuleProtocols = moduleProtocols;
            if (HasValue(moduleAddresses)) level.ModuleAddresses = moduleAddresses;
            if (HasValue(moduleBookings)) level.ModuleBookings = moduleBookings;
            if (HasValue(moduleInventory)) level.ModuleInventory = moduleInventory;
            if (HasValue(moduleFiles)) level.ModuleFiles = moduleFiles;
            if (HasValue(moduleAccount)) level.ModuleAccount = moduleAccount;
            if (HasValue(moduleTodo)) level.ModuleTodo = moduleTodo;
            if (HasValue(moduleVotings)) level.ModuleVotings = moduleVotings;
            if (HasValue(moduleForum)) level.ModuleForum = moduleForum;

            var created = await client.CreateChairmanLevelAsync(level, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing chairman level (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_chairman_level"), Description("Update a chairman level (only provided fields are changed)")]
    public async Task<string> UpdateChairmanLevel(
        [Description("The ID of the chairman level to update")] long id,
        [Description("New name")] string? name,
        [Description("New hex color")] string? color,
        [Description("New short label")] string? @short,
        [Description("New members-module permission ('R', 'W' or 'N')")] string? moduleMembers,
        [Description("New events-module permission ('R', 'W' or 'N')")] string? moduleEvents,
        [Description("New protocols-module permission ('R', 'W' or 'N')")] string? moduleProtocols,
        [Description("New addresses-module permission ('R', 'W' or 'N')")] string? moduleAddresses,
        [Description("New bookings-module permission ('R', 'W' or 'N')")] string? moduleBookings,
        [Description("New inventory-module permission ('R', 'W' or 'N')")] string? moduleInventory,
        [Description("New files-module permission ('R', 'W' or 'N')")] string? moduleFiles,
        [Description("New account-module permission ('R', 'W' or 'N')")] string? moduleAccount,
        [Description("New todo-module permission ('R', 'W' or 'N')")] string? moduleTodo,
        [Description("New votings-module permission ('R', 'W' or 'N')")] string? moduleVotings,
        [Description("New forum-module permission ('R', 'W' or 'N')")] string? moduleForum,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[ChairmanLevelFields.Name] = name!;
            if (HasValue(color)) patch[ChairmanLevelFields.Color] = color!;
            if (HasValue(@short)) patch[ChairmanLevelFields.Short] = @short!;
            if (HasValue(moduleMembers)) patch[ChairmanLevelFields.ModuleMembers] = moduleMembers!;
            if (HasValue(moduleEvents)) patch[ChairmanLevelFields.ModuleEvents] = moduleEvents!;
            if (HasValue(moduleProtocols)) patch[ChairmanLevelFields.ModuleProtocols] = moduleProtocols!;
            if (HasValue(moduleAddresses)) patch[ChairmanLevelFields.ModuleAddresses] = moduleAddresses!;
            if (HasValue(moduleBookings)) patch[ChairmanLevelFields.ModuleBookings] = moduleBookings!;
            if (HasValue(moduleInventory)) patch[ChairmanLevelFields.ModuleInventory] = moduleInventory!;
            if (HasValue(moduleFiles)) patch[ChairmanLevelFields.ModuleFiles] = moduleFiles!;
            if (HasValue(moduleAccount)) patch[ChairmanLevelFields.ModuleAccount] = moduleAccount!;
            if (HasValue(moduleTodo)) patch[ChairmanLevelFields.ModuleTodo] = moduleTodo!;
            if (HasValue(moduleVotings)) patch[ChairmanLevelFields.ModuleVotings] = moduleVotings!;
            if (HasValue(moduleForum)) patch[ChairmanLevelFields.ModuleForum] = moduleForum!;

            var updated = await client.UpdateChairmanLevelAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a chairman level by its unique identifier.</summary>
    [McpServerTool(Name = "delete_chairman_level"), Description("Delete a chairman level. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteChairmanLevel(
        [Description("The ID of the chairman level to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteChairmanLevelAsync(id, ct);
            return $"Chairman level with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
