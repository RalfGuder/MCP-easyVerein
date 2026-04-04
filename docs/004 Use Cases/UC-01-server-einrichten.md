# UC-01: Server einrichten und starten

> **Status:** Implementiert
> **Herkunft:** [US-001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-005](https://github.com/RalfGuder/MCP-easyVerein/issues/5), [US-006](https://github.com/RalfGuder/MCP-easyVerein/issues/6), [US-007](https://github.com/RalfGuder/MCP-easyVerein/issues/12)
> **Requirements:** FR-001, FR-002, FR-008, FR-041 bis FR-044, NFR-007

## Kurzbeschreibung

Der Vereinsadministrator richtet den MCP-Server ein und startet ihn, sodass ein KI-Assistent über das MCP-Protokoll auf die easyVerein API zugreifen kann.

## Akteure

| Akteur | Rolle |
|--------|-------|
| **Vereinsadministrator** | Primärer Akteur – konfiguriert und startet den Server |
| **KI-Assistent** | Nutznießer – kann nach dem Start MCP-Tools aufrufen |
| **easyVerein API** | Externes System – wird beim Start nicht kontaktiert, erst bei Tool-Aufrufen |

## Vorbedingungen

- .NET 8 Runtime ist installiert
- Ein gültiger easyVerein API-Key liegt vor
- Der Server ist als ausführbare Datei oder Docker-Container verfügbar

## Hauptszenario

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | Startet den Server mit Kommandozeilenparametern: `MCP.EasyVerein.Server --api-key <KEY>` |
| 2 | System | Liest Konfiguration in der Prioritätsreihenfolge: CLI-Parameter → Umgebungsvariablen → Standardwerte |
| 3 | System | Validiert die API-Version (Standard: v1.7) gegen die Liste unterstützter Versionen |
| 4 | System | Registriert alle MCP-Tools (MemberTools, InvoiceTools, EventTools, ContactDetailsTools) |
| 5 | System | Startet den stdio-Transport und wartet auf MCP-Verbindungen |
| 6 | KI-Assistent | Verbindet sich über stdio und ruft `tools/list` auf |
| 7 | System | Gibt die Liste aller verfügbaren MCP-Tools zurück |

## Alternativszenarien

### A1: Start über Umgebungsvariablen (statt CLI)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1a | Vereinsadministrator | Setzt `EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION` als Umgebungsvariablen |
| 1b | Vereinsadministrator | Startet den Server ohne CLI-Parameter |
| 2 | System | Übernimmt Werte aus Umgebungsvariablen |
| | | Weiter mit Hauptszenario Schritt 3 |

### A2: Start als Docker-Container

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1a | Vereinsadministrator | Startet Container: `docker run -e EASYVEREIN_API_KEY=<KEY> mcp-easyverein:latest` |
| | | Weiter mit Hauptszenario Schritt 2 |

### A3: Hilfemeldung anzeigen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1a | Vereinsadministrator | Startet den Server mit `--help` oder `-h` |
| 2a | System | Gibt die Hilfemeldung mit allen Parametern, Umgebungsvariablen und Standardwerten aus |
| 3a | System | Beendet sich ohne den Server zu starten |

## Fehlerfälle

### F1: Ungültige API-Version

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | System | Die angegebene API-Version wird nicht unterstützt |
| 3b | System | Gibt eine Fehlermeldung mit der nächsten unterstützten Version und der vollständigen Liste aus |
| 3c | System | Beendet sich mit Fehlercode |

### F2: Fehlende Konfigurationswerte

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2a | System | Ein Konfigurationswert fehlt auf allen Ebenen (CLI, Env-Var) |
| 2b | System | Gibt eine Warnung mit dem fehlenden Parameter und dem verwendeten Standardwert aus |
| 2c | System | Startet trotzdem weiter mit dem Standardwert |

## Nachbedingungen

- **Erfolg:** Der MCP-Server läuft und akzeptiert Verbindungen über stdio
- **Fehler (F1):** Der Server hat sich mit einer verständlichen Fehlermeldung beendet

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| Startup und CLI-Parsing | `src/MCP.EasyVerein.Server/Program.cs` |
| Konfigurationslogik | `src/MCP.EasyVerein.Application/Configuration/EasyVereinConfiguration.cs` |
| API-Version-Validierung | `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs` |
| Tests | `tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs`, `tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs` |
