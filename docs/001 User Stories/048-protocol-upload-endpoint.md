# User Story 048: ProtocolUpload-Endpoint implementieren

> **GitHub Issue:** [#62 – US-0048 ProtocolUpload-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/62)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Protokoll-Uploads (Anhänge) über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich Dateianhänge und Uploads zu Sitzungsprotokollen vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `ProtocolUpload`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ProtocolUploadFields`-Konstanten
- [ ] **ValueObject `ProtocolUploadFields.cs`:** Alle API-Feldnamen als Konstanten
- [ ] **Query-Klasse `ProtocolUploadQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [ ] **API-Client:** `ListProtocolUploadsAsync`, `GetProtocolUploadAsync`, `CreateProtocolUploadAsync`, `UpdateProtocolUploadAsync`, `DeleteProtocolUploadAsync`
- [ ] **MCP-Tools:** `ProtocolUploadTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `protocol-upload`-Endpoint analysieren
2. `ProtocolUploadFields.cs`, `ProtocolUpload.cs`, `ProtocolUploadQuery.cs` erstellen
3. `ApiQueries.cs` um ProtocolUpload-Query erweitern
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `ProtocolUploadTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- ProtocolUpload-Endpoint: `GET/POST/PATCH/DELETE /protocol-upload`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
