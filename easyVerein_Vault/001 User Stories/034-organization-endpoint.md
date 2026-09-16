# User Story 034: Organization-Endpoint implementieren

> **GitHub Issue:** [#41 – US-0034 Organization-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/41)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Organisationsdaten über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Stammdaten der Organisation vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Organization`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `OrganizationFields`-Konstanten
- [ ] **ValueObject `OrganizationFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `OrganizationQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListOrganizationsAsync`, `GetOrganizationAsync`, `CreateOrganizationAsync`, `UpdateOrganizationAsync`, `DeleteOrganizationAsync`
- [ ] **MCP-Tools:** `OrganizationTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `organization`-Endpoint analysieren
2. `OrganizationFields.cs`, `Organization.cs`, `OrganizationQuery.cs` erstellen
3. `ApiQueries.cs` um Organization-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `OrganizationTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Organization-Endpoint: `GET/POST/PATCH/DELETE /organization`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
