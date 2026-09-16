# User Story 044: PriceGroup-Endpoint implementieren

> **GitHub Issue:** [#46 – US-0044 PriceGroup-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/46)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Preisgruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Verwaltung von Preisgruppen für Beiträge und Gebühren vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `PriceGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `PriceGroupFields`-Konstanten
- [ ] **ValueObject `PriceGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `PriceGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListPriceGroupsAsync`, `GetPriceGroupAsync`, `CreatePriceGroupAsync`, `UpdatePriceGroupAsync`, `DeletePriceGroupAsync`
- [ ] **MCP-Tools:** `PriceGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `price-group`-Endpoint analysieren
2. `PriceGroupFields.cs`, `PriceGroup.cs`, `PriceGroupQuery.cs` erstellen
3. `ApiQueries.cs` um PriceGroup-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `PriceGroupTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- PriceGroup-Endpoint: `GET/POST/PATCH/DELETE /price-group`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
