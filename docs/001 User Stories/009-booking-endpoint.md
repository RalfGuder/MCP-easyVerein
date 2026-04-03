# User Story 009: Booking-Endpoint implementieren

> **GitHub Issue:** [#16 – US-0009 Booking-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/16)

## User Story

**Als** Vereinsadministrator,
**möchte ich** Buchungen über den MCP-Server abfragen, anlegen, bearbeiten und löschen können,
**damit** ich die Finanzbuchhaltung des Vereins vollständig über den MCP-Server verwalten kann.

## Akzeptanzkriterien

- [ ] **Entity `Booking`:** Domain-Entity mit allen API-Feldern und `[JsonPropertyName]`-Attributen über `BookingFields`-Konstanten
- [ ] **ValueObject `BookingFields.cs`:** Alle API-Feldnamen als Konstanten (analog zu `MemberFields`, `InvoiceFields` etc.)
- [ ] **Query-Klasse `BookingQuery.cs`:** Filterung nach ID, Datum und Mitglied (analog zu `ContactDetailsQuery`, `MemberQuery`)
- [ ] **API-Client:** `ListBookingsAsync`, `GetBookingAsync`, `CreateBookingAsync`, `UpdateBookingAsync`, `DeleteBookingAsync` im `IEasyVereinApiClient` und `EasyVereinApiClient`
- [ ] **MCP-Tools:** `BookingTools.cs` mit `ListBookings`, `GetBooking`, `CreateBooking`, `UpdateBooking`, `DeleteBooking` – inkl. Error-Handling
- [ ] **PATCH-Semantik:** Update sendet nur geänderte Felder als Dictionary (analog zu `UpdateContactDetails`)
- [ ] **Pagination:** Listen-Endpunkt ruft automatisch alle Seiten ab
- [ ] **Tests:** Unit-Tests für Entity, API-Client und Tools nach TDD (Red-Green-Refactor)

## Aufgaben

1. easyVerein API-Dokumentation für den `booking`-Endpoint analysieren (Feldnamen, Datentypen)
2. `BookingFields.cs` als ValueObject anlegen
3. `Booking.cs` Entity mit `BookingFields`-Konstanten erstellen
4. `BookingQuery.cs` für Standard-Filter (ID, Datum, Mitglied) implementieren
5. `ApiQueries.cs` um Booking-Query erweitern
6. `IEasyVereinApiClient` um Booking-CRUD-Methoden erweitern
7. `EasyVereinApiClient` implementieren (inkl. Pagination und PATCH-Dictionary)
8. `BookingTools.cs` als MCP-Tool-Klasse erstellen (inkl. Error-Handling)
9. `Program.cs` um Booking-Tools-Registrierung erweitern
10. Unit-Tests schreiben (TDD: Domain, Infrastructure, Tools)
11. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Booking-Endpoint: `GET/POST/PATCH/DELETE /booking`
- Feldauswahl via `query`-Parameter: `?query={field1,field2,...}`
- PATCH-Requests senden nur geänderte Felder als `Dictionary<string, object>` (nicht das gesamte Entity)
- Pagination: `?limit=100`, automatisch `next`-URL folgen
- Architektur konsistent mit bestehenden Entities (Member, ContactDetails, Invoice, Event)
- Priorität: **Mittel** – erweitert die Finanzverwaltung des MCP-Servers
