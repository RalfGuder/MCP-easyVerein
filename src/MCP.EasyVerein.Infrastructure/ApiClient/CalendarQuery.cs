using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the calendar API endpoint, including field selection and optional filters.
/// </summary>
internal class CalendarQuery
{
    /// <summary>Gets or sets an optional name filter.</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional color filter.</summary>
    internal string? Color { get; set; }

    /// <summary>Gets or sets an optional short name filter.</summary>
    internal string? Short { get; set; }

    /// <summary>Gets or sets an optional name negation filter.</summary>
    internal string? NameNot { get; set; }

    /// <summary>Gets or sets an optional color negation filter.</summary>
    internal string? ColorNot { get; set; }

    /// <summary>Gets or sets an optional short name negation filter.</summary>
    internal string? ShortNot { get; set; }

    /// <summary>Gets or sets an optional comma-separated IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional allowed groups filter.</summary>
    internal string? AllowedGroups { get; set; }

    /// <summary>Gets or sets an optional ordering criterion for the results.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets optional search terms to filter calendars.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The base field selection query requesting all calendar fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            CalendarFields.Id + "," +
            CalendarFields.Name + "," +
            CalendarFields.Color + "," +
            CalendarFields.Short + "," +
            CalendarFields.AllowedGroups +
            "{" +
                CalendarFields.Id +
            "}," +
            CalendarFields.LinkedItems + "," +
            CalendarFields.DeleteEventsAfterDeletion +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the calendar endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{CalendarFields.Name}={Name}");

        if (!string.IsNullOrEmpty(Color))
            parts.Add($"{CalendarFields.Color}={Color}");

        if (!string.IsNullOrEmpty(Short))
            parts.Add($"{CalendarFields.Short}={Short}");

        if (!string.IsNullOrEmpty(NameNot))
            parts.Add($"{CalendarFields.NameNot}={NameNot}");

        if (!string.IsNullOrEmpty(ColorNot))
            parts.Add($"{CalendarFields.ColorNot}={ColorNot}");

        if (!string.IsNullOrEmpty(ShortNot))
            parts.Add($"{CalendarFields.ShortNot}={ShortNot}");

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{CalendarFields.IdIn}={IdIn}");

        if (!string.IsNullOrEmpty(AllowedGroups))
            parts.Add($"{CalendarFields.AllowedGroups}={AllowedGroups}");

        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{CalendarFields.Ordering}={Ordering}");

        if (Search != null && Search.Length != 0)
            parts.Add($"{CalendarFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
