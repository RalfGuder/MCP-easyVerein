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

## Code-Konventionen

- Alle Members (inkl. private) müssen englische XML-Dokumentationskommentare (`/// <summary>`) haben

## Architektur

### Technologie-Stack

- **Sprache:** C# / .NET 8.0
- **MCP SDK:** ModelContextProtocol v1.2.0 (Stdio-Transport)
- **Testing:** xUnit 2.4.2, Moq 4.20.72, coverlet
- **CI/CD:** GitHub Actions (Ubuntu, Windows, macOS) + Nightly Build
- **Container:** Docker (SDK 8.0 → Runtime 8.0-alpine)

### Clean Architecture (4 Schichten)

1. **Domain** (`MCP.EasyVerein.Domain`) — Entities, Interfaces, Value Objects (keine externen Abhängigkeiten)
2. **Application** (`MCP.EasyVerein.Application`) — Konfiguration (CLI/Env/Defaults)
3. **Infrastructure** (`MCP.EasyVerein.Infrastructure`) — EasyVereinApiClient (HTTP), Query-Builder
4. **Server** (`MCP.EasyVerein.Server`) — MCP-Tools, DI-Setup, Program.cs

### Muster für neue Endpoints

Neue Endpoints folgen immer dem gleichen Ablauf:
1. Domain-Entity + Fields-ValueObject anlegen
2. `IEasyVereinApiClient`-Interface erweitern
3. `EasyVereinApiClient`-Implementierung + Query-Builder
4. Server-Tool-Klasse mit MCP-Attributen
5. Tool in `Program.cs` registrieren (`.WithTools<T>()`)

### Konfiguration (Priorität)

1. CLI-Parameter (`--api-key`, `--api-url`, `--api-version`)
2. Umgebungsvariablen (`EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`)
3. Defaults (URL: `https://easyverein.com/api`, Version: `v1.7`)

### API-Versionen

Unterstützt: v1.4, v1.5, v1.6, v1.7, v2.0 (Default: v1.7)

## Projektstatus

### Implementierte Endpoints (8)

| Endpoint       | User Story | MCP-Tools                                                |
|----------------|------------|----------------------------------------------------------|
| Member         | US-0001    | list, get, create, update (PATCH), delete                |
| ContactDetails | US-0001    | list, get, create, update (PATCH), delete                |
| Invoice        | US-0001    | list, get, create, delete                                |
| Event          | US-0001    | list, get, create, update (PATCH), delete                |
| Booking        | US-0009    | list, get, create, update (PATCH), delete                |
| Calendar       | US-0014    | list, get, create, update (PATCH), delete                |
| Announcement   | US-0010    | list, get, create, update (PATCH), delete                |
| BankAccount    | US-0011    | list, get, create, update (PATCH), delete                |

### HTTP-Methoden in der easyVerein API v1.7

- **PATCH** (partielles Update): Alle 53 Ressourcen unterstützen PATCH
- **PUT** (vollständiges Ersetzen): 28 Ressourcen unterstützen zusätzlich PUT
- **Nur PATCH** (kein PUT): booking-project, contact-details-log, custom-field-collection, custom-filter, custom-tax-rate, dosb-sport, invoice-item, organization-settings, organization-token, pass, pass-field, pass-template, passcreator-integration, protocol-upload, task-comment, voting, voting-question, wastebasket
- Unsere MCP-Tools nutzen ausschließlich **PATCH** (`HttpClient.PatchAsync`) für Updates
- Invoice hat noch kein Update-Tool implementiert

### Nächste anstehende Endpoints

- US-0012: Billing Account
- US-0013: Booking Project
- US-0015: Chairman Level
- US-0016: Contact Details Group

### Teststruktur

- **Domain.Tests** — Entity- und Value-Object-Tests (28)
- **Application.Tests** — Konfigurationsauflösung (13)
- **Infrastructure.Tests** — HTTP-Client mit gemocktem HttpMessageHandler (29)
- **Server.Tests** — noch leer (Platzhalter)
- **Gesamt: 70 Tests**

## Repository

- Owner: `RalfGuder`
- Repo: `MCP-easyVerein`
