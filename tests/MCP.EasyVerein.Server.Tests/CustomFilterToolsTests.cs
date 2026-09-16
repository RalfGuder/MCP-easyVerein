using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="CustomFilterTools"/> MCP tool wrapper.</summary>
public class CustomFilterToolsTests
{
    /// <summary>Filter rules used across the tests.</summary>
    private const string RulesJson = """{"condition":"AND","rules":[{"field":"date","operator":"greater_or_equal","value":"2024-01-01"}]}""";

    /// <summary>
    /// Verifies that <c>create_custom_filter</c> parses the rules JSON text into a JSON object
    /// and forwards name and model to the client.
    /// </summary>
    [Fact]
    public async Task CreateCustomFilter_ParsesRulesJson_AndPassesParameters()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        CustomFilter? captured = null;
        mock.Setup(c => c.CreateCustomFilterAsync(It.IsAny<CustomFilter>(), It.IsAny<CancellationToken>()))
            .Callback<CustomFilter, CancellationToken>((f, _) => captured = f)
            .ReturnsAsync(new CustomFilter { Id = 1, Name = "2024" });

        var tools = new CustomFilterTools(mock.Object);

        var result = await tools.CreateCustomFilter("2024", "bookingFilter", RulesJson, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("2024", captured!.Name);
        Assert.Equal("bookingFilter", captured.Model);
        Assert.Equal(JsonValueKind.Object, captured.Rules!.Value.ValueKind);
        Assert.Equal("AND", captured.Rules.Value.GetProperty("condition").GetString());
        Assert.Contains("2024", result);
    }

    /// <summary>
    /// Verifies that invalid rules JSON is reported as an error without calling the API.
    /// </summary>
    [Fact]
    public async Task CreateCustomFilter_WithInvalidRulesJson_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new CustomFilterTools(mock.Object);

        var result = await tools.CreateCustomFilter("X", "userFilter", "{not json", CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that rules which are not a JSON object (e.g. a plain string) are rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task CreateCustomFilter_WithNonObjectRules_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new CustomFilterTools(mock.Object);

        var result = await tools.CreateCustomFilter("X", "userFilter", "\"text\"", CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that updating only the name sends only the name and does not fetch the filter first.
    /// </summary>
    [Fact]
    public async Task UpdateCustomFilter_NameOnly_SendsOnlyName()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateCustomFilterAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new CustomFilter { Id = 5, Name = "Neu" });

        var tools = new CustomFilterTools(mock.Object);

        await tools.UpdateCustomFilter(5, name: "Neu", model: "null", rules: null, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Single(patch);
        Assert.Equal("Neu", patch["name"]);
        mock.Verify(c => c.GetCustomFilterAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies that updating rules without a model looks up the current model and sends it along,
    /// because the easyVerein API rejects a rules PATCH without the model ("Ungültiger Wert: rules").
    /// </summary>
    [Fact]
    public async Task UpdateCustomFilter_RulesWithoutModel_AddsCurrentModel()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetCustomFilterAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomFilter { Id = 5, Model = "bookingFilter" });
        object? captured = null;
        mock.Setup(c => c.UpdateCustomFilterAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new CustomFilter { Id = 5 });

        var tools = new CustomFilterTools(mock.Object);

        await tools.UpdateCustomFilter(5, name: null, model: null, rules: RulesJson, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Equal("bookingFilter", patch["model"]);
        var rules = Assert.IsType<JsonElement>(patch["rules"]);
        Assert.Equal("AND", rules.GetProperty("condition").GetString());
    }

    /// <summary>
    /// Verifies that updating rules together with an explicit model does not fetch the filter.
    /// </summary>
    [Fact]
    public async Task UpdateCustomFilter_RulesWithModel_DoesNotFetchFilter()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateCustomFilterAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new CustomFilter { Id = 5 });

        var tools = new CustomFilterTools(mock.Object);

        await tools.UpdateCustomFilter(5, name: null, model: "invoiceFilter", rules: RulesJson, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Equal("invoiceFilter", patch["model"]);
        Assert.True(patch.ContainsKey("rules"));
        mock.Verify(c => c.GetCustomFilterAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies that updating rules of a non-existent filter reports an error instead of sending a PATCH.
    /// </summary>
    [Fact]
    public async Task UpdateCustomFilter_RulesForMissingFilter_ReturnsError()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetCustomFilterAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomFilter?)null);

        var tools = new CustomFilterTools(mock.Object);

        var result = await tools.UpdateCustomFilter(5, name: null, model: null, rules: RulesJson, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        mock.Verify(c => c.UpdateCustomFilterAsync(It.IsAny<long>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task ListCustomFilters_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListCustomFiltersAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("401"));

        var tools = new CustomFilterTools(mock.Object);

        var result = await tools.ListCustomFilters(null, null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR: UnauthorizedAccessException", result);
    }
}
