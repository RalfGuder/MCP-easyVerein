using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="CustomTaxRateTools"/> MCP tool wrapper.</summary>
public class CustomTaxRateToolsTests
{
    /// <summary>
    /// Verifies that <c>create_custom_tax_rate</c> maps name and rate onto the entity.
    /// </summary>
    [Fact]
    public async Task CreateCustomTaxRate_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        CustomTaxRate? captured = null;
        mock.Setup(c => c.CreateCustomTaxRateAsync(It.IsAny<CustomTaxRate>(), It.IsAny<CancellationToken>()))
            .Callback<CustomTaxRate, CancellationToken>((r, _) => captured = r)
            .ReturnsAsync(new CustomTaxRate { Id = 1, TaxName = "Ermäßigt", CustomTaxRateValue = 7m });

        var tools = new CustomTaxRateTools(mock.Object);

        var result = await tools.CreateCustomTaxRate("Ermäßigt", 7m, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Ermäßigt", captured!.TaxName);
        Assert.Equal(7m, captured.CustomTaxRateValue);
        Assert.Contains("\"id\": 1", result);
    }

    /// <summary>
    /// Verifies that a rate outside 0–100 is rejected before calling the API
    /// (the API only accepts values up to 100).
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(100.01)]
    public async Task CreateCustomTaxRate_WithRateOutOfRange_ReturnsErrorWithoutCallingClient(double rate)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new CustomTaxRateTools(mock.Object);

        var result = await tools.CreateCustomTaxRate("X", (decimal)rate, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that <c>update_custom_tax_rate</c> sends only the provided fields (PATCH semantics).
    /// </summary>
    [Fact]
    public async Task UpdateCustomTaxRate_SendsOnlyProvidedFields()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateCustomTaxRateAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new CustomTaxRate { Id = 5 });

        var tools = new CustomTaxRateTools(mock.Object);

        await tools.UpdateCustomTaxRate(5, taxName: "null", customTaxRate: 2.25m, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Single(patch);
        Assert.Equal(2.25m, patch["customTaxRate"]);
    }

    /// <summary>
    /// Verifies that an update with a rate outside 0–100 is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task UpdateCustomTaxRate_WithRateOutOfRange_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new CustomTaxRateTools(mock.Object);

        var result = await tools.UpdateCustomTaxRate(5, taxName: null, customTaxRate: 123.45m, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task DeleteCustomTaxRate_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteCustomTaxRateAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("403"));

        var tools = new CustomTaxRateTools(mock.Object);

        var result = await tools.DeleteCustomTaxRate(9, CancellationToken.None);

        Assert.StartsWith("ERROR: HttpRequestException", result);
    }
}
