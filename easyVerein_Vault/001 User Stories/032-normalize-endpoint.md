# User Story 032: Normalize-Endpoint implementieren

> **GitHub Issue:** [#39 – US-0032 Normalize-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/39)

## User Story

**Als** Vereinsadministrator,
**möchte ich** die Normalisierungs-Funktion über den MCP-Server nutzen können,
**damit** ich Datenbereinigung und -normalisierung vollständig über den MCP-Server durchführen kann.

## Akzeptanzkriterien

- [ ] **Entity `Normalize`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `NormalizeFields`-Konstanten
- [ ] **ValueObject `NormalizeFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `NormalizeQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListNormalizesAsync`, `GetNormalizeAsync`, `CreateNormalizeAsync`, `UpdateNormalizeAsync`, `DeleteNormalizeAsync`
- [ ] **MCP-Tools:** `NormalizeTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `normalize`-Endpoint analysieren
2. `NormalizeFields.cs`, `Normalize.cs`, `NormalizeQuery.cs` erstellen
3. `ApiQueries.cs` um Normalize-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `NormalizeTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Normalize-Endpoint: `GET/POST/PATCH/DELETE /normalize`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
