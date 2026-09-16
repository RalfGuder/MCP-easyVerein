# User Story 022: DOSB-Sport-Endpoint implementieren

> **GitHub Issue:** [#29 – US-0022 DOSB-Sport-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/29)

## User Story

**Als** Vereinsadministrator,
**möchte ich** DOSB-Sportarten über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Sportarten-Zuordnung gemäß DOSB-Katalog vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `DosbSport`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `DosbSportFields`-Konstanten
- [ ] **ValueObject `DosbSportFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `DosbSportQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListDosbSportsAsync`, `GetDosbSportAsync`, `CreateDosbSportAsync`, `UpdateDosbSportAsync`, `DeleteDosbSportAsync`
- [ ] **MCP-Tools:** `DosbSportTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `dosb-sport`-Endpoint analysieren
2. `DosbSportFields.cs`, `DosbSport.cs`, `DosbSportQuery.cs` erstellen
3. `ApiQueries.cs` um DosbSport-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `DosbSportTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- DOSB-Sport-Endpoint: `GET/POST/PATCH/DELETE /dosb-sport`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
