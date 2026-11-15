# User Story 065: create_receipt übernimmt das Buchungsdatum als Belegdatum

> **GitHub Issue:** [#140 – US-0065 create_receipt übernimmt das Buchungsdatum als Belegdatum](https://github.com/RalfGuder/MCP-easyVerein/issues/140)

## User Story

**Als** Kassenwart, der beleglose Buchungen mit `create_receipt` kontiert,
**möchte ich**, dass der erzeugte Beleg das Datum der Buchung trägt (oder ein Datum, das ich angebe),
**damit** Belegdatum und Buchungsdatum übereinstimmen und der Beleg im richtigen Zeitraum und Geschäftsjahr erscheint.

## Ausgangslage

Bei der Buchungsprüfung 2026 am 2026-11-15 haben alle per `create_receipt` erzeugten Eigenbelege (Nr. 555–570) das Belegdatum 15.11.2026 bekommen, also den Tag der Anlage. Ursache: `InvoiceTools.CreateReceipt` legt den Entwurf ohne `date` an, und die API setzt dann das aktuelle Datum. `booking.Date` ist an dieser Stelle bereits geladen.

## Akzeptanzkriterien

- [x] Ohne Angabe übernimmt `create_receipt` das Datum der verknüpften Buchung als Belegdatum (`date` der Invoice).
- [x] Der neue optionale Parameter `date` (ISO 8601, z. B. `2026-07-09`) überschreibt das Buchungsdatum, z. B. mit dem Datum des Kassenbons.
- [x] Ein ungültiges `date` wird mit einer verständlichen Meldung abgelehnt, bevor etwas angelegt wird.
- [x] Hat die Buchung kein Datum und ist kein `date` angegeben, bricht das Tool mit einer verständlichen Meldung ab, statt still das heutige Datum zu verwenden.
- [x] Der Parameter ist im MCP-Schema wirklich optional (Default-Wert, vgl. PR #133).
- [ ] Tests nach TDD; Live-Prüfung an einem Testfall: Belegdatum = Buchungsdatum.

## Umsetzung

- Das Belegdatum wird im Finalisierungs-PATCH als `date` im Format `yyyy-MM-dd` gesendet; der Entwurf selbst bleibt ohne Datum.
- `Booking.Date` wird ohne eigenen Converter gelesen und dabei in die Zeitzone des Rechners umgerechnet. Das Belegdatum wird deshalb als Kalendertag in Europe/Berlin bestimmt, damit es auf jedem Server stimmt.

## Aufgaben

- Test: `CreateReceipt` übergibt `Date = booking.Date` an `CreateInvoiceAsync`.
- Test: der Parameter `date` überschreibt das Buchungsdatum; ein ungültiges Datum führt zu einer Fehlermeldung und es wird nichts angelegt.
- `InvoiceTools.CreateReceipt`: Parameter `date` ergänzen und `Date` im Entwurf setzen.
- Tool-Beschreibung um das Belegdatum ergänzen.

## Technische Hinweise

- `Invoice.Date` ist `DateTime?` (API-Feld `date`). Prüfen, dass die Serialisierung ein Datum ohne Zeitzonenverschiebung sendet (Buchungen kommen als `2026-10-02T00:00:00+02:00`).
- Die bereits angelegten Eigenbelege 555–570 werden **nicht** korrigiert. Finalisierte Belege sind über die API nicht änderbar; bei Bedarf in der easyVerein-Oberfläche anpassen.
