# User Story 054: Voting-Question-Endpoint implementieren

> **GitHub Issue:** [#58 – US-0054 Voting-Question-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/58)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Abstimmungsfragen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich einzelne Fragen innerhalb von Abstimmungen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `VotingQuestion`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `VotingQuestionFields`-Konstanten
- [ ] **ValueObject `VotingQuestionFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `VotingQuestionQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListVotingQuestionsAsync`, `GetVotingQuestionAsync`, `CreateVotingQuestionAsync`, `UpdateVotingQuestionAsync`, `DeleteVotingQuestionAsync`
- [ ] **MCP-Tools:** `VotingQuestionTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `voting-question`-Endpoint analysieren
2. `VotingQuestionFields.cs`, `VotingQuestion.cs`, `VotingQuestionQuery.cs` erstellen
3. `ApiQueries.cs` um VotingQuestion-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `VotingQuestionTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Voting-Question-Endpoint: `GET/POST/PATCH/DELETE /voting-question`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
