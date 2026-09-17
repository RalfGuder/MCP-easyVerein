namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Forum API field names used in JSON serialization and query building.</summary>
internal static class ForumFields
{
    /// <summary>API field name for the unique forum identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the owning organization reference (read-only).</summary>
    internal const string Org = "org";

    /// <summary>API field name for the reference to the latest post (read-only).</summary>
    internal const string LastPost = "last_post";

    /// <summary>API field name for the creation timestamp (read-only).</summary>
    internal const string Created = "created";

    /// <summary>API field name for the last-update timestamp (read-only).</summary>
    internal const string Updated = "updated";

    /// <summary>API field name for the forum name (required, max 100 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the URL slug derived from the name (read-only).</summary>
    internal const string Slug = "slug";

    /// <summary>API field name for the forum description.</summary>
    internal const string Description = "description";

    /// <summary>API field name for the forum image (file upload; read-only in this client).</summary>
    internal const string Image = "image";

    /// <summary>API field name for an external forum link (max 200 characters).</summary>
    internal const string Link = "link";

    /// <summary>API field name for the flag that counts how often the forum link was followed.</summary>
    internal const string LinkRedirects = "link_redirects";

    /// <summary>API field name for the forum kind (read-only choice).</summary>
    internal const string Type = "type";

    /// <summary>API field name for the number of posts directly in this forum (read-only).</summary>
    internal const string DirectPostsCount = "direct_posts_count";

    /// <summary>API field name for the number of topics directly in this forum (read-only).</summary>
    internal const string DirectTopicsCount = "direct_topics_count";

    /// <summary>API field name for the number of followed forum links (read-only).</summary>
    internal const string LinkRedirectsCount = "link_redirects_count";

    /// <summary>API field name for the sort position of the forum.</summary>
    internal const string Order = "order";

    /// <summary>API field name for the timestamp of the latest post (read-only).</summary>
    internal const string LastPostOn = "last_post_on";

    /// <summary>API field name for the flag that shows the forum in its parent's sub-forum list.</summary>
    internal const string DisplaySubForumList = "display_sub_forum_list";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for excluding a forum name.</summary>
    internal const string NameNot = "name__not";

    /// <summary>API query parameter for excluding a slug.</summary>
    internal const string SlugNot = "slug__not";

    /// <summary>API query parameter for forums created after a date.</summary>
    internal const string CreatedGt = "created__gt";

    /// <summary>API query parameter for forums created before a date.</summary>
    internal const string CreatedLt = "created__lt";

    /// <summary>API query parameter for forums updated after a date.</summary>
    internal const string UpdatedGt = "updated__gt";

    /// <summary>API query parameter for forums updated before a date.</summary>
    internal const string UpdatedLt = "updated__lt";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
