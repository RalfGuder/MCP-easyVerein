# User Story 035: AccountingPlan-Endpoint implementieren

> **GitHub Issue:** [#44 – US-0035 AccountingPlan-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/44)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Kontenpläne über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Kontenplan-Verwaltung für die Buchhaltung vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `AccountingPlan`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `AccountingPlanFields`-Konstanten
- [ ] **ValueObject `AccountingPlanFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `AccountingPlanQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListAccountingPlansAsync`, `GetAccountingPlanAsync`, `CreateAccountingPlanAsync`, `UpdateAccountingPlanAsync`, `DeleteAccountingPlanAsync`
- [ ] **MCP-Tools:** `AccountingPlanTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `accounting-plan`-Endpoint analysieren
2. `AccountingPlanFields.cs`, `AccountingPlan.cs`, `AccountingPlanQuery.cs` erstellen
3. `ApiQueries.cs` um AccountingPlan-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `AccountingPlanTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- AccountingPlan-Endpoint: `GET/POST/PATCH/DELETE /accounting-plan`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
