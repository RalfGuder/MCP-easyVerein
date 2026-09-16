# Design: Booking-Endpoint implementieren (US-009)

> **Datum:** 2026-04-04
> **Status:** Abgenommen
> **User Story:** [US-009 – Booking-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/16)
> **Requirement:** FR-045

## Kontext

Der MCP-easyVerein-Server unterstützt bereits CRUD-Operationen für Member, ContactDetails, Invoice und Event. Der Booking-Endpoint (`/booking`) erweitert die Finanzverwaltung um Buchungen. Die Implementierung folgt dem etablierten Muster mit konsistentem Error-Handling (Ansatz B: alle Tool-Methoden mit try/catch, PATCH-Dictionary wie ContactDetailsTools).

## API-Felder

Quelle: python-easyverein-Bibliothek, vom Benutzer bestätigt.

| JSON-Feldname | C#-Property | C#-Typ | Beschreibung |
|---|---|---|---|
| `id` | Id | `long` | Eindeutige ID |
| `amount` | Amount | `decimal?` | Buchungsbetrag |
| `bankAccount` | BankAccount | `long?` | Referenz auf Bankkonto |
| `billingAccount` | BillingAccount | `long?` | Referenz auf Abrechnungskonto |
| `description` | Description | `string?` | Beschreibung |
| `date` | Date | `DateTime?` | Buchungsdatum |
| `receiver` | Receiver | `string?` | Empfänger |
| `billingId` | BillingId | `string?` | Abrechnungs-ID |
| `blocked` | Blocked | `bool` | Gesperrt |
| `paymentDifference` | PaymentDifference | `decimal?` | Zahlungsdifferenz |
| `counterpartIban` | CounterpartIban | `string?` | Gegenkonto IBAN |
| `counterpartBic` | CounterpartBic | `string?` | Gegenkonto BIC |
| `twingleDonation` | TwingleDonation | `bool` | Twingle-Spende |
| `bookingProject` | BookingProject | `string?` | Buchungsprojekt-Referenz |
| `sphere` | Sphere | `string?` | Sphäre (Bereich) |
| `relatedInvoice` | RelatedInvoice | `List<long>?` | Verknüpfte Rechnungen |

## Architektur

### Domain Layer

**`BookingFields.cs`** — 16 Konstanten für API-Feldnamen (analog zu `InvoiceFields.cs`).

**`Booking.cs`** — Domain-Entity mit 16 Properties, jeweils mit `[JsonPropertyName(BookingFields.X)]`-Attributen und XML-Dokumentationskommentaren.

**`IEasyVereinApiClient.cs`** — 5 neue Methoden:
- `ListBookingsAsync(long? id, CancellationToken ct)`
- `GetBookingAsync(long id, CancellationToken ct)`
- `CreateBookingAsync(Booking booking, CancellationToken ct)`
- `UpdateBookingAsync(long id, object patchData, CancellationToken ct)`
- `DeleteBookingAsync(long id, CancellationToken ct)`

### Infrastructure Layer

**`BookingQuery.cs`** — Query-Klasse mit `Id`-Filter und `FieldQuery`-Konstante für Feldauswahl. `ToString()` baut den Query-String zusammen.

**`ApiQueries.cs`** — Neue `BookingQuery`-Instanz und `Booking`-Property.

**`EasyVereinApiClient.cs`** — 5 neue Methoden:

| Methode | HTTP | URL | Besonderheit |
|---|---|---|---|
| `ListBookingsAsync` | GET | `/booking?{query}&limit=100` | Pagination über `next` |
| `GetBookingAsync` | GET | `/booking/{id}{query}` | 404 → `null` |
| `CreateBookingAsync` | POST | `/booking` | JSON-Body |
| `UpdateBookingAsync` | PATCH | `/booking/{id}` | Dictionary → JSON (nur geänderte Felder) |
| `DeleteBookingAsync` | DELETE | `/booking/{id}` | `EnsureSuccessOrThrowAsync` |

`UpdateBookingAsync` folgt dem `UpdateContactDetailsAsync`-Muster: `object patchData` wird mit `JsonSerializer.Serialize(patchData, patchData.GetType())` serialisiert.

### Server Layer

**`BookingTools.cs`** — 5 MCP-Tools mit konsistentem Error-Handling (try/catch in allen Methoden):

| Tool-Name | Parameter | Beschreibung |
|---|---|---|
| `list_bookings` | `long? id` | Alle Buchungen auflisten (mit optionalem ID-Filter) |
| `get_booking` | `long id` | Einzelne Buchung abrufen |
| `create_booking` | `decimal amount`, `string receiver`, `string? description`, `string? date` | Neue Buchung anlegen |
| `update_booking` | `long id`, `decimal? amount`, `string? description`, `string? date`, `string? receiver` | Buchung bearbeiten (PATCH-Dictionary) |
| `delete_booking` | `long id` | Buchung löschen |

**`Program.cs`** — Registrierung: `.WithTools<BookingTools>()`

## Tests (TDD)

### `BookingEntityTests.cs` (neu)

- `JsonPropertyNames_AreCorrect` — JSON-Roundtrip-Test aller 16 Felder

### `EasyVereinApiClientTests.cs` (erweitert)

- `GetBookings_ReturnsBookings` — List-Endpunkt funktioniert
- `GetBooking_WithNotFound_ReturnsNull` — 404-Handling
- `GetBookings_WithUnauthorized_ThrowsUnauthorizedAccessException` — Auth-Fehler

## Dateien-Übersicht

| Datei | Aktion |
|---|---|
| `Domain/ValueObjects/BookingFields.cs` | Neu |
| `Domain/Entities/Booking.cs` | Neu |
| `Domain/Interfaces/IEasyVereinApiClient.cs` | Erweitert |
| `Infrastructure/ApiClient/BookingQuery.cs` | Neu |
| `Infrastructure/ApiClient/ApiQueries.cs` | Erweitert |
| `Infrastructure/ApiClient/EasyVereinApiClient.cs` | Erweitert |
| `Server/Tools/BookingTools.cs` | Neu |
| `Server/Program.cs` | Erweitert |
| `Domain.Tests/BookingEntityTests.cs` | Neu |
| `Infrastructure.Tests/EasyVereinApiClientTests.cs` | Erweitert |

## Designentscheidungen

1. **Ansatz B (konsistentes Error-Handling):** Alle 5 Tool-Methoden mit try/catch, nicht nur List (wie bei ContactDetailsTools/MemberTools)
2. **PATCH-Dictionary:** UpdateBooking nutzt `Dictionary<string, object>` statt Entity-Objekt (wie ContactDetailsTools)
3. **Update-Parameter beschränkt:** Nur `amount`, `description`, `date`, `receiver` (häufigste Felder)
4. **Query nur Id-Filter:** Datum- und Mitglied-Filter sind in der v1.7 API nicht als Query-Parameter verfügbar
5. **Keine Server-Tests:** Server.Tests-Projekt bleibt leer — eigenes Thema
