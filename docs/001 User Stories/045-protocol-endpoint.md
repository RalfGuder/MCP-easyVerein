# User Story 045: Protocol-Endpoint implementieren

> **GitHub Issue:** [#51 – US-0045 Protocol-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/51)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Sitzungsprotokolle über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Protokollierung von Sitzungen und Versammlungen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Protocol`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ProtocolFields`-Konstanten
- [ ] **ValueObject `ProtocolFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `ProtocolQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListProtocolsAsync`, `GetProtocolAsync`, `CreateProtocolAsync`, `UpdateProtocolAsync`, `DeleteProtocolAsync`
- [ ] **MCP-Tools:** `ProtocolTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `protocol`-Endpoint analysieren
2. `ProtocolFields.cs`, `Protocol.cs`, `ProtocolQuery.cs` erstellen
3. `ApiQueries.cs` um Protocol-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `ProtocolTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Protocol-Endpoint: `GET/POST/PATCH/DELETE /protocol`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
