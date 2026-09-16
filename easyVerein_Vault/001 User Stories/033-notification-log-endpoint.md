# User Story 033: Notification-Log-Endpoint implementieren

> **GitHub Issue:** [#40 – US-0033 Notification-Log-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/40)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Benachrichtigungsprotokolle über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich den Versandverlauf von Benachrichtigungen vollständig über den MCP-Server einsehen und verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `NotificationLog`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `NotificationLogFields`-Konstanten
- [ ] **ValueObject `NotificationLogFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `NotificationLogQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListNotificationLogsAsync`, `GetNotificationLogAsync`, `CreateNotificationLogAsync`, `UpdateNotificationLogAsync`, `DeleteNotificationLogAsync`
- [ ] **MCP-Tools:** `NotificationLogTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `notification-log`-Endpoint analysieren
2. `NotificationLogFields.cs`, `NotificationLog.cs`, `NotificationLogQuery.cs` erstellen
3. `ApiQueries.cs` um NotificationLog-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `NotificationLogTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Notification-Log-Endpoint: `GET/POST/PATCH/DELETE /notification-log`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
