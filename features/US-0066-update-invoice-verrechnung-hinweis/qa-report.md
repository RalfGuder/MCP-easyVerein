# QA-Bericht: US-0066 update_invoice – Hinweis zur Verrechnung sachlich richtig

**Datum:** 2026-11-27 · **Ergebnis:** bestanden

## Akzeptanzkriterien
| AK | Ergebnis | Nachweis |
|----|----------|----------|
| AK-1 | ✅ | `tools/list` am gestarteten Server (stdio) liefert die neue Beschreibung („usually settles the payment, also for finalized invoices … WARNING line only if paymentDifference remains non-zero“); Test `UpdateInvoice_Description_DescribesSettlement` |
| AK-2 | ✅ | `UpdateInvoice_LinkedAndSettled_NoWarning` |
| AK-3 | ✅ | `UpdateInvoice_NotSettled_WarningIsFactual`, `UpdateInvoice_LinkedButNotSettled_ReturnsWarning` (letzte Zeile beginnt mit `WARNING:`, Betrag 60, „not fully settled“, Teilzahlung/abweichender Betrag, Prüfung in der Oberfläche, keine Pauschalaussage) |
| AK-4 | ✅ | `UpdateInvoice_WithoutBookings_DoesNotReread`, `UpdateInvoice_DraftLinked_NoWarning` |
| AK-5 | ✅ | `UpdateInvoice_RereadThrows_ReturnsUpdateWithWarning` (Update-JSON + WARNING „could not be checked“ mit Fehlergrund, kein ERROR), `UpdateInvoice_RereadReturnsNull_ReturnsUpdateWithWarning` |

## Edge Cases
- EC-1 negativer Betrag: ✅ `UpdateInvoice_NegativeDifference_ReturnsWarningWithAmount`
- EC-2 neu gelesener Wert maßgeblich: ✅ `UpdateInvoice_UsesRereadPaymentDifference`, `UpdateInvoice_PatchSettledButRereadOpen_ReturnsWarning`
- EC-3 leere Liste: ✅ `UpdateInvoice_EmptyBookingList_DoesNotReread`
- Fehler beim PATCH selbst bleibt `ERROR:`: ✅ `UpdateInvoice_PatchThrows_ReturnsError`

## Sicherheit
Keine neuen Eingaben, keine neuen API-Aufrufe. Die Fehlermeldung des Neulesens erscheint einzeilig in der WARNING; sie enthält nur die Exception-Meldung des HTTP-Clients, keine Zugangsdaten.

## Testlauf
610 Tests grün (Domain 163, Application 13, Infrastructure 236, Server 198). Keine echten API-Aufrufe.
