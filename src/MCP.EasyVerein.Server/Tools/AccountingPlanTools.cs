using System.ComponentModel;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for the accounting plan (US-0035): read the enabled billing accounts and enable or
/// disable single accounts. easyVerein keeps exactly one plan per organization, so there are no
/// create or delete tools.
/// </summary>
/// <param name="client">The easyVerein API client.</param>
/// <param name="config">The configuration used to build billing-account URL references.</param>
[McpServerToolType]
public sealed class AccountingPlanTools(IEasyVereinApiClient client, EasyVereinConfiguration config)
{
    /// <summary>API field name of the enabled billing accounts in the PATCH body.</summary>
    private const string BillingAccountsField = "billingAccounts";

    /// <summary>Lists the accounting plans with their enabled billing accounts.</summary>
    /// <param name="limit">Entries per page (1-100).</param>
    /// <param name="page">Page number, starting at 1.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON page of accounting plans, or an error message.</returns>
    [McpServerTool(Name = "list_accounting_plans"), Description("List the accounting plan (Kontenplan) with all enabled billing accounts (id, number, name). A billing account can only be used for bookings and receipts if it is enabled here. Paged: limit (1-100, default 50) and page (from 1); the response holds items, totalCount and hasMore - request the next page while hasMore is true.")]
    public async Task<string> ListAccountingPlans(
        [Description("Entries per page (1-100, default 50)")] int limit = PageRequest.DefaultLimit,
        [Description("Page number, starting at 1 (default 1)")] int page = PageRequest.FirstPage,
        CancellationToken ct = default)
    {
        try
        {
            var plans = await client.ListAccountingPlansAsync(ToolJson.ToPageRequest(limit, page), ct);
            return ToolJson.SerializePage(plans);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves an accounting plan by its ID.</summary>
    /// <param name="id">The accounting plan ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The accounting plan as JSON, or a not-found or error message.</returns>
    [McpServerTool(Name = "get_accounting_plan"), Description("Retrieve the accounting plan (Kontenplan) by its ID, with all enabled billing accounts (id, number, name).")]
    public async Task<string> GetAccountingPlan(
        [Description("The ID of the accounting plan")] long id,
        CancellationToken ct = default)
    {
        try
        {
            var plan = await client.GetAccountingPlanAsync(id, ct);
            return plan != null ? ToolJson.Serialize(plan) : $"Accounting plan with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Enables and/or disables billing accounts in the accounting plan. Reads the current plan,
    /// applies the changes and sends the resulting set; without an effective change nothing is sent.
    /// </summary>
    /// <param name="id">The accounting plan ID.</param>
    /// <param name="addBillingAccountIds">Billing-account IDs to enable.</param>
    /// <param name="removeBillingAccountIds">Billing-account IDs to disable.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated plan as JSON, or a not-found or error message.</returns>
    [McpServerTool(Name = "update_accounting_plan"), Description("Enable (addBillingAccountIds) and/or disable (removeBillingAccountIds) billing accounts in the accounting plan (Kontenplan). Uses billing-account IDs, not account numbers. Changes the bookkeeping setup of the club: only call after the user explicitly asked for it.")]
    public async Task<string> UpdateAccountingPlan(
        [Description("The ID of the accounting plan")] long id,
        [Description("Billing-account IDs to enable (numeric, optional)")] long[]? addBillingAccountIds,
        [Description("Billing-account IDs to disable (numeric, optional)")] long[]? removeBillingAccountIds,
        CancellationToken ct = default)
    {
        try
        {
            var add = addBillingAccountIds ?? Array.Empty<long>();
            var remove = removeBillingAccountIds ?? Array.Empty<long>();
            if (add.Length == 0 && remove.Length == 0)
                return "ERROR: Nothing to change. Pass addBillingAccountIds and/or removeBillingAccountIds.";

            var conflicts = add.Intersect(remove).ToArray();
            if (conflicts.Length != 0)
                return $"ERROR: Billing-account IDs in both lists: {string.Join(", ", conflicts)}.";

            var plan = await client.GetAccountingPlanAsync(id, ct);
            if (plan is null)
                return $"Accounting plan with ID {id} not found.";

            var current = (plan.BillingAccounts ?? new()).Select(b => b.Id).ToHashSet();
            var target = current.Union(add).Except(remove).ToHashSet();
            if (target.SetEquals(current))
                return ToolJson.Serialize(plan);

            var patch = new Dictionary<string, object>
            {
                [BillingAccountsField] = target.Order()
                    .Select(b => $"{config.GetVersionedBaseUrl()}/billing-account/{b}")
                    .ToList()
            };
            await client.UpdateAccountingPlanAsync(id, patch, ct);

            var updated = await client.GetAccountingPlanAsync(id, ct);
            return updated != null ? ToolJson.Serialize(updated) : $"Accounting plan with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
