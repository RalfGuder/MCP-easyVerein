# User Story 061: create_invoice erweitern + update_invoice + create_receipt

> **GitHub Issue:** [#98 – US-0061 create_invoice-Erweiterung + create_receipt](https://github.com/RalfGuder/MCP-easyVerein/issues/98)

## User Story

**Als** Kassenwart,
**möchte ich** Belege (Receipts) inkl. Position und Buchungsverknüpfung in einem MCP-Aufruf anlegen können,
**damit** ich nach Anwendung einer Buchungsregel den fehlenden Beleg ohne manuelle curl-Aufrufe ergänzen kann.

## Kontext

Bei Klassifikation einer Buchung erzeugt easyVerein in vielen Fällen automatisch einen leeren Beleg + Position. In einigen Fällen (z. B. Entgeltabrechnung vom 04.05.2026, Buchung 240005841) **bleibt `relatedInvoice` leer**. Die Buchungsregel kann dann nicht vollständig angewendet werden (kein Beleg-Item klassifizierbar, paymentDifference ≠ 0).

Workaround heute (manuell via curl):
1. POST `/invoice` mit `isDraft=true` + `receiver` + minimalem Body
2. POST `/invoice-item` mit `relatedInvoice` + Klassifikations-Feldern
3. PATCH `/invoice/{id}` mit `isDraft=false` + `relatedBookings=[...]`

## Akzeptanzkriterien

- [x] **`create_invoice`** akzeptiert die zusätzlichen Parameter `receiver`, `relatedAddress`, `relatedBookings` (Liste numerischer Buchungs-IDs, intern zu URL-Refs konvertiert), `isReceipt`, `isDraft`. Bestehende Parameter bleiben unverändert.
- [x] **`update_invoice`** (neues Tool, PATCH-Semantik analog `update_booking_project`) erlaubt teilweise Aktualisierung. Mindestens unterstützt: `isDraft`, `isReceipt`, `relatedBookings`, `description`, `totalPrice`, `receiver`, `relatedAddress`, `paymentInformation`, `kind`.
- [x] **`create_receipt`** (neues high-level Tool) orchestriert in einem MCP-Aufruf: leerer Draft-Beleg (POST), eine Position (POST), Finalisierung mit Buchungs-Verknüpfung (PATCH). Parameter: `bookingId` (Pflicht, numerisch), `billingAccountId`, `sphere`, `costCentre`, optional `title`, `description`, `receiver`. `totalPrice` + `unitPrice` werden aus der Buchung übernommen (Betrag-absolut).
- [ ] Domain-Entity `Invoice.IsDraft` (bereits vorhanden) wird im Tool durchgereicht. Falls noch nicht serialisiert: ergänzen.
- [x] HTTP-PATCH nutzt `_httpClient.PatchAsync` mit explizitem `StringContent` (kein chunked encoding).
- [x] **Tests:**
  - +4 Infrastructure-Tests für `UpdateInvoiceAsync` (PATCH-Dictionary, isDraft-Toggle, relatedBookings, error path).
  - +2 Server-Tests für `create_invoice` mit neuen Feldern (receiver+isDraft setzt sich durch; relatedBookings konvertiert IDs → URLs).
  - +1 Server-Test für `create_receipt` happy path (mockt 3 HTTP-Calls in Reihenfolge POST-POST-PATCH; verifiziert Final-State).
- [x] CLAUDE.md "Invoice hat noch kein Update-Tool implementiert" entfernen / ändern.
- [x] Build sauber, alle Tests grün, Coverage über 70 %.

## Aufgaben

1. Domain: `Invoice.IsDraft` prüfen, ggf. mit `[JsonPropertyName]` ergänzen + `InvoiceFields.IsDraft`-Konstante.
2. Domain: `IEasyVereinApiClient.UpdateInvoiceAsync(long id, object patchData, CancellationToken)` ergänzen.
3. Infrastructure: `UpdateInvoiceAsync` implementieren (Pattern analog `UpdateInvoiceItemAsync`).
4. Infrastructure: Tests für PATCH-Path schreiben (TDD).
5. Server: `create_invoice` Tool um 5 Parameter erweitern (receiver, relatedAddress, relatedBookings-IDs, isReceipt, isDraft). URL-Ref-Konstruktion via `config.GetVersionedBaseUrl()`.
6. Server: `update_invoice` Tool (analog `update_invoice_item`).
7. Server: `create_receipt` Tool orchestriert 3 Calls.
8. Server: `.WithTools<InvoiceTools>()` ist schon registriert; nichts zusätzlich.
9. Tests Server-seitig (Mocks für drei Client-Methoden in `create_receipt`).
10. CLAUDE.md aktualisieren (Endpoint-Status, Doc-Punkt entfernen).

## Technische Hinweise

- easyVerein API verlangt mindestens eines von `receiver` oder `relatedAddress` bei Invoice-Erstellung.
- `isDraft=true` umgeht die Validierung `Folgende Felder müssen angegeben werden: invoiceItems` (siehe Session-Log).
- `relatedBookings` ist eine Liste von URL-Strings, vom Tool aus IDs zu konstruieren: `$"{config.GetVersionedBaseUrl()}/booking/{id}"`.
- `paymentDifference` wird API-seitig automatisch berechnet, sobald Position + Buchungs-Verknüpfung steht.
- Auto-Generation eines Belegs durch Booking-Update geschieht nicht immer (genaue Trigger-Bedingung unklar; nicht im Scope).
- Priorität: **Hoch**.
