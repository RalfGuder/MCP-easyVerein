# User Story 051: Task-Group-Endpoint implementieren

> **GitHub Issue:** [#50 – US-0051 Task-Group-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/50)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Aufgabengruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Gruppierung und Kategorisierung von Aufgaben vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `TaskGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `TaskGroupFields`-Konstanten
- [ ] **ValueObject `TaskGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `TaskGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListTaskGroupsAsync`, `GetTaskGroupAsync`, `CreateTaskGroupAsync`, `UpdateTaskGroupAsync`, `DeleteTaskGroupAsync`
- [ ] **MCP-Tools:** `TaskGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `task-group`-Endpoint analysieren
2. `TaskGroupFields.cs`, `TaskGroup.cs`, `TaskGroupQuery.cs` erstellen
3. `ApiQueries.cs` um TaskGroup-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `TaskGroupTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Task-Group-Endpoint: `GET/POST/PATCH/DELETE /task-group`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
