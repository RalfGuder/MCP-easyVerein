# User Story 012: Billing-Account-Endpoint implementieren

> **GitHub Issue:** [#19 – US-0012 Billing-Account-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/19)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Abrechnungskonten über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Buchhaltung des Vereins vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `BillingAccount`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `BillingAccountFields`-Konstanten
- [ ] **ValueObject `BillingAccountFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `BillingAccountQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListBillingAccountsAsync`, `GetBillingAccountAsync`, `CreateBillingAccountAsync`, `UpdateBillingAccountAsync`, `DeleteBillingAccountAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [ ] **MCP-Tools:** `BillingAccountTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `billing-account`-Endpoint analysieren
2. `BillingAccountFields.cs` als ValueObject anlegen
3. `BillingAccount.cs` Entity mit `BillingAccountFields`-Konstanten erstellen
4. `BillingAccountQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um BillingAccount-Query erweitern
6. `IEasyVereinApiClient` um BillingAccount-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `BillingAccountTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um BillingAccount-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Billing-Account-Endpoint: `GET/POST/PATCH/DELETE /billing-account`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
