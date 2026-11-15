# User Story 035: AccountingPlan-Endpoint implementieren

> **GitHub Issue:** [#44 – US-0035 AccountingPlan-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/44)

## User Story

**Als** Vereinsadministrator,
**möchte ich** den Kontenplan über den MCP-Server abfragen und einzelne Buchungskonten darin frei- oder abschalten können,
**damit** ich vor dem Buchen prüfen kann, ob ein Konto freigeschaltet ist, ohne die easyVerein-Oberfläche zu öffnen.

## Ausgangslage

- Bei der Buchungsprüfung am 2026-11-15 ließ sich nur im Browser prüfen, ob ein Konto im Kontenplan freigeschaltet ist. Ist es nicht freigeschaltet, zeigt „Buchung editieren“ einen leeren Dialog.
- Live (lesend) geprüft: Es gibt **einen** Kontenplan je Verein (ID 5430). Er enthält `billingAccounts` als Liste von URL-Referenzen (82 Konten); mit `query={id,billingAccounts{id,number,name}}` kommen Nummer und Name direkt mit. Weitere Felder: `org`, `created_at`, `updated_at`. Der Endpoint hat keine Filter.
- Ergebnis nebenbei: 64000 (58785) und 44000 (58311) sind freigeschaltet.

## Entscheidung (2026-11-15)

Umfang **lesen und Konten frei-/abschalten**, kein create/delete: ein zweiter Plan oder das Löschen des einzigen Plans ist nicht sinnvoll.

## Akzeptanzkriterien

- [x] Entity `AccountingPlan` mit `id`, `org`, `billingAccounts` (URL-Referenz oder Objekt mit id/number/name), `created_at`, `updated_at`; Konstanten in `AccountingPlanFields`.
- [x] `AccountingPlanQuery` mit verschachtelter Feldauswahl `billingAccounts{id,number,name}`.
- [x] API-Client: `ListAccountingPlansAsync` (mit Paging), `GetAccountingPlanAsync`, `UpdateAccountingPlanAsync`.
- [x] MCP-Tools `list_accounting_plans`, `get_accounting_plan`: Konten mit id, Nummer und Name (bleiben in der Ausgabe eingebettet, nicht nur als ID).
- [x] MCP-Tool `update_accounting_plan(id, addBillingAccountIds, removeBillingAccountIds)`: liest den Plan, sendet die resultierende Menge als URL-Referenzen per PATCH und liest danach neu ein; ohne echte Änderung kein PATCH; leere Angaben und eine ID in beiden Listen werden abgelehnt. Die Beschreibung verlangt eine ausdrückliche Anweisung des Nutzers.
- [x] Tests nach TDD.
- [x] Live-Prüfung lesend (`list_accounting_plans`, `get_accounting_plan`): 2026-11-15 bestanden, 1 Plan (5430), 82 Konten mit id/number/name, Nicht-gefunden-Fall korrekt.
- [ ] Live-Prüfung schreibend (`update_accounting_plan`): offen, weil sie den echten Kontenplan des Vereins ändern würde; erst bei einem echten Freischaltungsbedarf und nur auf Anweisung.

## Aufgaben

- Entity, Fields, Query, Client, Tools, Registrierung in `Program.cs`, `ToolJson`-Ausnahme für die eingebetteten Konten.
- Tests: Domain (Deserialisierung URL/Objekt), Infrastructure (Feldauswahl, GET 404, PATCH), Server (Ausgabe, add/remove, Validierung, kein PATCH ohne Änderung).

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- AccountingPlan-Endpoint: `GET/POST/PATCH/DELETE /accounting-plan`
- Architektur konsistent mit bestehenden Entities
- Priorität: **Mittel**
