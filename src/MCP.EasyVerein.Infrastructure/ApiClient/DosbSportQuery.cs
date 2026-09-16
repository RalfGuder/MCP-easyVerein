using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the dosb-sport API endpoint with field selection and filters.
/// </summary>
internal class DosbSportQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional title filter (exact match).</summary>
    internal string? Title { get; set; }

    /// <summary>Gets or sets an optional DOSB sport number filter.</summary>
    internal string? SportNumber { get; set; }

    /// <summary>Gets or sets an optional federation number filter.</summary>
    internal string? FederationNumber { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            DosbSportFields.Id + "," +
            DosbSportFields.Title + "," +
            DosbSportFields.SportNumber + "," +
            DosbSportFields.FederationNumber + "," +
            DosbSportFields.Org + "," +
            DosbSportFields.CreatedAt + "," +
            DosbSportFields.UpdatedAt +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{DosbSportFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Title))
            parts.Add($"{DosbSportFields.Title}={Uri.EscapeDataString(Title)}");
        if (!string.IsNullOrEmpty(SportNumber))
            parts.Add($"{DosbSportFields.SportNumber}={Uri.EscapeDataString(SportNumber)}");
        if (!string.IsNullOrEmpty(FederationNumber))
            parts.Add($"{DosbSportFields.FederationNumber}={Uri.EscapeDataString(FederationNumber)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{DosbSportFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{DosbSportFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
