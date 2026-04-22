using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class BillingAccountEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "name": "Buchungskonto 1200",
                "number": 1200,
                "defaultSphere": 9,
                "excludeInEur": false,
                "skr": "42",
                "deleted": false,
                "linkedBookings": 15
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var account = JsonSerializer.Deserialize<BillingAccount>(json, options);

        Assert.NotNull(account);
        Assert.Equal(42L, account.Id);
        Assert.Equal("Buchungskonto 1200", account.Name);
        Assert.Equal(1200, account.Number);
        Assert.Equal(9, account.DefaultSphere);
        Assert.False(account.ExcludeInEur);
        Assert.Equal("42", account.Skr);
        Assert.False(account.Deleted);
        Assert.Equal(15, account.LinkedBookings);
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
        var account = JsonSerializer.Deserialize<BillingAccount>(json, options);

        Assert.NotNull(account);
        Assert.Equal(99L, account.Id);
        Assert.Equal("Minimal", account.Name);
        Assert.Null(account.Number);
        Assert.Null(account.DefaultSphere);
        Assert.Null(account.ExcludeInEur);
        Assert.Null(account.Skr);
        Assert.Null(account.Deleted);
        Assert.Null(account.LinkedBookings);
    }
}
