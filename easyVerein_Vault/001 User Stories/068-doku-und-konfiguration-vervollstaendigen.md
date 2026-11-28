# User Story 068: Dokumentation vervollständigen und Konfigurationsbeispiele korrigieren

> **GitHub Issue:** [#160 – US-0068 Dokumentation vervollständigen und Konfigurationsbeispiele korrigieren](https://github.com/RalfGuder/MCP-easyVerein/issues/160)
> **Spec:** `features/US-0068-doku-konfiguration-docker/spec.md` (Vertrag für die Software Factory)
> **Herkunft:** Restpunkte aus [[001-easyverein-mcp-server|US-0001 (#1)]], [[005-verschiedene-api-versionen|US-0005 (#5)]] und [[006-technische-entscheidung-csharp|US-0006 (#6)]] (am 2026-11-28 geschlossen)
> **Aufgeteilt am 2026-11-28:** [[069-konfigurationsdatei|US-0069 Konfigurationsdatei]], [[070-api-version-pro-aufruf|US-0070 API-Version pro Lese-Aufruf]], [[071-http-transport-lokal|US-0071 Lokaler HTTP-Transport]]

## User Story

**Als** Betreiber und Entwickler des MCP-Servers,
**möchte ich** im README nachlesen können, wie der Server konfiguriert und gestartet wird, wie er aufgebaut ist und welche Endpoints er anbietet, und Beispiele, die wirklich funktionieren,
**damit** ich den Server ohne Blick in den Code einrichten und erweitern kann.

## Akzeptanzkriterien

Kurzfassung, verbindlich ist die Spec (AK-1 bis AK-13):

- [ ] README-Abschnitt **Konfiguration**: CLI-Parameter, Umgebungsvariablen, Defaults, Priorität, `--help`, Schlüssel ohne `Bearer `-Präfix
- [ ] README: unterstützte API-Versionen v1.4–v2.0, Standard v2.0, Fehlermeldung mit Vorschlag
- [ ] README: Docker-Beispiel mit `-i --rm -e EASYVEREIN_API_KEY` und `.mcp.json`-Eintrag für den Container; nativer Start mit `--project`
- [ ] README-Abschnitt **Projektstruktur**: vier Schichten, Abhängigkeitsrichtung, fünf Schritte für neue Endpoints
- [ ] README: Tabelle aller Endpoints mit Operationen
- [ ] `Dockerfile` setzt `EASYVEREIN_API_KEY` und `EASYVEREIN_API_URL` (statt `…_API_TOKEN`, `…_BASE_URL`)
- [ ] `.mcp.json.example` ohne `Bearer `-Präfix
- [ ] ADR `docs/adr/0001-csharp-dotnet8.md`
- [ ] Schlüssel mit `Bearer `-Präfix wird toleriert (Präfix wird entfernt)

## Aufgaben

- Übergabe an die Software Factory (Issue #160)
- Nach dem Merge: Spec-Status und diese Story nachziehen

## Technische Hinweise

- **Gefundene Fehler (2026-11-28):**
  - `Dockerfile` setzt `EASYVEREIN_API_TOKEN` und `EASYVEREIN_BASE_URL`, der Code liest `EASYVEREIN_API_KEY` und `EASYVEREIN_API_URL`.
  - Das README-Beispiel `docker run` ohne `-i` beendet den stdio-Server sofort.
  - `.mcp.json.example` erzeugt `Bearer Bearer …`, weil der Client das Präfix selbst setzt (`EasyVereinApiClient.cs:44`).
- Die Factory darf laut `docs/stack.md` `.github/workflows`, `Konten.md` und Daily Notes nicht ändern; README, `Dockerfile`, `docs/` und `src/` sind erlaubt.
