# User Story 025: Get-Token-Endpoint implementieren

> **GitHub Issue:** [#32 – US-0025 Get-Token-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/32)

## User Story

**Als** Vereinsadministrator,
**möchte ich** API-Tokens über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Token-Verwaltung für API-Zugriffe vollständig über den MCP-Server durchführen kann.

## Akzeptanzkriterien

- [ ] **Entity `GetToken`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `GetTokenFields`-Konstanten
- [ ] **ValueObject `GetTokenFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `GetTokenQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListGetTokensAsync`, `GetGetTokenAsync`, `CreateGetTokenAsync`, `UpdateGetTokenAsync`, `DeleteGetTokenAsync`
- [ ] **MCP-Tools:** `GetTokenTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `get-token`-Endpoint analysieren
2. `GetTokenFields.cs`, `GetToken.cs`, `GetTokenQuery.cs` erstellen
3. `ApiQueries.cs` um GetToken-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `GetTokenTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Get-Token-Endpoint: `GET/POST/PATCH/DELETE /get-token`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
