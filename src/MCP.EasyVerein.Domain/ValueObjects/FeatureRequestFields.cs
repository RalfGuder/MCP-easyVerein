namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Feature-Request API field names used in JSON serialization and query building.</summary>
internal static class FeatureRequestFields
{
    /// <summary>API field name for the unique feature-request identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the public label (e.g. 'C101'; read-only).</summary>
    internal const string Label = "label";

    /// <summary>API field name for the title (required on create).</summary>
    internal const string Title = "title";

    /// <summary>API field name for the description (required on create).</summary>
    internal const string Description = "description";

    /// <summary>API field name for the vendor's response (read-only).</summary>
    internal const string Response = "response";

    /// <summary>API field name for the author object (read-only; not selectable as nested field).</summary>
    internal const string Author = "author";

    /// <summary>API field name for the number of votes in favor (read-only).</summary>
    internal const string ProVotesCount = "proVotesCount";

    /// <summary>API field name for the number of votes against (read-only).</summary>
    internal const string ContraVotesCount = "contraVotesCount";

    /// <summary>API field name for whether the current user has voted (read-only).</summary>
    internal const string HasVoted = "hasVoted";

    /// <summary>API field name for the status code (read-only).</summary>
    internal const string Status = "status";

    /// <summary>API field name for the approval flag (read-only).</summary>
    internal const string Approved = "approved";

    /// <summary>API field name for the creation date (read-only).</summary>
    internal const string Date = "date";

    /// <summary>API field name for the category code (a numeric choice value).</summary>
    internal const string Category = "category";

    /// <summary>Nested field name for the organization of the author.</summary>
    internal const string Org = "org";

    /// <summary>Nested field name for the short name of the author's organization.</summary>
    internal const string Short = "short";

    /// <summary>Nested field name for the name of the author's organization.</summary>
    internal const string Name = "name";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for returning only requests authored by the current user.</summary>
    internal const string AuthorIsme = "author__isme";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search (allowed fields: title, description).</summary>
    internal const string Search = "search";

    /// <summary>Path segment of the vote-in-favor action.</summary>
    internal const string VoteForAction = "voteFor";

    /// <summary>Path segment of the vote-against action.</summary>
    internal const string VoteAgainstAction = "voteAgainst";
}
