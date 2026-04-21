using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing billing accounts (Buchungskonten) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class BillingAccountTools(IEasyVereinApiClient client)
{
    /// <summary>Lists billing accounts with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_billing_accounts"), Description("List all billing accounts")]
    public async Task<string> ListBillingAccounts(
        [Description("Exact name filter")] string? name,
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("SKR chart filter (e.g. '42')")] string? skr,
        [Description("Comma-separated SKR charts filter")] string? skrIn,
        [Description("Number greater-or-equal filter")] string? numberGte,
        [Description("Number less-or-equal filter")] string? numberLte,
        [Description("Deleted flag filter ('true' or 'false')")] string? deleted,
        [Description("accountingPlan-is-null filter ('true' or 'false')")] string? accountingPlanIsNull,
        [Description("Show only own billing accounts ('true' or 'false')")] string? showOwnBillingAccounts,
        [Description("Ordering (e.g. 'number' or '-name')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var accounts = await client.ListBillingAccountsAsync(
                name, idIn, skr, skrIn, numberGte, numberLte,
                deleted, accountingPlanIsNull, showOwnBillingAccounts,
                ordering, search, ct);
            return JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single billing account by its unique identifier.</summary>
    [McpServerTool(Name = "get_billing_account"), Description("Retrieve a billing account by its ID")]
    public async Task<string> GetBillingAccount(
        [Description("The ID of the billing account")] long id,
        CancellationToken ct)
    {
        try
        {
            var account = await client.GetBillingAccountAsync(id, ct);
            return account != null
                ? JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true })
                : $"Billing account with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new billing account in easyVerein.</summary>
    [McpServerTool(Name = "create_billing_account"), Description("Create a new billing account")]
    public async Task<string> CreateBillingAccount(
        [Description("The billing account name (required)")] string name,
        [Description("SKR account number (integer)")] int? number,
        [Description("Default SKR 42 sphere (integer)")] int? defaultSphere,
        [Description("Exclude the account in EUR reporting")] bool? excludeInEur,
        CancellationToken ct)
    {
        try
        {
            var account = new BillingAccount { Name = name };
            if (number.HasValue) account.Number = number.Value;
            if (defaultSphere.HasValue) account.DefaultSphere = defaultSphere.Value;
            if (excludeInEur.HasValue) account.ExcludeInEur = excludeInEur.Value;

            var created = await client.CreateBillingAccountAsync(account, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing billing account (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_billing_account"), Description("Update a billing account (only provided fields are changed)")]
    public async Task<string> UpdateBillingAccount(
        [Description("The ID of the billing account to update")] long id,
        [Description("New name")] string? name,
        [Description("New SKR account number")] int? number,
        [Description("New default SKR 42 sphere")] int? defaultSphere,
        [Description("New exclude-in-EUR flag")] bool? excludeInEur,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[BillingAccountFields.Name] = name!;
            if (number.HasValue) patch[BillingAccountFields.Number] = number.Value;
            if (defaultSphere.HasValue) patch[BillingAccountFields.DefaultSphere] = defaultSphere.Value;
            if (excludeInEur.HasValue) patch[BillingAccountFields.ExcludeInEur] = excludeInEur.Value;

            var updated = await client.UpdateBillingAccountAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a billing account by its unique identifier.</summary>
    [McpServerTool(Name = "delete_billing_account"), Description("Delete a billing account. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteBillingAccount(
        [Description("The ID of the billing account to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteBillingAccountAsync(id, ct);
            return $"Billing account with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
