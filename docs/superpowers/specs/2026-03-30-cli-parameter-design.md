# Design: CLI-Konfigurationsparameter (US-0007)

**Datum:** 2026-03-30
**Status:** Genehmigt
**Herkunft:** [GitHub Issue #12 – US-0007](https://github.com/RalfGuder/MCP-easyVerein/issues/12)

---

## Ziel

Der easyVerein MCP-Server soll beim Start die Parameter `--api-key`, `--api-url` und `--api-version` als Kommandozeilenargumente akzeptieren. CLI-Parameter überschreiben Umgebungsvariablen, die wiederum Standardwerte überschreiben. Fehlende Werte lösen eine Warnung aus, der Server startet aber weiter.

---

## Anforderungen

| ID | Beschreibung |
|----|-------------|
| FR-041 | CLI-Parameter `--api-key`, `--api-url`, `--api-version` |
| FR-042 | Priorität: CLI > Env-Var > Default |
| FR-043 | Warnung + Fallback auf Standardwert bei fehlendem Wert |
| FR-044 | `--help` mit vollständiger Parameterdokumentation |
| NFR-023 | README dokumentiert alle Konfigurationsquellen |

---

## Architektur

### Prioritätsreihenfolge (IConfiguration-Provider-Reihenfolge)

```
CLI-Parameter  (höchste Priorität, zuletzt registriert)
      ↓
Umgebungsvariablen  (via CreateApplicationBuilder automatisch)
      ↓
Standardwerte in EasyVereinConfiguration
```

Diese Reihenfolge ergibt sich natürlich aus der Registrierungsreihenfolge der `IConfiguration`-Provider in `Microsoft.Extensions.Configuration`.

### Betroffene Dateien

| Datei | Änderung |
|-------|----------|
| `src/MCP.EasyVerein.Server/Program.cs` | `--help`-Erkennung, Switch-Mappings, `IOptions<T>`-Registrierung |
| `src/MCP.EasyVerein.Application/Configuration/EasyVereinConfiguration.cs` | Konstanten aktualisieren, `FromConfiguration()` ergänzen |
| `src/MCP.EasyVerein.Server/MCP.EasyVerein.Server.csproj` | `Microsoft.Extensions.Configuration.CommandLine` prüfen/ergänzen |
| `tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs` | Bestehende Tests anpassen, neue CLI-Tests ergänzen |

---

## Detaildesign

### `Program.cs`

```csharp
// 1. --help vor Host-Start abfangen (FR-044)
if (args.Contains("--help") || args.Contains("-h"))
{
    PrintHelp();
    return;
}

var builder = Host.CreateApplicationBuilder(args);

// 2. Switch-Mappings: CLI-Parameter → IConfiguration-Keys (FR-041, FR-042)
var switchMappings = new Dictionary<string, string>
{
    ["--api-key"]     = "EASYVEREIN_API_KEY",
    ["--api-url"]     = "EASYVEREIN_API_URL",
    ["--api-version"] = "EASYVEREIN_API_VERSION"
};
builder.Configuration.AddCommandLine(args, switchMappings);

// 3. EasyVereinConfiguration via IOptions<T> binden (FR-041–FR-043)
builder.Services.AddOptions<EasyVereinConfiguration>()
    .Configure<IConfiguration, ILogger<EasyVereinConfiguration>>(
        (cfg, config, logger) =>
        {
            cfg.ApiKey     = Resolve(config, "EASYVEREIN_API_KEY",     string.Empty,                        "api-key",     logger);
            cfg.ApiUrl     = Resolve(config, "EASYVEREIN_API_URL",     EasyVereinConfiguration.DefaultApiUrl, "api-url",   logger);
            cfg.ApiVersion = Resolve(config, "EASYVEREIN_API_VERSION", ApiVersion.Default.Version,           "api-version", logger);
        });

static string Resolve(IConfiguration config, string key, string defaultValue,
                      string paramName, ILogger logger)
{
    var value = config[key];
    if (string.IsNullOrEmpty(value))
    {
        logger.LogWarning(
            "Konfigurationswert '{Key}' nicht gesetzt (weder --{Param} noch Umgebungsvariable). " +
            "Standardwert wird verwendet: '{Default}'", key, paramName, defaultValue);
        return defaultValue;
    }
    return value;
}

// 4. --help Ausgabe
static void PrintHelp()
{
    Console.WriteLine("""
        easyVerein MCP-Server

        Verwendung: MCP.EasyVerein.Server [Optionen]

        Optionen:
          --api-key <value>      API-Schlüssel        (Env: EASYVEREIN_API_KEY)
          --api-url <value>      API-Basis-URL        (Env: EASYVEREIN_API_URL,     Standard: https://easyverein.com/api)
          --api-version <value>  API-Version          (Env: EASYVEREIN_API_VERSION, Standard: v1.7)
          --help, -h             Diese Hilfe anzeigen

        Priorität: CLI-Parameter > Umgebungsvariablen > Standardwerte
        """);
}
```

### `EasyVereinConfiguration.cs`

- Konstante `EnvironmentVariableToken` → `EnvironmentVariableApiKey = "EASYVEREIN_API_KEY"`
- Neue Konstante `EnvironmentVariableApiUrl = "EASYVEREIN_API_URL"` (Umbenennung von `EnvironmentVariableBaseUrl`)
- Property `ApiToken` → `ApiKey`, `BaseUrl` → `ApiUrl` (konsistente Benennung)
- `FromEnvironment()` bleibt als Legacy-Methode für bestehende Tests, aber mit aktualisierten Konstantennamen
- Warnungsausgabe erfolgt in `Program.cs` via `ILogger`, nicht in der Konfigurationsklasse selbst

### `--help`-Ausgabe (FR-044)

```
easyVerein MCP-Server

Verwendung: MCP.EasyVerein.Server [Optionen]

Optionen:
  --api-key <value>      API-Schlüssel        (Env: EASYVEREIN_API_KEY)
  --api-url <value>      API-Basis-URL        (Env: EASYVEREIN_API_URL,     Standard: https://easyverein.com/api)
  --api-version <value>  API-Version          (Env: EASYVEREIN_API_VERSION, Standard: v1.7)
  --help, -h             Diese Hilfe anzeigen

Priorität: CLI-Parameter > Umgebungsvariablen > Standardwerte
```

---

## Testdesign (TDD)

Tests werden vor der Implementierung geschrieben (Red-Green-Refactor).

### Neue Tests in `EasyVereinConfigurationTests.cs`

| Test | Beschreibung |
|------|-------------|
| `FromConfiguration_WithAllCliArgs_UsesCliValues` | CLI-Werte werden korrekt übernommen |
| `FromConfiguration_WithEnvVarAndCliArg_CliWins` | CLI überschreibt Env-Var |
| `FromConfiguration_WithOnlyEnvVar_UsesEnvVar` | Env-Var wird bei fehlendem CLI-Arg verwendet |
| `FromConfiguration_WithNoValues_LogsWarningAndUsesDefaults` | Fehlende Werte → Warnung + Standardwert |

### Anpassungen bestehender Tests

- `FromEnvironment_WithoutToken_ThrowsInvalidOperation` bleibt (Legacy-Methode bleibt erhalten)
- Konstantennamen in Tests aktualisieren: `EnvironmentVariableToken` → `EnvironmentVariableApiKey`

---

## Breaking Changes

| Was | Vorher | Nachher |
|-----|--------|---------|
| Env-Var für API-Key | `EASYVEREIN_API_TOKEN` | `EASYVEREIN_API_KEY` |
| Env-Var für Basis-URL | `EASYVEREIN_BASE_URL` | `EASYVEREIN_API_URL` |
| Property in `EasyVereinConfiguration` | `ApiToken`, `BaseUrl` | `ApiKey`, `ApiUrl` |

---

## Offene Punkte

- `Microsoft.Extensions.Configuration.CommandLine` ist via `Microsoft.Extensions.Hosting` bereits transitiv vorhanden — expliziter Package-Verweis wahrscheinlich nicht nötig (zu prüfen)
- API-Version-Validierung (`ApiVersion.Create()`) bleibt beim Start erhalten; ungültige Version führt weiterhin zu einer Exception (gewollt)
