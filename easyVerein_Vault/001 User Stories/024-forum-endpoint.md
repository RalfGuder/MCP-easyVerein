# User Story 024: Forum-Endpoint implementieren

> **GitHub Issue:** [#31 – US-0024 Forum-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/31)

## User Story

**Als** Vereinsadministrator,
**möchte ich** die Foren des Vereins über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich das Vereinsforum vollständig über den MCP-Server verwalten kann.

> **Präzisierung (2026-09-17):** Der Endpoint `forum` verwaltet **Foren** (Kategorien des Mitgliederforums), keine einzelnen Beiträge. Themen und Beiträge haben eigene Endpoints und gehören nicht zu dieser Story.

## Akzeptanzkriterien

- [x] **Entity `Forum`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `ForumFields`-Konstanten
- [x] **ValueObject `ForumFields.cs`:** Alle API-Feldnamen als Konstanten
- [x] **Query-Klasse `ForumQuery.cs`:** Filterung nach ID und weiteren Standard-Feldern
- [x] **API-Client:** `ListForumsAsync`, `GetForumAsync`, `CreateForumAsync`, `UpdateForumAsync`, `DeleteForumAsync`
- [x] **MCP-Tools:** `ForumTools.cs` mit allen CRUD-Operationen – inkl. Error-Handling
- [x] **Pflichtfelder:** `create_forum` prüft `name` (Pflicht, max. 100 Zeichen) vor dem API-Aufruf; `update_forum` prüft die Länge
- [x] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary
- [x] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor), +23 (3 Domain + 11 Infrastructure + 9 Server)

## Aufgaben

1. easyVerein API-Dokumentation für den `forum`-Endpoint analysieren
2. `ForumFields.cs`, `Forum.cs`, `ForumQuery.cs` erstellen
3. ~~`ApiQueries.cs` um Forum-Query erweitern~~ (entfällt seit US-0062: Query-Instanz pro Aufruf)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um CRUD-Methoden erweitern
5. `ForumTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoints:
  - `GET/POST /forum` (Allow: GET, POST, OPTIONS)
  - `GET/PATCH/DELETE /forum/{pk}` (**kein PUT**)
  - Die Spec enthält **kein Body-Schema**; die Felder stammen aus `OPTIONS /forum` und `GET /forum/descriptions`.
- Felder im Selektor: `id`, `org`, `last_post`, `created`, `updated`, `name`, `slug`, `description`, `image`, `link`, `link_redirects`, `type`, `direct_posts_count`, `direct_topics_count`, `link_redirects_count`, `order`, `last_post_on`, `display_sub_forum_list`
  - Schreibbar: `name` (Pflicht, max. 100), `description`, `link` (URL, max. 200), `link_redirects`, `order`, `display_sub_forum_list`
  - `image` ist ein Datei-Upload (multipart) und wird vom Client nur gelesen
  - Nicht übernommen: `viewed` (benutzerbezogen), `userGroups` (read-only, Struktur unbekannt), `_description_rendered`, MPTT-Interna (`lft`, `rght`, `tree_id`, `level`)
  - `last_post` wird als ID (`LastPostId`) über `FlexibleIdConverter` gelesen
- Filter: `id__in`, `name`, `name__not`, `slug`, `slug__not`, `type`, `created__gt/__lt`, `updated__gt/__lt`, `ordering`, `search` – alle live mit einem vorhandenen Datensatz geprüft
- **Löschen ist endgültig:** Es gibt keinen Papierkorb für Foren (`wastebasket/forum` → 404); `deleted=true` wird ignoriert.

### Live-Verifikation (2026-09-17)

Mit den gebauten Tool-Klassen; Schreibtest auf Freigabe des Product Owners mit Testforen „MCP-Test … – bitte ignorieren“.

- Lesen: Liste, Filter (u. a. `type=1` → leer), Einzelabruf, fehlende ID → „not found“
- Anlegen, PATCH (`name`, `link`, `link_redirects`, `order`, `display_sub_forum_list`), Längenprüfung, Löschen, Abruf nach dem Löschen → „not found“
- Alle 11 Testforen wieder entfernt

**Auffälligkeiten:**

- **Einmaliger HTTP 500 beim ersten Anlegen:** Das Forum wurde **trotzdem** angelegt. Mit Roh-Requests (jedes Feld einzeln, alle Felder kombiniert, doppelter Name) und zwei weiteren Aufrufen über das Tool nicht reproduzierbar (immer 201). Vermutlich ein serverseitiger Einzelfall; nach einem 500 bei `create_forum` die Liste prüfen, bevor erneut angelegt wird.
- **`order` wird von der API umnummeriert:** Mit jedem neu angelegten Forum stieg `order` des bestehenden Vereinsforums (0 → 12 nach 11 Anlagen). Der Ursprungswert 0 wurde per PATCH wiederhergestellt. Ein beim Anlegen übergebenes `order` wird nicht zuverlässig übernommen; per PATCH wirkt es.

- Priorität: **Mittel**
