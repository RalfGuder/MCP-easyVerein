using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Application.Configuration;

/// <summary>
/// Konfiguration für den easyVerein MCP-Server (FR-008, FR-013).
/// </summary>
public class EasyVereinConfiguration
{
    public const string EnvironmentVariableToken = "EASYVEREIN_API_TOKEN";
    public const string EnvironmentVariableBaseUrl = "EASYVEREIN_BASE_URL";
    public const string EnvironmentVariableApiVersion = "EASYVEREIN_API_VERSION";

    public const string DefaultBaseUrl = "https://easyverein.com/api";

    public string ApiToken { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = DefaultBaseUrl;
    public string ApiVersion { get; set; } = Domain.ValueObjects.ApiVersion.Default.Version;

    /// <summary>
    /// Erstellt Konfiguration aus Umgebungsvariablen (FR-008).
    /// </summary>
    public static EasyVereinConfiguration FromEnvironment()
    {
        var token = Environment.GetEnvironmentVariable(EnvironmentVariableToken);
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException(
                $"Umgebungsvariable '{EnvironmentVariableToken}' ist nicht gesetzt. " +
                "Bitte setzen Sie Ihren easyVerein API-Token.");

        var config = new EasyVereinConfiguration
        {
            ApiToken = token,
            BaseUrl = Environment.GetEnvironmentVariable(EnvironmentVariableBaseUrl) ?? DefaultBaseUrl,
            ApiVersion = Environment.GetEnvironmentVariable(EnvironmentVariableApiVersion)
                         ?? Domain.ValueObjects.ApiVersion.Default.Version
        };

        // API-Version validieren (FR-015)
        Domain.ValueObjects.ApiVersion.Create(config.ApiVersion);

        return config;
    }

    /// <summary>
    /// Gibt die vollständige Basis-URL inkl. Version zurück.
    /// </summary>
    public string GetVersionedBaseUrl(string? versionOverride = null)
    {
        var version = versionOverride ?? ApiVersion;
        if (versionOverride != null)
            Domain.ValueObjects.ApiVersion.Create(versionOverride);

        return $"{BaseUrl.TrimEnd('/')}/{version}";
    }
}
