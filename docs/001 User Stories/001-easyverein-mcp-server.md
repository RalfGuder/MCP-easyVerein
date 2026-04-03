# User Story 001: easyVerein API als lokalen MCP-Server bereitstellen

> **GitHub Issue:** [#1 – US-0001 easyVerein API als lokalen MCP-Server bereitstellen](https://github.com/RalfGuder/MCP-easyVerein/issues/1)

## User Story

**Als** Vereinsverwalter, der einen KI-Assistenten wie Claude nutzt,
**möchte ich** über einen lokalen MCP-Server auf die easyVerein API zugreifen können,
**damit** ich Vereinsdaten wie Mitglieder, Rechnungen und Veranstaltungen direkt aus dem KI-Assistenten heraus abfragen und verwalten kann, ohne die easyVerein-Oberfläche manuell bedienen zu müssen.

## Akzeptanzkriterien

- [x] MCP-Server startet lokal und ist über stdio erreichbar *(umgesetzt in `Program.cs` mit `WithStdioServerTransport()`)*
- [ ] MCP-Server ist über SSE erreichbar *(Could – noch nicht umgesetzt)*
- [x] Authentifizierung mit easyVerein API-Token funktioniert *(umgesetzt in `EasyVereinApiClient`, Header `Authorization: Token ...`)*
- [x] Lese-Zugriff auf Mitgliederdaten ist als MCP-Tool verfügbar *(umgesetzt: `ListMembers`, `GetMember`)*
- [x] CRUD-Operationen für Mitglieder als MCP-Tools *(umgesetzt: `ListMembers`, `GetMember`, `CreateMember`, `UpdateMember`, `DeleteMember`)*
- [x] CRUD-Operationen für Rechnungen als MCP-Tools *(umgesetzt: `ListInvoices`, `GetInvoice`, `CreateInvoice`, `DeleteInvoice`)*
- [x] CRUD-Operationen für Veranstaltungen als MCP-Tools *(umgesetzt: `ListEvents`, `GetEvent`, `CreateEvent`, `DeleteEvent`)*
- [x] CRUD-Operationen für Kontaktdaten als MCP-Tools *(umgesetzt: `ListContacts`, `GetContact`, `CreateContact`, `DeleteContact`)*
- [x] Konfiguration über Umgebungsvariablen (`EASYVEREIN_API_TOKEN`, `EASYVEREIN_BASE_URL`) *(umgesetzt in `EasyVereinConfiguration`)*
- [ ] Konfiguration über Konfigurationsdatei als Alternative *(Should – noch nicht umgesetzt)*
- [x] Dokumentation (README) mit Installations- und Konfigurationsanleitung *(Systemvoraussetzungen dokumentiert)*
- [x] Fehlerbehandlung bei ungültigen API-Tokens *(umgesetzt: `UnauthorizedAccessException` bei HTTP 401/403)*
- [x] Fehlerbehandlung bei Netzwerkfehlern *(umgesetzt: `SocketException`, `TaskCanceledException`)*

## Aufgaben

- [x] **MCP-Server implementieren**, der lokal ausgeführt wird und die easyVerein REST-API kapselt
- [x] **Authentifizierung** über easyVerein API-Token unterstützen
- [x] **CRUD-Operationen** für Mitglieder, Rechnungen, Veranstaltungen, Kontaktdaten als MCP-Tools
- [x] **Konfiguration** über Umgebungsvariablen ermöglichen
- [ ] **Konfiguration** über Konfigurationsdatei ermöglichen (Should)

## Technische Hinweise

- Implementiert mit ModelContextProtocol SDK 1.2.0 für .NET
- Clean Architecture: Domain → Application → Infrastructure → Server
- 25 xUnit-Tests, alle grün
- Dockerfile vorhanden (Multi-Stage, Alpine-basiert)
- Die easyVerein API-Dokumentation ist unter https://easyverein.com/api/ verfügbar
