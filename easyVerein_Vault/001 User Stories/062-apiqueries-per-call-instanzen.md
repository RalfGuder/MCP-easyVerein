# User Story 062: ApiQueries Static-Singleton durch per-call Query-Instanzen ersetzen

> **GitHub Issue:** [#102 – US-0062 ApiQueries Static-Singleton durch per-call Query-Instanzen ersetzen](https://github.com/RalfGuder/MCP-easyVerein/issues/102)

## User Story

**Als** Maintainer des MCP-easyVerein-Servers,
**möchte ich** Query-Builder pro Aufruf instanziieren statt geteilte static Singletons zu nutzen,
**damit** Filter aus früheren Aufrufen nicht in nachfolgende API-Requests lecken und gleichzeitige MCP-Tool-Calls keine Race-Conditions erzeugen.

## Hintergrund

`Infrastructure/ApiClient/ApiQueries.cs` exponiert für jede Entität eine `internal static readonly XxxQuery`-Instanz mit mutable Properties. `ListXxxAsync` setzt darauf Filter, `ToString()` materialisiert daraus den Query-String. Folgen:

1. **Filter-Leak** in nachfolgende GETs (Single-Resource-Endpoints). Ursache von Bug #2 / PR #101 (`get_booking` 404 nach `list_bookings` mit Filtern).
2. **Race-Conditions** bei parallelen MCP-Tool-Calls — zwei gleichzeitige `list_invoice_items`-Calls überschreiben sich gegenseitig die Filter-Properties.
3. **Inversion of Control gebrochen** — der API-Client liest static-state statt expliziter Parameter.

PR #101 hat die Klasse nur für `GetBookingAsync` umgangen (`BookingQuery.FieldQuery` direkt verwendet). Das löst das Symptom für eine einzige Methode, nicht das Pattern.

## Akzeptanzkriterien

- [ ] `Infrastructure/ApiClient/ApiQueries.cs` entfernt
- [ ] Alle 11 Query-Klassen (Announcement, BankAccount, BillingAccount, Booking, BookingProject, Calendar, ChairmanLevel, ContactDetails, Event, InvoiceItem, Member) werden in jedem `ListXxxAsync`-Aufruf lokal als `new XxxQuery {...}` instanziiert
- [ ] Jede Query-Klasse exponiert `FieldQuery` als `internal const string` (analog `BookingQuery.FieldQuery` aus PR #101)
- [ ] Alle `GetXxxAsync`-Methoden verwenden ausschließlich `XxxQuery.FieldQuery` — keine Filter im URL-String
- [ ] Regressions-Tests pro Endpoint: nach `ListXxxAsync(...mit Filtern...)` enthält die nachfolgende `GetXxxAsync(id)`-URL **keine** Filter
- [ ] Concurrency-Test: zwei parallele `ListXxxAsync`-Calls mit unterschiedlichen Filtern führen zu URLs, die jeweils nur ihre eigenen Filter enthalten
- [ ] Alle bestehenden Tests bleiben grün; öffentliche `IEasyVereinApiClient`-Signaturen unverändert
- [ ] Coverage ≥ 70 %

## Aufgaben

- Inventarisierung aller `XxxQuery`-Klassen und der `Get*Async` / `List*Async`-Aufrufer
- Pro Query-Klasse: `FieldQuery` von `private` auf `internal` hochziehen (falls noch nicht)
- `ListXxxAsync` jeweils refactorieren: lokale `new XxxQuery { Filter1 = ..., Filter2 = ... }`-Instanz
- `GetXxxAsync` umstellen auf `XxxQuery.FieldQuery`
- `ApiQueries.cs` löschen
- Regressions-Test pro Endpoint (List→Get-Sequenz; Concurrency-Smoke)

## Technische Hinweise

- Vorbild: PR #101 hat `BookingQuery.FieldQuery` bereits umgestellt
- `Invoice` hat keine Query-Klasse, nutzt `ApiQueries.Invoice` als `public const string` — wird mitentfernt, Konstante an `InvoiceQuery.FieldQuery` migrieren oder als Konstante in `EasyVereinApiClient` belassen
- `CapturingFakeHttpHandler` (single-call URI) und `MultiPageFakeHttpHandler` (jetzt mit `LastRequestUri` seit PR #101) bilden die Test-Basis
- Keine Breaking Changes — `ApiQueries` ist `internal`
- Branch: `feature/US-0062-per-call-query-instanzen`

Verlinkt mit PR #101 (initialer Fix für `BookingQuery`).
