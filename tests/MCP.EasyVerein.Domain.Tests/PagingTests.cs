using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Tests;

/// <summary>Unit tests for <see cref="PageRequest"/> and <see cref="PagedResult{T}"/> (US-0064).</summary>
public class PagingTests
{
    /// <summary>Verifies the defaults: page 1 with 50 entries and offset 0.</summary>
    [Fact]
    public void PageRequest_Default_IsFirstPageWithFiftyEntries()
    {
        var request = new PageRequest();

        Assert.Equal(1, request.Page);
        Assert.Equal(50, request.Limit);
        Assert.Equal(0, request.Offset);
    }

    /// <summary>Verifies that the offset is computed from page and limit.</summary>
    [Theory]
    [InlineData(1, 100, 0)]
    [InlineData(2, 50, 50)]
    [InlineData(3, 20, 40)]
    public void PageRequest_Offset_IsComputedFromPageAndLimit(int page, int limit, int expectedOffset)
    {
        var request = new PageRequest(limit, page);

        Assert.Equal(expectedOffset, request.Offset);
    }

    /// <summary>Verifies that a limit outside 1..100 is rejected with a clear message.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void PageRequest_InvalidLimit_Throws(int limit)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(limit, 1));

        Assert.Equal("limit", ex.ParamName);
        Assert.Contains("1 und 100", ex.Message);
    }

    /// <summary>Verifies that a page below 1 is rejected with a clear message.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void PageRequest_InvalidPage_Throws(int page)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(50, page));

        Assert.Equal("page", ex.ParamName);
        Assert.Contains("mindestens 1", ex.Message);
    }

    /// <summary>Verifies that a paged result exposes its items and paging metadata.</summary>
    [Fact]
    public void PagedResult_ExposesItemsAndMetadata()
    {
        var result = new PagedResult<string>(new[] { "a", "b" }, totalCount: 95, hasMore: true, page: 1, limit: 2);

        Assert.Equal(new[] { "a", "b" }, result.Items);
        Assert.Equal(95, result.TotalCount);
        Assert.True(result.HasMore);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.Limit);
    }

    /// <summary>Verifies that a paged result behaves as a read-only list of its items.</summary>
    [Fact]
    public void PagedResult_ActsAsReadOnlyListOfItems()
    {
        IReadOnlyList<string> result = new PagedResult<string>(new[] { "a", "b" }, 2, false, 1, 50);

        Assert.Equal(2, result.Count);
        Assert.Equal("b", result[1]);
        Assert.Equal(new[] { "a", "b" }, result.ToList());
    }

    /// <summary>Verifies that <see cref="PagedResult{T}.FromCompleteList"/> wraps a complete list as a single page.</summary>
    [Fact]
    public void PagedResult_FromCompleteList_WrapsCompleteListAsSinglePage()
    {
        var result = PagedResult<string>.FromCompleteList(new[] { "a", "b", "c" });

        Assert.Equal(3, result.TotalCount);
        Assert.False(result.HasMore);
        Assert.Equal(1, result.Page);
        Assert.Equal(3, result.Limit);
    }

    /// <summary>Verifies that a paged result rejects a null item list.</summary>
    [Fact]
    public void PagedResult_NullItems_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PagedResult<string>(null!, 0, false, 1, 50));
    }
}
