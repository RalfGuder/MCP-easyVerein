# User Story 015: Chairman-Level-Endpoint implementieren

> **GitHub Issue:** [#22 – US-0015 Chairman-Level-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/22)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Vorstandsebenen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Berechtigungsstruktur des Vereins vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `ChairmanLevel`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ChairmanLevelFields`-Konstanten
- [x] **ValueObject `ChairmanLevelFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `ChairmanLevelQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListChairmanLevelsAsync`, `GetChairmanLevelAsync`, `CreateChairmanLevelAsync`, `UpdateChairmanLevelAsync`, `DeleteChairmanLevelAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [x] **MCP-Tools:** `ChairmanLevelTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `chairman-level`-Endpoint analysieren
2. `ChairmanLevelFields.cs` als ValueObject anlegen
3. `ChairmanLevel.cs` Entity mit `ChairmanLevelFields`-Konstanten erstellen
4. `ChairmanLevelQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um ChairmanLevel-Query erweitern
6. `IEasyVereinApiClient` um ChairmanLevel-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `ChairmanLevelTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um ChairmanLevel-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Chairman-Level-Endpoint: `GET/POST/PATCH/DELETE /chairman-level`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
