# User Story 026: Inventory-Object-Endpoint implementieren

> **GitHub Issue:** [#33 – US-0026 Inventory-Object-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/33)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Inventarobjekte über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Inventarverwaltung des Vereins vollständig über den MCP-Server durchführen kann.

## Akzeptanzkriterien

- [ ] **Entity `InventoryObject`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `InventoryObjectFields`-Konstanten
- [ ] **ValueObject `InventoryObjectFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `InventoryObjectQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListInventoryObjectsAsync`, `GetInventoryObjectAsync`, `CreateInventoryObjectAsync`, `UpdateInventoryObjectAsync`, `DeleteInventoryObjectAsync`
- [ ] **MCP-Tools:** `InventoryObjectTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `inventory-object`-Endpoint analysieren
2. `InventoryObjectFields.cs`, `InventoryObject.cs`, `InventoryObjectQuery.cs` erstellen
3. `ApiQueries.cs` um InventoryObject-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `InventoryObjectTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Inventory-Object-Endpoint: `GET/POST/PATCH/DELETE /inventory-object`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
