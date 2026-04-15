namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Contains constant field names for the easyVerein Announcement API resource.
/// </summary>
internal static class AnnouncementFields
{
    /// <summary>The unique identifier field.</summary>
    internal const string Id = "id";

    /// <summary>The HTML text content field.</summary>
    internal const string Text = "text";

    /// <summary>The start date/time field.</summary>
    internal const string Start = "start";

    /// <summary>The end date/time field.</summary>
    internal const string End = "end";

    /// <summary>The show banner flag field.</summary>
    internal const string ShowBanner = "showBanner";

    /// <summary>The dismissible flag field.</summary>
    internal const string IsDismissible = "isDismissible";

    /// <summary>The public visibility flag field.</summary>
    internal const string IsPublic = "isPublic";

    /// <summary>The normal members visibility flag field.</summary>
    internal const string ShowForNormalMembers = "showForNormalMembers";

    /// <summary>The platform identifier field.</summary>
    internal const string Platform = "platform";

    /// <summary>The banner level field (e.g. success, warning).</summary>
    internal const string BannerLevel = "bannerLevel";

    /// <summary>The account type visibility field.</summary>
    internal const string AccountTypeVisibility = "accountTypeVisibility";

    /// <summary>The ordering query parameter.</summary>
    internal const string Ordering = "ordering";

    /// <summary>The search query parameter.</summary>
    internal const string Search = "search";
}
