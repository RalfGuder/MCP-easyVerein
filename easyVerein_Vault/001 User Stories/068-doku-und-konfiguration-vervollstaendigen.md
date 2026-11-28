# User Story 068: Dokumentation und Konfiguration vervollständigen

> **GitHub Issue:** [#160 – US-0068 Dokumentation und Konfiguration vervollständigen](https://github.com/RalfGuder/MCP-easyVerein/issues/160)
> **Herkunft:** Restpunkte aus [[001-easyverein-mcp-server|US-0001 (#1)]], [[005-verschiedene-api-versionen|US-0005 (#5)]] und [[006-technische-entscheidung-csharp|US-0006 (#6)]] (am 2026-11-28 geschlossen)

## User Story

**Als** Betreiber und Entwickler des MCP-Servers,
**möchte ich** eine vollständige Dokumentation zu Konfiguration, API-Versionen und Projektaufbau sowie flexiblere Konfigurations- und Transportwege,
**damit** ich den Server ohne Blick in den Code einrichten und erweitern kann und ihn auch außerhalb von stdio und Umgebungsvariablen betreiben kann.

## Akzeptanzkriterien

### Dokumentation

- [ ] Das README hat einen Abschnitt **Konfiguration**: CLI-Parameter (`--api-key`, `--api-url`, `--api-version`), Umgebungsvariablen (`EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`), Defaults (URL `https://easyverein.com/api`, Version `v2.0`) und die Priorität (CLI vor Umgebung vor Default). *(aus #5)*
- [ ] Das README listet die unterstützten API-Versionen (v1.4, v1.5, v1.6, v1.7, v2.0) und beschreibt die Fehlermeldung mit Vorschlag der nächstmöglichen Version. *(aus #5)*
- [ ] Das README hat einen Abschnitt **Projektstruktur**: die vier Schichten Domain, Application, Infrastructure, Server, ihre Abhängigkeitsrichtung und das Muster für neue Endpoints. *(aus #6)*
- [ ] Die Entscheidung für C# / .NET 8 ist als ADR dokumentiert (Kontext, Entscheidung, Alternativen, Konsequenzen). *(aus #6)*
- [ ] README und `Dockerfile` verwenden `EASYVEREIN_API_KEY` statt `EASYVEREIN_API_TOKEN`. Das Docker-Beispiel aus dem README startet den Server mit gültigem Schlüssel.

### Konfiguration und Transport

- [ ] Die Konfiguration kann alternativ aus einer Konfigurationsdatei kommen. *(aus #1, Should)*
- [ ] Die API-Version kann pro MCP-Tool-Aufruf überschrieben werden. *(aus #5, Should; Infrastruktur vorhanden über `GetVersionedBaseUrl(override)`)*
- [ ] Der MCP-Server ist zusätzlich über SSE bzw. HTTP erreichbar; stdio bleibt der Standard. *(aus #1, Could)*

## Aufgaben

- Spec mit `/write-spec` schreiben (offene Fragen siehe unten), danach Übergabe an die Software Factory
- README: Abschnitte Konfiguration, API-Versionen, Projektstruktur
- ADR anlegen
- `Dockerfile` und README auf `EASYVEREIN_API_KEY` korrigieren
- Konfigurationsdatei, Versions-Override und SSE nach Spec umsetzen (TDD)

## Technische Hinweise

- **Gefundener Fehler:** `Dockerfile:22` setzt `ENV EASYVEREIN_API_TOKEN=""`, `README.md:17` zeigt `docker run --env EASYVEREIN_API_TOKEN=<token>`. Der Code liest `EASYVEREIN_API_KEY` (`EasyVereinConfiguration.EnvironmentVariableApiKey`). Das dokumentierte Docker-Beispiel funktioniert deshalb nicht.
- Offene Fragen für die Spec:
  - Konfigurationsdatei: Format (JSON?), Ort (Arbeitsverzeichnis, Benutzerprofil, `--config <pfad>`?), Rang in der Priorität (zwischen Umgebung und Default?). Der API-Schlüssel darf nicht versehentlich ins Repo gelangen.
  - Versions-Override: optionaler Parameter `apiVersion` an jedem Tool (rund 120 Tools) oder nur an ausgewählten? Neue Tool-Parameter immer mit Standardwert anlegen (Lehre aus PR #133).
  - SSE: Das MCP SDK v1.2.0 bietet HTTP-Transport über ASP.NET Core. Port, Bindung nur an `localhost`, Authentifizierung? Auswahl per `--transport stdio|http`?
  - Ablageort der ADR (`docs/adr/` oder `easyVerein_Vault/`).
- Die Factory darf laut `docs/stack.md` `.github/workflows`, `Konten.md` und Daily Notes nicht ändern; README, `Dockerfile`, `docs/` und `src/` sind erlaubt.
