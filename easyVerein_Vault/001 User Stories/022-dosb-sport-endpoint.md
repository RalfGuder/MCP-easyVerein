# User Story 022: DOSB-Sport-Endpoint implementieren (nur lesend)

> **GitHub Issue:** [#29 – US-0022 DOSB-Sport-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/29)

## User Story

**Als** Vereinsadministrator,
**möchte ich** die DOSB-Sportarten des Vereins über den MCP-Server abfragen können,
**damit** ich die Sportarten-Zuordnung gemäß DOSB-Katalog über den MCP-Server einsehen kann.

> **Umfang angepasst (2026-09-16):** Ursprünglich war komplettes CRUD geplant. Die easyVerein-API v2.0 erlaubt für `dosb-sport` jedoch nur Lesen (siehe Technische Hinweise). Auf Entscheidung des Product Owners wird der Endpoint **nur lesend** umgesetzt.

## Akzeptanzkriterien

- [x] **Entity `DosbSport`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `DosbSportFields`-Konstanten
- [x] **ValueObject `DosbSportFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `DosbSportQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListDosbSportsAsync`, `GetDosbSportAsync` (kein Create/Update/Delete, weil die API es nicht erlaubt)
- [x] **MCP-Tools:** `DosbSportTools.cs` mit `list_dosb_sports` und `get_dosb_sport` – inkl. Error-Handling; keine Schreib-Tools (per Test abgesichert)
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `dosb-sport`-Endpoint analysieren
2. `DosbSportFields.cs`, `DosbSport.cs`, `DosbSportQuery.cs` erstellen
3. ~~`ApiQueries.cs` um DosbSport-Query erweitern~~ (entfällt seit US-0062: Query-Instanz pro Aufruf)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um Lese-Methoden erweitern
5. `DosbSportTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- DOSB-Sport-Endpoint: nur `GET /dosb-sport` und `GET /dosb-sport/{pk}`
  - Spec v2.0: POST und PATCH sind als „This method is not allowed“ dokumentiert, DELETE fehlt
  - Live (2026-09-16): `DELETE`, `PUT`, `OPTIONS` → HTTP 405. POST/PATCH werden nicht auf Methodenebene abgewiesen, wurden aber bewusst nicht schreibend getestet: Da DELETE gesperrt ist, ließe sich ein Test-Eintrag nicht wieder entfernen.
- Sportarten entstehen offiziell über `PATCH /member/{pk}/set-dosb` (Body `dosb-sport`: Liste von Titeln; legt fehlende Sportarten an). Das ist ein möglicher Folge-Kandidat für eine eigene User Story.
- Felder (laut `descriptions`): `id`, `title` (max. 70), `sportNumber` (max. 50), `federationNumber` (max. 50), `org`, `created_at`, `updated_at`
- Filter: `id__in`, `title`, `sportNumber`, `federationNumber`, `ordering`, `search`
- Der Verein hat derzeit **keine** DOSB-Sportarten. Die Feldnamen des Selektors ließen sich deshalb nicht live validieren (bei leerer Liste prüft die API sie nicht). Sie stammen aus dem `descriptions`-Endpoint.
- End-to-End mit den gebauten Tool-Klassen verifiziert: `list` (ungefiltert und gefiltert) → `[]`, `get` einer fehlenden ID → Hinweistext
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
