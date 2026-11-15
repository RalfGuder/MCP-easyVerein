using System.Net;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using MCP.EasyVerein.Infrastructure.ApiClient;

namespace MCP.EasyVerein.Infrastructure.Tests;

/// <summary>Tests for page-wise list requests of <see cref="EasyVereinApiClient"/> (US-0064).</summary>
public class PagingApiClientTests
{
    /// <summary>Creates a client for API version v2.0 backed by the given handler.</summary>
    /// <param name="handler">The fake HTTP handler.</param>
    /// <returns>The configured client.</returns>
    private static EasyVereinApiClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://easyverein.com/api") };
        var config = new EasyVereinConfiguration
        {
            ApiKey = "test-token",
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v2.0"
        };
        return new EasyVereinApiClient(httpClient, config);
    }

    /// <summary>Builds a list response body with the given count, next URL and result IDs.</summary>
    /// <param name="count">The total count reported by the API.</param>
    /// <param name="next">The URL of the next page, or <c>null</c>.</param>
    /// <param name="ids">The IDs of the results on this page.</param>
    /// <returns>The serialized JSON body.</returns>
    private static string Page(int count, string? next, params long[] ids) =>
        JsonSerializer.Serialize(new { count, next, results = ids.Select(id => new { id, name = $"K{id}" }) });

    /// <summary>Verifies that a page request fetches exactly one page with limit and offset.</summary>
    [Fact]
    public async Task List_WithPaging_RequestsSinglePageWithLimitAndOffset()
    {
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, Page(95, "https://easyverein.com/api/v2.0/bank-account?limit=20&offset=60", 41, 42))
        });
        var client = CreateClient(handler);

        var result = await client.ListBankAccountsAsync(paging: new PageRequest(20, 3));

        var query = handler.LastRequestUri!.Query;
        Assert.Contains("limit=20", query);
        Assert.Contains("offset=40", query);
        Assert.Equal(2, result.Count);
        Assert.Equal(95, result.TotalCount);
        Assert.True(result.HasMore);
        Assert.Equal(3, result.Page);
        Assert.Equal(20, result.Limit);
    }

    /// <summary>Verifies that filters are kept when a page is requested.</summary>
    [Fact]
    public async Task List_WithPagingAndFilter_KeepsFilterInQuery()
    {
        var handler = new MultiPageFakeHttpHandler(new[] { (HttpStatusCode.OK, Page(1, null, 7)) });
        var client = CreateClient(handler);

        await client.ListBankAccountsAsync(name: "Handkasse", paging: new PageRequest(10, 1));

        var query = handler.LastRequestUri!.Query;
        Assert.Contains("name=Handkasse", query);
        Assert.Contains("limit=10", query);
        Assert.Contains("offset=0", query);
    }

    /// <summary>Verifies that the last page reports no further pages.</summary>
    [Fact]
    public async Task List_WithPagingOnLastPage_HasMoreIsFalse()
    {
        var handler = new MultiPageFakeHttpHandler(new[] { (HttpStatusCode.OK, Page(3, null, 3)) });
        var client = CreateClient(handler);

        var result = await client.ListBankAccountsAsync(paging: new PageRequest(2, 2));

        Assert.Single(result);
        Assert.Equal(3, result.TotalCount);
        Assert.False(result.HasMore);
    }

    /// <summary>Verifies that without a page request all pages are fetched and returned as one page.</summary>
    [Fact]
    public async Task List_WithoutPaging_FetchesAllPagesAsSinglePage()
    {
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, Page(3, "https://easyverein.com/api/v2.0/bank-account?limit=100&offset=2", 1, 2)),
            (HttpStatusCode.OK, Page(3, null, 3))
        });
        var client = CreateClient(handler);

        var result = await client.ListBankAccountsAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.False(result.HasMore);
        Assert.Equal(1, result.Page);
    }

    /// <summary>
    /// Verifies that every list method of the client interface returns a paged result and
    /// accepts an optional <see cref="PageRequest"/> named <c>paging</c> right before the cancellation token.
    /// </summary>
    [Fact]
    public void AllListMethods_AcceptPagingAndReturnPagedResult()
    {
        var listMethods = typeof(IEasyVereinApiClient).GetMethods()
            .Where(m => m.Name.StartsWith("List", StringComparison.Ordinal) || m.Name == "GetInvoicesAsync")
            .ToList();

        Assert.NotEmpty(listMethods);
        Assert.All(listMethods, method =>
        {
            var returnType = method.ReturnType.GetGenericArguments().Single();
            Assert.True(returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(PagedResult<>),
                $"{method.Name} returns {returnType.Name}");
            var parameters = method.GetParameters();
            var paging = parameters[^2];
            Assert.Equal("paging", paging.Name);
            Assert.Equal(typeof(PageRequest), paging.ParameterType);
            Assert.True(paging.HasDefaultValue, $"{method.Name}: paging must be optional");
        });
    }
}
