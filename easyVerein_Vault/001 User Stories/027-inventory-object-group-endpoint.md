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
7. [x] Unit-Tests schreiben (+30: 4 Domain, 11 Infrastructure, 15 Server → 425 gesamt)
8. [x] Manuelle Verifikation gegen die easyVerein API – lesend und schreibend erledigt

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoint: `/inventory-object-group` (`GET`, `POST`) und `/{pk}` (`GET`, `PUT`, `PATCH`, `DELETE`); die Tools nutzen nur `PATCH`
- `DELETE` verschiebt in den Papierkorb (`wastebasket/inventory-object-group/{pk}`), endgültiges Löschen dort per `DELETE`
- Schreibbare Felder: `name` (max 200), `color` (Hex, max 7), `short` (max 4) – laut Spec alle drei für Gruppen erforderlich, im Tool vorab geprüft
- Nur lesend: `id`, `org`, `created_at`, `updated_at`, `_deleteAfterDate`, `_deletedBy`, `linkedItems` (Anzahl verknüpfter Inventarobjekte → `int?`, live belegt)
- Filter: `id__in`, `name`, `color`, `short`, `deleted`, `ordering`, `search` (name, short, color)
- Nicht im Umfang: `bulk-create`/`bulk-update`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**

## Live-Verifikation (2026-09-17 lesend / 2026-09-22 schreibend)

- Lesend über die gebauten DLLs: `list` leer (Verein hat 0 Gruppen), `deleted=true` leer, `get 1` → „not found“.
- **Pflichtfelder bestätigt:** `POST {}` → 400 „Folgende Felder müssen angegeben werden: name, short, color“; `POST {"name": …}` → 400 für `short` und `color`. Die Vorabprüfung im Tool deckt sich exakt mit der API.
- **Anlegen, `get`, `list`, PATCH, alle Filter** (`name`, `color`, `short`, `search`, Nicht-Treffer) live geprüft – alle erwartungsgemäß.
- **Löschen:** `DELETE` → Papierkorb (`wastebasket/inventory-object-group/{pk}` → 200), `get` danach „not found“, Hard-Delete → 204, danach 404.
- **`linkedItems` ist eine Anzahl, kein Array:** Mit einem verknüpften Inventarobjekt springt der Wert von `0` auf `1` (auch ohne Feld-Selektor). Entity deshalb von `JsonElement?` auf `int?` umgestellt – konsistent mit `Calendar.LinkedItems`.
- **Keine Nebenwirkungen:** Inventarobjekt „Zelt“ (335646309) unverändert, danach 0 Gruppen und 0 Einträge im Papierkorb.

### Befund für eine Folge-Story

`inventory-object.inventoryObjectGroups` ist laut `OPTIONS` read-only, lässt sich aber per `PATCH` setzen (200, Verknüpfung wirksam). Derselbe Fehler in den Metadaten wie bei `lendingResponsible` in [[026-inventory-object-endpoint]]. Beim Test hat der PATCH zusätzlich das beim Anlegen automatisch gesetzte `lendingResponsible` auf `null` zurückgesetzt. Gehört in eine eigene Story, nicht in diesen PR.
