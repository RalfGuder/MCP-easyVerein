namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Chairman Level API field names used in JSON serialization and query building.</summary>
internal static class ChairmanLevelFields
{
    /// <summary>API field name for the unique chairman level identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the chairman level name.</summary>
    internal const string Name = "name";

    /// <summary>API field name for the hex color value.</summary>
    internal const string Color = "color";

    /// <summary>API field name for the short label (max 4 characters).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the members-module permission (R/W/N).</summary>
    internal const string ModuleMembers = "module_members";

    /// <summary>API field name for the events-module permission (R/W/N).</summary>
    internal const string ModuleEvents = "module_events";

    /// <summary>API field name for the protocols-module permission (R/W/N).</summary>
    internal const string ModuleProtocols = "module_protocols";

    /// <summary>API field name for the addresses-module permission (R/W/N).</summary>
    internal const string ModuleAddresses = "module_addresses";

    /// <summary>API field name for the bookings-module permission (R/W/N).</summary>
    internal const string ModuleBookings = "module_bookings";

    /// <summary>API field name for the inventory-module permission (R/W/N).</summary>
    internal const string ModuleInventory = "module_inventory";

    /// <summary>API field name for the files-module permission (R/W/N).</summary>
    internal const string ModuleFiles = "module_files";

    /// <summary>API field name for the account-module permission (R/W/N).</summary>
    internal const string ModuleAccount = "module_account";

    /// <summary>API field name for the todo-module permission (R/W/N).</summary>
    internal const string ModuleTodo = "module_todo";

    /// <summary>API field name for the votings-module permission (R/W/N).</summary>
    internal const string ModuleVotings = "module_votings";

    /// <summary>API field name for the forum-module permission (R/W/N).</summary>
    internal const string ModuleForum = "module_forum";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
