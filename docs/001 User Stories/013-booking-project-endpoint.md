# User Story 013: Booking-Project-Endpoint implementieren

> **GitHub Issue:** [#20 – US-0013 Booking-Project-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/20)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Buchungsprojekte über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Buchungen thematisch gruppieren und die Finanzverwaltung strukturiert über den MCP-Server steuern kann.

## Akzeptanzkriterien

- [ ] **Entity `BookingProject`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `BookingProjectFields`-Konstanten
- [ ] **ValueObject `BookingProjectFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `BookingProjectQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListBookingProjectsAsync`, `GetBookingProjectAsync`, `CreateBookingProjectAsync`, `UpdateBookingProjectAsync`, `DeleteBookingProjectAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [ ] **MCP-Tools:** `BookingProjectTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `booking-project`-Endpoint analysieren
2. `BookingProjectFields.cs` als ValueObject anlegen
3. `BookingProject.cs` Entity mit `BookingProjectFields`-Konstanten erstellen
4. `BookingProjectQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um BookingProject-Query erweitern
6. `IEasyVereinApiClient` um BookingProject-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `BookingProjectTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um BookingProject-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Booking-Project-Endpoint: `GET/POST/PATCH/DELETE /booking-project`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
