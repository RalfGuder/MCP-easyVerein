# User Story 026: Inventory-Object-Endpoint implementieren

> **GitHub Issue:** [#33 – US-0026 Inventory-Object-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/33)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Inventarobjekte über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Inventarverwaltung des Vereins vollständig über den MCP-Server durchführen kann.

## Akzeptanzkriterien

- [x] **Entity `InventoryObject`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `InventoryObjectFields`-Konstanten
- [x] **ValueObject `InventoryObjectFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `InventoryObjectQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListInventoryObjectsAsync`, `GetInventoryObjectAsync`, `CreateInventoryObjectAsync`, `UpdateInventoryObjectAsync`, `DeleteInventoryObjectAsync`
- [x] **MCP-Tools:** `InventoryObjectTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. [x] easyVerein API-Dokumentation für den `inventory-object`-Endpoint analysieren
2. [x] `InventoryObjectFields.cs`, `InventoryObject.cs`, `InventoryObjectQuery.cs` erstellen
3. [x] ~~`ApiQueries.cs` um InventoryObject-Query erweitern~~ – entfällt seit US-0062 (Query wird pro Aufruf erzeugt, `FieldQuery` als `internal const`)
4. [x] `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. [x] `InventoryObjectTools.cs` als MCP-Tool-Klasse erstellen
6. [x] `Program.cs` um Registrierung erweitern
7. [x] Unit-Tests schreiben (+27: 3 Domain, 11 Infrastructure, 13 Server → 394 gesamt)
8. [ ] Manuelle Verifikation gegen die easyVerein API – lesend erledigt, schreibend offen

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoint: `/inventory-object` (`GET`, `POST`) und `/inventory-object/{pk}` (`GET`, `PUT`, `PATCH`, `DELETE`); die Tools nutzen nur `PATCH`
- `DELETE` verschiebt das Objekt in den Papierkorb (`wastebasket/inventory-object/{pk}`), endgültiges Löschen dort per `DELETE`
- Laut `OPTIONS` gibt es **keine Pflichtfelder**
- Schreibbare Felder: `name`, `identifier`, `locationName` (je max. 500 Zeichen, vorab geprüft), `description`, `pieces`, `price`, `purchaseDate`, `lendingAvailable`, `lendingResponsible`
- `lendingResponsible` ist laut Spec schreibbar (integer), laut `OPTIONS` read-only → live zu klären
- Nur lesend gemappt: `org`, `picture` (Upload bräuchte Multipart), `currentlyLend`, `lendings`, `customFields`, `inventoryObjectGroups`, `locationObject`, `lastLendDate`, `lastReturnDate`, `created_at`, `updated_at`, `_deleteAfterDate`, `_deletedBy`
- Die API liefert Fremdschlüssel als URL (`lendingResponsible`, `locationObject`) → `FlexibleIdConverter`; `price` als String → `FlexibleDecimalConverter`; `lastLendDate`/`lastReturnDate` nur als Datum → `FlexibleDateTimeConverter`
- `purchaseDate` wird im Tool geprüft (`YYYY-MM-DD` oder ISO 8601)
- Filter: `id__in`, `name`, `identifier`, `lendingAvailable`, `deleted`, `locationObject(__not)`, `inventoryObjectGroups(__not)`, `lending__state`, `ordering`, `search` (durchsucht `name`, `description`, `locationName`); `customfilter`, `usesessionfilter`, `showReminders` bewusst nicht umgesetzt
- Nicht im Umfang: Unterressource `custom-fields`, `bulk-create`/`bulk-update`, `mass-action`

## Live-Verifikation (2026-09-17)

- Lesend über die gebauten DLLs: `list` liefert 1 Objekt („Zelt“, 335646309). Der Feld-Selektor wird mit vorhandenem Datensatz akzeptiert (alle 23 Felder gültig).
- Filter `name` + `lendingAvailable` + `deleted=false` → 1, `search=Zelt` → 1, `deleted=true` → 0.
- `get` 335646309: URL-Fremdschlüssel und Preis-String korrekt umgewandelt; `get` 1 → „not found“.
- Schreibend (Anlegen, PATCH, `lendingResponsible`, Löschen mit Papierkorb): **offen**.
