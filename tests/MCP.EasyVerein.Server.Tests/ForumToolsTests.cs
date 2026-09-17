using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="ForumTools"/> MCP tool wrapper.</summary>
public class ForumToolsTests
{
    /// <summary>
    /// Verifies that <c>create_forum</c> maps all writable parameters onto the entity.
    /// </summary>
    [Fact]
    public async Task CreateForum_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Forum? captured = null;
        mock.Setup(c => c.CreateForumAsync(It.IsAny<Forum>(), It.IsAny<CancellationToken>()))
            .Callback<Forum, CancellationToken>((f, _) => captured = f)
            .ReturnsAsync(new Forum { Id = 1, Name = "Vorstand" });

        var tools = new ForumTools(mock.Object);

        var result = await tools.CreateForum(
            "Vorstand", "Interne Abstimmung", "https://example.org", true, 3, false, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Vorstand", captured!.Name);
        Assert.Equal("Interne Abstimmung", captured.Description);
        Assert.Equal("https://example.org", captured.Link);
        Assert.True(captured.LinkRedirects);
        Assert.Equal(3, captured.Order);
        Assert.False(captured.DisplaySubForumList);
        Assert.Contains("\"id\": 1", result);
    }

    /// <summary>
    /// Verifies that a missing name (required by the API) is rejected before calling the API.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("null")]
    public async Task CreateForum_WithoutName_ReturnsErrorWithoutCallingClient(string name)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new ForumTools(mock.Object);

        var result = await tools.CreateForum(name, null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that a name longer than 100 characters is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task CreateForum_WithTooLongName_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new ForumTools(mock.Object);

        var result = await tools.CreateForum(new string('x', 101), null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that <c>update_forum</c> sends only the provided fields (PATCH semantics).
    /// </summary>
    [Fact]
    public async Task UpdateForum_SendsOnlyProvidedFields()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateForumAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new Forum { Id = 5 });

        var tools = new ForumTools(mock.Object);

        await tools.UpdateForum(5, name: "null", description: "Neu", link: null,
            linkRedirects: null, order: 7, displaySubForumList: true, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Equal(3, patch.Count);
        Assert.Equal("Neu", patch["description"]);
        Assert.Equal(7, patch["order"]);
        Assert.Equal(true, patch["display_sub_forum_list"]);
    }

    /// <summary>
    /// Verifies that an update with a name longer than 100 characters is rejected before calling the API.
    /// </summary>
    [Fact]
    public async Task UpdateForum_WithTooLongName_ReturnsErrorWithoutCallingClient()
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new ForumTools(mock.Object);

        var result = await tools.UpdateForum(5, new string('x', 101), null, null, null, null, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that <c>get_forum</c> reports a missing forum instead of returning JSON.
    /// </summary>
    [Fact]
    public async Task GetForum_WhenNotFound_ReturnsNotFoundMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetForumAsync(8, It.IsAny<CancellationToken>())).ReturnsAsync((Forum?)null);

        var tools = new ForumTools(mock.Object);

        var result = await tools.GetForum(8, CancellationToken.None);

        Assert.Equal("Forum with ID 8 not found.", result);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task DeleteForum_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.DeleteForumAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("403"));

        var tools = new ForumTools(mock.Object);

        var result = await tools.DeleteForum(9, CancellationToken.None);

        Assert.StartsWith("ERROR: HttpRequestException", result);
    }
}
