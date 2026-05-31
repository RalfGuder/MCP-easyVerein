using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the contact-details-log API endpoint with field selection and filters.
/// </summary>
internal class ContactDetailsLogQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional exact-date filter.</summary>
    internal string? Date { get; set; }

    /// <summary>Gets or sets an optional date greater-or-equal filter.</summary>
    internal string? DateGte { get; set; }

    /// <summary>Gets or sets an optional date less-or-equal filter.</summary>
    internal string? DateLte { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            ContactDetailsLogFields.Id + "," +
            ContactDetailsLogFields.Creator + "," +
            ContactDetailsLogFields.CreatorName + "," +
            ContactDetailsLogFields.Kind + "," +
            ContactDetailsLogFields.RelatedAddress + "," +
            ContactDetailsLogFields.RelatedFile + "," +
            ContactDetailsLogFields.Date + "," +
            ContactDetailsLogFields.Description + "," +
            ContactDetailsLogFields.Shared +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{ContactDetailsLogFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Date))
            parts.Add($"{ContactDetailsLogFields.Date}={Uri.EscapeDataString(Date)}");
        if (!string.IsNullOrEmpty(DateGte))
            parts.Add($"{ContactDetailsLogFields.DateGte}={Uri.EscapeDataString(DateGte)}");
        if (!string.IsNullOrEmpty(DateLte))
            parts.Add($"{ContactDetailsLogFields.DateLte}={Uri.EscapeDataString(DateLte)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{ContactDetailsLogFields.Ordering}={Ordering}");

        return string.Join("&", parts);
    }
}
