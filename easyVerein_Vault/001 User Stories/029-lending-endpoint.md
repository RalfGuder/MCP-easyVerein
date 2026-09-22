# User Story 029: Lending-Endpoint implementieren

> **GitHub Issue:** [#36 – US-0029 Lending-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/36)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Ausleihen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Ausleihe von Vereinsgegenständen an Mitglieder vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [x] **Entity `Lending`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `LendingFields`-Konstanten
- [x] **ValueObject `LendingFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `LendingQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListLendingsAsync`, `GetLendingAsync`, `CreateLendingAsync`, `UpdateLendingAsync`, `DeleteLendingAsync`
- [x] **MCP-Tools:** `LendingTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. [x] easyVerein API-Dokumentation für den `lending`-Endpoint analysieren
2. [x] `LendingFields.cs`, `Lending.cs`, `LendingQuery.cs` erstellen
3. [x] ~~`ApiQueries.cs` um Lending-Query erweitern~~ – entfällt seit US-0062 (Query pro Aufruf, `FieldQuery` als `internal const`)
4. [x] `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. [x] `LendingTools.cs` als MCP-Tool-Klasse erstellen
6. [x] `Program.cs` um Registrierung erweitern
7. [x] Unit-Tests schreiben (+36: 8 Domain, 10 Infrastructure, 18 Server → 461 gesamt)
8. [x] Manuelle Verifikation gegen die easyVerein API – lesend und schreibend erledigt

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoint: `/lending` (`GET`, `POST`) und `/{pk}` (`GET`, `PUT`, `PATCH`, `DELETE`); die Tools nutzen nur `PATCH`
- `DELETE` verschiebt in den Papierkorb (`wastebasket/lending/{pk}`), endgültiges Löschen dort per `DELETE`
- Die Spec enthält **kein Response-Schema**; die Felder stammen aus `OPTIONS /lending` und `/lending/descriptions`
- Schreibbare Felder: `parentInventoryObject`, `borrowAddress`, `borrowingDate` (Datum), `returnDate` (Datum), `quantity`, `borrowTime` (Uhrzeit), `returnTime` (Uhrzeit), `state`
- `state` akzeptiert nur die Kleinschreibung `lent`, `inquiry`, `returned` – im Tool vorab geprüft
- Nur lesend: `id`, `org`, `name` (von der API erzeugt, z. B. „Ausleihe von Ralf Guder"), `created_at`, `updated_at`, `_deleteAfterDate`, `_deletedBy`
- `borrowAddress` verweist auf **contact-details**, nicht auf `member`; ein `borrowMember`-Feld gibt es nicht, nur den gleichnamigen Filter
- Filter: `id__in`, `parentInventoryObject(__not)`, `borrowMember(__not)`, `borrowAddress(__not)`, `state`/`state__ne`, `borrowingDate`/`__gte`/`__lte`, `returnDate`/`__gte`/`__lte`, `quantity`/`__gt`/`__lt`, `futureReturnDate`, `deleted`, `ordering`, `search`
- **Neuer Domain-Baustein `DateOnlyJsonConverter`:** liest `yyyy-MM-dd` und volle ISO-Datetimes, schreibt aber immer die Datums-Form, die die API für `date`-Felder erwartet. `FlexibleDateTimeConverter` schreibt volles ISO und passt hier nicht.
- Nicht im Umfang: `bulk-create`/`bulk-update` sowie `POST /lending/notification` (versendet echte Benachrichtigungen; PO-Entscheid: eigene Story mit eigener Sicherheitsabwägung)
- Priorität: **Mittel**

## Live-Verifikation (2026-09-22)

Bestand vor dem Test: 1 Ausleihe (37839, Zelt an Ralf Guder, `state: lent`) und 1 verleihbares Objekt (Zelt, 335646309).

- **Vier Pflichtfelder bestätigt:** `POST {}` → 400 „Folgende Felder müssen angegeben werden: borrowingDate, quantity, borrowAddress, parentInventoryObject". Das Tool prüft alle vier vorab.
- **`parentInventoryObject` und `borrowAddress` sind schreibbar**, obwohl `OPTIONS` beide als `read_only` meldet – der dritte Fall dieser Art nach `pieces`/`lendingResponsible` (US-0026) und `inventoryObjectGroups` (US-0027).
- Anlegen, `get`, PATCH und alle geprüften Filter (`state`, `parentInventoryObject`, `futureReturnDate`, `id__in`) erwartungsgemäß.
- **Uhrzeiten:** Die API nimmt `HH:mm` an und normalisiert auf `HH:mm:ss` (`14:30` → `14:30:00`).
- **Datumsformat:** `yyyy-MM-dd` wird angenommen – der `DateOnlyJsonConverter` erzeugt genau diese Form.
- Löschen: Papierkorb (200) → `get` danach „not found" → Hard-Delete 204 → anschließend 404.
- **Keine Nebenwirkungen:** Bestandsausleihe 37839 und das Inventarobjekt „Zelt" unverändert; danach wieder genau 1 Ausleihe.
