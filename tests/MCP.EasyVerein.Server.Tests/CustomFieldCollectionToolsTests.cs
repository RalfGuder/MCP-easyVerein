using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="CustomFieldCollectionTools"/> MCP tool wrapper.</summary>
public class CustomFieldCollectionToolsTests
{
    /// <summary>
    /// Verifies that <c>update_custom_field_collection</c> sends only the provided fields
    /// (PATCH semantics) and ignores null or literal "null" values.
    /// </summary>
    [Fact]
    public async Task UpdateCustomFieldCollection_SendsOnlyProvidedFields()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        object? captured = null;
        mock.Setup(c => c.UpdateCustomFieldCollectionAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, patch, _) => captured = patch)
            .ReturnsAsync(new CustomFieldCollection { Id = 5, Position = 7 });

        var tools = new CustomFieldCollectionTools(mock.Object);

        await tools.UpdateCustomFieldCollection(5, name: "null", orderSequence: null, position: 7, CancellationToken.None);

        var patch = Assert.IsType<Dictionary<string, object>>(captured);
        Assert.Single(patch);
        Assert.Equal(7, patch["position"]);
    }

    /// <summary>
    /// Verifies that <c>create_custom_field_collection</c> maps all parameters onto the entity.
    /// </summary>
    [Fact]
    public async Task CreateCustomFieldCollection_PassesParameters_ToClient()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        CustomFieldCollection? captured = null;
        mock.Setup(c => c.CreateCustomFieldCollectionAsync(It.IsAny<CustomFieldCollection>(), It.IsAny<CancellationToken>()))
            .Callback<CustomFieldCollection, CancellationToken>((col, _) => captured = col)
            .ReturnsAsync(new CustomFieldCollection { Id = 1, Name = "Vereinsdaten" });

        var tools = new CustomFieldCollectionTools(mock.Object);

        var result = await tools.CreateCustomFieldCollection("Vereinsdaten", orderSequence: 2, position: 3, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Vereinsdaten", captured!.Name);
        Assert.Equal(2, captured.OrderSequence);
        Assert.Equal(3, captured.Position);
        Assert.Contains("Vereinsdaten", result);
    }

    /// <summary>
    /// Verifies that client exceptions are returned as an <c>ERROR:</c> message instead of being thrown.
    /// </summary>
    [Fact]
    public async Task GetCustomFieldCollection_OnException_ReturnsErrorMessage()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetCustomFieldCollectionAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("401"));

        var tools = new CustomFieldCollectionTools(mock.Object);

        var result = await tools.GetCustomFieldCollection(9, CancellationToken.None);

        Assert.StartsWith("ERROR: UnauthorizedAccessException", result);
    }
}
