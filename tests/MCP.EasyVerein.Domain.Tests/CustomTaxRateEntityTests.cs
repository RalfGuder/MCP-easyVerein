using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class CustomTaxRateEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 1962,
                "taxName": "Ermäßigt",
                "customTaxRate": "7.00",
                "countryCode": "DE",
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "created_at": "2026-09-16T16:57:38.716264+02:00",
                "updated_at": "2026-09-16T16:57:42.856086+02:00"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var rate = JsonSerializer.Deserialize<CustomTaxRate>(json, options);

        Assert.NotNull(rate);
        Assert.Equal(1962L, rate.Id);
        Assert.Equal("Ermäßigt", rate.TaxName);
        Assert.Equal(7.00m, rate.CustomTaxRateValue);
        Assert.Equal("DE", rate.CountryCode);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", rate.Org);
        Assert.NotNull(rate.CreatedAt);
        Assert.NotNull(rate.UpdatedAt);
    }

    [Fact]
    public void JsonPropertyNames_WithNumericRateAndNullOrg_AreCorrect()
    {
        var json = """
            {
                "id": 1272,
                "taxName": "",
                "customTaxRate": 19,
                "countryCode": "DE",
                "org": null
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var rate = JsonSerializer.Deserialize<CustomTaxRate>(json, options);

        Assert.NotNull(rate);
        Assert.Equal(19m, rate.CustomTaxRateValue);
        Assert.Null(rate.Org);
        Assert.Null(rate.CreatedAt);
    }

    [Fact]
    public void Serialize_WritesRateAsNumber()
    {
        var rate = new CustomTaxRate { TaxName = "Test", CustomTaxRateValue = 2.25m };

        var json = JsonSerializer.Serialize(rate);

        Assert.Contains("\"customTaxRate\":2.25", json);
        Assert.Contains("\"taxName\":\"Test\"", json);
    }
}
