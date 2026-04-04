# User Story 049: Task-Endpoint implementieren

> **GitHub Issue:** [#45 – US-0049 Task-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/45)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Aufgaben über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Aufgabenverwaltung des Vereins vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `EasyVereinTask`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `EasyVereinTaskFields`-Konstanten
- [ ] **ValueObject `EasyVereinTaskFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `EasyVereinTaskQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListEasyVereinTasksAsync`, `GetEasyVereinTaskAsync`, `CreateEasyVereinTaskAsync`, `UpdateEasyVereinTaskAsync`, `DeleteEasyVereinTaskAsync`
- [ ] **MCP-Tools:** `EasyVereinTaskTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `task`-Endpoint analysieren
2. `EasyVereinTaskFields.cs`, `EasyVereinTask.cs`, `EasyVereinTaskQuery.cs` erstellen
3. `ApiQueries.cs` um EasyVereinTask-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `EasyVereinTaskTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Task-Endpoint: `GET/POST/PATCH/DELETE /task`
- Hinweis: C#-Klassenname `EasyVereinTask` statt `Task`, um Namenskonflikte mit `System.Threading.Tasks.Task` zu vermeiden.
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
