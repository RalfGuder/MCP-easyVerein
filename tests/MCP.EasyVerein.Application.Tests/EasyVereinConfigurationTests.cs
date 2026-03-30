using MCP.EasyVerein.Application.Configuration;

namespace MCP.EasyVerein.Application.Tests;

public class EasyVereinConfigurationTests
{
    // --- Bestehende Tests ---

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var config = new EasyVereinConfiguration();

        Assert.Equal("https://easyverein.com/api", config.ApiUrl);
        Assert.Equal("v1.7", config.ApiVersion);
        Assert.Equal(string.Empty, config.ApiKey);
    }

    [Fact]
    public void GetVersionedBaseUrl_ReturnsCorrectUrl()
    {
        var config = new EasyVereinConfiguration
        {
            ApiUrl = "https://easyverein.com/api",
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
    public void FromEnvironment_WithoutApiKey_ThrowsInvalidOperation()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
        Assert.Throws<InvalidOperationException>(() => EasyVereinConfiguration.FromEnvironment());
    }

    [Fact]
    public void FromEnvironment_WithApiKey_ReturnsConfig()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v1.7");

        try
        {
            var config = EasyVereinConfiguration.FromEnvironment();
            Assert.Equal("test-token", config.ApiKey);
            Assert.Equal("v1.7", config.ApiVersion);
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }

    [Fact]
    public void FromEnvironment_WithInvalidVersion_Throws()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v99");

        try
        {
            Assert.Throws<ArgumentException>(() => EasyVereinConfiguration.FromEnvironment());
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }

    // --- Neue Tests: FromConfiguration ---

    [Fact]
    public void FromConfiguration_WithAllValues_ReturnsCorrectConfig()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey]     = "my-api-key",
                [EasyVereinConfiguration.EnvironmentVariableApiUrl]     = "https://custom.api",
                [EasyVereinConfiguration.EnvironmentVariableApiVersion] = "v1.6"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal("my-api-key", config.ApiKey);
        Assert.Equal("https://custom.api", config.ApiUrl);
        Assert.Equal("v1.6", config.ApiVersion);
    }

    [Fact]
    public void FromConfiguration_WithMissingApiKey_UsesEmptyDefaultAndLogsWarning()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var loggerMock = new Mock<ILogger>();
        loggerMock.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(true);

        var config = EasyVereinConfiguration.FromConfiguration(configuration, loggerMock.Object);

        Assert.Equal(string.Empty, config.ApiKey);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("EASYVEREIN_API_KEY")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void FromConfiguration_WithMissingApiUrl_UsesDefaultUrl()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "test-key"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal(EasyVereinConfiguration.DefaultApiUrl, config.ApiUrl);
    }

    [Fact]
    public void FromConfiguration_WithMissingApiVersion_UsesDefaultVersion()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "test-key"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal("v1.7", config.ApiVersion);
    }

    [Fact]
    public void FromConfiguration_WithInvalidApiVersion_Throws()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey]     = "test-key",
                [EasyVereinConfiguration.EnvironmentVariableApiVersion] = "v99"
            })
            .Build();

        Assert.Throws<ArgumentException>(() =>
            EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance));
    }

    [Fact]
    public void FromConfiguration_LaterProviderWins_CliOverridesEnvVar()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "env-key"
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "cli-key"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal("cli-key", config.ApiKey);
    }
}
