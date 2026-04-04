# User Story 031: Member-Group-Endpoint implementieren

> **GitHub Issue:** [#38 – US-0031 Member-Group-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/38)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Mitgliedergruppen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Gruppenzuordnung von Mitgliedern vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `MemberGroup`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `MemberGroupFields`-Konstanten
- [ ] **ValueObject `MemberGroupFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `MemberGroupQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListMemberGroupsAsync`, `GetMemberGroupAsync`, `CreateMemberGroupAsync`, `UpdateMemberGroupAsync`, `DeleteMemberGroupAsync`
- [ ] **MCP-Tools:** `MemberGroupTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `member-group`-Endpoint analysieren
2. `MemberGroupFields.cs`, `MemberGroup.cs`, `MemberGroupQuery.cs` erstellen
3. `ApiQueries.cs` um MemberGroup-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `MemberGroupTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Member-Group-Endpoint: `GET/POST/PATCH/DELETE /member-group`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
