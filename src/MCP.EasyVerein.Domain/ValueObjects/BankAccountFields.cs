namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Bank Account API field names used in JSON serialization and query building.</summary>
internal static class BankAccountFields
{
    /// <summary>API field name for the unique bank account identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the bank account name.</summary>
    internal const string Name = "name";

    /// <summary>API field name for the hex color value.</summary>
    internal const string Color = "color";

    /// <summary>API field name for the short label.</summary>
    internal const string Short = "short";

    /// <summary>API field name for the related billing account.</summary>
    internal const string BillingAccount = "billingAccount";

    /// <summary>API field name for the account holder name.</summary>
    internal const string AccountHolder = "accountHolder";

    /// <summary>API field name for the bank name.</summary>
    internal const string BankName = "bankName";

    /// <summary>API field name for the IBAN.</summary>
    internal const string Iban = "IBAN";

    /// <summary>API field name for the BIC.</summary>
    internal const string Bic = "BIC";

    /// <summary>API field name for the initial balance (Startsaldo).</summary>
    internal const string Startsaldo = "startsaldo";

    /// <summary>API field name for the import balance (Importsaldo).</summary>
    internal const string ImportSaldo = "importSaldo";

    /// <summary>API field name for the accounting sphere (SKR 42).</summary>
    internal const string Sphere = "sphere";

    /// <summary>API field name for the compute-startsaldo-on-import flag.</summary>
    internal const string ComputeStartsaldoOnImport = "computeStartsaldoOnImport";

    /// <summary>API field name for the last online banking import date.</summary>
    internal const string LastImportedDate = "last_imported_date";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
