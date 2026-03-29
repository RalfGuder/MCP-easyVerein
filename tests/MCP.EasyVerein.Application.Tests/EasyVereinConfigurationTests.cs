using MCP.EasyVerein.Application.Configuration;

namespace MCP.EasyVerein.Application.Tests;

public class EasyVereinConfigurationTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var config = new EasyVereinConfiguration();

        Assert.Equal("https://easyverein.com/api", config.BaseUrl);
        Assert.Equal("v1.7", config.ApiVersion);
        Assert.Equal(string.Empty, config.ApiToken);
    }

    [Fact]
    public void GetVersionedBaseUrl_ReturnsCorrectUrl()
    {
        var config = new EasyVereinConfiguration
        {
            BaseUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };

        Assert.Equal("https://easyverein.com/api/v1.7", config.GetVersionedBaseUrl());
    }

    [Fact]
    public void GetVersionedBaseUrl_WithOverride_UsesOverride()
    {
        var config = new EasyVereinConfiguration { ApiVersion = "v1.7" };
        Assert.Equal("https://easyverein.com/api/v1.6", config.GetVersionedBaseUrl("v1.6"));
    }

    [Fact]
    public void GetVersionedBaseUrl_WithInvalidOverride_Throws()
    {
        var config = new EasyVereinConfiguration();
        Assert.Throws<ArgumentException>(() => config.GetVersionedBaseUrl("v99"));
    }

    [Fact]
    public void FromEnvironment_WithoutToken_ThrowsInvalidOperation()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableToken, null);
        Assert.Throws<InvalidOperationException>(() => EasyVereinConfiguration.FromEnvironment());
    }

    [Fact]
    public void FromEnvironment_WithToken_ReturnsConfig()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableToken, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v1.7");

        try
        {
            var config = EasyVereinConfiguration.FromEnvironment();
            Assert.Equal("test-token", config.ApiToken);
            Assert.Equal("v1.7", config.ApiVersion);
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableToken, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }

    [Fact]
    public void FromEnvironment_WithInvalidVersion_Throws()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableToken, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v99");

        try
        {
            Assert.Throws<ArgumentException>(() => EasyVereinConfiguration.FromEnvironment());
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableToken, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }
}
