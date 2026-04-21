namespace MCP.EasyVerein.Domain.Helpers;

/// <summary>
/// Helpers for working with easyVerein API URL-form foreign-key references such as
/// <c>https://easyverein.com/api/v1.7/contact-details/345175845</c>.
/// </summary>
public static class UrlReference
{
    /// <summary>
    /// Extracts the trailing numeric identifier from an easyVerein resource URL.
    /// </summary>
    /// <param name="url">The URL or null.</param>
    /// <returns>The parsed identifier, or <c>null</c> if the URL is null, empty, or has no trailing number.</returns>
    public static long? ExtractId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var trimmed = url.TrimEnd('/');
        var slash = trimmed.LastIndexOf('/');
        if (slash < 0 || slash == trimmed.Length - 1) return null;
        var tail = trimmed[(slash + 1)..];
        return long.TryParse(tail, out var id) ? id : null;
    }
}
