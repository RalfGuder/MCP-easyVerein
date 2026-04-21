# User Story 011: Bank-Account-Endpoint implementieren

> **GitHub Issue:** [#18 – US-0011 Bank-Account-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/18)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Bankkonten über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Finanzverwaltung des Vereins vollständig über den MCP-Server steuern kann.

## Akzeptanzkriterien

- [x] **Entity `BankAccount`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `BankAccountFields`-Konstanten
- [x] **ValueObject `BankAccountFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `BankAccountQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListBankAccountsAsync`, `GetBankAccountAsync`, `CreateBankAccountAsync`, `UpdateBankAccountAsync`, `DeleteBankAccountAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [x] **MCP-Tools:** `BankAccountTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `bank-account`-Endpoint analysieren
2. `BankAccountFields.cs` als ValueObject anlegen
3. `BankAccount.cs` Entity mit `BankAccountFields`-Konstanten erstellen
4. `BankAccountQuery.cs` für Standard-Filter implementieren
5. `ApiQueries.cs` um BankAccount-Query erweitern
6. `IEasyVereinApiClient` um BankAccount-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `BankAccountTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um BankAccount-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Bank-Account-Endpoint: `GET/POST/PATCH/DELETE /bank-account`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>`
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**

**Status:** Implementiert am 2026-04-20 auf Branch `feature/US-0011-bank-account-endpoint`.
