# User Story 063: Gruppenzuordnung für Inventarobjekte

> **GitHub Issue:** [#127 – US-0063 Gruppenzuordnung für Inventarobjekte](https://github.com/RalfGuder/MCP-easyVerein/issues/127)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Inventarobjekte beim Anlegen und beim Bearbeiten einer oder mehreren Inventarobjekt-Gruppen zuordnen können,
**damit** ich mein Inventar über den MCP-Server kategorisieren kann, ohne die Zuordnung in der easyVerein-Oberfläche nachpflegen zu müssen.

## Hintergrund

Bei der Live-Verifikation von US-0027 ([PR #126](https://github.com/RalfGuder/MCP-easyVerein/pull/126)) sind zwei Dinge aufgefallen:

1. `inventory-object.inventoryObjectGroups` ist laut `OPTIONS` **read-only**, lässt sich aber per `PATCH` problemlos setzen (HTTP 200, die Verknüpfung wird wirksam — `linkedItems` der Gruppe springt von `0` auf `1`). Derselbe Metadaten-Fehler wie bei `lendingResponsible` in US-0026.
2. Derselbe `PATCH` hat das beim Anlegen automatisch vergebene `lendingResponsible` (`member/8252487`) auf `null` zurückgesetzt, obwohl das Feld **nicht** im Request-Body stand.

Punkt 2 ist möglicherweise kein Sonderfall der Gruppenzuordnung, sondern betrifft jeden `PATCH` auf `inventory-object` — dann würde bereits das bestehende `update_inventory_object` bei jedem Aufruf stillschweigend den Verleih-Verantwortlichen löschen. Das ist vor der Umsetzung zu klären.

## Analyse `lendingResponsible` (2026-09-30)

Live-Testskript mit fünf Fällen, jeweils gegen ein frisch angelegtes Testobjekt. Alle Testdaten wurden danach per Papierkorb + Hard-Delete entfernt, das Bestandsobjekt „Zelt" (335646309) blieb unverändert.

**Ergebnis: Kein `PATCH` setzt `lendingResponsible` zurück.** Der Befund aus US-0027 war ein Scheinbefund.

| Fall | PATCH-Body | `lendingResponsible` danach | Gruppen danach |
|---|---|---|---|
| A | nur `name` | unverändert (`member/4424352`) | – |
| B | nur `inventoryObjectGroups` | unverändert | gesetzt |
| C | `name` + `inventoryObjectGroups` | unverändert | gesetzt |
| D | `inventoryObjectGroups` + `lendingResponsible` explizit | unverändert | gesetzt |
| E | `inventoryObjectGroups: []` (nach vorheriger Zuordnung) | unverändert | geleert |

**Eigentliche Ursache — Phantom-Default beim Anlegen:**

- `POST /inventory-object` **ohne** `lendingResponsible` liefert in der Response `lendingResponsible: member/8252487`.
- Dieser Wert wird **nicht gespeichert**: Ein direkt folgendes `GET` (mit und ohne Feld-Selektor) liefert `null`.
- `8252487` ist auch **keine gültige Referenz**: `POST` mit `"lendingResponsible": 8252487` → 400 „Folgende Referenz ist ungültig oder darf nicht verwendet werden: 8252487, Feld: lendingResponsible".
- In US-0026/US-0027 wurde der Response-Wert als gespeicherter Default gedeutet; der spätere `null`-Wert nach dem PATCH war schlicht der tatsächliche Zustand.

**Folgerungen:**

- `update_inventory_object` verliert **keine** Daten; die Priorität der Story bleibt mittel.
- `inventoryObjectGroups` ist per `PATCH` setzbar und mit `[]` leerbar, trotz `read_only` in `OPTIONS`.
- Mit einem gültigen Mitglied (`4424352`) wird `lendingResponsible` beim Anlegen korrekt gespeichert.
- `create_inventory_object` gibt bisher die POST-Response zurück und zeigt dadurch den Phantom-Wert `8252487` an; die Parameterbeschreibung „defaults to the API user" ist falsch.

**Maßnahme (PO-Entscheid 2026-09-30):** `create_inventory_object` liest das Objekt nach dem `POST` per `GET` neu und gibt den tatsächlich gespeicherten Zustand zurück (+1 Request). Die Parameterbeschreibung wird korrigiert: Ohne Angabe wird kein Verleih-Verantwortlicher gespeichert.

## Umsetzung (2026-09-30)

- **Domain:** Neuer `FlexibleIdListConverter` (liest Zahlen, numerische Strings und URL-Referenzen, schreibt ein Integer-Array). `InventoryObject.InventoryObjectGroups` (`List<string>?`, URLs) → `InventoryObjectGroupIds` (`List<long>?`).
- **Infrastructure:** Keine Code-Änderung nötig; Regressionstests für POST mit IDs, GET mit URL→ID und PATCH mit `[]`.
- **Server:** `create_inventory_object` und `update_inventory_object` haben den Parameter `inventoryObjectGroups` (`long[]?`). Bei `update` ersetzt die Liste die Zuordnung, `[]` leert sie. Vor dem Senden wird jede ID per `GET inventory-object-group/{id}` geprüft; unbekannte IDs → `ERROR: Inventory object group(s) not found: …`.
- **Re-Read nach Create:** `create_inventory_object` gibt den per `GET` gelesenen, gespeicherten Zustand zurück (Fallback: POST-Response). Beschreibung von `lendingResponsible` korrigiert.
- **Tests:** +16 (Domain 7, Infrastructure 3, Server 6) → **477 grün**.

### Live-Verifikation (gebaute Tools, ohne MCP-Neustart)

| # | Schritt | Ergebnis |
|---|---|---|
| 1 | Anlegen mit Gruppe, ohne Verantwortlichen | Gruppe gespeichert, kein Phantom-Wert `8252487` in der Tool-Ausgabe |
| 2 | Anlegen mit `lendingResponsible` 4424352 + zwei Gruppen | beides gespeichert |
| 3 | Anlegen mit unbekannter Gruppe (ID 1) | verständliche Fehlermeldung, nichts angelegt |
| 4 | Zuordnung auf andere Gruppe ändern | ersetzt, Verantwortlicher unverändert |
| 5 | Nur `name` ändern | Gruppen und Verantwortlicher unverändert |
| 6 | Zuordnung mit `[]` leeren | geleert |
| 7 | Update mit unbekannter Gruppe | Fehlermeldung, Objekt unverändert |

Alle Testobjekte und -gruppen per Papierkorb + Hard-Delete entfernt; Bestandsobjekt „Zelt" unverändert.

## Akzeptanzkriterien

- [x] **Live-Analyse `lendingResponsible`:** Geklärt und dokumentiert, ob *jeder* `PATCH` auf `inventory-object` das Feld auf `null` setzt oder nur ein `PATCH`, der `inventoryObjectGroups` enthält. Testmatrix: PATCH nur `name`, PATCH nur `inventoryObjectGroups`, PATCH mit beiden, jeweils gegen ein Testobjekt mit gesetztem `lendingResponsible`.
- [x] **Maßnahme abgeleitet:** Auf Basis der Analyse ist entschieden und im Dokument festgehalten, wie die Tools damit umgehen (Wert bewahren, explizit fordern oder nur dokumentieren). Bei bestätigtem Datenverlust in `update_inventory_object` wird das als Bug mitbehoben.
- [x] **`create_inventory_object`:** Neuer optionaler Parameter für die Gruppenzuordnung (Liste von Gruppen-IDs).
- [x] **`update_inventory_object`:** Neuer optionaler Parameter für die Gruppenzuordnung; PATCH-Semantik bleibt erhalten (nur übergebene Felder werden gesendet).
- [x] **Entity/Serialisierung:** `inventoryObjectGroups` ist lesend (URL-Referenzen → IDs) und schreibend (IDs) korrekt abgebildet.
- [x] **Validierung:** Nicht existierende Gruppen-IDs führen zu einer verständlichen Fehlermeldung statt zu einem rohen API-Fehler.
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor), Coverage bleibt ≥ 70 %.
- [x] **Live-Verifikation:** Anlegen mit Gruppe, Zuordnung ändern, Zuordnung leeren, Bestandsdaten vorher/nachher vergleichen, alle Testdaten wieder entfernt.
- [x] **Dokumentation:** Der `OPTIONS`-Befund (`read_only` falsch) und das `lendingResponsible`-Verhalten sind im Story-Dokument festgehalten.

## Aufgaben

1. [x] Live-Testskript für die `lendingResponsible`-Testmatrix schreiben und ausführen
2. [x] Ergebnis auswerten, Maßnahme festlegen, Akzeptanzkriterien ggf. schärfen
3. [x] `InventoryObjectFields` / `InventoryObject` für schreibbares `inventoryObjectGroups` anpassen
4. [x] `EasyVereinApiClient`: Gruppenzuordnung in Create- und Update-Pfad unterstützen
5. [x] `InventoryObjectTools`: Parameter in `CreateInventoryObject` und `UpdateInventoryObject` ergänzen, inkl. Validierung
6. [x] Unit-Tests nach TDD (Domain, Infrastructure, Server)
7. [x] Live-Verifikation gegen die easyVerein API
8. [x] Story-Dokument und `CLAUDE.md` nachziehen

## Technische Hinweise

- Endpoint: `PATCH /inventory-object/{pk}` mit `{"inventoryObjectGroups": [<gruppenId>, …]}` — live mit HTTP 200 bestätigt
- Lesend liefert die API URL-Referenzen (`https://easyverein.com/api/v2.0/inventory-object-group/<id>`) → `UrlReference.ExtractId` bzw. `FlexibleIdConverter` nutzen (siehe US-0017)
- Schreibend erwartet die API Integer-IDs, keine URLs
- **`OPTIONS`-Metadaten nicht blind vertrauen** — wiederkehrendes Muster, zuletzt bei `pieces` und `lendingResponsible` in US-0026
- Gruppen-Endpoint ist seit US-0027 vorhanden (`list_inventory_object_groups` liefert gültige IDs)
- Verwandte Stories: US-0026 (Inventory Object), US-0027 (Inventory Object Group), US-0029 (Lending, baut auf `lendingResponsible` auf)
- Priorität: **Mittel** — nach US-0029 einplanen; wird die Analyse einen Datenverlust in `update_inventory_object` bestätigen, ist die Priorität neu zu bewerten
