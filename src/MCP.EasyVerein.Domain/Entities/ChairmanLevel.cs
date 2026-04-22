using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a chairman level (Vorstandsebene) from the easyVerein API.
/// </summary>
public class ChairmanLevel
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the chairman level name (required, max 200 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color value (max 7 chars). Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.Color)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label (max 4 chars). Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the members-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_members</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleMembers)]
    public string? ModuleMembers { get; set; }

    /// <summary>Gets or sets the events-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_events</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleEvents)]
    public string? ModuleEvents { get; set; }

    /// <summary>Gets or sets the protocols-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_protocols</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleProtocols)]
    public string? ModuleProtocols { get; set; }

    /// <summary>Gets or sets the addresses-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_addresses</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleAddresses)]
    public string? ModuleAddresses { get; set; }

    /// <summary>Gets or sets the bookings-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_bookings</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleBookings)]
    public string? ModuleBookings { get; set; }

    /// <summary>Gets or sets the inventory-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_inventory</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleInventory)]
    public string? ModuleInventory { get; set; }

    /// <summary>Gets or sets the files-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_files</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleFiles)]
    public string? ModuleFiles { get; set; }

    /// <summary>Gets or sets the account-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_account</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleAccount)]
    public string? ModuleAccount { get; set; }

    /// <summary>Gets or sets the todo-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_todo</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleTodo)]
    public string? ModuleTodo { get; set; }

    /// <summary>Gets or sets the votings-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_votings</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleVotings)]
    public string? ModuleVotings { get; set; }

    /// <summary>Gets or sets the forum-module permission ('R', 'W' or 'N'). Maps to API field '<c>module_forum</c>'.</summary>
    [JsonPropertyName(ChairmanLevelFields.ModuleForum)]
    public string? ModuleForum { get; set; }
}
