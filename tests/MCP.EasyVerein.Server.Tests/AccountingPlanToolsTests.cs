using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Tests for the accounting-plan MCP tools (US-0035).</summary>
public class AccountingPlanToolsTests
{
    /// <summary>The ID of the accounting plan used in the tests.</summary>
    private const long PlanId = 5430;

    /// <summary>The versioned base URL of the test configuration.</summary>
    private const string BaseUrl = "https://easyverein.com/api/v2.0";

    /// <summary>Creates a v2.0 configuration.</summary>
    /// <returns>The configuration.</returns>
    private static EasyVereinConfiguration CreateConfig() => new()
    {
        ApiKey = "test",
        ApiUrl = "https://easyverein.com/api",
        ApiVersion = "v2.0"
    };

    /// <summary>Creates a plan with the given billing-account IDs.</summary>
    /// <param name="ids">The enabled billing-account IDs.</param>
    /// <returns>The plan.</returns>
    private static AccountingPlan Plan(params long[] ids) => new()
    {
        Id = PlanId,
        BillingAccounts = ids.Select(id => new BillingAccount { Id = id, Number = (int)(id % 100000), Name = $"Konto {id}" }).ToList()
    };

    /// <summary>Creates a mock that returns <paramref name="plan"/> on GET and records the PATCH.</summary>
    /// <param name="plan">The plan returned by GET.</param>
    /// <param name="patch">Receives the PATCH body.</param>
    /// <returns>The mock.</returns>
    private static Mock<IEasyVereinApiClient> CreateMock(AccountingPlan? plan, Action<Dictionary<string, object>>? patch = null)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetAccountingPlanAsync(PlanId, It.IsAny<CancellationToken>())).ReturnsAsync(plan);
        mock.Setup(c => c.UpdateAccountingPlanAsync(PlanId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, p, _) => patch?.Invoke((Dictionary<string, object>)p))
            .ReturnsAsync(new AccountingPlan { Id = PlanId });
        return mock;
    }

    /// <summary>Verifies that the plan is listed with billing accounts including number and name.</summary>
    [Fact]
    public async Task ListAccountingPlans_WritesBillingAccountsWithNumberAndName()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListAccountingPlansAsync(It.IsAny<PageRequest?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PagedResult<AccountingPlan>.FromCompleteList(new[] { Plan(57780) }));

        var result = await new AccountingPlanTools(mock.Object, CreateConfig()).ListAccountingPlans();

        Assert.Contains("\"billingAccounts\":[{\"id\":57780", result);
        Assert.Contains("\"name\":\"Konto 57780\"", result);
        Assert.Contains("\"totalCount\":1", result);
    }

    /// <summary>Verifies that adding and removing accounts patches the resulting set as URL references.</summary>
    [Fact]
    public async Task UpdateAccountingPlan_AddAndRemove_PatchesResultingSet()
    {
        Dictionary<string, object>? patch = null;
        var mock = CreateMock(Plan(1, 2, 3), p => patch = p);

        var result = await new AccountingPlanTools(mock.Object, CreateConfig())
            .UpdateAccountingPlan(PlanId, addBillingAccountIds: new long[] { 4 }, removeBillingAccountIds: new long[] { 2 });

        Assert.DoesNotContain("ERROR", result);
        var urls = Assert.IsAssignableFrom<IEnumerable<string>>(patch!["billingAccounts"]);
        Assert.Equal(new[] { $"{BaseUrl}/billing-account/1", $"{BaseUrl}/billing-account/3", $"{BaseUrl}/billing-account/4" }, urls);
        mock.Verify(c => c.GetAccountingPlanAsync(PlanId, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    /// <summary>Verifies that a call without changes is rejected without API calls.</summary>
    [Fact]
    public async Task UpdateAccountingPlan_NothingGiven_ReturnsError()
    {
        var mock = CreateMock(Plan(1));

        var result = await new AccountingPlanTools(mock.Object, CreateConfig()).UpdateAccountingPlan(PlanId, null, null);

        Assert.StartsWith("ERROR", result);
        mock.Verify(c => c.UpdateAccountingPlanAsync(It.IsAny<long>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that an ID in both lists is rejected without API calls.</summary>
    [Fact]
    public async Task UpdateAccountingPlan_IdInBothLists_ReturnsError()
    {
        var mock = CreateMock(Plan(1));

        var result = await new AccountingPlanTools(mock.Object, CreateConfig())
            .UpdateAccountingPlan(PlanId, new long[] { 5 }, new long[] { 5 });

        Assert.StartsWith("ERROR", result);
        Assert.Contains("5", result);
        mock.Verify(c => c.GetAccountingPlanAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that no PATCH is sent when the set does not change.</summary>
    [Fact]
    public async Task UpdateAccountingPlan_NoEffectiveChange_DoesNotPatch()
    {
        var mock = CreateMock(Plan(1, 2));

        var result = await new AccountingPlanTools(mock.Object, CreateConfig())
            .UpdateAccountingPlan(PlanId, addBillingAccountIds: new long[] { 1 }, removeBillingAccountIds: new long[] { 9 });

        Assert.Contains("\"id\":5430", result);
        mock.Verify(c => c.UpdateAccountingPlanAsync(It.IsAny<long>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that an unknown plan gives a readable message.</summary>
    [Fact]
    public async Task UpdateAccountingPlan_PlanNotFound_ReturnsMessage()
    {
        var mock = CreateMock(null);

        var result = await new AccountingPlanTools(mock.Object, CreateConfig())
            .UpdateAccountingPlan(PlanId, new long[] { 1 }, null);

        Assert.Contains("not found", result);
    }
}
