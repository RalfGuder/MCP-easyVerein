# User Story 053: Voting-Endpoint implementieren

> **GitHub Issue:** [#55 – US-0053 Voting-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/55)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Abstimmungen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Durchführung und Verwaltung von Vereinsabstimmungen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Voting`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `VotingFields`-Konstanten
- [ ] **ValueObject `VotingFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `VotingQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListVotingsAsync`, `GetVotingAsync`, `CreateVotingAsync`, `UpdateVotingAsync`, `DeleteVotingAsync`
- [ ] **MCP-Tools:** `VotingTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `voting`-Endpoint analysieren
2. `VotingFields.cs`, `Voting.cs`, `VotingQuery.cs` erstellen
3. `ApiQueries.cs` um Voting-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `VotingTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Voting-Endpoint: `GET/POST/PATCH/DELETE /voting`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
