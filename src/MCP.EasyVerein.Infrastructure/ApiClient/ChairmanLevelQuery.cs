using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the chairman-level API endpoint with field selection and filters.
/// </summary>
internal class ChairmanLevelQuery
{
    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional short-label filter (exact match).</summary>
    internal string? Short { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all chairman-level fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            ChairmanLevelFields.Id + "," +
            ChairmanLevelFields.Name + "," +
            ChairmanLevelFields.Color + "," +
            ChairmanLevelFields.Short + "," +
            ChairmanLevelFields.ModuleMembers + "," +
            ChairmanLevelFields.ModuleEvents + "," +
            ChairmanLevelFields.ModuleProtocols + "," +
            ChairmanLevelFields.ModuleAddresses + "," +
            ChairmanLevelFields.ModuleBookings + "," +
            ChairmanLevelFields.ModuleInventory + "," +
            ChairmanLevelFields.ModuleFiles + "," +
            ChairmanLevelFields.ModuleAccount + "," +
            ChairmanLevelFields.ModuleTodo + "," +
            ChairmanLevelFields.ModuleVotings + "," +
            ChairmanLevelFields.ModuleForum +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{ChairmanLevelFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Short))
            parts.Add($"{ChairmanLevelFields.Short}={Uri.EscapeDataString(Short)}");
        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{ChairmanLevelFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{ChairmanLevelFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{ChairmanLevelFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
