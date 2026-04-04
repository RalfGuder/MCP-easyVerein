# User Story 041: PassTemplate-Endpoint implementieren

> **GitHub Issue:** [#59 – US-0041 PassTemplate-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/59)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Pass-Vorlagen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Vorlagen für digitale Pässe vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `PassTemplate`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `PassTemplateFields`-Konstanten
- [ ] **ValueObject `PassTemplateFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `PassTemplateQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListPassTemplatesAsync`, `GetPassTemplateAsync`, `CreatePassTemplateAsync`, `UpdatePassTemplateAsync`, `DeletePassTemplateAsync`
- [ ] **MCP-Tools:** `PassTemplateTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `pass-template`-Endpoint analysieren
2. `PassTemplateFields.cs`, `PassTemplate.cs`, `PassTemplateQuery.cs` erstellen
3. `ApiQueries.cs` um PassTemplate-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `PassTemplateTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- PassTemplate-Endpoint: `GET/POST/PATCH/DELETE /pass-template`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
