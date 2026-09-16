# User Story 043: Post-Endpoint implementieren

> **GitHub Issue:** [#43 – US-0043 Post-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/43)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Forenbeiträge (Posts) über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich einzelne Beiträge im Vereinsforum vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Post`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `PostFields`-Konstanten
- [ ] **ValueObject `PostFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `PostQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListPostsAsync`, `GetPostAsync`, `CreatePostAsync`, `UpdatePostAsync`, `DeletePostAsync`
- [ ] **MCP-Tools:** `PostTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `post`-Endpoint analysieren
2. `PostFields.cs`, `Post.cs`, `PostQuery.cs` erstellen
3. `ApiQueries.cs` um Post-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `PostTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Post-Endpoint: `GET/POST/PATCH/DELETE /post`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
