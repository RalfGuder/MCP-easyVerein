using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the forum API endpoint with field selection and filters.
/// </summary>
internal class ForumQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional forum name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional forum name to exclude.</summary>
    internal string? NameNot { get; set; }

    /// <summary>Gets or sets an optional slug filter (exact match).</summary>
    internal string? Slug { get; set; }

    /// <summary>Gets or sets an optional slug to exclude.</summary>
    internal string? SlugNot { get; set; }

    /// <summary>Gets or sets an optional forum kind filter.</summary>
    internal int? Type { get; set; }

    /// <summary>Gets or sets an optional lower bound for the creation date.</summary>
    internal string? CreatedGt { get; set; }

    /// <summary>Gets or sets an optional upper bound for the creation date.</summary>
    internal string? CreatedLt { get; set; }

    /// <summary>Gets or sets an optional lower bound for the update date.</summary>
    internal string? UpdatedGt { get; set; }

    /// <summary>Gets or sets an optional upper bound for the update date.</summary>
    internal string? UpdatedLt { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            ForumFields.Id + "," +
            ForumFields.Org + "," +
            ForumFields.LastPost + "," +
            ForumFields.Created + "," +
            ForumFields.Updated + "," +
            ForumFields.Name + "," +
            ForumFields.Slug + "," +
            ForumFields.Description + "," +
            ForumFields.Image + "," +
            ForumFields.Link + "," +
            ForumFields.LinkRedirects + "," +
            ForumFields.Type + "," +
            ForumFields.DirectPostsCount + "," +
            ForumFields.DirectTopicsCount + "," +
            ForumFields.LinkRedirectsCount + "," +
            ForumFields.Order + "," +
            ForumFields.LastPostOn + "," +
            ForumFields.DisplaySubForumList +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        AddText(parts, ForumFields.IdIn, IdIn);
        AddText(parts, ForumFields.Name, Name);
        AddText(parts, ForumFields.NameNot, NameNot);
        AddText(parts, ForumFields.Slug, Slug);
        AddText(parts, ForumFields.SlugNot, SlugNot);
        if (Type.HasValue)
            parts.Add($"{ForumFields.Type}={Type.Value}");
        AddText(parts, ForumFields.CreatedGt, CreatedGt);
        AddText(parts, ForumFields.CreatedLt, CreatedLt);
        AddText(parts, ForumFields.UpdatedGt, UpdatedGt);
        AddText(parts, ForumFields.UpdatedLt, UpdatedLt);
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{ForumFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{ForumFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }

    /// <summary>Appends an escaped text filter to the query parts when a value is set.</summary>
    private static void AddText(List<string> parts, string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            parts.Add($"{key}={Uri.EscapeDataString(value)}");
    }
}
