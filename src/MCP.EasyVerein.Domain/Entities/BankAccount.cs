using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a bank account from the easyVerein API.
/// </summary>
public class BankAccount
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the bank account name. Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color value. Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Color)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label. Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the related billing account ID. Maps to API field '<c>billingAccount</c>'.</summary>
    [JsonPropertyName(BankAccountFields.BillingAccount)]
    public long? BillingAccount { get; set; }

    /// <summary>Gets or sets the account holder name. Maps to API field '<c>accountHolder</c>'.</summary>
    [JsonPropertyName(BankAccountFields.AccountHolder)]
    public string? AccountHolder { get; set; }

    /// <summary>Gets or sets the bank name. Maps to API field '<c>bankName</c>'.</summary>
    [JsonPropertyName(BankAccountFields.BankName)]
    public string? BankName { get; set; }

    /// <summary>Gets or sets the IBAN. Maps to API field '<c>IBAN</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Iban)]
    public string? Iban { get; set; }

    /// <summary>Gets or sets the BIC. Maps to API field '<c>BIC</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Bic)]
    public string? Bic { get; set; }

    /// <summary>Gets or sets the initial balance. Maps to API field '<c>startsaldo</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Startsaldo)]
    public decimal? Startsaldo { get; set; }

    /// <summary>Gets or sets the import balance. Maps to API field '<c>importSaldo</c>'.</summary>
    [JsonPropertyName(BankAccountFields.ImportSaldo)]
    public decimal? ImportSaldo { get; set; }

    /// <summary>Gets or sets the accounting sphere (SKR 42). Maps to API field '<c>sphere</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Sphere)]
    public int? Sphere { get; set; }

    /// <summary>Gets or sets whether to compute startsaldo on import. Maps to API field '<c>computeStartsaldoOnImport</c>'.</summary>
    [JsonPropertyName(BankAccountFields.ComputeStartsaldoOnImport)]
    public bool? ComputeStartsaldoOnImport { get; set; }

    /// <summary>Gets or sets the last online-banking import date. Maps to API field '<c>last_imported_date</c>'.</summary>
    [JsonPropertyName(BankAccountFields.LastImportedDate)]
    public DateTime? LastImportedDate { get; set; }
}
