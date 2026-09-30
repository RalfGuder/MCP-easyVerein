using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="LendingTools"/> MCP tool wrapper.</summary>
public class LendingToolsTests
{
    /// <summary>Verifies that <c>list_lendings</c> passes all filters to the client.</summary>
    [Fact]
    public async Task ListLendings_PassesFilters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListLendingsAsync(
                "1,2", 335646309L, 11L, 4424352L, 12L, 335646225L, 13L,
                "lent", "returned",
                "2025-10-27", "2025-10-01", "2025-10-31",
                "2025-11-03", "2025-11-01", "2025-11-30",
                1, 0, 5, true, false, "-borrowingDate",
                It.Is<string[]>(s => s.Length == 1 && s[0] == "Zelt"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Lending> { new() { Id = 37839, State = "lent" } });

        var tools = new LendingTools(mock.Object);

        var result = await tools.ListLendings(
            "1,2", 335646309L, 11L, 4424352L, 12L, 335646225L, 13L,
            "lent", "returned",
            "2025-10-27", "2025-10-01", "2025-10-31",
            "2025-11-03", "2025-11-01", "2025-11-30",
            1, 0, 5, true, false, "-borrowingDate", new[] { "Zelt" }, CancellationToken.None);

        Assert.Contains("\"id\": 37839", result);
    }

    /// <summary>Verifies that <c>get_lending</c> returns a readable message when nothing is found.</summary>
    [Fact]
    public async Task GetLending_WhenNotFound_ReturnsMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetLendingAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lending?)null);

        var tools = new LendingTools(mock.Object);

        var result = await tools.GetLending(999, CancellationToken.None);

        Assert.Contains("999", result);
        Assert.Contains("not found", result);
    }

    /// <summary>Verifies that <c>create_lending</c> maps all parameters onto the entity.</summary>
    [Fact]
    public async Task CreateLending_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Lending? captured = null;
        mock.Setup(c => c.CreateLendingAsync(It.IsAny<Lending>(), It.IsAny<CancellationToken>()))
            .Callback<Lending, CancellationToken>((l, _) => captured = l)
            .ReturnsAsync(new Lending { Id = 555 });

        var tools = new LendingTools(mock.Object);

        var result = await tools.CreateLending(
            335646309, 335646225, "2025-10-27", "2025-11-03", 2, "14:30", "09:15:00", "lent",
            CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(335646309L, captured!.ParentInventoryObjectId);
        Assert.Equal(335646225L, captured.BorrowAddressId);
        Assert.Equal(new DateTime(2025, 10, 27), captured.BorrowingDate);
        Assert.Equal(new DateTime(2025, 11, 3), captured.ReturnDate);
        Assert.Equal(2, captured.Quantity);
        Assert.Equal("14:30", captured.BorrowTime);
        Assert.Equal("09:15:00", captured.ReturnTime);
        Assert.Equal("lent", captured.State);
        Assert.Contains("\"id\": 555", result);
    }

    /// <summary>
    /// Verifies that each field the API requires is checked before the request is sent.
    /// The API rejects a create without them with HTTP 400 (verified live on 2026-09-22).
    /// </summary>
    [Theory]
    [InlineData(null, 335646225L, "2025-10-27", 1, "parentInventoryObject")]
    [InlineData(335646309L, null, "2025-10-27", 1, "borrowAddress")]
    [InlineData(335646309L, 335646225L, null, 1, "borrowingDate")]
    [InlineData(335646309L, 335646225L, "2025-10-27", null, "quantity")]
    public async Task CreateLending_WithoutRequiredField_ReturnsError_WithoutCallingApi(
        long? parentInventoryObject, long? borrowAddress, string? borrowingDate, int? quantity, string expectedField)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var tools = new LendingTools(mock.Object);

        var result = await tools.CreateLending(
            parentInventoryObject, borrowAddress, borrowingDate, null, quantity, null, null, "lent",
            CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains(expectedField, result);
        mock.Verify(c => c.CreateLendingAsync(It.IsAny<Lending>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that an unknown lending state is rejected before the API is called.</summary>
    [Theory]
    [InlineData("Lent")]
    [InlineData("borrowed")]
    [InlineData("RETURNED")]
    public async Task CreateLending_WithInvalidState_ReturnsError_WithoutCallingApi(string state)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var tools = new LendingTools(mock.Object);

        var result = await tools.CreateLending(
            335646309, null, null, null, 1, null, null, state, CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("lent", result);
        Assert.Contains("inquiry", result);
        Assert.Contains("returned", result);
        mock.Verify(c => c.CreateLendingAsync(It.IsAny<Lending>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that a malformed date is rejected before the API is called.</summary>
    [Fact]
    public async Task CreateLending_WithInvalidDate_ReturnsError_WithoutCallingApi()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var tools = new LendingTools(mock.Object);

        var result = await tools.CreateLending(
            335646309, null, "27.13.2025", null, 1, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("27.13.2025", result);
        mock.Verify(c => c.CreateLendingAsync(It.IsAny<Lending>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that a malformed time is rejected before the API is called.</summary>
    [Fact]
    public async Task CreateLending_WithInvalidTime_ReturnsError_WithoutCallingApi()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var tools = new LendingTools(mock.Object);

        var result = await tools.CreateLending(
            335646309, null, null, null, 1, "25:99", null, null, CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("25:99", result);
        mock.Verify(c => c.CreateLendingAsync(It.IsAny<Lending>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that a non-positive quantity is rejected before the API is called.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateLending_WithNonPositiveQuantity_ReturnsError_WithoutCallingApi(int quantity)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var tools = new LendingTools(mock.Object);

        var result = await tools.CreateLending(
            335646309, null, null, null, quantity, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("quantity", result);
        mock.Verify(c => c.CreateLendingAsync(It.IsAny<Lending>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that <c>update_lending</c> only sends the fields that were supplied.</summary>
    [Fact]
    public async Task UpdateLending_SendsOnlyProvidedFields()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Dictionary<string, object>? captured = null;
        mock.Setup(c => c.UpdateLendingAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = (Dictionary<string, object>)patch)
            .ReturnsAsync(new Lending { Id = 5, State = "returned" });

        var tools = new LendingTools(mock.Object);

        await tools.UpdateLending(5, null, null, null, "2025-11-03", null, null, "10:00", "returned",
            CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(3, captured!.Count);
        Assert.Equal("2025-11-03", captured["returnDate"]);
        Assert.Equal("10:00", captured["returnTime"]);
        Assert.Equal("returned", captured["state"]);
        Assert.False(captured.ContainsKey("quantity"));
        Assert.False(captured.ContainsKey("borrowingDate"));
    }

    /// <summary>Verifies that an unknown lending state is rejected on update as well.</summary>
    [Fact]
    public async Task UpdateLending_WithInvalidState_ReturnsError_WithoutCallingApi()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var tools = new LendingTools(mock.Object);

        var result = await tools.UpdateLending(5, null, null, null, null, null, null, null, "zurueck",
            CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        mock.Verify(c => c.UpdateLendingAsync(It.IsAny<long>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>Verifies that <c>delete_lending</c> reports the move to the wastebasket.</summary>
    [Fact]
    public async Task DeleteLending_ReturnsWastebasketMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteLendingAsync(42, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var tools = new LendingTools(mock.Object);

        var result = await tools.DeleteLending(42, CancellationToken.None);

        Assert.Contains("42", result);
        Assert.Contains("wastebasket", result);
    }

    /// <summary>Verifies that API failures are reported as an ERROR string instead of throwing.</summary>
    [Fact]
    public async Task ListLendings_WhenClientThrows_ReturnsErrorString()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListLendingsAsync(
                It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>(),
                It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("bad token"));

        var tools = new LendingTools(mock.Object);

        var result = await tools.ListLendings(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("bad token", result);
    }
}
