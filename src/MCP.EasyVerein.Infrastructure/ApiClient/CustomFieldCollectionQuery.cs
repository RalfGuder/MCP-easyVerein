using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the custom-field-collection API endpoint with field selection and filters.
/// </summary>
internal class CustomFieldCollectionQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional position filter.</summary>
    internal int? Position { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            CustomFieldCollectionFields.Id + "," +
            CustomFieldCollectionFields.Name + "," +
            CustomFieldCollectionFields.OrderSequence + "," +
            CustomFieldCollectionFields.Position +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{CustomFieldCollectionFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (Position.HasValue)
            parts.Add($"{CustomFieldCollectionFields.Position}={Position.Value}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{CustomFieldCollectionFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{CustomFieldCollectionFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
