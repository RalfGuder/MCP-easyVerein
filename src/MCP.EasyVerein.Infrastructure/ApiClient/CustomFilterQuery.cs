using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the custom-filter API endpoint with field selection and filters.
/// </summary>
internal class CustomFilterQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional filter-model filter.</summary>
    internal string? Model { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of filter models.</summary>
    internal string? ModelIn { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms (allowed field: name).</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            CustomFilterFields.Id + "," +
            CustomFilterFields.Name + "," +
            CustomFilterFields.Model + "," +
            CustomFilterFields.Rules + "," +
            CustomFilterFields.CreatedAt + "," +
            CustomFilterFields.UpdatedAt +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{CustomFilterFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{CustomFilterFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Model))
            parts.Add($"{CustomFilterFields.Model}={Uri.EscapeDataString(Model)}");
        if (!string.IsNullOrEmpty(ModelIn))
            parts.Add($"{CustomFilterFields.ModelIn}={Uri.EscapeDataString(ModelIn)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{CustomFilterFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{CustomFilterFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
