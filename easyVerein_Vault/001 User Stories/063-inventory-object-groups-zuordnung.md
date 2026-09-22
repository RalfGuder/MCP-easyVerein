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

## Akzeptanzkriterien

- [ ] **Live-Analyse `lendingResponsible`:** Geklärt und dokumentiert, ob *jeder* `PATCH` auf `inventory-object` das Feld auf `null` setzt oder nur ein `PATCH`, der `inventoryObjectGroups` enthält. Testmatrix: PATCH nur `name`, PATCH nur `inventoryObjectGroups`, PATCH mit beiden, jeweils gegen ein Testobjekt mit gesetztem `lendingResponsible`.
- [ ] **Maßnahme abgeleitet:** Auf Basis der Analyse ist entschieden und im Dokument festgehalten, wie die Tools damit umgehen (Wert bewahren, explizit fordern oder nur dokumentieren). Bei bestätigtem Datenverlust in `update_inventory_object` wird das als Bug mitbehoben.
- [ ] **`create_inventory_object`:** Neuer optionaler Parameter für die Gruppenzuordnung (Liste von Gruppen-IDs).
- [ ] **`update_inventory_object`:** Neuer optionaler Parameter für die Gruppenzuordnung; PATCH-Semantik bleibt erhalten (nur übergebene Felder werden gesendet).
- [ ] **Entity/Serialisierung:** `inventoryObjectGroups` ist lesend (URL-Referenzen → IDs) und schreibend (IDs) korrekt abgebildet.
- [ ] **Validierung:** Nicht existierende Gruppen-IDs führen zu einer verständlichen Fehlermeldung statt zu einem rohen API-Fehler.
- [ ] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor), Coverage bleibt ≥ 70 %.
- [ ] **Live-Verifikation:** Anlegen mit Gruppe, Zuordnung ändern, Zuordnung leeren, Bestandsdaten vorher/nachher vergleichen, alle Testdaten wieder entfernt.
- [ ] **Dokumentation:** Der `OPTIONS`-Befund (`read_only` falsch) und das `lendingResponsible`-Verhalten sind im Story-Dokument festgehalten.

## Aufgaben

1. [ ] Live-Testskript für die `lendingResponsible`-Testmatrix schreiben und ausführen
2. [ ] Ergebnis auswerten, Maßnahme festlegen, Akzeptanzkriterien ggf. schärfen
3. [ ] `InventoryObjectFields` / `InventoryObject` für schreibbares `inventoryObjectGroups` anpassen
4. [ ] `EasyVereinApiClient`: Gruppenzuordnung in Create- und Update-Pfad unterstützen
5. [ ] `InventoryObjectTools`: Parameter in `CreateInventoryObject` und `UpdateInventoryObject` ergänzen, inkl. Validierung
6. [ ] Unit-Tests nach TDD (Domain, Infrastructure, Server)
7. [ ] Live-Verifikation gegen die easyVerein API
8. [ ] Story-Dokument und `CLAUDE.md` nachziehen

## Technische Hinweise

- Endpoint: `PATCH /inventory-object/{pk}` mit `{"inventoryObjectGroups": [<gruppenId>, …]}` — live mit HTTP 200 bestätigt
- Lesend liefert die API URL-Referenzen (`https://easyverein.com/api/v2.0/inventory-object-group/<id>`) → `UrlReference.ExtractId` bzw. `FlexibleIdConverter` nutzen (siehe US-0017)
- Schreibend erwartet die API Integer-IDs, keine URLs
- **`OPTIONS`-Metadaten nicht blind vertrauen** — wiederkehrendes Muster, zuletzt bei `pieces` und `lendingResponsible` in US-0026
- Gruppen-Endpoint ist seit US-0027 vorhanden (`list_inventory_object_groups` liefert gültige IDs)
- Verwandte Stories: US-0026 (Inventory Object), US-0027 (Inventory Object Group), US-0029 (Lending, baut auf `lendingResponsible` auf)
- Priorität: **Mittel** — nach US-0029 einplanen; wird die Analyse einen Datenverlust in `update_inventory_object` bestätigen, ist die Priorität neu zu bewerten
