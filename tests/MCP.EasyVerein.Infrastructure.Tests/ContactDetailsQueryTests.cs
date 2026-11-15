using System.Net;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Infrastructure.ApiClient;

namespace MCP.EasyVerein.Infrastructure.Tests;

/// <summary>
/// Tests for the filters of the contact details list request. The easyVerein API v2.0 knows
/// <c>id__in</c>, <c>firstName</c>, <c>familyName</c> and <c>search</c>; it silently ignores
/// <c>id</c> and <c>name</c>.
/// </summary>
public class ContactDetailsQueryTests
{
    /// <summary>Sends a list request with the given filters and returns the decoded query string.</summary>
    /// <param name="call">The list call to execute against the client.</param>
    /// <returns>The URL-decoded query string of the request.</returns>
    private static async Task<string> QueryOf(Func<EasyVereinApiClient, Task> call)
    {
        var body = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new MultiPageFakeHttpHandler(new[] { (HttpStatusCode.OK, body) });
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://easyverein.com/api") };
        var client = new EasyVereinApiClient(httpClient, new EasyVereinConfiguration
        {
            ApiKey = "test-token",
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v2.0"
        });

        await call(client);
        return Uri.UnescapeDataString(handler.LastRequestUri!.Query);
    }

    /// <summary>Verifies that the ID filter is sent as <c>id__in</c>, because the API ignores <c>id</c>.</summary>
    [Fact]
    public async Task List_WithId_SendsIdIn()
    {
        var query = await QueryOf(c => c.ListContactDetailsAsync(id: 360050964));

        Assert.Contains("id__in=360050964", query);
        Assert.DoesNotContain("&id=", query);
    }

    /// <summary>Verifies that first and family name are sent as their own filters.</summary>
    [Fact]
    public async Task List_WithNames_SendsFirstAndFamilyName()
    {
        var query = await QueryOf(c => c.ListContactDetailsAsync(firstName: "Anneliese", familyName: "Oehme"));

        Assert.Contains("firstName=Anneliese", query);
        Assert.Contains("familyName=Oehme", query);
    }

    /// <summary>
    /// Verifies that all search terms go into one comma-joined <c>search</c> parameter. The API
    /// combines comma-separated terms with AND; of repeated parameters it evaluates only the last.
    /// </summary>
    [Fact]
    public async Task List_WithSearch_SendsOneCommaJoinedParameter()
    {
        var query = await QueryOf(c => c.ListContactDetailsAsync(search: new[] { "Anneliese", "Oehme" }));

        Assert.Contains("&search=Anneliese,Oehme", query);
        Assert.Single(query.Split('&'), part => part.StartsWith("search=", StringComparison.Ordinal));
        Assert.DoesNotContain("familyName=", query);
        Assert.DoesNotContain("name=Anneliese", query);
    }

    /// <summary>Verifies that empty search terms are skipped.</summary>
    [Fact]
    public async Task List_WithBlankSearchTerms_SkipsThem()
    {
        var query = await QueryOf(c => c.ListContactDetailsAsync(search: new[] { " ", "Oehme" }));

        Assert.Contains("&search=Oehme", query);
        Assert.DoesNotContain("search= ", query);
    }
}
