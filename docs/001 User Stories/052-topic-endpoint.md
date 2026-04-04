# User Story 052: Topic-Endpoint implementieren

> **GitHub Issue:** [#53 – US-0052 Topic-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/53)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Forenthemen (Topics) über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Verwaltung von Diskussionsthemen im Vereinsforum vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Topic`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `TopicFields`-Konstanten
- [ ] **ValueObject `TopicFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `TopicQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListTopicsAsync`, `GetTopicAsync`, `CreateTopicAsync`, `UpdateTopicAsync`, `DeleteTopicAsync`
- [ ] **MCP-Tools:** `TopicTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `topic`-Endpoint analysieren
2. `TopicFields.cs`, `Topic.cs`, `TopicQuery.cs` erstellen
3. `ApiQueries.cs` um Topic-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `TopicTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Topic-Endpoint: `GET/POST/PATCH/DELETE /topic`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
