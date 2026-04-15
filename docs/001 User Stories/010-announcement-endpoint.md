# User Story 010: Announcement-Endpoint implementieren

> **GitHub Issue:** [#17 – US-0010 Announcement-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/17)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Ankündigungen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Vereinskommunikation vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `Announcement`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `AnnouncementFields`-Konstanten
- [x] **ValueObject `AnnouncementFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `AnnouncementQuery.cs`:** Filterung nach ID, Datum und Mitglied
- [x] **API-Client:** `ListAnnouncementsAsync`, `GetAnnouncementAsync`, `CreateAnnouncementAsync`, `UpdateAnnouncementAsync`, `DeleteAnnouncementAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [x] **MCP-Tools:** `AnnouncementTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `announcement`-Endpoint analysieren
2. `AnnouncementFields.cs` als ValueObject anlegen
3. `Announcement.cs` Entity mit `AnnouncementFields`-Konstanten erstellen
4. `AnnouncementQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um Announcement-Query erweitern
6. `IEasyVereinApiClient` um Announcement-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `AnnouncementTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um Announcement-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Announcement-Endpoint: `GET/POST/PATCH/DELETE /announcement`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
