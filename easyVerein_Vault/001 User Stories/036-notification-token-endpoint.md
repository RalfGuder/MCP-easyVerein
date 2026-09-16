# User Story 036: NotificationToken-Endpoint implementieren

> **GitHub Issue:** [#47 – US-0036 NotificationToken-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/47)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Benachrichtigungs-Tokens über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Verwaltung von Benachrichtigungs-Tokens vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `NotificationToken`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `NotificationTokenFields`-Konstanten
- [ ] **ValueObject `NotificationTokenFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `NotificationTokenQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListNotificationTokensAsync`, `GetNotificationTokenAsync`, `CreateNotificationTokenAsync`, `UpdateNotificationTokenAsync`, `DeleteNotificationTokenAsync`
- [ ] **MCP-Tools:** `NotificationTokenTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `notification-token`-Endpoint analysieren
2. `NotificationTokenFields.cs`, `NotificationToken.cs`, `NotificationTokenQuery.cs` erstellen
3. `ApiQueries.cs` um NotificationToken-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `NotificationTokenTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- NotificationToken-Endpoint: `GET/POST/PATCH/DELETE /notification-token`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
