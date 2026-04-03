# User Story 016: Contact-Details-Group-Endpoint implementieren

> **GitHub Issue:** [#23 – US-0016 Contact-Details-Group-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/23)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Kontaktdaten-Gruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Kontakte thematisch gruppieren und die Mitgliederverwaltung strukturiert über den MCP-Server steuern kann.

## Akzeptanzkriterien

- [ ] **Entity `ContactDetailsGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ContactDetailsGroupFields`-Konstanten
- [ ] **ValueObject `ContactDetailsGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `ContactDetailsGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListContactDetailsGroupsAsync`, `GetContactDetailsGroupAsync`, `CreateContactDetailsGroupAsync`, `UpdateContactDetailsGroupAsync`, `DeleteContactDetailsGroupAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [ ] **MCP-Tools:** `ContactDetailsGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `contact-details-group`-Endpoint analysieren
2. `ContactDetailsGroupFields.cs` als ValueObject anlegen
3. `ContactDetailsGroup.cs` Entity mit `ContactDetailsGroupFields`-Konstanten erstellen
4. `ContactDetailsGroupQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um ContactDetailsGroup-Query erweitern
6. `IEasyVereinApiClient` um ContactDetailsGroup-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `ContactDetailsGroupTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um ContactDetailsGroup-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Contact-Details-Group-Endpoint: `GET/POST/PATCH/DELETE /contact-details-group`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
