# User Story 014: Calendar-Endpoint implementieren

> **GitHub Issue:** [#21 – US-0014 Calendar-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/21)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Kalendereinträge über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Terminverwaltung des Vereins vollständig über den MCP-Server steuern kann.

## Akzeptanzkriterien

- [ ] **Entity `Calendar`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CalendarFields`-Konstanten
- [ ] **ValueObject `CalendarFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `CalendarQuery.cs`:** Filterung nach ID, Datum und weiteren Standard-Feldern
- [ ] **API-Client:** `ListCalendarsAsync`, `GetCalendarAsync`, `CreateCalendarAsync`, `UpdateCalendarAsync`, `DeleteCalendarAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [ ] **MCP-Tools:** `CalendarTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `calendar`-Endpoint analysieren
2. `CalendarFields.cs` als ValueObject anlegen
3. `Calendar.cs` Entity mit `CalendarFields`-Konstanten erstellen
4. `CalendarQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um Calendar-Query erweitern
6. `IEasyVereinApiClient` um Calendar-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `CalendarTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um Calendar-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Calendar-Endpoint: `GET/POST/PATCH/DELETE /calendar`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
