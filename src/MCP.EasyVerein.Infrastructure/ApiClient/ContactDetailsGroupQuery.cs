using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the contact-details-group API endpoint with field selection and filters.
/// </summary>
internal class ContactDetailsGroupQuery
{
    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional color filter (exact match).</summary>
    internal string? Color { get; set; }

    /// <summary>Gets or sets an optional short-label filter (exact match).</summary>
    internal string? Short { get; set; }

    /// <summary>Gets or sets an optional soft-delete filter.</summary>
    internal bool? Deleted { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms (allowed fields: name, short).</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            ContactDetailsGroupFields.Id + "," +
            ContactDetailsGroupFields.Name + "," +
            ContactDetailsGroupFields.Color + "," +
            ContactDetailsGroupFields.Short + "," +
            ContactDetailsGroupFields.OrderSequence +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{ContactDetailsGroupFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Color))
            parts.Add($"{ContactDetailsGroupFields.Color}={Uri.EscapeDataString(Color)}");
        if (!string.IsNullOrEmpty(Short))
            parts.Add($"{ContactDetailsGroupFields.Short}={Uri.EscapeDataString(Short)}");
        if (Deleted.HasValue)
            parts.Add($"{ContactDetailsGroupFields.Deleted}={(Deleted.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{ContactDetailsGroupFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{ContactDetailsGroupFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{ContactDetailsGroupFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
