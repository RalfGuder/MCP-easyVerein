# User Story 064: Kompakte MCP-Antworten (Paginierung, ID-Referenzen, kompaktes JSON)

> **GitHub Issue:** [#134 – US-0064 Kompakte MCP-Antworten](https://github.com/RalfGuder/MCP-easyVerein/issues/134)

## User Story

**Als** Nutzer des easyVerein-MCP-Servers in einem KI-Assistenten,
**möchte ich**, dass die list-Tools ihre Ergebnisse seitenweise begrenzen und kompakt ausgeben,
**damit** die Antworten das Token-Limit des Assistenten nicht überschreiten und weniger Kontext verbrauchen.

## Ausgangslage

Prüfung der Schnittstelle am 2026-11-15:

- `list_events` ab 01.11.2026 lieferte 95 Termine mit rund 85.000 Zeichen. Die Antwort überschritt das Token-Limit des Clients und wurde nur als Datei abgelegt.
- `list_bookings` gibt `bankAccount`, `billingAccount` und jedes Element von `relatedInvoice` als volles Objekt aus, obwohl außer der `id` fast alle Felder `null` sind (bei `relatedInvoice` rund 60 Felder je Buchung).
- Der API-Client holt immer **alle** Seiten (`limit=100` je Seite) und gibt alles zusammen zurück.
- Alle 24 Tool-Klassen serialisieren mit `new JsonSerializerOptions { WriteIndented = true }`, ohne `null`-Felder wegzulassen.

## Akzeptanzkriterien

- [x] Alle `list_*`-Tools haben die optionalen Parameter `limit` und `page`.
- [x] `limit` hat einen Standardwert (Vorschlag: 50) und ist auf höchstens 100 begrenzt; ungültige Werte (≤ 0) werden mit einer verständlichen Meldung abgelehnt.
- [x] `page` beginnt bei 1; ohne Angabe wird Seite 1 geliefert.
- [x] Die Antwort eines list-Tools enthält Angaben zur Seitennavigation: Gesamtanzahl (`totalCount`) und ob weitere Seiten existieren (`hasMore`).
- [x] Verschachtelte Referenzen werden nur als ID ausgegeben, z. B. `"bankAccount": 361879047`, `"billingAccount": 58770`, `"relatedInvoice": [607989039]`. Details sind über die passenden `get_*`-Tools abrufbar.
- [x] Felder mit dem Wert `null` entfallen in allen Tool-Antworten.
- [x] Alle Tool-Antworten sind kompaktes JSON (ohne Einrückung).
- [x] Die `JsonSerializerOptions` werden zentral als eine wiederverwendete Instanz bereitgestellt statt je Aufruf neu erzeugt.
- [x] Die Tool-Beschreibungen nennen `limit`/`page` und weisen darauf hin, wie weitere Seiten abgerufen werden.
- [x] Die erneute Prüfung von `list_events` ab 01.11.2026 bleibt unter dem Token-Limit (live 2026-11-15: 95 Termine in 2 Seiten à 50, Seite 2 ≈ 22.000 Zeichen statt 85.000 gesamt).
- [x] Tests nach TDD (Red-Green-Refactor), Mindest-Coverage 70 % bleibt erhalten.

## Umsetzungsentscheidungen

- `IEasyVereinApiClient.List*Async` (und `GetInvoicesAsync`) nehmen optional `PageRequest paging` und liefern `PagedResult<T>`. `PagedResult<T>` implementiert `IReadOnlyList<T>`; ohne `paging` werden wie bisher alle Seiten geholt.
- Die ID-Ausgabe gilt für alle Tool-Antworten (list, get, create, update). Sie wird nur in der Tool-Ausgabe angewendet (`ToolJson`, Resolver-Modifier); API-Payloads bleiben unverändert.
- Ausnahmen, die eingebettet bleiben, weil sie der eigentliche Inhalt sind: `Member.contactDetails` (Name, Anschrift) und `Event.locationObject` (für Orte gibt es kein `get`-Tool).
- Live-Befund: easyVerein paginiert über `page` und ignoriert `offset`; v2.0 liefert kein `count`, nur `next`/`previous`/`current`. `totalCount` wird daher auf der letzten Seite berechnet (`(page−1)·limit + Einträge`) und fehlt, solange weitere Seiten existieren.
- Ungültige Werte für `limit`/`page` liefern `ERROR: ArgumentOutOfRangeException: limit muss zwischen 1 und 100 liegen.` bzw. `page muss mindestens 1 sein.`

## Aufgaben

- Ergebnis-Typ für seitenweise Listen in der Domain anlegen (Einträge, `totalCount`, `hasMore`, `page`, `limit`).
- `IEasyVereinApiClient`: list-Methoden um `limit`/`page` erweitern; nur die angeforderte Seite abrufen (`limit` + `offset` bzw. `page` der easyVerein-API) und `count`/`next` der API auswerten.
- `EasyVereinApiClient.BuildListUrl` und die Seiten-Sammellogik anpassen; Methoden, die intern alle Seiten benötigen, behalten diese Möglichkeit.
- JSON-Converter für Referenzobjekte schreiben, der nur die `id` ausgibt (Entities `BankAccount`, `BillingAccount`, `Invoice` als Referenz in `Booking` und weiteren Entities).
- Zentrale `JsonSerializerOptions` (`DefaultIgnoreCondition = WhenWritingNull`, `WriteIndented = false`) im Server-Projekt anlegen und in allen Tool-Klassen nutzen.
- Alle `list_*`-Tools um die Parameter `limit` und `page` erweitern (optional im MCP-Schema, vgl. PR #133).
- Tests: Domain (Ergebnis-Typ, Converter), Infrastructure (URL mit limit/offset, Auswertung von count/next), Server (Parameterprüfung, Ausgabe ohne null, ID-Referenzen).
- README und CLAUDE.md (Teststruktur, Testanzahl) aktualisieren.

## Technische Hinweise

- easyVerein liefert Listen paginiert mit `count`, `next`, `previous` und `results`; `limit` ist dort auf 100 begrenzt.
- Fremdschlüssel kommen von der API teils als URL-String (z. B. `bookingProject`) und teils als Objekt; `UrlReference.ExtractId` und `FlexibleIdConverter` können wiederverwendet werden.
- Optionale Tool-Parameter müssen im MCP-Schema wirklich optional sein (Default-Wert im C#-Parameter), sonst verlangt der Client sie als Pflichtfeld (siehe PR #133).
- Die Ausgabe nur als ID ändert das Antwortformat. Prompts und Abläufe, die `bankAccount.id` o. Ä. lesen (z. B. `review_porto_buchungen`), sind mit anzupassen.
- Der laufende MCP-Server sperrt `src/*/bin`; Tests mit `dotnet test --artifacts-path <temp>` ausführen.
