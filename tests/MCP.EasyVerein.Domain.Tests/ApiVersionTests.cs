using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Tests;

public class ApiVersionTests
{
    [Fact]
    public void SupportedVersions_Contains_V17()
    {
        Assert.Contains("v1.7", ApiVersion.SupportedVersions);
    }

    [Fact]
    public void Default_Is_V17()
    {
        Assert.Equal("v1.7", ApiVersion.Default.Version);
    }

    [Fact]
    public void Create_WithSupportedVersion_Succeeds()
    {
        var version = ApiVersion.Create("v1.7");
        Assert.Equal("v1.7", version.Version);
    }

    [Fact]
    public void Create_WithUnsupportedVersion_ThrowsWithSuggestion()
    {
        var ex = Assert.Throws<ArgumentException>(() => ApiVersion.Create("v1.8"));
        Assert.Contains("v1.8", ex.Message);
        Assert.Contains("v1.7", ex.Message); // Vorschlag
    }

    [Fact]
    public void Create_WithNullOrEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ApiVersion.Create(""));
        Assert.Throws<ArgumentException>(() => ApiVersion.Create(null!));
    }

    [Fact]
    public void IsSupported_ReturnsTrueForValid()
    {
        Assert.True(ApiVersion.IsSupported("v1.7"));
    }

    [Fact]
    public void IsSupported_ReturnsFalseForInvalid()
    {
        Assert.False(ApiVersion.IsSupported("v99"));
    }

    [Fact]
    public void GetClosestVersion_ReturnsNearest()
    {
        var closest = ApiVersion.GetClosestVersion("v1.6");
        Assert.NotNull(closest);
    }

    [Fact]
    public void SupportedVersions_Contains_V20()
    {
        Assert.Contains("v2.0", ApiVersion.SupportedVersions);
    }

    [Fact]
    public void Create_WithV20_Succeeds()
    {
        var version = ApiVersion.Create("v2.0");
        Assert.Equal("v2.0", version.Version);
    }
}
