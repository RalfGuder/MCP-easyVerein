# User Story 024: Forum-Endpoint implementieren

> **GitHub Issue:** [#31 – US-0024 Forum-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/31)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Forenbeiträge über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich das Vereinsforum vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Forum`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ForumFields`-Konstanten
- [ ] **ValueObject `ForumFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `ForumQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListForumsAsync`, `GetForumAsync`, `CreateForumAsync`, `UpdateForumAsync`, `DeleteForumAsync`
- [ ] **MCP-Tools:** `ForumTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `forum`-Endpoint analysieren
2. `ForumFields.cs`, `Forum.cs`, `ForumQuery.cs` erstellen
3. `ApiQueries.cs` um Forum-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `ForumTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Forum-Endpoint: `GET/POST/PATCH/DELETE /forum`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
