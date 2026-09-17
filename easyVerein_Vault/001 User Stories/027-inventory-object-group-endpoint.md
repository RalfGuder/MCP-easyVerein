# User Story 027: Inventory-Object-Group-Endpoint implementieren

> **GitHub Issue:** [#34 – US-0027 Inventory-Object-Group-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/34)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Inventarobjekt-Gruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Kategorisierung und Gruppierung von Inventargegenständen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `InventoryObjectGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `InventoryObjectGroupFields`-Konstanten
- [x] **ValueObject `InventoryObjectGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `InventoryObjectGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListInventoryObjectGroupsAsync`, `GetInventoryObjectGroupAsync`, `CreateInventoryObjectGroupAsync`, `UpdateInventoryObjectGroupAsync`, `DeleteInventoryObjectGroupAsync`
- [x] **MCP-Tools:** `InventoryObjectGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. [x] easyVerein API-Dokumentation für den `inventory-object-group`-Endpoint analysieren
2. [x] `InventoryObjectGroupFields.cs`, `InventoryObjectGroup.cs`, `InventoryObjectGroupQuery.cs` erstellen
3. [x] ~~`ApiQueries.cs` um InventoryObjectGroup-Query erweitern~~ – entfällt seit US-0062 (Query pro Aufruf, `FieldQuery` als `internal const`)
4. [x] `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. [x] `InventoryObjectGroupTools.cs` als MCP-Tool-Klasse erstellen
6. [x] `Program.cs` um Registrierung erweitern
7. [x] Unit-Tests schreiben (+29: 3 Domain, 11 Infrastructure, 15 Server → 424 gesamt)
8. [ ] Manuelle Verifikation gegen die easyVerein API – lesend erledigt, Schreibtest offen

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoint: `/inventory-object-group` (`GET`, `POST`) und `/{pk}` (`GET`, `PUT`, `PATCH`, `DELETE`); die Tools nutzen nur `PATCH`
- `DELETE` verschiebt in den Papierkorb (`wastebasket/inventory-object-group/{pk}`), endgültiges Löschen dort per `DELETE`
- Schreibbare Felder: `name` (max 200), `color` (Hex, max 7), `short` (max 4) – laut Spec alle drei für Gruppen erforderlich, im Tool vorab geprüft
- Nur lesend: `id`, `org`, `created_at`, `updated_at`, `_deleteAfterDate`, `_deletedBy`, `linkedItems` (Struktur unbekannt → `JsonElement?`)
- Filter: `id__in`, `name`, `color`, `short`, `deleted`, `ordering`, `search` (name, short, color)
- Nicht im Umfang: `bulk-create`/`bulk-update`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**

## Live-Verifikation (2026-09-17)

- Lesend über die gebauten DLLs: `list` leer (Verein hat 0 Gruppen), `deleted=true` leer, `get 1` → „not found“.
- **Schreibtest offen:** Skript liegt bereit (`scratchpad/grp_live_write.ps1`); der Auto-Mode blockiert Schreibzugriffe, der PO startet es per `!`. Zu klären: tatsächliche Pflichtfelder und Struktur von `linkedItems`.
