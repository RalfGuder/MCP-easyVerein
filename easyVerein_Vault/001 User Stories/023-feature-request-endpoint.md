# User Story 023: Feature-Request-Endpoint implementieren

> **GitHub Issue:** [#30 – US-0023 Feature-Request-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/30)

## User Story

**Als** Vereinsadministrator,
**möchte ich** das öffentliche Ideen-Board von easyVerein (Feature-Requests) über den MCP-Server durchsuchen, eigene Wünsche einreichen und über Wünsche abstimmen können,
**damit** ich Verbesserungsvorschläge für easyVerein direkt aus dem MCP-Server heraus verfolgen und unterstützen kann.

> **Umfang angepasst (2026-09-16):** Ursprünglich war komplettes CRUD geplant, mit der Annahme, es handle sich um Vereinsdaten. Tatsächlich ist `feature-request` das **öffentliche Produkt-Ideen-Board des Herstellers**. Die API erlaubt weder Bearbeiten noch Löschen (HTTP 405). Auf Entscheidung des Product Owners: Lesen, Anlegen und Abstimmen.

## Akzeptanzkriterien

- [x] **Entity `FeatureRequest`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `FeatureRequestFields`-Konstanten; eingebetteter Autor als `FeatureRequestAuthor` (inkl. Organisation)
- [x] **ValueObject `FeatureRequestFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `FeatureRequestQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListFeatureRequestsAsync`, `GetFeatureRequestAsync`, `CreateFeatureRequestAsync`, `VoteFeatureRequestAsync` (kein Update/Delete, weil die API es nicht erlaubt)
- [x] **MCP-Tools:** `FeatureRequestTools.cs` mit `list_feature_requests`, `get_feature_request`, `create_feature_request`, `vote_feature_request` – inkl. Error-Handling; die beiden öffentlich wirksamen Tools tragen einen **PUBLIC ACTION**-Hinweis und verlangen eine ausdrückliche Bestätigung
- [x] **Pflichtfelder:** `create_feature_request` prüft `title` und `description` vor dem API-Aufruf
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `feature-request`-Endpoint analysieren
2. `FeatureRequestFields.cs`, `FeatureRequest.cs`, `FeatureRequestAuthor.cs`, `FeatureRequestQuery.cs` erstellen
3. ~~`ApiQueries.cs` um FeatureRequest-Query erweitern~~ (entfällt seit US-0062: Query-Instanz pro Aufruf)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um Lese-, Anlege- und Abstimm-Methoden erweitern
5. `FeatureRequestTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API (nur lesend, siehe unten)

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoints:
  - `GET /feature-request`, `GET /feature-request/{pk}`
  - `POST /feature-request` (Pflicht: `title`, `description`; `category` ist ein numerisches Auswahlfeld; `status`/`approved` werden ignoriert)
  - `GET /feature-request/{pk}/voteFor` und `GET /feature-request/{pk}/voteAgainst` – Abstimmung per **GET**. Der in der Spec dokumentierte Body (`$votingQuestionId`, `email`) ist vom `voting`-Endpoint kopiert und wird nicht gesendet.
  - `PATCH`, `PUT`, `DELETE` → HTTP 405
- Felder: `id`, `label`, `title`, `description`, `response`, `author` (`{id, org{id, short, name}}`), `proVotesCount`, `contraVotesCount`, `hasVoted`, `status`, `approved`, `date`, `category`
  - `author` muss im Selektor **ohne** Unterauswahl angefragt werden (`author{…}` → HTTP 400 „is not a nested field“)
  - **Anonyme Autoren:** 163 von 1.965 Einträgen liefern `{"id": "Unbekannt", "org": {"id": "", …}}` → IDs als `long?` mit `FlexibleIdConverter` (ergibt `null`). Das fiel erst beim Live-Test auf; der Fall ist per Regressionstest abgesichert.
  - Die Bedeutung der Zahlencodes von `status` und `category` ist nicht dokumentiert (`/descriptions` → 404)
- Filter: `id__in`, `status`, `category`, `author__isme`, `ordering`, `search` (`title`, `description`)
- Live-Verifikation (2026-09-16) mit den gebauten Tool-Klassen, **nur lesend**: alle 1.965 Einträge (12 s, IDs eindeutig), Filter Status/Kategorie, Abruf eines anonymen Eintrags, fehlende ID. `create_feature_request` und `vote_feature_request` wurden bewusst **nicht** live aufgerufen, weil sie öffentlich wirken; sie sind nur per Unit-Test abgesichert.
- Priorität: **Mittel**
