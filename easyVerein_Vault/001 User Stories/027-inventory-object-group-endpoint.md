# User Story 027: Inventory-Object-Group-Endpoint implementieren

> **GitHub Issue:** [#34 – US-0027 Inventory-Object-Group-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/34)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Inventarobjekt-Gruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Kategorisierung und Gruppierung von Inventargegenständen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `InventoryObjectGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `InventoryObjectGroupFields`-Konstanten
- [ ] **ValueObject `InventoryObjectGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `InventoryObjectGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListInventoryObjectGroupsAsync`, `GetInventoryObjectGroupAsync`, `CreateInventoryObjectGroupAsync`, `UpdateInventoryObjectGroupAsync`, `DeleteInventoryObjectGroupAsync`
- [ ] **MCP-Tools:** `InventoryObjectGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `inventory-object-group`-Endpoint analysieren
2. `InventoryObjectGroupFields.cs`, `InventoryObjectGroup.cs`, `InventoryObjectGroupQuery.cs` erstellen
3. `ApiQueries.cs` um InventoryObjectGroup-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `InventoryObjectGroupTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Inventory-Object-Group-Endpoint: `GET/POST/PATCH/DELETE /inventory-object-group`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
