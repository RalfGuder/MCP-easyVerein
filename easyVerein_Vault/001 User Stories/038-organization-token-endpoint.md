# User Story 038: OrganizationToken-Endpoint implementieren

> **GitHub Issue:** [#52 – US-0038 OrganizationToken-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/52)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Organisations-Tokens über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die API-Token-Verwaltung der Organisation vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `OrganizationToken`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `OrganizationTokenFields`-Konstanten
- [ ] **ValueObject `OrganizationTokenFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `OrganizationTokenQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListOrganizationTokensAsync`, `GetOrganizationTokenAsync`, `CreateOrganizationTokenAsync`, `UpdateOrganizationTokenAsync`, `DeleteOrganizationTokenAsync`
- [ ] **MCP-Tools:** `OrganizationTokenTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `organization-token`-Endpoint analysieren
2. `OrganizationTokenFields.cs`, `OrganizationToken.cs`, `OrganizationTokenQuery.cs` erstellen
3. `ApiQueries.cs` um OrganizationToken-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `OrganizationTokenTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- OrganizationToken-Endpoint: `GET/POST/PATCH/DELETE /organization-token`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
