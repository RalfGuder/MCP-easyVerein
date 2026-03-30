using ApiVersionVO = MCP.EasyVerein.Domain.ValueObjects.ApiVersion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MCP.EasyVerein.Application.Configuration;

/// <summary>
/// Konfiguration für den easyVerein MCP-Server (FR-008, FR-013, FR-041–FR-044).
/// </summary>
public class EasyVereinConfiguration
{
    public const string EnvironmentVariableApiKey     = "EASYVEREIN_API_KEY";
    public const string EnvironmentVariableApiUrl     = "EASYVEREIN_API_URL";
    public const string EnvironmentVariableApiVersion = "EASYVEREIN_API_VERSION";

    public const string DefaultApiUrl = "https://easyverein.com/api";

    public string ApiKey     { get; init; } = string.Empty;
    public string ApiUrl     { get; init; } = DefaultApiUrl;
    public string ApiVersion { get; init; } = ApiVersionVO.Default.Version;

    /// <summary>
    /// Erstellt Konfiguration aus Umgebungsvariablen (FR-008). Legacy-Methode.
    /// </summary>
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
    /// Erstellt Konfiguration aus IConfiguration (FR-041–FR-043).
    /// CLI-Parameter überschreiben Env-Vars (Priorität via IConfiguration-Provider-Reihenfolge).
    /// Fehlende Werte lösen eine Warnung aus; es wird der Standardwert verwendet.
    /// </summary>
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
    /// Gibt die vollständige Basis-URL inkl. Version zurück.
    /// </summary>
    public string GetVersionedBaseUrl(string? versionOverride = null)
    {
        var version = versionOverride ?? ApiVersion;
        if (versionOverride != null)
            ApiVersionVO.Create(versionOverride);

        return $"{ApiUrl.TrimEnd('/')}/{version}";
    }
}
