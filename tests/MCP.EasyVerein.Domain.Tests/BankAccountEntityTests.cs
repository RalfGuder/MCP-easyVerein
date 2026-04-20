using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class BankAccountEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "name": "Vereinskonto",
                "color": "#3366ff",
                "short": "VK",
                "billingAccount": 1200,
                "accountHolder": "TSV Musterhausen e.V.",
                "bankName": "Sparkasse Musterhausen",
                "IBAN": "DE89370400440532013000",
                "BIC": "COBADEFFXXX",
                "startsaldo": 1250.50,
                "importSaldo": 980.25,
                "sphere": 9,
                "computeStartsaldoOnImport": true,
                "last_imported_date": "2026-04-15T00:00:00"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var account = JsonSerializer.Deserialize<BankAccount>(json, options);

        Assert.NotNull(account);
        Assert.Equal(42L, account.Id);
        Assert.Equal("Vereinskonto", account.Name);
        Assert.Equal("#3366ff", account.Color);
        Assert.Equal("VK", account.Short);
        Assert.Equal(1200L, account.BillingAccount);
        Assert.Equal("TSV Musterhausen e.V.", account.AccountHolder);
        Assert.Equal("Sparkasse Musterhausen", account.BankName);
        Assert.Equal("DE89370400440532013000", account.Iban);
        Assert.Equal("COBADEFFXXX", account.Bic);
        Assert.Equal(1250.50m, account.Startsaldo);
        Assert.Equal(980.25m, account.ImportSaldo);
        Assert.Equal(9, account.Sphere);
        Assert.True(account.ComputeStartsaldoOnImport);
        Assert.Equal(new DateTime(2026, 4, 15), account.LastImportedDate);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "name": "Minimal"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var account = JsonSerializer.Deserialize<BankAccount>(json, options);

        Assert.NotNull(account);
        Assert.Equal(99L, account.Id);
        Assert.Equal("Minimal", account.Name);
        Assert.Null(account.Iban);
        Assert.Null(account.Bic);
        Assert.Null(account.Startsaldo);
        Assert.Null(account.LastImportedDate);
    }
}
