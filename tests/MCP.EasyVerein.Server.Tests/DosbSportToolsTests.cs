using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the read-only <see cref="DosbSportTools"/> MCP tool wrapper.</summary>
public class DosbSportToolsTests
{
    /// <summary>
    /// Verifies that <c>list_dosb_sports</c> forwards all filters to the client and serializes the result.
    /// </summary>
    [Fact]
    public async Task ListDosbSports_ForwardsFilters_AndSerializesResult()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.ListDosbSportsAsync("1,2", "Fußball", "042", "07", "title",
                It.Is<string[]?>(s => s != null && s[0] == "Ball"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DosbSport> { new() { Id = 55, Title = "Fußball" } });

        var tools = new DosbSportTools(mock.Object);

        var result = await tools.ListDosbSports("1,2", "Fußball", "042", "07", "title", new[] { "Ball" }, CancellationToken.None);

        Assert.Contains("\"id\": 55", result);
        Assert.Contains("Fu", result);
    }

    /// <summary>
    /// Verifies that <c>get_dosb_sport</c> reports a missing sport with a readable message.
    /// </summary>
    [Fact]
    public async Task GetDosbSport_WhenNotFound_ReturnsNotFoundMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetDosbSportAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DosbSport?)null);

        var tools = new DosbSportTools(mock.Object);

        var result = await tools.GetDosbSport(9, CancellationToken.None);

        Assert.Equal("DOSB sport with ID 9 not found.", result);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task GetDosbSport_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetDosbSportAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("401"));

        var tools = new DosbSportTools(mock.Object);

        var result = await tools.GetDosbSport(9, CancellationToken.None);

        Assert.StartsWith("ERROR: UnauthorizedAccessException", result);
    }

    /// <summary>
    /// Verifies that the tool class exposes no write operations, because the API only allows reading DOSB sports.
    /// </summary>
    [Fact]
    public void DosbSportTools_ExposesOnlyReadOperations()
    {
        var toolNames = typeof(DosbSportTools).GetMethods()
            .Select(m => m.GetCustomAttributes(typeof(ModelContextProtocol.Server.McpServerToolAttribute), false)
                .Cast<ModelContextProtocol.Server.McpServerToolAttribute>().FirstOrDefault()?.Name)
            .Where(n => n != null)
            .OrderBy(n => n)
            .ToArray();

        Assert.Equal(new[] { "get_dosb_sport", "list_dosb_sports" }, toolNames);
    }
}
