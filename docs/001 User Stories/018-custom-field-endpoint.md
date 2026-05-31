# User Story 018: Custom-Field-Endpoint implementieren

> **GitHub Issue:** [#25 – US-0018 Custom-Field-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/25)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Felder über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich individuelle Datenfelder für die Vereinsverwaltung vollständig über den MCP-Server pflegen kann.

## Akzeptanzkriterien

- [x] **Entity `CustomField`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomFieldFields`-Konstanten
- [x] **ValueObject `CustomFieldFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `CustomFieldQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListCustomFieldsAsync`, `GetCustomFieldAsync`, `CreateCustomFieldAsync`, `UpdateCustomFieldAsync`, `DeleteCustomFieldAsync`
- [x] **MCP-Tools:** `CustomFieldTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-field`-Endpoint analysieren
2. `CustomFieldFields.cs`, `CustomField.cs`, `CustomFieldQuery.cs` erstellen
3. `ApiQueries.cs` um CustomField-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomFieldTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Field-Endpoint: `GET/POST/PATCH/PUT/DELETE /custom-field` (unsere Tools nutzen nur PATCH)
- FK-Feld `collection` (nullable) nutzt `FlexibleIdConverter` (URL-String ↔ Integer)
- Feldnamen gemischt snake_case/camelCase (`settings_type`, `member_show`, aber `needsAdminApproval`)
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**

**Status:** ✅ Implementiert auf Branch `feature/US-0018-custom-field` (TDD). +11 Tests (2 Domain + 9 Infrastructure), gesamt 224 grün. Live-Verifikation ausstehend (Server-Reconnect).
