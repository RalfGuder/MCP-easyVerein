# User Story 019: Custom-Field-Collection-Endpoint implementieren

> **GitHub Issue:** [#26 – US-0019 Custom-Field-Collection-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/26)

## User Story

**Als** Vereinsadministrator,
**möchte ich** benutzerdefinierte Feldsammlungen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich zusammengehörige benutzerdefinierte Felder gruppieren und strukturiert über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `CustomFieldCollection`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `CustomFieldCollectionFields`-Konstanten
- [ ] **ValueObject `CustomFieldCollectionFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `CustomFieldCollectionQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListCustomFieldCollectionsAsync`, `GetCustomFieldCollectionAsync`, `CreateCustomFieldCollectionAsync`, `UpdateCustomFieldCollectionAsync`, `DeleteCustomFieldCollectionAsync`
- [ ] **MCP-Tools:** `CustomFieldCollectionTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `custom-field-collection`-Endpoint analysieren
2. `CustomFieldCollectionFields.cs`, `CustomFieldCollection.cs`, `CustomFieldCollectionQuery.cs` erstellen
3. `ApiQueries.cs` um CustomFieldCollection-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `CustomFieldCollectionTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Custom-Field-Collection-Endpoint: `GET/POST/PATCH/DELETE /custom-field-collection`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
