# User Story 040: PassField-Endpoint implementieren

> **GitHub Issue:** [#56 – US-0040 PassField-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/56)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Pass-Felder über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Felddefinitionen für digitale Pässe vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `PassField`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `PassFieldFields`-Konstanten
- [ ] **ValueObject `PassFieldFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `PassFieldQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListPassFieldsAsync`, `GetPassFieldAsync`, `CreatePassFieldAsync`, `UpdatePassFieldAsync`, `DeletePassFieldAsync`
- [ ] **MCP-Tools:** `PassFieldTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `pass-field`-Endpoint analysieren
2. `PassFieldFields.cs`, `PassField.cs`, `PassFieldQuery.cs` erstellen
3. `ApiQueries.cs` um PassField-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `PassFieldTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- PassField-Endpoint: `GET/POST/PATCH/DELETE /pass-field`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
