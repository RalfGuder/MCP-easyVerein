using ApiVersionVO = MCP.EasyVerein.Domain.ValueObjects.ApiVersion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MCP.EasyVerein.Application.Configuration;

/// <summary>
/// Configuration for the easyVerein MCP server (FR-008, FR-013, FR-041 to FR-044).
/// </summary>
public class EasyVereinConfiguration
{
    /// <summary>
    /// Environment variable name for the easyVerein API key.
    /// </summary>
    public const string EnvironmentVariableApiKey     = "EASYVEREIN_API_KEY";

    /// <summary>
    /// Environment variable name for the easyVerein API base URL.
    /// </summary>
    public const string EnvironmentVariableApiUrl     = "EASYVEREIN_API_URL";

    /// <summary>
    /// Environment variable name for the easyVerein API version.
    /// </summary>
    public const string EnvironmentVariableApiVersion = "EASYVEREIN_API_VERSION";

    /// <summary>
    /// Default base URL for the easyVerein API.
    /// </summary>
    public const string DefaultApiUrl = "https://easyverein.com/api";

    /// <summary>
    /// Gets the API key used for authentication with the easyVerein API.
    /// </summary>
    public string ApiKey     { get; init; } = string.Empty;

    /// <summary>
    /// Gets the base URL of the easyVerein API.
    /// </summary>
    public string ApiUrl     { get; init; } = DefaultApiUrl;

    /// <summary>
    /// Gets the API version string used for endpoint routing.
    /// </summary>
    public string ApiVersion { get; init; } = ApiVersionVO.Default.Version;

    /// <summary>
    /// Creates a configuration from environment variables (FR-008). Legacy method.
    /// </summary>
    /// <returns>A new <see cref="EasyVereinConfiguration"/> populated from environment variables.</returns>
    public static EasyVereinConfiguration FromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable(EnvironmentVariableApiKey);
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException(
                $"Umgebungsvariable '{EnvironmentVariableApiKey}' ist nicht gesetzt. " +
                "Bitte setzen Sie Ihren easyVerein API-Schlüssel.");

        var config = new EasyVereinConfiguration
        {
            ApiKey     = apiKey,
            ApiUrl     = Environment.GetEnvironmentVariable(EnvironmentVariableApiUrl) ?? DefaultApiUrl,
            ApiVersion = Environment.GetEnvironmentVariable(EnvironmentVariableApiVersion)
                         ?? ApiVersionVO.Default.Version
        };

        // API-Version validieren (FR-015)
        ApiVersionVO.Create(config.ApiVersion);

        return config;
    }

    /// <summary>
    /// Creates a configuration from an <see cref="IConfiguration"/> instance (FR-041 to FR-043).
    /// CLI parameters override environment variables via the IConfiguration provider order.
    /// Missing values trigger a warning and fall back to defaults.
    /// </summary>
    /// <param name="configuration">The configuration source containing API settings.</param>
    /// <param name="logger">Logger used to report missing configuration values.</param>
    /// <returns>A new <see cref="EasyVereinConfiguration"/> populated from the configuration source.</returns>
    public static EasyVereinConfiguration FromConfiguration(IConfiguration configuration, ILogger logger)
    {
        var apiVersion = Resolve(
            configuration, EnvironmentVariableApiVersion,
            ApiVersionVO.Default.Version,
            "api-version", logger);

        // Ungültige API-Version führt zu einer Exception (gewollt, FR-015)
        ApiVersionVO.Create(apiVersion);

        return new EasyVereinConfiguration
        {
            ApiKey     = Resolve(configuration, EnvironmentVariableApiKey, string.Empty, "api-key", logger),
            ApiUrl     = Resolve(configuration, EnvironmentVariableApiUrl, DefaultApiUrl, "api-url", logger),
            ApiVersion = apiVersion
        };
    }

    /// <summary>
    /// Resolves a configuration value by key, logging a warning and returning the default if not found.
    /// </summary>
    /// <param name="configuration">The configuration source to read from.</param>
    /// <param name="key">The configuration key to look up.</param>
    /// <param name="defaultValue">The default value to use when the key is not set.</param>
    /// <param name="paramName">The CLI parameter name shown in the warning message.</param>
    /// <param name="logger">Logger for reporting missing values.</param>
    /// <returns>The resolved configuration value or the default.</returns>
    private static string Resolve(
        IConfiguration configuration, string key, string defaultValue,
        string paramName, ILogger logger)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            logger.LogWarning(
                "Konfigurationswert '{Key}' nicht gesetzt (weder --{Param} noch Umgebungsvariable). " +
                "Standardwert wird verwendet: '{Default}'",
                key, paramName, defaultValue);
            return defaultValue;
        }
        return value;
    }

    /// <summary>
    /// Returns the fully qualified base URL including the API version segment.
    /// </summary>
    /// <param name="versionOverride">Optional version string to use instead of the configured version.</param>
    /// <returns>The versioned base URL (e.g. "https://easyverein.com/api/v1.7").</returns>
    public string GetVersionedBaseUrl(string? versionOverride = null)
    {
        var version = versionOverride ?? ApiVersion;
        if (versionOverride != null)
            ApiVersionVO.Create(versionOverride);

        return $"{ApiUrl.TrimEnd('/')}/{version}";
    }
}
