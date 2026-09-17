using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="InventoryObjectGroupTools"/> MCP tool wrapper.</summary>
public class InventoryObjectGroupToolsTests
{
    /// <summary>
    /// Verifies that <c>list_inventory_object_groups</c> passes all filters to the client.
    /// </summary>
    [Fact]
    public async Task ListInventoryObjectGroups_PassesFilters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListInventoryObjectGroupsAsync(
                "1,2", "Zelte", "#ff8800", "ZLT", false, "-name",
                It.Is<string[]>(s => s.Length == 1 && s[0] == "Zelt"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InventoryObjectGroup> { new() { Id = 7, Name = "Zelte" } });

        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.ListInventoryObjectGroups(
            "1,2", "Zelte", "#ff8800", "ZLT", false, "-name", new[] { "Zelt" }, CancellationToken.None);

        Assert.Contains("\"id\": 7", result);
    }

    /// <summary>
    /// Verifies that <c>create_inventory_object_group</c> maps all parameters onto the entity.
    /// </summary>
    [Fact]
    public async Task CreateInventoryObjectGroup_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        InventoryObjectGroup? captured = null;
        mock.Setup(c => c.CreateInventoryObjectGroupAsync(It.IsAny<InventoryObjectGroup>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryObjectGroup, CancellationToken>((g, _) => captured = g)
            .ReturnsAsync(new InventoryObjectGroup { Id = 1, Name = "Zelte" });

        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.CreateInventoryObjectGroup("Zelte", "#ff8800", "ZLT", CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Zelte", captured!.Name);
        Assert.Equal("#ff8800", captured.Color);
        Assert.Equal("ZLT", captured.Short);
        Assert.Contains("\"id\": 1", result);
    }

    /// <summary>
    /// Verifies that a missing required field (name, color or short) is rejected before calling the API.
    /// </summary>
    [Theory]
    [InlineData(null, "#ff8800", "ZLT", "name")]
    [InlineData("  ", "#ff8800", "ZLT", "name")]
    [InlineData("Zelte", null, "ZLT", "color")]
    [InlineData("Zelte", "null", "ZLT", "color")]
    [InlineData("Zelte", "#ff8800", "", "short")]
    public async Task CreateInventoryObjectGroup_WithoutRequiredField_ReturnsErrorWithoutCallingClient(
        string? name, string? color, string? @short, string expectedField)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.CreateInventoryObjectGroup(name, color, @short, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        Assert.Contains(expectedField, result);
    }

    /// <summary>
    /// Verifies that values exceeding the API length limits (200/7/4) are rejected before calling the API.
    /// </summary>
    [Theory]
    [InlineData(201, 7, 4, "200")]
    [InlineData(10, 8, 4, "7")]
    [InlineData(10, 7, 5, "4")]
    public async Task CreateInventoryObjectGroup_WithTooLongValue_ReturnsErrorWithoutCallingClient(
        int nameLength, int colorLength, int shortLength, string expectedLimit)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.CreateInventoryObjectGroup(
            new string('n', nameLength), "#" + new string('a', colorLength - 1), new string('s', shortLength),
            CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        Assert.Contains(expectedLimit, result);
    }

    /// <summary>
    /// Verifies that <c>update_inventory_object_group</c> sends only the provided fields (PATCH semantics).
    /// </summary>
    [Fact]
    public async Task UpdateInventoryObjectGroup_SendsOnlyProvidedFields()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateInventoryObjectGroupAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new InventoryObjectGroup { Id = 5 });

        var tools = new InventoryObjectGroupTools(mock.Object);

        await tools.UpdateInventoryObjectGroup(5, name: "null", color: "#00ff00", @short: "NEU", CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Equal(2, patch.Count);
        Assert.Equal("#00ff00", patch["color"]);
        Assert.Equal("NEU", patch["short"]);
    }

    /// <summary>
    /// Verifies that an update with a too-long short label is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task UpdateInventoryObjectGroup_WithTooLongShort_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.UpdateInventoryObjectGroup(5, null, null, "ZUVIEL", CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        Assert.Contains("4", result);
    }

    /// <summary>
    /// Verifies that <c>get_inventory_object_group</c> reports a missing group instead of returning JSON.
    /// </summary>
    [Fact]
    public async Task GetInventoryObjectGroup_WhenNotFound_ReturnsNotFoundMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetInventoryObjectGroupAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryObjectGroup?)null);

        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.GetInventoryObjectGroup(8, CancellationToken.None);

        Assert.Equal("Inventory object group with ID 8 not found.", result);
    }

    /// <summary>
    /// Verifies that the delete confirmation mentions the wastebasket.
    /// </summary>
    [Fact]
    public async Task DeleteInventoryObjectGroup_ReturnsWastebasketHint()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteInventoryObjectGroupAsync(9, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.DeleteInventoryObjectGroup(9, CancellationToken.None);

        Assert.Contains("ID 9", result);
        Assert.Contains("wastebasket", result);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task DeleteInventoryObjectGroup_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteInventoryObjectGroupAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("403"));

        var tools = new InventoryObjectGroupTools(mock.Object);

        var result = await tools.DeleteInventoryObjectGroup(9, CancellationToken.None);

        Assert.StartsWith("ERROR: HttpRequestException", result);
    }
}
