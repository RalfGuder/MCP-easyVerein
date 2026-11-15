using System.Collections;

namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// A page of list results together with the paging metadata reported by the API (US-0064).
/// Acts as a read-only list of its <see cref="Items"/>, so callers that need only the
/// entries can keep treating it as a plain list.
/// </summary>
/// <typeparam name="T">The entity type of the list items.</typeparam>
public sealed class PagedResult<T> : IReadOnlyList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    /// <param name="items">The entries on this page.</param>
    /// <param name="totalCount">
    /// The total number of entries across all pages, or <c>null</c> if the API does not report it
    /// (easyVerein v2.0 sends no <c>count</c>).
    /// </param>
    /// <param name="hasMore">Whether further pages exist after this one.</param>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="limit">The number of entries per page that was requested.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
    public PagedResult(IReadOnlyList<T> items, int? totalCount, bool hasMore, int page, int limit)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        TotalCount = totalCount;
        HasMore = hasMore;
        Page = page;
        Limit = limit;
    }

    /// <summary>Gets the entries on this page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Gets the total number of entries across all pages, or <c>null</c> if unknown.</summary>
    public int? TotalCount { get; }

    /// <summary>Gets a value indicating whether further pages exist after this one.</summary>
    public bool HasMore { get; }

    /// <summary>Gets the 1-based page number.</summary>
    public int Page { get; }

    /// <summary>Gets the number of entries per page that was requested.</summary>
    public int Limit { get; }

    /// <summary>Gets the number of entries on this page.</summary>
    public int Count => Items.Count;

    /// <summary>Gets the entry at the given position on this page.</summary>
    /// <param name="index">The zero-based position.</param>
    /// <returns>The entry at <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the page.</exception>
    public T this[int index] => Items[index];

    /// <summary>
    /// Wraps a complete, unpaged list as a single page without further pages.
    /// </summary>
    /// <param name="items">All entries.</param>
    /// <returns>A result whose total count and limit equal the number of entries.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
    public static PagedResult<T> FromCompleteList(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return new PagedResult<T>(items, items.Count, hasMore: false, PageRequest.FirstPage, items.Count);
    }

    /// <summary>Returns an enumerator over the entries on this page.</summary>
    /// <returns>An enumerator over <see cref="Items"/>.</returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    /// <summary>Returns a non-generic enumerator over the entries on this page.</summary>
    /// <returns>An enumerator over <see cref="Items"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
