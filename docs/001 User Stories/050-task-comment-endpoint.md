# User Story 050: Task-Comment-Endpoint implementieren

> **GitHub Issue:** [#48 – US-0050 Task-Comment-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/48)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Aufgaben-Kommentare über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Kommentare und Diskussionen zu Aufgaben vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `TaskComment`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `TaskCommentFields`-Konstanten
- [ ] **ValueObject `TaskCommentFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `TaskCommentQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListTaskCommentsAsync`, `GetTaskCommentAsync`, `CreateTaskCommentAsync`, `UpdateTaskCommentAsync`, `DeleteTaskCommentAsync`
- [ ] **MCP-Tools:** `TaskCommentTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `task-comment`-Endpoint analysieren
2. `TaskCommentFields.cs`, `TaskComment.cs`, `TaskCommentQuery.cs` erstellen
3. `ApiQueries.cs` um TaskComment-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `TaskCommentTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Task-Comment-Endpoint: `GET/POST/PATCH/DELETE /task-comment`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
