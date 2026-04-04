# User Story 029: Lending-Endpoint implementieren

> **GitHub Issue:** [#36 – US-0029 Lending-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/36)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Ausleihen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Ausleihe von Vereinsgegenständen an Mitglieder vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Lending`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `LendingFields`-Konstanten
- [ ] **ValueObject `LendingFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `LendingQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListLendingsAsync`, `GetLendingAsync`, `CreateLendingAsync`, `UpdateLendingAsync`, `DeleteLendingAsync`
- [ ] **MCP-Tools:** `LendingTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `lending`-Endpoint analysieren
2. `LendingFields.cs`, `Lending.cs`, `LendingQuery.cs` erstellen
3. `ApiQueries.cs` um Lending-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `LendingTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Lending-Endpoint: `GET/POST/PATCH/DELETE /lending`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
