# User Story 047: ProtocolElementComment-Endpoint implementieren

> **GitHub Issue:** [#61 – US-0047 ProtocolElementComment-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/61)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Kommentare zu Protokollelementen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Kommentare und Anmerkungen zu einzelnen Tagesordnungspunkten vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `ProtocolElementComment`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ProtocolElementCommentFields`-Konstanten
- [ ] **ValueObject `ProtocolElementCommentFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `ProtocolElementCommentQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListProtocolElementCommentsAsync`, `GetProtocolElementCommentAsync`, `CreateProtocolElementCommentAsync`, `UpdateProtocolElementCommentAsync`, `DeleteProtocolElementCommentAsync`
- [ ] **MCP-Tools:** `ProtocolElementCommentTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `protocol-element-comment`-Endpoint analysieren
2. `ProtocolElementCommentFields.cs`, `ProtocolElementComment.cs`, `ProtocolElementCommentQuery.cs` erstellen
3. `ApiQueries.cs` um ProtocolElementComment-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `ProtocolElementCommentTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- ProtocolElementComment-Endpoint: `GET/POST/PATCH/DELETE /protocol-element-comment`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
