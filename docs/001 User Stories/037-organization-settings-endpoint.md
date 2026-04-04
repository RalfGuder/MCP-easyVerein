# User Story 037: OrganizationSettings-Endpoint implementieren

> **GitHub Issue:** [#49 – US-0037 OrganizationSettings-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/49)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Organisationseinstellungen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Einstellungen der Organisation vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `OrganizationSettings`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `OrganizationSettingsFields`-Konstanten
- [ ] **ValueObject `OrganizationSettingsFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `OrganizationSettingsQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListOrganizationSettingsAsync`, `GetOrganizationSettingsAsync`, `CreateOrganizationSettingsAsync`, `UpdateOrganizationSettingsAsync`, `DeleteOrganizationSettingsAsync`
- [ ] **MCP-Tools:** `OrganizationSettingsTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `organization-settings`-Endpoint analysieren
2. `OrganizationSettingsFields.cs`, `OrganizationSettings.cs`, `OrganizationSettingsQuery.cs` erstellen
3. `ApiQueries.cs` um OrganizationSettings-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `OrganizationSettingsTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- OrganizationSettings-Endpoint: `GET/POST/PATCH/DELETE /organization-settings`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
