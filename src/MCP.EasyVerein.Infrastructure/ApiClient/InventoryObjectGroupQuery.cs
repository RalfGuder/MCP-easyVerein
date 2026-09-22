using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the inventory-object-group API endpoint with field selection and filters.
/// </summary>
internal class InventoryObjectGroupQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional group name filter.</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional hex color filter.</summary>
    internal string? Color { get; set; }

    /// <summary>Gets or sets an optional short-label filter.</summary>
    internal string? Short { get; set; }

    /// <summary>Gets or sets an optional soft-deleted filter.</summary>
    internal bool? Deleted { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            InventoryObjectGroupFields.Id + "," +
            InventoryObjectGroupFields.Org + "," +
            InventoryObjectGroupFields.DeleteAfterDate + "," +
            InventoryObjectGroupFields.DeletedBy + "," +
            InventoryObjectGroupFields.CreatedAt + "," +
            InventoryObjectGroupFields.UpdatedAt + "," +
            InventoryObjectGroupFields.Name + "," +
            InventoryObjectGroupFields.Color + "," +
            InventoryObjectGroupFields.Short + "," +
            InventoryObjectGroupFields.LinkedItems +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        AddText(parts, InventoryObjectGroupFields.IdIn, IdIn);
        AddText(parts, InventoryObjectGroupFields.Name, Name);
        AddText(parts, InventoryObjectGroupFields.Color, Color);
        AddText(parts, InventoryObjectGroupFields.Short, Short);
        if (Deleted.HasValue)
            parts.Add($"{InventoryObjectGroupFields.Deleted}={(Deleted.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{InventoryObjectGroupFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{InventoryObjectGroupFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }

    /// <summary>Appends an escaped text filter to the query parts when a value is set.</summary>
    private static void AddText(List<string> parts, string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            parts.Add($"{key}={Uri.EscapeDataString(value)}");
    }
}
