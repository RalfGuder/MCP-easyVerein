# User Story 017: Contact-Details-Log-Endpoint implementieren

> **GitHub Issue:** [#24 – US-0017 Contact-Details-Log-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/24)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Änderungsprotokolle von Kontaktdaten über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Änderungen an Kontaktdaten nachvollziehen und die Datenpflege über den MCP-Server auditieren kann.

## Akzeptanzkriterien

- [x] **Entity `ContactDetailsLog`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ContactDetailsLogFields`-Konstanten
- [x] **ValueObject `ContactDetailsLogFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `ContactDetailsLogQuery.cs`:** Filterung nach ID, Datum und Kontakt-ID
- [x] **API-Client:** `ListContactDetailsLogsAsync`, `GetContactDetailsLogAsync`, `CreateContactDetailsLogAsync`, `UpdateContactDetailsLogAsync`, `DeleteContactDetailsLogAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [x] **MCP-Tools:** `ContactDetailsLogTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `contact-details-log`-Endpoint analysieren
2. `ContactDetailsLogFields.cs` als ValueObject anlegen
3. `ContactDetailsLog.cs` Entity mit `ContactDetailsLogFields`-Konstanten erstellen
4. `ContactDetailsLogQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um ContactDetailsLog-Query erweitern
6. `IEasyVereinApiClient` um ContactDetailsLog-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `ContactDetailsLogTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um ContactDetailsLog-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Contact-Details-Log-Endpoint: `GET/POST/PATCH/DELETE /contact-details-log`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
- FK-Felder `creator`/`relatedAddress`: `long?` mit neuem `FlexibleIdConverter` (liest Number oder URL-String → ID via `UrlReference.ExtractId`, schreibt Integer)
- `date` nutzt `FlexibleDateTimeConverter`

**Status:** ✅ Implementiert auf Branch `feature/US-0017-contact-details-log` (TDD). +17 Tests (8 Domain inkl. 6 `FlexibleIdConverter`, 9 Infrastructure), gesamt 213 grün.
