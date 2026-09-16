# User Story 021: Custom-Tax-Rate-Endpoint implementieren

> **GitHub Issue:** [#28 – US-0021 Custom-Tax-Rate-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/28)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Steuersätze über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Steuerkonfiguration für die Rechnungsstellung vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `CustomTaxRate`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomTaxRateFields`-Konstanten
- [x] **ValueObject `CustomTaxRateFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `CustomTaxRateQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListCustomTaxRatesAsync`, `GetCustomTaxRateAsync`, `CreateCustomTaxRateAsync`, `UpdateCustomTaxRateAsync`, `DeleteCustomTaxRateAsync`
- [x] **MCP-Tools:** `CustomTaxRateTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-tax-rate`-Endpoint analysieren
2. `CustomTaxRateFields.cs`, `CustomTaxRate.cs`, `CustomTaxRateQuery.cs` erstellen
3. ~~`ApiQueries.cs` um CustomTaxRate-Query erweitern~~ (entfällt seit US-0062: Query-Instanz pro Aufruf)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomTaxRateTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Tax-Rate-Endpoint: `GET/POST/PATCH/DELETE /custom-tax-rate` (nur PATCH; PUT → HTTP 405)
- Felder: `id`, `taxName` (Pflicht, max. 600 Zeichen), `customTaxRate` (Pflicht, 0–100, kommt als String-Dezimalzahl), `countryCode`, `org`, `created_at`, `updated_at` (letzte vier nur lesbar, beim Schreiben ausgelassen). Property heißt `CustomTaxRateValue`, weil ein Member nicht wie die Klasse heißen darf.
- Filter: `id__in`, `taxName`, `taxName__ne`, `customTaxRate`, `customTaxRate__ne`, `org__isnull`, `deleted`, `showAllowedToUse`, `ordering`, `search` (nur `taxName`)

### Live-Befunde (2026-09-16, API v2.0)

- Die Liste enthält **108 Standardsätze** (EU-Länder, `org: null`, `taxName` leer) plus vereinseigene Sätze (`org` gesetzt, `countryCode` leer). Der Verein hat derzeit **keine** eigenen Sätze.
- `showAllowedToUse=true` → die im Verein nutzbaren Sätze: DE 0/5/7/16/19 % plus eigene Sätze.
- Ein Filter `countryCode` wird **ignoriert** (steht nicht in der Spec) → nicht angeboten.
- `taxName` und `customTaxRate` sind beim Anlegen Pflicht; Werte über 100 → HTTP 400. Die Tools prüfen den Bereich 0–100 vorab.
- DELETE verschiebt in den Papierkorb; endgültig löschen über `DELETE /wastebasket/custom-tax-rate/{pk}`.
- End-to-End mit den gebauten Tool-Klassen verifiziert (list inkl. Pagination und Filter, get, create, update, delete); der Test-Satz wurde vollständig entfernt.
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
