using System.Net;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Infrastructure.ApiClient;

namespace MCP.EasyVerein.Infrastructure.Tests;

public class EasyVereinApiClientTests
{
    private static EasyVereinApiClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://easyverein.com/api")
        };
        var config = new EasyVereinConfiguration
        {
            ApiKey = "test-token",
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };
        return new EasyVereinApiClient(httpClient, config);
    }

    // ------------------------------------------------------------------ //
    // Constructor
    // ------------------------------------------------------------------ //

    [Fact]
    public void Constructor_SetsAuthorizationHeader()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, "{}");
        var httpClient = new HttpClient(handler);
        var config = new EasyVereinConfiguration { ApiKey = "my-secret-token" };

        _ = new EasyVereinApiClient(httpClient, config);

        Assert.Contains(httpClient.DefaultRequestHeaders.GetValues("Authorization"),
            v => v == "Bearer my-secret-token");
    }

    // ------------------------------------------------------------------ //
    // Members
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetMembers_ReturnsMembers()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, emailOrUserName = "max@test.de" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListMembersAsync();

        Assert.Single(result);
        Assert.Equal("max@test.de", result[0].EmailOrUserName);
    }

    [Fact]
    public async Task GetMembers_FollowsPagination_ReturnsAllPages()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, emailOrUserName = "user1@test.de" } },
            next = "https://easyverein.com/api/v1.7/member?query=...&limit=100&page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, emailOrUserName = "user2@test.de" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(
            new[] { (HttpStatusCode.OK, page1), (HttpStatusCode.OK, page2) });
        var client = CreateClient(handler);

        var result = await client.ListMembersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("user1@test.de", result[0].EmailOrUserName);
        Assert.Equal("user2@test.de", result[1].EmailOrUserName);
    }

    [Fact]
    public async Task GetMembers_SendsQueryParameter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListMembersAsync();

        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("query=", handler.LastRequestUri!.Query);
        Assert.Contains("limit=100", handler.LastRequestUri!.Query);
    }

    [Fact]
    public async Task GetMembers_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListMembersAsync());
    }

    [Fact]
    public async Task GetMembers_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Invalid query field: _invalidField\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.ListMembersAsync());

        Assert.Contains("400", ex.Message);
        Assert.Contains("Invalid query field", ex.Message);
    }

    [Fact]
    public async Task GetMembers_WithInternalServerError_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Internal server error occurred\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.InternalServerError, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.ListMembersAsync());

        Assert.Contains("500", ex.Message);
        Assert.Contains("Internal server error", ex.Message);
    }

    [Fact]
    public async Task GetMember_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Bad request for member\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetMemberAsync(1));

        Assert.Contains("400", ex.Message);
        Assert.Contains("Bad request for member", ex.Message);
    }

    [Fact]
    public async Task DeleteMember_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Cannot delete member\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.DeleteMemberAsync(1));

        Assert.Contains("400", ex.Message);
        Assert.Contains("Cannot delete member", ex.Message);
    }

    [Fact]
    public async Task GetMember_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetMemberAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteMember_WithForbidden_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Forbidden, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.DeleteMemberAsync(1));
    }

    // ------------------------------------------------------------------ //
    // Invoices
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetInvoices_ReturnsInvoices()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, invNumber = "R-001", totalPrice = 50.00m }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.GetInvoicesAsync();

        Assert.Single(result);
        Assert.Equal("R-001", result[0].InvoiceNumber);
        Assert.Equal(50.00m, result[0].TotalPrice);
    }

    // ------------------------------------------------------------------ //
    // Events
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetEvents_ReturnsEvents()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, name = "Jahresversammlung" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.GetEventsAsync();

        Assert.Single(result);
        Assert.Equal("Jahresversammlung", result[0].Name);
    }

    // ------------------------------------------------------------------ //
    // ContactDetails
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetContactDetails_ReturnsContactDetails()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, firstName = "Anna", familyName = "Schmidt" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListContactDetailsAsync();

        Assert.Single(result);
        Assert.Equal("Anna", result[0].FirstName);
        Assert.Equal("Schmidt", result[0].FamilyName);
    }
}

// ------------------------------------------------------------------ //
// Test helpers
// ------------------------------------------------------------------ //

/// <summary>
/// Einfacher Fake-HttpHandler für Unit-Tests (einzelne Antwort).
/// </summary>
public class FakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    public FakeHttpHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}

/// <summary>
/// Fake-HttpHandler der nacheinander mehrere Antworten zurückgibt (für Paginierungs-Tests).
/// </summary>
public class MultiPageFakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<(HttpStatusCode StatusCode, string Content)> _responses;

    public MultiPageFakeHttpHandler(IEnumerable<(HttpStatusCode, string)> responses)
    {
        _responses = new Queue<(HttpStatusCode, string)>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_responses.TryDequeue(out var entry))
            throw new InvalidOperationException("Keine weiteren Antworten in der Queue.");

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = entry.StatusCode,
            Content = new StringContent(entry.Content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}

/// <summary>
/// Fake-HttpHandler der die zuletzt gesendete Request-URI speichert.
/// </summary>
public class CapturingFakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    public Uri? LastRequestUri { get; private set; }

    public CapturingFakeHttpHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}
