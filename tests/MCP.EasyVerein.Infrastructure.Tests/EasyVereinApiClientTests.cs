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
            ApiToken = "test-token",
            BaseUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };
        return new EasyVereinApiClient(httpClient, config);
    }

    [Fact]
    public async Task GetMembers_ReturnsMembers()
    {
        var members = new List<Member>
        {
            new() { Id = 1, FirstName = "Max", LastName = "Mustermann", Email = "max@test.de" }
        };
        var handler = new FakeHttpHandler(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { results = members }));

        var client = CreateClient(handler);
        var result = await client.GetMembersAsync();

        Assert.Single(result);
        Assert.Equal("Max", result[0].FirstName);
    }

    [Fact]
    public async Task GetMembers_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.GetMembersAsync());
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

    [Fact]
    public async Task GetInvoices_ReturnsInvoices()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = 1, InvoiceNumber = "R-001", Amount = 50.00m }
        };
        var handler = new FakeHttpHandler(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { results = invoices }));

        var client = CreateClient(handler);
        var result = await client.GetInvoicesAsync();

        Assert.Single(result);
        Assert.Equal("R-001", result[0].InvoiceNumber);
    }

    [Fact]
    public async Task GetEvents_ReturnsEvents()
    {
        var events = new List<Event>
        {
            new() { Id = 1, Name = "Jahresversammlung" }
        };
        var handler = new FakeHttpHandler(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { results = events }));

        var client = CreateClient(handler);
        var result = await client.GetEventsAsync();

        Assert.Single(result);
        Assert.Equal("Jahresversammlung", result[0].Name);
    }

    [Fact]
    public async Task GetContacts_ReturnsContacts()
    {
        var contacts = new List<Contact>
        {
            new() { Id = 1, FirstName = "Anna", LastName = "Schmidt" }
        };
        var handler = new FakeHttpHandler(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { results = contacts }));

        var client = CreateClient(handler);
        var result = await client.GetContactsAsync();

        Assert.Single(result);
        Assert.Equal("Anna", result[0].FirstName);
    }

    [Fact]
    public void Constructor_SetsAuthorizationHeader()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, "{}");
        var httpClient = new HttpClient(handler);
        var config = new EasyVereinConfiguration { ApiToken = "my-secret-token" };

        _ = new EasyVereinApiClient(httpClient, config);

        Assert.Contains(httpClient.DefaultRequestHeaders.GetValues("Authorization"),
            v => v == "Token my-secret-token");
    }
}

/// <summary>
/// Einfacher Fake-HttpHandler für Unit-Tests.
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
