namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Describes a single page of a list request (US-0064). Pages start at 1; the limit is
/// capped at the easyVerein maximum of 100 entries per page.
/// </summary>
public sealed record PageRequest
{
    /// <summary>The number of entries per page used when no limit is given.</summary>
    public const int DefaultLimit = 50;

    /// <summary>The maximum number of entries per page accepted by the easyVerein API.</summary>
    public const int MaxLimit = 100;

    /// <summary>The number of the first page.</summary>
    public const int FirstPage = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRequest"/> record.
    /// </summary>
    /// <param name="limit">The number of entries per page (1 to <see cref="MaxLimit"/>).</param>
    /// <param name="page">The 1-based page number.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="limit"/> is outside 1..<see cref="MaxLimit"/> or <paramref name="page"/> is below 1.
    /// </exception>
    public PageRequest(int limit = DefaultLimit, int page = FirstPage)
    {
        if (limit < 1 || limit > MaxLimit)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit,
                $"limit muss zwischen 1 und {MaxLimit} liegen.");
        }

        if (page < FirstPage)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page,
                $"page muss mindestens {FirstPage} sein.");
        }

        Limit = limit;
        Page = page;
    }

    /// <summary>Gets the number of entries per page.</summary>
    public int Limit { get; }

    /// <summary>Gets the 1-based page number.</summary>
    public int Page { get; }
}
