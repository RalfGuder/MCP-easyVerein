# UC-06: API-Version konfigurieren

> **Status:** Implementiert
> **Herkunft:** [US-005](https://github.com/RalfGuder/MCP-easyVerein/issues/5)
> **Requirements:** FR-010 bis FR-015, NFR-004

## Kurzbeschreibung

Der Vereinsadministrator konfiguriert die easyVerein API-Version, die der MCP-Server für alle API-Aufrufe verwendet. Der Server unterstützt v1.4, v1.5, v1.6 und v1.7 (Standard).

## Akteure

| Akteur | Rolle |
|--------|-------|
| **Vereinsadministrator** | Primärer Akteur – wählt die API-Version |

## Vorbedingungen

- Der Server ist noch nicht gestartet (Versionswahl erfolgt beim Start)

## Hauptszenario: Version über CLI-Parameter setzen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | Startet Server mit `--api-version v1.6` |
| 2 | System | Liest den CLI-Parameter `--api-version` |
| 3 | System | Validiert `v1.6` gegen die Liste unterstützter Versionen |
| 4 | System | Konfiguriert alle API-Aufrufe mit Basis-URL `https://easyverein.com/api/v1.6` |
| 5 | System | Startet den Server |

## Alternativszenarien

### A1: Version über Umgebungsvariable setzen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1a | Vereinsadministrator | Setzt `EASYVEREIN_API_VERSION=v1.5` |
| 1b | Vereinsadministrator | Startet Server ohne `--api-version` |
| 2 | System | Liest Version aus Umgebungsvariable |
| | | Weiter mit Hauptszenario Schritt 3 |

### A2: Standardversion verwenden

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1a | Vereinsadministrator | Startet Server ohne Versionsangabe |
| 2 | System | Keine Version in CLI oder Umgebungsvariable gefunden |
| 3 | System | Gibt Warnung aus und verwendet Standardversion v1.7 |

## Fehlerfälle

### F1: Ungültige API-Version

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | System | Version `v1.8` ist nicht in der Liste unterstützter Versionen |
| 3b | System | Ermittelt die nächste unterstützte Version (`v1.7`) |
| 3c | System | Gibt Fehlermeldung aus: „API-Version 'v1.8' wird nicht unterstützt. Nächste unterstützte Version: v1.7. Unterstützte Versionen: v1.4, v1.5, v1.6, v1.7" |
| 3d | System | Beendet sich mit Fehlercode |

### F2: Leere API-Version

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | System | Version ist leer oder null |
| 3b | System | Wirft `ArgumentException`: „API-Version darf nicht leer sein." |

## Nachbedingungen

- **Erfolg:** Server läuft mit der gewählten API-Version
- **Fehler:** Server hat sich mit hilfreicher Fehlermeldung beendet

## Unterstützte Versionen

| Version | Status |
|---------|--------|
| v1.4 | Unterstützt |
| v1.5 | Unterstützt |
| v1.6 | Unterstützt |
| **v1.7** | **Standard** |

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| Versionsvalidierung | `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs` |
| Konfigurationsauflösung | `src/MCP.EasyVerein.Application/Configuration/EasyVereinConfiguration.cs` |
| URL-Konstruktion | `EasyVereinApiClient.BuildUrl()` über `GetVersionedBaseUrl()` |
| Tests | `tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs` (8 Tests) |
