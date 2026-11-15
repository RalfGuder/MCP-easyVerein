# User Story 066: update_invoice verrechnet zugeordnete Zahlungen

> **GitHub Issue:** [#141 – US-0066 update_invoice verrechnet zugeordnete Zahlungen](https://github.com/RalfGuder/MCP-easyVerein/issues/141)

## User Story

**Als** Kassenwart, der Zahlungen per MCP einer bestehenden Rechnung zuordnet,
**möchte ich**, dass `update_invoice` mit `relatedBookingIds` die Zahlung auch verrechnet (Zahlungsdifferenz und Zahlstatus aktualisiert),
**damit** ich die Zuordnung nicht zusätzlich in der easyVerein-Oberfläche wiederholen muss.

## Ausgangslage

Am 2026-11-15 wurde die Buchung „Beitrag Oehme“ (60 €, 02.10.2026) per `update_invoice(relatedBookingIds=[247581501])` mit der bereits finalisierten Rechnung 416 (Mitgliedsbeitrag, 60 €) verknüpft. Die Verknüpfung kam an, `paymentDifference` der Rechnung blieb aber bei 60. Erst die Zuordnung in der easyVerein-Oberfläche hat die Zahlung verrechnet.

Gegenprobe: Die 15 per `create_receipt` erzeugten Eigenbelege (555–570) sind alle verrechnet (`paymentDifference` 0 auf Beleg und Buchung). `create_receipt` setzt `relatedBookings` im **selben PATCH, der den Entwurf finalisiert** (`isDraft=false`).

**Hypothese:** easyVerein berechnet die Zahlungsdifferenz nur beim Finalisieren neu, nicht bei einer späteren Änderung von `relatedBookings` an einer finalisierten Rechnung.

## Akzeptanzkriterien

- [ ] Die Ursache ist an einem Testfall belegt und im Dokument festgehalten. Wird dabei angelegt oder verändert, nur nach ausdrücklicher Freigabe.
- [ ] Gibt es einen API-Weg, der verrechnet (z. B. `relatedInvoice` an der Buchung setzen, `paymentDifference` setzen oder einen eigenen Endpoint), nutzt `update_invoice` ihn bei `relatedBookingIds`.
- [x] Gibt es keinen API-Weg, meldet `update_invoice` nach dem Verknüpfen, wenn `paymentDifference` ungleich 0 bleibt, mit einem klaren Hinweis, die Zahlung in der Oberfläche zuzuordnen; die Tool-Beschreibung nennt die Einschränkung.
- [x] Tests nach TDD (Warnung).
- [ ] Live-Prüfung an einem Testfall (offen, siehe Stand).

## Stand 2026-11-15

- **Umgesetzt (PR, Warnung):** `update_invoice` liest die Rechnung nach dem Verknüpfen neu ein. Bleibt bei einer finalisierten Rechnung `paymentDifference` ungleich 0, endet die Antwort mit einer `WARNING`-Zeile und dem Hinweis, die Zahlung in der easyVerein-Oberfläche zuzuordnen. Die Tool-Beschreibung nennt die Einschränkung.
- **OpenAPI-Analyse:** Am Rechnungs-PATCH ist `relatedBookings` schreibbar, `paymentDifference` nicht. Am Buchungs-PATCH ist `paymentDifference` schreibbar, `relatedInvoice` steht nicht in der Spezifikation. `PATCH /invoice/{pk}/update-invoice` („Update invoice (and optional items) atomically“) ist ohne Body-Beschreibung. `PATCH /booking/mass-action` gilt nur für Massenänderungen per Filter.
- **Offen:** Ursache und verrechnender API-Weg (Kriterien 1 und 2). Belegen lässt sich das nur mit einem schreibenden Test. Echte Vereinsdaten sollen dafür nicht verändert werden; nötig ist ein eigener Testfall (Testrechnung und Testbuchung), der danach entfernt wird, und zwar nur nach Freigabe.

## Aufgaben

- OpenAPI (`easyVerein_Vault/api/easyverein-v2.0.json`) nach Endpoints und Feldern zur Zahlungszuordnung durchsuchen (invoice, booking, `paymentDifference`, `relatedInvoice`, `relatedBookings`).
- Testfall mit dem Nutzer abstimmen (Testrechnung + Testbuchung oder vorhandene offene Rechnung); Varianten prüfen:
  1. `relatedBookings` an der finalisierten Rechnung (Ist-Verhalten)
  2. `relatedInvoice` an der Buchung
  3. Entwurf mit `relatedBookings` finalisieren (Verhalten von `create_receipt`)
- Je nach Ergebnis umsetzen: verrechnender API-Weg oder Hinweis und Rückmeldung im Tool.

## Technische Hinweise

- Finalisierte Rechnungen sind über die API weitgehend unveränderlich (siehe Memory „Beleg-Unveränderlichkeit Workaround“).
- Geschäftsjahr 2025 ist abgeschlossen; Testfälle nur in 2026 und nur nach Freigabe.
