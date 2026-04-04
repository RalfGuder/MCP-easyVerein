# User Story 055: Website-Endpoint implementieren

> **GitHub Issue:** [#60 – US-0055 Website-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/60)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Webseiten über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Verwaltung der Vereins-Webseiten vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Website`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `WebsiteFields`-Konstanten
- [ ] **ValueObject `WebsiteFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `WebsiteQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListWebsitesAsync`, `GetWebsiteAsync`, `CreateWebsiteAsync`, `UpdateWebsiteAsync`, `DeleteWebsiteAsync`
- [ ] **MCP-Tools:** `WebsiteTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `website`-Endpoint analysieren
2. `WebsiteFields.cs`, `Website.cs`, `WebsiteQuery.cs` erstellen
3. `ApiQueries.cs` um Website-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `WebsiteTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Website-Endpoint: `GET/POST/PATCH/DELETE /website`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
