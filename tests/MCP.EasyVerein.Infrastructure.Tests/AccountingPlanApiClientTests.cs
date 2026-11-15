using System.Net;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Infrastructure.ApiClient;

namespace MCP.EasyVerein.Infrastructure.Tests;

/// <summary>Tests for the accounting-plan methods of <see cref="EasyVereinApiClient"/> (US-0035).</summary>
public class AccountingPlanApiClientTests
{
    /// <summary>The expected field selection with nested billing-account details.</summary>
    private const string FieldSelection = "query={id,org,billingAccounts{id,number,name},created_at,updated_at}";

    /// <summary>Creates a v2.0 client backed by the given handler.</summary>
    /// <param name="handler">The fake HTTP handler.</param>
    /// <returns>The client.</returns>
    private static EasyVereinApiClient CreateClient(HttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://easyverein.com/api") },
            new EasyVereinConfiguration { ApiKey = "t", ApiUrl = "https://easyverein.com/api", ApiVersion = "v2.0" });

    /// <summary>Verifies that listing requests the plan with nested billing-account details.</summary>
    [Fact]
    public async Task ListAccountingPlans_RequestsNestedBillingAccounts()
    {
        var body = JsonSerializer.Serialize(new
        {
            next = (string?)null,
            results = new[] { new { id = 5430, billingAccounts = new[] { new { id = 57780, number = 13720, name = "Geldtransit" } } } }
        });
        var handler = new MultiPageFakeHttpHandler(new[] { (HttpStatusCode.OK, body) });

        var result = await CreateClient(handler).ListAccountingPlansAsync();

        Assert.Equal("/api/v2.0/accounting-plan", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains(FieldSelection, Uri.UnescapeDataString(handler.LastRequestUri.Query));
        var plan = Assert.Single(result);
        Assert.Equal("Geldtransit", plan.BillingAccounts![0].Name);
    }

    /// <summary>Verifies that a single plan is fetched with the same field selection.</summary>
    [Fact]
    public async Task GetAccountingPlan_RequestsPlanById()
    {
        var handler = new MultiPageFakeHttpHandler(new[] { (HttpStatusCode.OK, "{\"id\":5430}") });

        var plan = await CreateClient(handler).GetAccountingPlanAsync(5430);

        Assert.Equal(5430, plan!.Id);
        Assert.Equal("/api/v2.0/accounting-plan/5430", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains(FieldSelection, Uri.UnescapeDataString(handler.LastRequestUri.Query));
    }

    /// <summary>Verifies that an unknown plan yields <c>null</c>.</summary>
    [Fact]
    public async Task GetAccountingPlan_NotFound_ReturnsNull()
    {
        var handler = new MultiPageFakeHttpHandler(new[] { (HttpStatusCode.NotFound, "{}") });

        Assert.Null(await CreateClient(handler).GetAccountingPlanAsync(1));
    }

    /// <summary>Verifies that an update is sent as PATCH to the plan with the given body.</summary>
    [Fact]
    public async Task UpdateAccountingPlan_SendsPatch()
    {
        var handler = new RecordingHandler("{\"id\":5430}");
        var patch = new Dictionary<string, object> { ["billingAccounts"] = new[] { "https://easyverein.com/api/v2.0/billing-account/1" } };

        var plan = await CreateClient(handler).UpdateAccountingPlanAsync(5430, patch);

        Assert.Equal(5430, plan.Id);
        Assert.Equal(HttpMethod.Patch, handler.Method);
        Assert.Equal("/api/v2.0/accounting-plan/5430", handler.Uri!.AbsolutePath);
        Assert.Contains("billing-account/1", handler.Body);
    }

    /// <summary>Records method, URI and body of the last request.</summary>
    private sealed class RecordingHandler(string response) : HttpMessageHandler
    {
        /// <summary>Gets the HTTP method of the last request.</summary>
        public HttpMethod? Method { get; private set; }

        /// <summary>Gets the URI of the last request.</summary>
        public Uri? Uri { get; private set; }

        /// <summary>Gets the body of the last request.</summary>
        public string Body { get; private set; } = string.Empty;

        /// <summary>Records the request and returns the configured response.</summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Method = request.Method;
            Uri = request.RequestUri;
            Body = request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
