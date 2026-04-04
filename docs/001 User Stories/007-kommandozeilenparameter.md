# User Story 007: Kommandozeilenparameter für API-Konfiguration

> **GitHub Issue:** [#12 – US-0007 Kommandozeilenparameter für API-Konfiguration](https://github.com/RalfGuder/MCP-easyVerein/issues/12)

## User Story

**Als** Entwickler oder Administrator,
**möchte ich** API-URL, API-Version und API-Key als Kommandozeilenparameter (`--api-url`, `--api-version`, `--api-key`) beim Start des MCP-Servers übergeben können,
**damit** ich den Server flexibel in verschiedenen Umgebungen (Dev, Test, Prod) betreiben kann, ohne Umgebungsvariablen manuell setzen zu müssen.

## Akzeptanzkriterien

- [x] Der Server akzeptiert die Parameter `--api-url`, `--api-version` und `--api-key` beim Start *(umgesetzt in `Program.cs` via `AddCommandLine` mit Switch-Mappings)*
- [x] Übergebene Kommandozeilenparameter überschreiben gleichnamige Umgebungsvariablen *(IConfiguration-Provider-Reihenfolge: CLI zuletzt registriert → höchste Priorität)*
- [x] Fehlt ein Parameter, wird der Wert aus der entsprechenden Umgebungsvariable (`EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`, `EASYVEREIN_API_KEY`) verwendet *(umgesetzt in `EasyVereinConfiguration.FromConfiguration()` via `Resolve()`)*
- [x] Fehlt der Parameter und die Umgebungsvariable, wird eine Warnung ausgegeben und ein Standardwert genutzt *(umgesetzt: `logger.LogWarning` in `Resolve()`)*
- [x] Die Parameter sind in der Hilfemeldung (`--help`) dokumentiert *(umgesetzt: `PrintHelp()` in `Program.cs`)*

## Aufgaben

- [x] Kommandozeilenparser integrieren *(verwendet `Microsoft.Extensions.Configuration.CommandLine` statt `System.CommandLine`)*
- [x] Parameter `--api-url`, `--api-version`, `--api-key` implementieren *(Switch-Mappings in `Program.cs`)*
- [x] Priorisierungslogik CLI > Env-Var > Default implementieren *(IConfiguration-Provider-Reihenfolge)*
- [x] Warnung bei fehlenden Pflichtangaben implementieren *(LogWarning in `EasyVereinConfiguration.Resolve()`)*
- [x] Tests nach TDD-Prinzip schreiben *(6 Tests in `EasyVereinConfigurationTests`: FromConfiguration_*)*
- [x] Hilfemeldung (`--help`) ergänzen *(PrintHelp() in `Program.cs`)*

## Technische Hinweise

- Prioritätsreihenfolge: CLI-Parameter → Umgebungsvariablen → Standardwerte
- Umgebungsvariablennamen: `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`, `EASYVEREIN_API_KEY`
- Verwendet `Microsoft.Extensions.Configuration.CommandLine` (statt `System.CommandLine`) – nahtlose Integration mit `IConfiguration`
- TDD: 6 Tests für `FromConfiguration` in `EasyVereinConfigurationTests`
- Abgeschlossen via Feature-Branch und PR (nach US-008)
