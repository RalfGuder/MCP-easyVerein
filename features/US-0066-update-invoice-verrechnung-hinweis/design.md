# Design: US-0066 update_invoice – Hinweis zur Verrechnung sachlich richtig

## Ansatz
Nur das Tool `update_invoice` in `src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs` ändert sich. API-Client, Domain und andere Tools bleiben unverändert.

## Bausteine
- **Tool-Beschreibung (AK-1):** Linking über `relatedBookingIds` verrechnet die Zahlung in der Regel, auch bei finalisierten Rechnungen. Das Tool liest die Rechnung danach neu ein; die Antwort endet nur dann mit einer `WARNING:`-Zeile, wenn `paymentDifference` ungleich 0 bleibt. Die frühere Aussage „verrechnet bei finalisierten Rechnungen nicht“ entfällt.
- **Ablauf nach dem PATCH:**
  - Ohne `relatedBookingIds`, mit leerer Liste oder bei Entwurf → kein Neulesen, keine WARNING (AK-4, EC-3, unverändert).
  - Neulesen liefert `paymentDifference` 0 → nur Rechnungs-JSON (AK-2).
  - Neulesen liefert `paymentDifference` ≠ 0 (auch negativ) → JSON + `WARNING:` mit dem gelesenen Betrag, „nicht vollständig verrechnet“, mögliche Gründe Teilzahlung oder abweichender Betrag, Empfehlung: Beträge und Zuordnung in der easyVerein-Oberfläche prüfen (AK-3, EC-1). Maßgeblich ist der neu gelesene Wert (EC-2).
  - Neulesen wirft eine Exception oder liefert `null` → JSON des Updates + `WARNING:` „Verrechnung konnte nicht geprüft werden“ mit Grund (AK-5). Annahme aus der Prüfung: `null` gilt als Fehlschlag. Ein Abbruch (`OperationCanceledException`) wird ebenfalls so behandelt, damit das gelungene Update nie verschwiegen wird.
  - Fehler beim PATCH selbst → weiterhin `ERROR:` (unverändert).
- Die WARNING bleibt einzeilig und die letzte Zeile der Antwort.

## Tests
`UpdateInvoicePaymentTests` (gemockter `IEasyVereinApiClient`) wird erweitert: Wortlaut der WARNING (offener Betrag, Gründe, keine Pauschalaussage), negativer Betrag, leere Liste, Neulesen wirft / liefert null, Beschreibung über das `Description`-Attribut. Keine echten API-Aufrufe.
