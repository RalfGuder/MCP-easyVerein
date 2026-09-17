using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="InventoryObjectTools"/> MCP tool wrapper.</summary>
public class InventoryObjectToolsTests
{
    /// <summary>
    /// Verifies that <c>list_inventory_objects</c> passes all filters to the client.
    /// </summary>
    [Fact]
    public async Task ListInventoryObjects_PassesFilters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListInventoryObjectsAsync(
                "1,2", "Zelt", "594", true, false, 10L, 11L, "12", "13", "lent", "-name",
                It.Is<string[]>(s => s.Length == 1 && s[0] == "Zelt"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InventoryObject> { new() { Id = 7, Name = "Zelt" } });

        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.ListInventoryObjects(
            "1,2", "Zelt", "594", true, false, 10, 11, "12", "13", "lent", "-name",
            new[] { "Zelt" }, CancellationToken.None);

        Assert.Contains("\"id\": 7", result);
    }

    /// <summary>
    /// Verifies that <c>create_inventory_object</c> maps all writable parameters onto the entity.
    /// </summary>
    [Fact]
    public async Task CreateInventoryObject_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        InventoryObject? captured = null;
        mock.Setup(c => c.CreateInventoryObjectAsync(It.IsAny<InventoryObject>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryObject, CancellationToken>((o, _) => captured = o)
            .ReturnsAsync(new InventoryObject { Id = 1, Name = "Beamer" });

        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.CreateInventoryObject(
            "Beamer", "B-01", "Epson", 2, 450.5m, "2025-03-21", "Lager", true, 4424352, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Beamer", captured!.Name);
        Assert.Equal("B-01", captured.Identifier);
        Assert.Equal("Epson", captured.Description);
        Assert.Equal(2, captured.Pieces);
        Assert.Equal(450.5m, captured.Price);
        Assert.Equal(new DateTime(2025, 3, 21), captured.PurchaseDate!.Value.Date);
        Assert.Equal("Lager", captured.LocationName);
        Assert.True(captured.LendingAvailable);
        Assert.Equal(4424352L, captured.LendingResponsibleId);
        Assert.Contains("\"id\": 1", result);
    }

    /// <summary>
    /// Verifies that the literal string "null" is treated as an absent value on create.
    /// </summary>
    [Fact]
    public async Task CreateInventoryObject_WithNullLiterals_LeavesFieldsUnset()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        InventoryObject? captured = null;
        mock.Setup(c => c.CreateInventoryObjectAsync(It.IsAny<InventoryObject>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryObject, CancellationToken>((o, _) => captured = o)
            .ReturnsAsync(new InventoryObject { Id = 1 });

        var tools = new InventoryObjectTools(mock.Object);

        await tools.CreateInventoryObject(
            "Beamer", "null", "null", 1, null, "null", "null", null, null, CancellationToken.None);

        Assert.Null(captured!.Identifier);
        Assert.Null(captured.Description);
        Assert.Null(captured.PurchaseDate);
        Assert.Null(captured.LocationName);
    }

    /// <summary>
    /// Verifies that text fields longer than 500 characters are rejected before calling the API.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task CreateInventoryObject_WithTooLongText_ReturnsErrorWithoutCallingClient(int field)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectTools(mock.Object);
        var tooLong = new string('x', 501);

        var result = await tools.CreateInventoryObject(
            field == 0 ? tooLong : "Beamer",
            field == 1 ? tooLong : null,
            null, 1, null, null,
            field == 2 ? tooLong : null,
            null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        Assert.Contains("500", result);
    }

    /// <summary>
    /// Verifies that a missing piece count (required by the API, despite OPTIONS) is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task CreateInventoryObject_WithoutPieces_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.CreateInventoryObject(
            "Beamer", null, null, null, null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        Assert.Contains("pieces", result);
    }

    /// <summary>
    /// Verifies that an unparsable purchase date is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task CreateInventoryObject_WithInvalidPurchaseDate_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.CreateInventoryObject(
            "Beamer", null, null, 1, null, "gestern", null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
        Assert.Contains("purchase date", result);
    }

    /// <summary>
    /// Verifies that <c>update_inventory_object</c> sends only the provided fields (PATCH semantics).
    /// </summary>
    [Fact]
    public async Task UpdateInventoryObject_SendsOnlyProvidedFields()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateInventoryObjectAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new InventoryObject { Id = 5 });

        var tools = new InventoryObjectTools(mock.Object);

        await tools.UpdateInventoryObject(5, name: "null", identifier: null, description: "Neu",
            pieces: 3, price: 12.5m, purchaseDate: "2025-04-01", locationName: null,
            lendingAvailable: false, lendingResponsible: 99, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Equal(6, patch.Count);
        Assert.Equal("Neu", patch["description"]);
        Assert.Equal(3, patch["pieces"]);
        Assert.Equal(12.5m, patch["price"]);
        Assert.Equal("2025-04-01", patch["purchaseDate"]);
        Assert.Equal(false, patch["lendingAvailable"]);
        Assert.Equal(99L, patch["lendingResponsible"]);
    }

    /// <summary>
    /// Verifies that an update with a too-long name is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task UpdateInventoryObject_WithTooLongName_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.UpdateInventoryObject(
            5, new string('x', 501), null, null, null, null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that an update with an unparsable purchase date is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task UpdateInventoryObject_WithInvalidPurchaseDate_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.UpdateInventoryObject(
            5, null, null, null, null, null, "31.02.2025", null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that <c>get_inventory_object</c> reports a missing object instead of returning JSON.
    /// </summary>
    [Fact]
    public async Task GetInventoryObject_WhenNotFound_ReturnsNotFoundMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetInventoryObjectAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryObject?)null);

        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.GetInventoryObject(8, CancellationToken.None);

        Assert.Equal("Inventory object with ID 8 not found.", result);
    }

    /// <summary>
    /// Verifies that the delete confirmation mentions the wastebasket.
    /// </summary>
    [Fact]
    public async Task DeleteInventoryObject_ReturnsWastebasketHint()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteInventoryObjectAsync(9, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.DeleteInventoryObject(9, CancellationToken.None);

        Assert.Contains("ID 9", result);
        Assert.Contains("wastebasket", result);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task DeleteInventoryObject_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteInventoryObjectAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("403"));

        var tools = new InventoryObjectTools(mock.Object);

        var result = await tools.DeleteInventoryObject(9, CancellationToken.None);

        Assert.StartsWith("ERROR: HttpRequestException", result);
    }
}
