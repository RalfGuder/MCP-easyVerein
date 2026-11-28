# User Story 071: Lokaler HTTP-Transport

> **GitHub Issue:** [#164 – US-0071 Lokaler HTTP-Transport](https://github.com/RalfGuder/MCP-easyVerein/issues/164)
> **Spec:** `features/US-0071-http-transport-lokal/spec.md` (Vertrag für die Software Factory)
> **Herkunft:** [[068-doku-und-konfiguration-vervollstaendigen|US-0068]] (aufgeteilt am 2026-11-28), ursprünglich [[001-easyverein-mcp-server|US-0001 (#1)]] („über SSE erreichbar“)
> **Abhängig von:** US-0068, US-0069 (Felder `transport` und `port` in der Konfigurationsdatei)

## User Story

**Als** Betreiber,
**möchte ich** den Server einmal lokal starten und mehrere MCP-Clients auf demselben Rechner per HTTP verbinden,
**damit** nicht jede Sitzung einen eigenen Prozess braucht, ohne den Server für das Netz zu öffnen.

## Akzeptanzkriterien

Kurzfassung, verbindlich ist die Spec (AK-1 bis AK-14):

- [ ] `--transport stdio|http` (Standard stdio), `--port` (Standard 8765), auch per Umgebungsvariable und Konfigurationsdatei
- [ ] HTTP unter `http://127.0.0.1:8765/mcp`, nur über Loopback erreichbar
- [ ] Fremder `Origin`-Header → HTTP 403, kein Tool wird ausgeführt
- [ ] Ungültiger Transport/Port oder belegter Port → Start bricht mit Meldung ab
- [ ] Gleiche Tools und Prompts wie über stdio; mehrere Clients gleichzeitig
- [ ] `--help` und README beschreiben den HTTP-Start; Docker bleibt stdio

## Aufgaben

- Nach US-0069 an die Software Factory übergeben (Issue #164)

## Technische Hinweise

- Im MCP-Standard ist der eigenständige SSE-Transport durch Streamable HTTP abgelöst. In .NET braucht er das Paket `ModelContextProtocol.AspNetCore`.
- Netzbetrieb (Auth, TLS) ist bewusst ausgeschlossen: Der Server hält einen Vereinsschlüssel mit Schreibrechten.
