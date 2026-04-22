# US-0058 create_invoice um fehlende Felder erweitern

**Issue:** [#90](https://github.com/RalfGuder/MCP-easyVerein/issues/90)

## User Story

**Als** MCP-Nutzer mit Buchhaltungs-/DATEV-Integration,
**möchte ich** beim Anlegen einer Rechnung über `create_invoice` zusätzliche Felder für Referenznummer, Zahlungsinformation, Mahnwesen, DATEV-Kennzahlen sowie Modus und Angebotsstatus setzen können,
**damit** neu angelegte Rechnungen direkt alle notwendigen Metadaten enthalten und nicht nachträglich per PATCH ergänzt werden müssen.

## Ausgangslage

Das aktuelle Tool `create_invoice` (`src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs:66`) akzeptiert nur vier Parameter (`invoiceNumber`, `totalPrice`, `description`, `kind`). Laut OpenAPI-Spezifikation (`docs/api/easyverein-v2.0.yaml` Z. 22140–22326) nimmt der Endpunkt `POST /api/v2.0/invoice` jedoch über 30 Felder entgegen. Für typische Workflows fehlen u.a. die Felder für Referenznummer, Zahlungsinformation, Mahnstufe und DATEV-Nummern.

## Akzeptanzkriterien

- [ ] `create_invoice` akzeptiert zusätzlich die optionalen Parameter `refNumber`, `paymentInformation`, `actualCallStateName`, `callStateDelayDays`, `accnumber`, `guid`, `mode` und `offerStatus`.
- [ ] Alle neuen Parameter sind nullable; nicht gesetzte Werte werden **nicht** im POST-Body mitgesendet (keine Default-Überschreibung serverseitig).
- [ ] Die bisherigen Parameter (`invoiceNumber`, `totalPrice`, `description`, `kind`) bleiben in Reihenfolge und Semantik unverändert (non-breaking).
- [ ] MCP-Tool-Beschreibung (`[Description]`) nennt die erlaubten Werte für `mode`, `kind` und `paymentInformation` aus der API-Doku.
- [ ] `isDraft` wird **nicht** als Parameter ergänzt; das Thema `invoiceItems`-Pflicht bei Non-Draft wird in einer separaten User Story adressiert.
- [ ] xUnit-Tests in `tests/MCP.EasyVerein.Server.Tests` (neu oder erweitert) decken ab:
  - Tool serialisiert alle neuen Felder korrekt in den POST-Body.
  - Nicht übergebene Felder erscheinen nicht im JSON-Body.
- [ ] Gesamtzahl der Tests erhöht; Build und Test-Suite bleiben grün (Coverage ≥ 70 %).

## Aufgaben

- Feature-Branch `feature/US-0058-create-invoice-pflichtfelder` anlegen.
- `CreateInvoice`-Signatur in `InvoiceTools.cs` um acht optionale Parameter erweitern.
- Invoice-Konstruktion so umbauen, dass nur gesetzte Parameter ins Entity übernommen werden (damit `JsonIgnoreCondition.WhenWritingNull` nicht-gesetzte Felder weglässt).
- Prüfen, ob `JsonSerializerOptions` in `EasyVereinApiClient.CreateInvoiceAsync` bereits `WhenWritingNull` nutzt; ggf. für diesen Serializer-Pfad aktivieren.
- xUnit-Tests für die neuen Parameter schreiben (Tool-Tests mit gemocktem `IEasyVereinApiClient` oder HTTP-Mock).
- PR gegen `main` erstellen, CI auf grün prüfen.

## Technische Hinweise

- API-Feldnamen in `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceFields.cs` sind bereits vorhanden (Konstanten: `RefNumber`, `PaymentInformation`, `ActualCallStateName`, `CallStateDelayDays`, `AccountNumber` = `accnumber`, `Guid`, `Mode`, `OfferStatus`).
- `Invoice`-Entity deckt alle Properties bereits ab (`src/MCP.EasyVerein.Domain/Entities/Invoice.cs`).
- `paymentInformation` erlaubt laut Spec: `'Nothing'`, `'Account'`, `'Debit'`, `'Cash'`; `kind` erlaubt: `'Balance'`, `'Donation'`, `'Membership'`, `'Revenue'`, `'Expense'`, `'Cancel'`, `'Credit'`, `'Selfissuedreceipt'`.
- Serializer-Verhalten beachten: Derzeit werden nicht-nullable `bool`-Properties (`Gross`, `IsDraft`, `IsTemplate`, …) unabhängig vom Parameter-Input immer mit `false` mitgesendet. Das ist außerhalb des Scopes dieser Story und wird dokumentiert, aber nicht behoben.
- Keine neuen Domain-Felder notwendig; die Story bewegt sich rein in der Server-Schicht.
