using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using ModelContextProtocol.Server;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="FeatureRequestTools"/> MCP tool wrapper.</summary>
public class FeatureRequestToolsTests
{
    /// <summary>
    /// Verifies that <c>create_feature_request</c> maps title, description and category onto the entity.
    /// </summary>
    [Fact]
    public async Task CreateFeatureRequest_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        FeatureRequest? captured = null;
        mock.Setup(c => c.CreateFeatureRequestAsync(It.IsAny<FeatureRequest>(), It.IsAny<CancellationToken>()))
            .Callback<FeatureRequest, CancellationToken>((r, _) => captured = r)
            .ReturnsAsync(new FeatureRequest { Id = 6800, Title = "T" });

        var tools = new FeatureRequestTools(mock.Object);

        var result = await tools.CreateFeatureRequest("T", "D", 6, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("T", captured!.Title);
        Assert.Equal("D", captured.Description);
        Assert.Equal(6, captured.Category);
        Assert.Contains("6800", result);
    }

    /// <summary>
    /// Verifies that empty title or description is rejected before calling the API (both are required).
    /// </summary>
    [Theory]
    [InlineData("", "D")]
    [InlineData("T", " ")]
    public async Task CreateFeatureRequest_WithMissingRequiredField_ReturnsErrorWithoutCallingClient(string title, string description)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new FeatureRequestTools(mock.Object);

        var result = await tools.CreateFeatureRequest(title, description, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that <c>vote_feature_request</c> forwards the vote direction to the client.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task VoteFeatureRequest_ForwardsDirection(bool inFavor)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.VoteFeatureRequestAsync(7, inFavor, It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"ok\":true}");

        var tools = new FeatureRequestTools(mock.Object);

        var result = await tools.VoteFeatureRequest(7, inFavor, CancellationToken.None);

        Assert.Contains("7", result);
        mock.Verify(c => c.VoteFeatureRequestAsync(7, inFavor, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task VoteFeatureRequest_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.VoteFeatureRequestAsync(7, true, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("HTTP 400"));

        var tools = new FeatureRequestTools(mock.Object);

        var result = await tools.VoteFeatureRequest(7, true, CancellationToken.None);

        Assert.StartsWith("ERROR: HttpRequestException", result);
    }

    /// <summary>
    /// Verifies that the public-facing tools warn about their visibility in their description,
    /// and that no update or delete tool exists (the API rejects both with HTTP 405).
    /// </summary>
    [Fact]
    public void FeatureRequestTools_WarnAboutPublicEffect_AndOfferNoUpdateOrDelete()
    {
        var tools = typeof(FeatureRequestTools).GetMethods()
            .Select(m => new
            {
                Attr = m.GetCustomAttributes(typeof(McpServerToolAttribute), false).Cast<McpServerToolAttribute>().FirstOrDefault(),
                Description = m.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                    .Cast<System.ComponentModel.DescriptionAttribute>().FirstOrDefault()?.Description
            })
            .Where(t => t.Attr != null)
            .ToDictionary(t => t.Attr!.Name!, t => t.Description ?? string.Empty);

        Assert.Equal(
            new[] { "create_feature_request", "get_feature_request", "list_feature_requests", "vote_feature_request" },
            tools.Keys.OrderBy(k => k).ToArray());
        Assert.Contains("PUBLIC", tools["create_feature_request"]);
        Assert.Contains("PUBLIC", tools["vote_feature_request"]);
    }
}
