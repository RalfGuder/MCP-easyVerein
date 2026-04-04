# User Story 030: Location-Endpoint implementieren

> **GitHub Issue:** [#37 – US-0030 Location-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/37)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Veranstaltungsorte über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Standortverwaltung des Vereins vollständig über den MCP-Server durchführen kann.

## Akzeptanzkriterien

- [ ] **Entity `Location`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `LocationFields`-Konstanten
- [ ] **ValueObject `LocationFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `LocationQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListLocationsAsync`, `GetLocationAsync`, `CreateLocationAsync`, `UpdateLocationAsync`, `DeleteLocationAsync`
- [ ] **MCP-Tools:** `LocationTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `location`-Endpoint analysieren
2. `LocationFields.cs`, `Location.cs`, `LocationQuery.cs` erstellen
3. `ApiQueries.cs` um Location-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `LocationTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Location-Endpoint: `GET/POST/PATCH/DELETE /location`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
