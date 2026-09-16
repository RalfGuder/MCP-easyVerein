# User Story 020: Custom-Filter-Endpoint implementieren

> **GitHub Issue:** [#27 – US-0020 Custom-Filter-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/27)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Filter über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich gespeicherte Filterkriterien für Mitglieder- und Kontaktabfragen über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `CustomFilter`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomFilterFields`-Konstanten
- [ ] **ValueObject `CustomFilterFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `CustomFilterQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListCustomFiltersAsync`, `GetCustomFilterAsync`, `CreateCustomFilterAsync`, `UpdateCustomFilterAsync`, `DeleteCustomFilterAsync`
- [ ] **MCP-Tools:** `CustomFilterTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-filter`-Endpoint analysieren
2. `CustomFilterFields.cs`, `CustomFilter.cs`, `CustomFilterQuery.cs` erstellen
3. `ApiQueries.cs` um CustomFilter-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomFilterTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Filter-Endpoint: `GET/POST/PATCH/DELETE /custom-filter`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
