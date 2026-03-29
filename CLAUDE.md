# MCP-easyVerein – Projektregeln

## Projekt

Lokaler MCP-Server (Model Context Protocol) zur Anbindung der easyVerein API.
Lizenz: MIT.

## Sprache

- Code, Commits und technische Dokumentation: **Deutsch oder Englisch** nach Kontext
- User Stories und Issue-Kommunikation: **Deutsch**

## Branch-Strategie (GitHub Flow)

- `main` ist immer stabil und deploybar
- Für jede User Story / jedes Issue einen Feature-Branch erstellen
- Namenskonvention: `feature/US-XXXX-kurzbeschreibung` oder `fix/kurzbeschreibung`
- Änderungen über Pull Requests in `main` integrieren
- Feature-Branches nach Merge löschen

## User Stories

- Jede User Story wird als GitHub Issue **und** als Markdown-Dokument unter `docs/user stories/` gepflegt
- Dateiname: `XXX-kurzbeschreibung.md` (z.B. `001-easyverein-mcp-server.md`)
- Issue und Dokument sind **gegenseitig verlinkt**
- Format:
  - **Als** [Rolle], **möchte ich** [Funktion], **damit** [Nutzen]
  - Akzeptanzkriterien als Checkliste
  - Aufgaben
  - Technische Hinweise
- Issue-Titel: `US-XXXX Kurzbeschreibung`
- Bei neuen User Stories: iterativ Fragen stellen, um Akzeptanzkriterien zu erfassen

## TDD (Test-Driven Development)

- Jede Funktionalität wird nach dem Red-Green-Refactor-Zyklus entwickelt
- Tests werden **vor** der Implementierung geschrieben
- Mindest-Code-Coverage: **70%**
- CI/CD-Pipeline prüft Tests und Coverage vor Merge
- Pre-Commit Hooks führen Tests vor jedem Commit aus

## Commit-Nachrichten

- Präfix: `docs:`, `feat:`, `fix:`, `refactor:`, `test:`, `chore:`
- Kurze, aussagekräftige Beschreibung auf Deutsch oder Englisch
- Bei Bezug zu Issues: `Verlinkt mit GitHub Issue #X`

## Repository

- Owner: `RalfGuder`
- Repo: `MCP-easyVerein`
