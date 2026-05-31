# User Story 016: Contact-Details-Group-Endpoint implementieren

> **GitHub Issue:** [#23 – US-0016 Contact-Details-Group-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/23)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Kontaktdaten-Gruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Kontakte thematisch gruppieren und die Mitgliederverwaltung strukturiert über den MCP-Server steuern kann.

## Akzeptanzkriterien

- [x] **Entity `ContactDetailsGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ContactDetailsGroupFields`-Konstanten
- [x] **ValueObject `ContactDetailsGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `ContactDetailsGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern (per-call Instanzen gemäß US-0062)
- [x] **API-Client:** `ListContactDetailsGroupsAsync`, `GetContactDetailsGroupAsync`, `CreateContactDetailsGroupAsync`, `UpdateContactDetailsGroupAsync`, `DeleteContactDetailsGroupAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [x] **MCP-Tools:** `ContactDetailsGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests für Entity und API-Client nach TDD (Red-Green-Refactor) — 2 Domain + 9 Infrastructure

## Umsetzung (PR #104)

- Branch: `feature/US-0016-contact-details-group`
- Commit: `feat(US-0016): Contact-Details-Group-Endpoint implementieren` (76fa725)
- 11 neue Tests (2 Domain + 9 Infrastructure), Gesamt 196 grün
- API-Felder: `id`, `name`, `color`, `short`, `orderSequence` (camelCase im API)
- Filter: `name`, `color`, `short`, `deleted`, `id__in`, `ordering`, `search` (auf `name`, `short` begrenzt)
- Update ausschließlich PATCH (Endpoint ist PATCH-only, kein PUT API-seitig)

## Aufgaben

1. easyVerein API-Dokumentation für den `contact-details-group`-Endpoint analysieren
2. `ContactDetailsGroupFields.cs` als ValueObject anlegen
3. `ContactDetailsGroup.cs` Entity mit `ContactDetailsGroupFields`-Konstanten erstellen
4. `ContactDetailsGroupQuery.cs` für Standard-Filter implementieren (per-call Instanzen gemäß US-0062)
5. `IEasyVereinApiClient` um ContactDetailsGroup-CRUD-Methoden erweitern
6. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
7. `ContactDetailsGroupTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
8. `Program.cs` um ContactDetailsGroup-Tools-Registrierung erweitern
9. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
10. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Contact-Details-Group-Endpoint: `GET/POST/PATCH/DELETE /contact-details-group`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
