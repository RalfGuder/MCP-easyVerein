# User Story 020: Custom-Filter-Endpoint implementieren

> **GitHub Issue:** [#27 – US-0020 Custom-Filter-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/27)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Filter über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich gespeicherte Filterkriterien für Mitglieder- und Kontaktabfragen über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `CustomFilter`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomFilterFields`-Konstanten
- [x] **ValueObject `CustomFilterFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `CustomFilterQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListCustomFiltersAsync`, `GetCustomFilterAsync`, `CreateCustomFilterAsync`, `UpdateCustomFilterAsync`, `DeleteCustomFilterAsync`
- [x] **MCP-Tools:** `CustomFilterTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-filter`-Endpoint analysieren
2. `CustomFilterFields.cs`, `CustomFilter.cs`, `CustomFilterQuery.cs` erstellen
3. ~~`ApiQueries.cs` um CustomFilter-Query erweitern~~ (entfällt seit US-0062: Query-Instanz pro Aufruf)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomFilterTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Filter-Endpoint: `GET/POST/PATCH/DELETE /custom-filter` (nur PATCH; PUT → HTTP 405)
- Felder: `id`, `name` (max. 64 Zeichen), `model`, `rules`, `created_at`, `updated_at` (automatisch gesetzt, beim Schreiben ausgelassen); Filter: `id__in`, `name`, `model`, `model__in`, `ordering`, `search` (nur `name`)
- `model`-Werte: `userFilter`, `userStatisticsFilter`, `addressStatisticsFilter`, `inventoryStatisticsFilter`, `bookingFilter`, `addressFilter`, `invoiceFilter`, `eventFilter`, `inventoryFilter`, `oposFilter`, `protocolFilter`, `addressLogFilter`, `taskFilter`, `mailingsFilter`, `changeLogsFilter`, `emailLogsFilter`, `loginLogsFilter`

### Live-Befunde (2026-09-16, API v2.0), abweichend von der Spec

- **`rules` ist ein JSON-Objekt**, kein String (die Spec sagt „string“). Als String gesendet → HTTP 400 „… Es müssen die Schlüssel "condition" und "rules" angegeben sein!“. In der Entity als `JsonElement?` abgebildet; die Tools nehmen JSON-Text entgegen und senden ihn als Objekt.
- **`model` ist beim Anlegen Pflicht**, obwohl die Spec es im POST-Body nicht führt → sonst HTTP 400 „Feld: model“.
- **`rules` per PATCH nur zusammen mit `model` änderbar**, sonst HTTP 400 „Ungültiger Wert: rules“. `update_custom_filter` ergänzt deshalb automatisch das aktuelle `model` (per GET), wenn nur `rules` übergeben wird.
- **DELETE löscht endgültig**, einen Papierkorb gibt es nicht (`/wastebasket/custom-filter/{pk}` → 404).
- End-to-End mit den gebauten Tool-Klassen verifiziert: list/get auf bestehenden Filtern, create/update (rules-only, name-only)/delete mit einem Test-Filter; die 6 Bestandsfilter blieben unverändert.
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
