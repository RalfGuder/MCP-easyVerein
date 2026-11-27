# US-0066: update_invoice – Hinweis zur Verrechnung sachlich richtig

**Status:** Approved
**Erstellt:** 2026-11-27

## Abhängigkeiten
- Baut auf Commit 1891fed (2026-11-15) auf: `update_invoice` liest die Rechnung nach dem Verknüpfen von Buchungen neu ein und warnt, wenn `paymentDifference` ungleich 0 bleibt.

## Ausgangslage
Beschreibung und Warntext von `update_invoice` behaupten heute pauschal, easyVerein verrechne Buchungen, die per API mit einer finalisierten Rechnung verknüpft werden, nie. Der Live-Test vom 2026-11-27 widerlegt das (Vault-Story 066, Abschnitt „Live-Test 2026-11-27“). Bei Beleg 413 und Buchung 240005835 hat `update_invoice(relatedBookingIds=[…])` sofort verrechnet: `paymentDifference` 0 an Rechnung und Buchung. Die Warnung selbst bleibt sinnvoll, wenn die Differenz nach dem Verknüpfen ungleich 0 bleibt, etwa bei Teilzahlung, falschem Betrag oder einer Verrechnung, die easyVerein nicht vornimmt.

## User Stories
- Als Kassenwart, der per MCP Zahlungen einer Rechnung zuordnet, möchte ich eine zutreffende Rückmeldung, ob die Zahlung verrechnet ist, damit ich nur dann in der easyVerein-Oberfläche nacharbeite, wenn wirklich etwas offen ist.
- Als LLM-Agent möchte ich eine Tool-Beschreibung, die das tatsächliche Verhalten beschreibt, damit ich `update_invoice` zum Zuordnen von Zahlungen nutze, statt den Nutzer unnötig auf die Oberfläche zu verweisen.

## Außerhalb des Umfangs
- Ein anderer API-Weg zur Verrechnung (`relatedInvoice` an der Buchung, `PATCH /invoice/{pk}/update-invoice`, `mass-action`)
- Klärung, ob Mitgliedsbeitragsrechnungen (`kind=membership`) anders verrechnen; falls nötig, eigene Story
- Änderungen an `create_receipt` oder anderen Tools
- Automatisches Nachverrechnen oder Ändern von `paymentDifference`

## Akzeptanzkriterien
- [ ] AK-1: Angenommen `tools/list` wird abgerufen, dann sagt die Beschreibung von `update_invoice`, dass das Verknüpfen von Buchungen über `relatedBookingIds` die Zahlung in der Regel verrechnet, auch bei finalisierten Rechnungen. Die Antwort endet mit einer WARNING-Zeile, wenn danach `paymentDifference` ungleich 0 bleibt. Die Beschreibung behauptet nicht mehr, dass easyVerein bei finalisierten Rechnungen per API nicht verrechnet.
- [ ] AK-2: Angenommen eine finalisierte Rechnung, und nach dem Verknüpfen liest das Tool `paymentDifference` 0, wenn `update_invoice` mit `relatedBookingIds` aufgerufen wird, dann enthält die Antwort das Rechnungs-JSON und keine WARNING-Zeile (wie bisher).
- [ ] AK-3: Angenommen eine finalisierte Rechnung, und nach dem Verknüpfen liest das Tool `paymentDifference` ungleich 0 (z. B. 60), wenn `update_invoice` mit `relatedBookingIds` aufgerufen wird, dann endet die Antwort mit einer WARNING-Zeile. Sie nennt den offenen Betrag und sagt, dass die Zahlung nicht vollständig verrechnet ist. Als mögliche Gründe nennt sie Teilzahlung oder abweichenden Betrag und empfiehlt, Beträge und Zuordnung in der easyVerein-Oberfläche zu prüfen. Sie behauptet nicht, dass die API grundsätzlich nicht verrechnet.
- [ ] AK-4: Angenommen `update_invoice` wird ohne `relatedBookingIds` oder für einen Entwurf aufgerufen, dann liest das Tool die Rechnung nicht neu und gibt keine WARNING aus (wie bisher).
- [ ] AK-5: Angenommen das Verknüpfen gelingt, aber das erneute Lesen der Rechnung schlägt fehl, wenn `update_invoice` mit `relatedBookingIds` aufgerufen wird, dann enthält die Antwort trotzdem das Ergebnis des Updates. Eine WARNING-Zeile meldet, dass die Verrechnung nicht geprüft werden konnte. Die Antwort ist keine reine ERROR-Meldung, die den gelungenen Update verschweigt.

## Edge Cases
- EC-1: `paymentDifference` ist nach dem Verknüpfen negativ (Überzahlung oder Ausgabenbeleg mit falschem Vorzeichen) → WARNING wie AK-3 mit dem gelesenen Betrag.
- EC-2: Die Antwort des Updates zeigt schon `paymentDifference` 0, das erneute Lesen liefert aber ungleich 0 → maßgeblich ist der neu gelesene Wert (wie bisher).
- EC-3: `relatedBookingIds` ist eine leere Liste → kein erneutes Lesen, keine WARNING (wie bisher).

## Technische Anforderungen
- Keine Aufrufe der echten easyVerein-API in Tests. HTTP-Antworten werden gemockt, wie in `UpdateInvoicePaymentTests`.
- Die WARNING-Zeile bleibt die letzte Zeile der Antwort und beginnt mit `WARNING:`. Agenten erkennen sie daran.

## Offene Fragen
- Keine

## Product Decisions
- Live-Test statt Annahme: Verknüpfen über `relatedBookings` verrechnet auch an finalisierten Rechnungen (Beleg 413, 2026-11-27). Darum nur Text und Rückmeldung korrigieren, keinen anderen API-Weg bauen. (2026-11-27)
- Die Warnung bleibt bedingt: Sie greift nur, wenn nach dem erneuten Lesen eine Differenz bleibt, und deckt so Teilzahlungen und unerwartetes Verhalten ab. (2026-11-27)
- Ob Mitgliedsbeitragsrechnungen anders verrechnen, wird nicht in dieser Story geklärt, weil es gerade keinen Testfall gibt. (2026-11-27)
- Scheitert das erneute Lesen, gewinnt das Ergebnis des Updates: Der Nutzer muss erfahren, dass die Verknüpfung gesetzt ist, statt nur einen Fehler zu sehen. (2026-11-27)
