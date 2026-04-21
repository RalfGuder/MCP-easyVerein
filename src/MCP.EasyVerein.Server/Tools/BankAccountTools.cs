using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing bank accounts via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class BankAccountTools(IEasyVereinApiClient client)
{
    /// <summary>Lists bank accounts with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_bank_accounts"), Description("List all bank accounts")]
    public async Task<string> ListBankAccounts(
        [Description("Exact name filter")] string? name,
        [Description("Exact IBAN filter")] string? iban,
        [Description("Exact BIC filter")] string? bic,
        [Description("Exact account-holder filter")] string? accountHolder,
        [Description("Exact bank-name filter")] string? bankName,
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Ordering (e.g. 'name' or '-startsaldo')")] string? ordering,
        [Description("Search terms (name, short, bankName, accountHolder)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var accounts = await client.ListBankAccountsAsync(
                name, iban, bic, accountHolder, bankName, idIn, ordering, search, ct);
            return JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single bank account by its unique identifier.</summary>
    [McpServerTool(Name = "get_bank_account"), Description("Retrieve a bank account by its ID")]
    public async Task<string> GetBankAccount(
        [Description("The ID of the bank account")] long id,
        CancellationToken ct)
    {
        try
        {
            var account = await client.GetBankAccountAsync(id, ct);
            return account != null
                ? JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true })
                : $"Bank account with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new bank account in easyVerein.</summary>
    [McpServerTool(Name = "create_bank_account"), Description("Create a new bank account")]
    public async Task<string> CreateBankAccount(
        [Description("The bank account name (required)")] string name,
        [Description("Hex color (max. 7 chars, e.g. #3366ff)")] string? color,
        [Description("Short label (max. 4 chars)")] string? @short,
        [Description("Related billing account ID")] long? billingAccount,
        [Description("Account holder name")] string? accountHolder,
        [Description("Bank name")] string? bankName,
        [Description("IBAN (max. 32 chars)")] string? iban,
        [Description("BIC (max. 11 chars)")] string? bic,
        [Description("Initial balance (decimal)")] decimal? startsaldo,
        [Description("Import balance (decimal)")] decimal? importSaldo,
        [Description("Accounting sphere (SKR 42, default 9)")] int? sphere,
        [Description("Compute startsaldo on import")] bool? computeStartsaldoOnImport,
        [Description("Last online-banking import date (ISO 8601)")] string? lastImportedDate,
        CancellationToken ct)
    {
        try
        {
            var account = new BankAccount { Name = name };

            if (HasValue(color)) account.Color = color;
            if (HasValue(@short)) account.Short = @short;
            if (billingAccount.HasValue) account.BillingAccount = billingAccount.Value;
            if (HasValue(accountHolder)) account.AccountHolder = accountHolder;
            if (HasValue(bankName)) account.BankName = bankName;
            if (HasValue(iban)) account.Iban = iban;
            if (HasValue(bic)) account.Bic = bic;
            if (startsaldo.HasValue) account.Startsaldo = startsaldo.Value;
            if (importSaldo.HasValue) account.ImportSaldo = importSaldo.Value;
            if (sphere.HasValue) account.Sphere = sphere.Value;
            if (computeStartsaldoOnImport.HasValue) account.ComputeStartsaldoOnImport = computeStartsaldoOnImport.Value;
            if (DateTime.TryParse(lastImportedDate, out var lid)) account.LastImportedDate = lid;

            var created = await client.CreateBankAccountAsync(account, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing bank account (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_bank_account"), Description("Update a bank account (only provided fields are changed)")]
    public async Task<string> UpdateBankAccount(
        [Description("The ID of the bank account to update")] long id,
        [Description("New name")] string? name,
        [Description("New hex color")] string? color,
        [Description("New short label")] string? @short,
        [Description("New related billing account ID")] long? billingAccount,
        [Description("New account holder")] string? accountHolder,
        [Description("New bank name")] string? bankName,
        [Description("New IBAN")] string? iban,
        [Description("New BIC")] string? bic,
        [Description("New initial balance")] decimal? startsaldo,
        [Description("New import balance")] decimal? importSaldo,
        [Description("New accounting sphere")] int? sphere,
        [Description("New compute-startsaldo-on-import flag")] bool? computeStartsaldoOnImport,
        [Description("New last import date (ISO 8601)")] string? lastImportedDate,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();

            if (HasValue(name)) patch[BankAccountFields.Name] = name!;
            if (HasValue(color)) patch[BankAccountFields.Color] = color!;
            if (HasValue(@short)) patch[BankAccountFields.Short] = @short!;
            if (billingAccount.HasValue) patch[BankAccountFields.BillingAccount] = billingAccount.Value;
            if (HasValue(accountHolder)) patch[BankAccountFields.AccountHolder] = accountHolder!;
            if (HasValue(bankName)) patch[BankAccountFields.BankName] = bankName!;
            if (HasValue(iban)) patch[BankAccountFields.Iban] = iban!;
            if (HasValue(bic)) patch[BankAccountFields.Bic] = bic!;
            if (startsaldo.HasValue) patch[BankAccountFields.Startsaldo] = startsaldo.Value;
            if (importSaldo.HasValue) patch[BankAccountFields.ImportSaldo] = importSaldo.Value;
            if (sphere.HasValue) patch[BankAccountFields.Sphere] = sphere.Value;
            if (computeStartsaldoOnImport.HasValue) patch[BankAccountFields.ComputeStartsaldoOnImport] = computeStartsaldoOnImport.Value;
            if (DateTime.TryParse(lastImportedDate, out var lid)) patch[BankAccountFields.LastImportedDate] = lid;

            var updated = await client.UpdateBankAccountAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a bank account by its unique identifier.</summary>
    [McpServerTool(Name = "delete_bank_account"), Description("Delete a bank account. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteBankAccount(
        [Description("The ID of the bank account to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteBankAccountAsync(id, ct);
            return $"Bank account with ID {id} has been deleted.";
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
