using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the bank-account API endpoint with field selection and filters.
/// </summary>
internal class BankAccountQuery
{
    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional IBAN filter (exact match).</summary>
    internal string? Iban { get; set; }

    /// <summary>Gets or sets an optional BIC filter (exact match).</summary>
    internal string? Bic { get; set; }

    /// <summary>Gets or sets an optional account holder filter (exact match).</summary>
    internal string? AccountHolder { get; set; }

    /// <summary>Gets or sets an optional bank name filter (exact match).</summary>
    internal string? BankName { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all bank-account fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            BankAccountFields.Id + "," +
            BankAccountFields.Name + "," +
            BankAccountFields.Color + "," +
            BankAccountFields.Short + "," +
            BankAccountFields.BillingAccount + "," +
            BankAccountFields.AccountHolder + "," +
            BankAccountFields.BankName + "," +
            BankAccountFields.Iban + "," +
            BankAccountFields.Bic + "," +
            BankAccountFields.Startsaldo + "," +
            BankAccountFields.ImportSaldo + "," +
            BankAccountFields.Sphere + "," +
            BankAccountFields.ComputeStartsaldoOnImport + "," +
            BankAccountFields.LastImportedDate +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{BankAccountFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Iban))
            parts.Add($"{BankAccountFields.Iban}={Uri.EscapeDataString(Iban)}");
        if (!string.IsNullOrEmpty(Bic))
            parts.Add($"{BankAccountFields.Bic}={Uri.EscapeDataString(Bic)}");
        if (!string.IsNullOrEmpty(AccountHolder))
            parts.Add($"{BankAccountFields.AccountHolder}={Uri.EscapeDataString(AccountHolder)}");
        if (!string.IsNullOrEmpty(BankName))
            parts.Add($"{BankAccountFields.BankName}={Uri.EscapeDataString(BankName)}");
        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{BankAccountFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{BankAccountFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{BankAccountFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
