using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the announcement API endpoint with field selection and filters.
/// </summary>
internal class AnnouncementQuery
{
    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            AnnouncementFields.Id + "," +
            AnnouncementFields.Text + "," +
            AnnouncementFields.Start + "," +
            AnnouncementFields.End + "," +
            AnnouncementFields.ShowBanner + "," +
            AnnouncementFields.IsDismissible + "," +
            AnnouncementFields.IsPublic + "," +
            AnnouncementFields.ShowForNormalMembers + "," +
            AnnouncementFields.Platform + "," +
            AnnouncementFields.BannerLevel + "," +
            AnnouncementFields.AccountTypeVisibility +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{AnnouncementFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{AnnouncementFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
