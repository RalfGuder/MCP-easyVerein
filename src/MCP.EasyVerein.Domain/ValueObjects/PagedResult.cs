namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// A page of list results together with the paging metadata reported by the API (US-0064).
/// </summary>
/// <typeparam name="T">The entity type of the list items.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    /// <param name="items">The entries on this page.</param>
    /// <param name="totalCount">The total number of entries across all pages.</param>
    /// <param name="hasMore">Whether further pages exist after this one.</param>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="limit">The number of entries per page that was requested.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
    public PagedResult(IReadOnlyList<T> items, int totalCount, bool hasMore, int page, int limit)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        TotalCount = totalCount;
        HasMore = hasMore;
        Page = page;
        Limit = limit;
    }

    /// <summary>Gets the entries on this page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Gets the total number of entries across all pages.</summary>
    public int TotalCount { get; }

    /// <summary>Gets a value indicating whether further pages exist after this one.</summary>
    public bool HasMore { get; }

    /// <summary>Gets the 1-based page number.</summary>
    public int Page { get; }

    /// <summary>Gets the number of entries per page that was requested.</summary>
    public int Limit { get; }
}
