using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the event API endpoint, including field selection and optional filters.
/// </summary>
internal class EventQuery
{
    /// <summary>Gets or sets an optional name filter.</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional start date greater than or equal filter.</summary>
    internal string? StartGte { get; set; }

    /// <summary>Gets or sets an optional start date less than or equal filter.</summary>
    internal string? StartLte { get; set; }

    /// <summary>Gets or sets an optional end date greater than or equal filter.</summary>
    internal string? EndGte { get; set; }

    /// <summary>Gets or sets an optional end date less than or equal filter.</summary>
    internal string? EndLte { get; set; }

    /// <summary>Gets or sets an optional calendar ID filter.</summary>
    internal string? Calendar { get; set; }

    /// <summary>Gets or sets an optional canceled filter.</summary>
    internal string? Canceled { get; set; }

    /// <summary>Gets or sets an optional public visibility filter.</summary>
    internal string? IsPublic { get; set; }

    /// <summary>Gets or sets an optional comma-separated IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional ordering criterion for the results.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets optional search terms to filter events.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The base field selection query requesting all event fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            EventFields.Id + "," +
            EventFields.Name + "," +
            EventFields.Description + "," +
            EventFields.Prologue + "," +
            EventFields.Note + "," +
            EventFields.Start + "," +
            EventFields.End + "," +
            EventFields.AllDay + "," +
            EventFields.LocationName + "," +
            EventFields.LocationObject + "," +
            EventFields.Parent + "," +
            EventFields.MinParticipators + "," +
            EventFields.MaxParticipators + "," +
            EventFields.StartParticipation + "," +
            EventFields.EndParticipation + "," +
            EventFields.Access + "," +
            EventFields.Weekdays + "," +
            EventFields.SendMailCheck + "," +
            EventFields.ShowMemberArea + "," +
            EventFields.IsPublic + "," +
            EventFields.MassParticipations + "," +
            EventFields.Canceled + "," +
            EventFields.IsReservation + "," +
            EventFields.Creator + "," +
            EventFields.ReservationParentEvent + "," +
            EventFields.Calendar +
            "{" +
                EventFields.Id +
            "}" +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the event endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{EventFields.Name}={Name}");

        if (!string.IsNullOrEmpty(StartGte))
            parts.Add($"{EventFields.StartGte}={StartGte}");

        if (!string.IsNullOrEmpty(StartLte))
            parts.Add($"{EventFields.StartLte}={StartLte}");

        if (!string.IsNullOrEmpty(EndGte))
            parts.Add($"{EventFields.EndGte}={EndGte}");

        if (!string.IsNullOrEmpty(EndLte))
            parts.Add($"{EventFields.EndLte}={EndLte}");

        if (!string.IsNullOrEmpty(Calendar))
            parts.Add($"{EventFields.Calendar}={Calendar}");

        if (!string.IsNullOrEmpty(Canceled))
            parts.Add($"{EventFields.Canceled}={Canceled}");

        if (!string.IsNullOrEmpty(IsPublic))
            parts.Add($"{EventFields.IsPublic}={IsPublic}");

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{EventFields.IdIn}={IdIn}");

        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{EventFields.Ordering}={Ordering}");

        if (Search != null && Search.Length != 0)
            parts.Add($"{EventFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
