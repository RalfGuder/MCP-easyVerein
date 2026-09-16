# User Story 039: Pass-Endpoint implementieren

> **GitHub Issue:** [#54 – US-0039 Pass-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/54)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Pässe/Ausweise über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Verwaltung digitaler Pässe und Ausweise vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Pass`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `PassFields`-Konstanten
- [ ] **ValueObject `PassFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `PassQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListPassesAsync`, `GetPassAsync`, `CreatePassAsync`, `UpdatePassAsync`, `DeletePassAsync`
- [ ] **MCP-Tools:** `PassTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `pass`-Endpoint analysieren
2. `PassFields.cs`, `Pass.cs`, `PassQuery.cs` erstellen
3. `ApiQueries.cs` um Pass-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `PassTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Pass-Endpoint: `GET/POST/PATCH/DELETE /pass`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
