# User Story 069: Konfigurationsdatei als zusätzliche Quelle

> **GitHub Issue:** [#162 – US-0069 Konfigurationsdatei als zusätzliche Quelle](https://github.com/RalfGuder/MCP-easyVerein/issues/162)
> **Spec:** `features/US-0069-konfigurationsdatei/spec.md` (Vertrag für die Software Factory)
> **Herkunft:** [[068-doku-und-konfiguration-vervollstaendigen|US-0068]] (aufgeteilt am 2026-11-28), ursprünglich [[001-easyverein-mcp-server|US-0001 (#1)]]
> **Abhängig von:** US-0068

## User Story

**Als** Betreiber,
**möchte ich** Schlüssel, URL und API-Version in einer Datei im Benutzerprofil oder unter einem per `--config` gewählten Pfad hinterlegen,
**damit** der Schlüssel nicht in der `.mcp.json` des Projekts steht und ich zwischen Zugängen wechseln kann.

## Akzeptanzkriterien

Kurzfassung, verbindlich ist die Spec (AK-1 bis AK-15):

- [ ] Fundorte: `--config <pfad>` vor `EASYVEREIN_CONFIG` vor Standarddatei (`%APPDATA%\mcp-easyverein\config.json` bzw. `~/.config/mcp-easyverein/config.json`, `XDG_CONFIG_HOME` beachtet)
- [ ] JSON mit `apiKey`, `apiUrl`, `apiVersion`; alle optional, unbekannte Felder ignoriert
- [ ] Priorität: CLI vor Umgebungsvariable vor Datei vor Standardwert, je Wert
- [ ] Fehlende angegebene Datei oder ungültiges JSON → Start bricht mit Meldung auf stderr ab; Schlüssel nie in Meldungen
- [ ] Implizite `appsettings.json` im Arbeitsverzeichnis wird nicht mehr gelesen
- [ ] `--help` und README beschreiben die Datei

## Aufgaben

- Nach US-0068 an die Software Factory übergeben (Issue #162)

## Technische Hinweise

- `Host.CreateApplicationBuilder` liest heute implizit `appsettings.json` aus dem Arbeitsverzeichnis; das entfällt bewusst (Verhaltensänderung).
