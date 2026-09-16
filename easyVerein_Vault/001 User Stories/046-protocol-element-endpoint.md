# User Story 046: ProtocolElement-Endpoint implementieren

> **GitHub Issue:** [#57 – US-0046 ProtocolElement-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/57)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Protokollelemente (Tagesordnungspunkte) über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich einzelne Tagesordnungspunkte in Sitzungsprotokollen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `ProtocolElement`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ProtocolElementFields`-Konstanten
- [ ] **ValueObject `ProtocolElementFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `ProtocolElementQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListProtocolElementsAsync`, `GetProtocolElementAsync`, `CreateProtocolElementAsync`, `UpdateProtocolElementAsync`, `DeleteProtocolElementAsync`
- [ ] **MCP-Tools:** `ProtocolElementTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `protocol-element`-Endpoint analysieren
2. `ProtocolElementFields.cs`, `ProtocolElement.cs`, `ProtocolElementQuery.cs` erstellen
3. `ApiQueries.cs` um ProtocolElement-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `ProtocolElementTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- ProtocolElement-Endpoint: `GET/POST/PATCH/DELETE /protocol-element`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
