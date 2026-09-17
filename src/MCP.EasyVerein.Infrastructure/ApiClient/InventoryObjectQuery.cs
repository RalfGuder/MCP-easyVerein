using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the inventory-object API endpoint with field selection and filters.
/// </summary>
internal class InventoryObjectQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional object name filter.</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional article number filter.</summary>
    internal string? Identifier { get; set; }

    /// <summary>Gets or sets an optional filter for objects that can be lent.</summary>
    internal bool? LendingAvailable { get; set; }

    /// <summary>Gets or sets an optional soft-deleted filter.</summary>
    internal bool? Deleted { get; set; }

    /// <summary>Gets or sets an optional location ID filter.</summary>
    internal long? LocationObject { get; set; }

    /// <summary>Gets or sets an optional location ID to exclude.</summary>
    internal long? LocationObjectNot { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of group IDs filter.</summary>
    internal string? InventoryObjectGroups { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of group IDs to exclude.</summary>
    internal string? InventoryObjectGroupsNot { get; set; }

    /// <summary>Gets or sets an optional lending state filter.</summary>
    internal string? LendingState { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            InventoryObjectFields.Id + "," +
            InventoryObjectFields.Org + "," +
            InventoryObjectFields.LendingResponsible + "," +
            InventoryObjectFields.InventoryObjectGroups + "," +
            InventoryObjectFields.Picture + "," +
            InventoryObjectFields.CurrentlyLend + "," +
            InventoryObjectFields.Lendings + "," +
            InventoryObjectFields.CustomFields + "," +
            InventoryObjectFields.LocationObject + "," +
            InventoryObjectFields.LastLendDate + "," +
            InventoryObjectFields.LastReturnDate + "," +
            InventoryObjectFields.CreatedAt + "," +
            InventoryObjectFields.UpdatedAt + "," +
            InventoryObjectFields.DeleteAfterDate + "," +
            InventoryObjectFields.DeletedBy + "," +
            InventoryObjectFields.Name + "," +
            InventoryObjectFields.Identifier + "," +
            InventoryObjectFields.Description + "," +
            InventoryObjectFields.Pieces + "," +
            InventoryObjectFields.Price + "," +
            InventoryObjectFields.PurchaseDate + "," +
            InventoryObjectFields.LocationName + "," +
            InventoryObjectFields.LendingAvailable +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        AddText(parts, InventoryObjectFields.IdIn, IdIn);
        AddText(parts, InventoryObjectFields.Name, Name);
        AddText(parts, InventoryObjectFields.Identifier, Identifier);
        AddBool(parts, InventoryObjectFields.LendingAvailable, LendingAvailable);
        AddBool(parts, InventoryObjectFields.Deleted, Deleted);
        if (LocationObject.HasValue)
            parts.Add($"{InventoryObjectFields.LocationObject}={LocationObject.Value}");
        if (LocationObjectNot.HasValue)
            parts.Add($"{InventoryObjectFields.LocationObjectNot}={LocationObjectNot.Value}");
        AddText(parts, InventoryObjectFields.InventoryObjectGroups, InventoryObjectGroups);
        AddText(parts, InventoryObjectFields.InventoryObjectGroupsNot, InventoryObjectGroupsNot);
        AddText(parts, InventoryObjectFields.LendingState, LendingState);
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{InventoryObjectFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{InventoryObjectFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }

    /// <summary>Appends an escaped text filter to the query parts when a value is set.</summary>
    private static void AddText(List<string> parts, string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            parts.Add($"{key}={Uri.EscapeDataString(value)}");
    }

    /// <summary>Appends a lowercase boolean filter to the query parts when a value is set.</summary>
    private static void AddBool(List<string> parts, string key, bool? value)
    {
        if (value.HasValue)
            parts.Add($"{key}={(value.Value ? "true" : "false")}");
    }
}
