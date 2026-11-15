using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

/// <summary>Tests for the <see cref="AccountingPlan"/> entity (US-0035).</summary>
public class AccountingPlanEntityTests
{
    /// <summary>The serializer options used by the API client.</summary>
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    /// <summary>Verifies that billing accounts given as URL references are read with their IDs.</summary>
    [Fact]
    public void Deserialize_BillingAccountsAsUrls_ReadsIds()
    {
        const string json = """
            {"id":5430,"org":"https://easyverein.com/api/v2.0/organization/30189",
             "billingAccounts":["https://easyverein.com/api/v2.0/billing-account/57541",
                                "https://easyverein.com/api/v2.0/billing-account/57780"],
             "created_at":"2026-05-27T06:23:50.800579+02:00","updated_at":"2026-11-15T16:23:30.138962+01:00"}
            """;

        var plan = JsonSerializer.Deserialize<AccountingPlan>(json, Options)!;

        Assert.Equal(5430, plan.Id);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", plan.Org);
        Assert.Equal(new long[] { 57541, 57780 }, plan.BillingAccounts!.Select(b => b.Id));
        Assert.NotNull(plan.CreatedAt);
        Assert.NotNull(plan.UpdatedAt);
    }

    /// <summary>Verifies that embedded billing accounts are read with number and name.</summary>
    [Fact]
    public void Deserialize_BillingAccountsAsObjects_ReadsNumberAndName()
    {
        const string json = """
            {"id":5430,"billingAccounts":[{"id":57780,"number":13720,"name":"Geldtransit"}]}
            """;

        var plan = JsonSerializer.Deserialize<AccountingPlan>(json, Options)!;

        var account = Assert.Single(plan.BillingAccounts!);
        Assert.Equal(57780, account.Id);
        Assert.Equal(13720, account.Number);
        Assert.Equal("Geldtransit", account.Name);
    }
}
