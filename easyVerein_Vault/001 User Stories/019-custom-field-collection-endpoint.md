# User Story 019: Custom-Field-Collection-Endpoint implementieren

> **GitHub Issue:** [#26 – US-0019 Custom-Field-Collection-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/26)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Feldsammlungen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich zusammengehörige benutzerdefinierte Felder gruppieren und strukturiert über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `CustomFieldCollection`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomFieldCollectionFields`-Konstanten
- [x] **ValueObject `CustomFieldCollectionFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `CustomFieldCollectionQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListCustomFieldCollectionsAsync`, `GetCustomFieldCollectionAsync`, `CreateCustomFieldCollectionAsync`, `UpdateCustomFieldCollectionAsync`, `DeleteCustomFieldCollectionAsync`
- [x] **MCP-Tools:** `CustomFieldCollectionTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-field-collection`-Endpoint analysieren
2. `CustomFieldCollectionFields.cs`, `CustomFieldCollection.cs`, `CustomFieldCollectionQuery.cs` erstellen
3. ~~`ApiQueries.cs` um CustomFieldCollection-Query erweitern~~ (entfällt seit US-0062: Query-Instanz pro Aufruf)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomFieldCollectionTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Field-Collection-Endpoint: `GET/POST/PATCH/DELETE /custom-field-collection` (nur PATCH, kein PUT)
- Felder laut v2.0-Spec: `id`, `name` (Pflicht, max. 200 Zeichen), `orderSequence`, `position`; Filter: `id__in`, `position`, `ordering`, `search`
- Live-Verifikation (2026-09-16): Anlegen/Liste/Abruf/PATCH/DELETE erfolgreich; der Feld-Selektor `{id,name,orderSequence,position}` wird akzeptiert, unbekannte Felder (z. B. `color`) → HTTP 400. Bei **leerer** Ergebnisliste validiert die API Feldnamen nicht.
- Die API liefert zusätzlich `org`, `created_at`, `updated_at`, `_deleteAfterDate`, `_deletedBy` (nicht in der Entity abgebildet)
- Gelöschte Collections landen im Papierkorb: endgültig löschen über `DELETE /wastebasket/custom-field-collection/{pk}`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
