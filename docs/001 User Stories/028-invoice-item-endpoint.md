# User Story 028: Invoice-Item-Endpoint implementieren

> **GitHub Issue:** [#35 – US-0028 Invoice-Item-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/35)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Rechnungspositionen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich einzelne Positionen auf Rechnungen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `InvoiceItem`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `InvoiceItemFields`-Konstanten
- [ ] **ValueObject `InvoiceItemFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `InvoiceItemQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListInvoiceItemsAsync`, `GetInvoiceItemAsync`, `CreateInvoiceItemAsync`, `UpdateInvoiceItemAsync`, `DeleteInvoiceItemAsync`
- [ ] **MCP-Tools:** `InvoiceItemTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `invoice-item`-Endpoint analysieren
2. `InvoiceItemFields.cs`, `InvoiceItem.cs`, `InvoiceItemQuery.cs` erstellen
3. `ApiQueries.cs` um InvoiceItem-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `InvoiceItemTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Invoice-Item-Endpoint: `GET/POST/PATCH/DELETE /invoice-item`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
