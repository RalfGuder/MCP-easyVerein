using System.Text.Json;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceChargesTests
{
    [Fact]
    public void Deserialize_MapsAllThreeFields()
    {
        var json = """{"charge":1.23,"chargeBack":0.50,"total":18.88}""";
        var options = new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };
        var c = JsonSerializer.Deserialize<InvoiceCharges>(json, options);
        Assert.NotNull(c);
        Assert.Equal(1.23m, c!.Charge);
        Assert.Equal(0.50m, c.ChargeBack);
        Assert.Equal(18.88m, c.Total);
    }
}
