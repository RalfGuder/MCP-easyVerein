using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the feature-request API endpoint with field selection and filters.
/// </summary>
internal class FeatureRequestQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional status code filter.</summary>
    internal string? Status { get; set; }

    /// <summary>Gets or sets an optional category code filter.</summary>
    internal string? Category { get; set; }

    /// <summary>Gets or sets an optional own-requests filter.</summary>
    internal bool? AuthorIsme { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms (allowed fields: title, description).</summary>
    internal string[]? Search { get; set; }

    /// <summary>
    /// Field selection only, without any filters. Use for single-resource GETs.
    /// <c>author</c> is requested as a whole because the API rejects it as a nested selection.
    /// </summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            FeatureRequestFields.Id + "," +
            FeatureRequestFields.Label + "," +
            FeatureRequestFields.Title + "," +
            FeatureRequestFields.Description + "," +
            FeatureRequestFields.Response + "," +
            FeatureRequestFields.Author + "," +
            FeatureRequestFields.ProVotesCount + "," +
            FeatureRequestFields.ContraVotesCount + "," +
            FeatureRequestFields.HasVoted + "," +
            FeatureRequestFields.Status + "," +
            FeatureRequestFields.Approved + "," +
            FeatureRequestFields.Date + "," +
            FeatureRequestFields.Category +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{FeatureRequestFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Status))
            parts.Add($"{FeatureRequestFields.Status}={Uri.EscapeDataString(Status)}");
        if (!string.IsNullOrEmpty(Category))
            parts.Add($"{FeatureRequestFields.Category}={Uri.EscapeDataString(Category)}");
        if (AuthorIsme.HasValue)
            parts.Add($"{FeatureRequestFields.AuthorIsme}={(AuthorIsme.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{FeatureRequestFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{FeatureRequestFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
