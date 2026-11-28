# User Story 070: API-Version pro Lese-Aufruf wählen

> **GitHub Issue:** [#163 – US-0070 API-Version pro Lese-Aufruf wählen](https://github.com/RalfGuder/MCP-easyVerein/issues/163)
> **Spec:** `features/US-0070-api-version-pro-aufruf/spec.md` (Vertrag für die Software Factory)
> **Herkunft:** [[068-doku-und-konfiguration-vervollstaendigen|US-0068]] (aufgeteilt am 2026-11-28), ursprünglich [[005-verschiedene-api-versionen|US-0005 (#5)]]
> **Abhängig von:** US-0068

## User Story

**Als** Entwickler des MCP-Servers,
**möchte ich** dieselben Daten gezielt in einer anderen API-Version abrufen,
**damit** ich Unterschiede zwischen den Versionen prüfen kann, ohne den Server umzukonfigurieren.

## Akzeptanzkriterien

Kurzfassung, verbindlich ist die Spec (AK-1 bis AK-9):

- [ ] Alle `list_*`- und `get_*`-Tools außer `get_token` haben den optionalen Parameter `apiVersion`; ohne Angabe gilt die konfigurierte Version
- [ ] Alle Seiten eines `list_*`-Aufrufs nutzen dieselbe Version
- [ ] Ungültige Version → Fehlermeldung mit Vorschlag, kein API-Aufruf
- [ ] Nicht lesbare Antwort oder API-Ablehnung → verständliche Fehlermeldung mit Version und Ressource
- [ ] Schreibende Tools haben keinen `apiVersion`-Parameter
- [ ] README beschreibt den Parameter

## Aufgaben

- Nach US-0068 an die Software Factory übergeben (Issue #163)

## Technische Hinweise

- Nur `Member` liest v1.7- und v2.0-Format nachweislich (US-0057). Dual-Format für weitere Entities ist nicht Teil dieser Story.
- Neue Tool-Parameter mit Standardwert anlegen (Lehre aus PR #133).
