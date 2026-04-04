# User Story 023: Feature-Request-Endpoint implementieren

> **GitHub Issue:** [#30 – US-0023 Feature-Request-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/30)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Feature-Requests über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Funktionswünsche und Verbesserungsvorschläge der Mitglieder vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `FeatureRequest`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `FeatureRequestFields`-Konstanten
- [ ] **ValueObject `FeatureRequestFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `FeatureRequestQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListFeatureRequestsAsync`, `GetFeatureRequestAsync`, `CreateFeatureRequestAsync`, `UpdateFeatureRequestAsync`, `DeleteFeatureRequestAsync`
- [ ] **MCP-Tools:** `FeatureRequestTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `feature-request`-Endpoint analysieren
2. `FeatureRequestFields.cs`, `FeatureRequest.cs`, `FeatureRequestQuery.cs` erstellen
3. `ApiQueries.cs` um FeatureRequest-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `FeatureRequestTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Feature-Request-Endpoint: `GET/POST/PATCH/DELETE /feature-request`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
