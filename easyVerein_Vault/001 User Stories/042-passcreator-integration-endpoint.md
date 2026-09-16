# User Story 042: PasscreatorIntegration-Endpoint implementieren

> **GitHub Issue:** [#42 – US-0042 PasscreatorIntegration-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/42)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Passcreator-Integrationen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Passcreator-Integration für digitale Mitgliedsausweise vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `PasscreatorIntegration`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `PasscreatorIntegrationFields`-Konstanten
- [ ] **ValueObject `PasscreatorIntegrationFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `PasscreatorIntegrationQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListPasscreatorIntegrationsAsync`, `GetPasscreatorIntegrationAsync`, `CreatePasscreatorIntegrationAsync`, `UpdatePasscreatorIntegrationAsync`, `DeletePasscreatorIntegrationAsync`
- [ ] **MCP-Tools:** `PasscreatorIntegrationTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `passcreator-integration`-Endpoint analysieren
2. `PasscreatorIntegrationFields.cs`, `PasscreatorIntegration.cs`, `PasscreatorIntegrationQuery.cs` erstellen
3. `ApiQueries.cs` um PasscreatorIntegration-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `PasscreatorIntegrationTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- PasscreatorIntegration-Endpoint: `GET/POST/PATCH/DELETE /passcreator-integration`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
