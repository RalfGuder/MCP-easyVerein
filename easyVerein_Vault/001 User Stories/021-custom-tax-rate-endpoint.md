# User Story 021: Custom-Tax-Rate-Endpoint implementieren

> **GitHub Issue:** [#28 – US-0021 Custom-Tax-Rate-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/28)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Steuersätze über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Steuerkonfiguration für die Rechnungsstellung vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `CustomTaxRate`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomTaxRateFields`-Konstanten
- [ ] **ValueObject `CustomTaxRateFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `CustomTaxRateQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListCustomTaxRatesAsync`, `GetCustomTaxRateAsync`, `CreateCustomTaxRateAsync`, `UpdateCustomTaxRateAsync`, `DeleteCustomTaxRateAsync`
- [ ] **MCP-Tools:** `CustomTaxRateTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-tax-rate`-Endpoint analysieren
2. `CustomTaxRateFields.cs`, `CustomTaxRate.cs`, `CustomTaxRateQuery.cs` erstellen
3. `ApiQueries.cs` um CustomTaxRate-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomTaxRateTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Tax-Rate-Endpoint: `GET/POST/PATCH/DELETE /custom-tax-rate`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
