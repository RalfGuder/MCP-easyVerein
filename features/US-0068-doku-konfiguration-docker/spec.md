# US-0068: Dokumentation vervollständigen und Konfigurationsbeispiele korrigieren

**Status:** Planned
**Erstellt:** 2026-11-28

## Abhängigkeiten
- Keine. Restpunkte aus US-0001 (#1), US-0005 (#5), US-0006 (#6); Issue #160, Vault-Story 068.
- Die fachlichen Erweiterungen sind eigene Specs: US-0069 (Konfigurationsdatei), US-0070 (API-Version pro Tool-Aufruf), US-0071 (HTTP-Transport). Diese Spec beschreibt nur den heutigen Stand.

## Ausgangslage
- Das README erklärt weder die Konfiguration noch die API-Versionen noch den Aufbau des Projekts. Eine Übersicht der Endpoints steht nur in `CLAUDE.md`.
- Die Beispiele passen nicht zum Code. Der Code liest `EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL` und `EASYVEREIN_API_VERSION`.
  - `Dockerfile` setzt `EASYVEREIN_API_TOKEN` und `EASYVEREIN_BASE_URL`.
  - Das README zeigt `docker run --env EASYVEREIN_API_TOKEN=<token> mcp-easyverein`, ohne `-i`. Ein stdio-Server beendet sich ohne offene Standardeingabe sofort.
  - Das README zeigt `dotnet run` ohne `--project`.
  - `.mcp.json.example` trägt `"Bearer <token>"` als Schlüssel ein. Der Client setzt `Bearer ` aber selbst davor, so entsteht `Bearer Bearer …`.
- Es gibt keine dokumentierte Architekturentscheidung zu C# / .NET 8.

## User Stories
- Als Betreiber, der den Server einrichtet, möchte ich im README nachlesen können, wie ich Schlüssel, URL und API-Version setze, damit ich den Server ohne Blick in den Code starte.
- Als Betreiber mit Docker möchte ich ein Beispiel, das wirklich funktioniert, damit Claude Code den Container als MCP-Server nutzen kann.
- Als Entwickler, der einen neuen Endpoint anbindet, möchte ich den Aufbau des Projekts und die Gründe für C# / .NET 8 nachlesen können, damit ich mich an die Architektur halte.
- Als Betreiber mit einer alten `.mcp.json` (Schlüssel mit `Bearer `-Präfix) möchte ich, dass der Server trotzdem funktioniert, damit ich nach dem Update nichts umstellen muss.

## Außerhalb des Umfangs
- Konfigurationsdatei (US-0069), API-Version pro Tool-Aufruf (US-0070), HTTP/SSE-Transport (US-0071)
- TDD-Abschnitt, Pre-Commit-Hooks und Coverage-Schwelle (bleiben in #3)
- Beschreibung einzelner Tool-Parameter im README
- Änderungen an `CLAUDE.md`, `.github/workflows`, `Konten.md`, Daily Notes
- Englische Übersetzung des README

## Akzeptanzkriterien

### README – Konfiguration
- [ ] AK-1: Angenommen das README wird gelesen, dann hat es einen Abschnitt „Konfiguration“. Der Abschnitt nennt für Schlüssel, URL und API-Version jeweils den CLI-Parameter (`--api-key`, `--api-url`, `--api-version`), die Umgebungsvariable (`EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`) und den Standardwert (kein Schlüssel, `https://easyverein.com/api`, `v2.0`). Er nennt die Priorität CLI-Parameter vor Umgebungsvariable vor Standardwert und den Parameter `--help`.
- [ ] AK-2: Angenommen das README wird gelesen, dann sagt der Abschnitt, dass der Schlüssel ohne `Bearer `-Präfix eingetragen wird.
- [ ] AK-3: Angenommen das README wird gelesen, dann listet es die unterstützten API-Versionen v1.4, v1.5, v1.6, v1.7 und v2.0 mit v2.0 als Standard. Es beschreibt, dass eine nicht unterstützte Version beim Start mit einer Fehlermeldung samt Vorschlag der nächstmöglichen Version abgelehnt wird.

### README – Start und Beispiele
- [ ] AK-4: Angenommen das README wird gelesen, dann zeigt das Docker-Beispiel `docker run -i --rm -e EASYVEREIN_API_KEY=<token> mcp-easyverein` (sinngemäß, mit `-i`) und kein `EASYVEREIN_API_TOKEN` mehr.
- [ ] AK-5: Angenommen das README wird gelesen, dann zeigt es einen `.mcp.json`-Eintrag, mit dem Claude Code den Container startet (`command: "docker"`, `args` mit `run -i --rm -e EASYVEREIN_API_KEY …`).
- [ ] AK-6: Angenommen das README wird gelesen, dann zeigt der native Start `dotnet run --project src/MCP.EasyVerein.Server` und verweist auf `.mcp.json.example`.

### README – Projektstruktur und Endpoints
- [ ] AK-7: Angenommen das README wird gelesen, dann hat es einen Abschnitt „Projektstruktur“. Er beschreibt die vier Schichten Domain, Application, Infrastructure und Server mit ihrer Aufgabe und der Abhängigkeitsrichtung (Domain ohne externe Abhängigkeiten). Er nennt die fünf Schritte für einen neuen Endpoint: Entity mit Fields-ValueObject, Interface, Client mit Query-Builder, Tool-Klasse, Registrierung in `Program.cs`. Er verlinkt die ADR aus AK-11.
- [ ] AK-8: Angenommen das README wird gelesen, dann enthält es eine Tabelle aller angebundenen Endpoints mit den verfügbaren Operationen (list, get, create, update, delete bzw. Sonderfälle wie `create_receipt`, `vote`, nur lesend). Die Tabelle stimmt mit den in `Program.cs` registrierten Tool-Klassen überein.

### Korrekturen
- [ ] AK-9: Angenommen das `Dockerfile` wird gelesen, dann setzt es `EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL` und `EASYVEREIN_API_VERSION` und keine Variablen `EASYVEREIN_API_TOKEN` oder `EASYVEREIN_BASE_URL` mehr. Das Image baut weiterhin (CI-Job „Docker Build“ grün).
- [ ] AK-10: Angenommen `.mcp.json.example` wird gelesen, dann steht als Schlüssel ein Platzhalter ohne `Bearer `-Präfix (z. B. `<your-easyverein-api-token-here>`).

### ADR
- [ ] AK-11: Angenommen `docs/adr/0001-csharp-dotnet8.md` wird gelesen, dann dokumentiert die Datei die Entscheidung für C# / .NET 8 (LTS) mit den Abschnitten Status, Kontext, Entscheidung, betrachtete Alternativen und Konsequenzen. Sie nennt die Plattformen Windows, Linux und macOS, die Clean Architecture mit vier Schichten, das MCP SDK und Docker.

### Verhalten: Schlüssel mit „Bearer “-Präfix
- [ ] AK-12: Angenommen der Schlüssel ist als `Bearer abc123` konfiguriert (per CLI-Parameter oder Umgebungsvariable), wenn der Server eine Anfrage an easyVerein sendet, dann lautet der Header `Authorization: Bearer abc123` und nicht `Bearer Bearer abc123`.
- [ ] AK-13: Angenommen der Schlüssel ist als `abc123` konfiguriert, dann lautet der Header `Authorization: Bearer abc123` (wie bisher).

## Edge Cases
- EC-1: Präfix in anderer Schreibweise (`bearer abc123`, `BEARER abc123`) → wird wie AK-12 entfernt.
- EC-2: Präfix mit mehreren Leerzeichen oder führenden/abschließenden Leerzeichen (`  Bearer   abc123 `) → Header `Bearer abc123`.
- EC-3: Schlüssel besteht nur aus `Bearer` oder `Bearer ` → wird wie ein fehlender Schlüssel behandelt (Verhalten wie heute bei leerem Schlüssel).
- EC-4: Schlüssel, der zufällig mit „Bearer“ ohne Leerzeichen beginnt (`Bearerxyz`) → bleibt unverändert.

## Technische Anforderungen
- README bleibt Deutsch; bestehende Abschnitte (Antwortformat, Branch-Strategie, Lizenz) bleiben erhalten.
- Keine echten Schlüssel in Beispielen; nur Platzhalter.
- Kein Aufruf der echten easyVerein-API in Tests.

## Offene Fragen
- Keine.

## Product Decisions
- Vier Teile von #160 als eigene Specs (US-0068 bis US-0071): Jeder Teil ist einzeln testbar und auslieferbar (2026-11-28).
- ADR unter `docs/adr/`, nicht im Vault: technische Doku gehört ins Repo neben `docs/stack.md` (2026-11-28).
- „Bearer “-Präfix wird toleriert statt abgelehnt: Alte Konfigurationen nach dem bisherigen Beispiel laufen weiter (2026-11-28).
- Docker-Beispiel mit `-i --rm` und `.mcp.json`-Eintrag: Ohne offene Standardeingabe beendet sich ein stdio-Server sofort (2026-11-28).
- Endpoint-Tabelle im README, ohne Parameterdetails: Überblick für Betreiber, Details liefert `tools/list` (2026-11-28).
