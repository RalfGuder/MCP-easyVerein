# Design-Spezifikation: Event-Endpoint Modernisierung

> **Datum:** 2026-04-12
> **Scope:** Modernisierung des bestehenden Event-Endpoints nach Calendar/Booking-Muster

## Überblick

Nachrüstung des Event-Endpoints mit Error-Handling, Filter-Parametern, Query-Builder und Update-Tool. Folgt dem etablierten Muster der neueren Endpoints (Booking, Calendar).

## API-Verifizierung (curl)

Die folgenden Filter wurden am 2026-04-12 gegen die easyVerein API v1.7 verifiziert:

**Funktionierende Filter:**
- `start__gte`, `start__lte` — Startdatum-Bereich (ISO 8601)
- `end__gte`, `end__lte` — Enddatum-Bereich (ISO 8601)
- `name` — exakter Namensfilter
- `calendar` — Kalender-ID-Filter
- `canceled` — Boolean (abgesagt ja/nein)
- `isPublic` — Boolean (öffentlich ja/nein)
- `id__in` — Komma-separierte IDs
- `ordering` — Sortierung (z.B. `start`, `-start`)
- `search` — Volltextsuche

**Neues Feld:** `calendar{id}` — nested Query liefert Calendar-Objekt mit `id` statt URL

**Testdaten:** 223 Events gesamt, 23 Events im Mai 2026

## 1. Domain Layer

### EventFields.cs (Erweiterung)

Neue Filter-Konstanten hinzufügen zum bestehenden `EventFields.cs`:

| Konstante | Wert | Verwendung |
|-----------|------|------------|
| `Calendar` | `"calendar"` | Entity + Filter |
| `StartGte` | `"start__gte"` | Filter |
| `StartLte` | `"start__lte"` | Filter |
| `EndGte` | `"end__gte"` | Filter |
| `EndLte` | `"end__lte"` | Filter |
| `IdIn` | `"id__in"` | Filter |
| `Ordering` | `"ordering"` | Filter |
| `Search` | `"search"` | Filter |

Hinweis: `name`, `canceled`, `isPublic` existieren bereits als Entity-Konstanten und können direkt als Filter verwendet werden.

### Event.cs (Erweiterung)

Neues Property:
```
Calendar (Calendar?) — [JsonPropertyName(EventFields.Calendar)], nested query calendar{id}
```

### IEasyVereinApiClient.cs (Änderung)

- `GetEventsAsync(CancellationToken)` → `ListEventsAsync(...)` mit 11 optionalen Filter-Parametern:
  - `name`, `startGte`, `startLte`, `endGte`, `endLte`, `calendar`, `canceled`, `isPublic`, `idIn`, `ordering`, `search`
- `UpdateEventAsync` existiert bereits im Interface — keine Änderung nötig

## 2. Infrastructure Layer

### EventQuery.cs (Neu)

Query-Builder nach Calendar-Muster:
- `FieldQuery`: alle 25 bestehenden Entity-Felder + `calendar{id}` als nested query
- 11 Filter-Properties: `Name`, `StartGte`, `StartLte`, `EndGte`, `EndLte`, `Calendar`, `Canceled`, `IsPublic`, `IdIn`, `Ordering`, `Search`
- `ToString()` baut Query-String dynamisch

### ApiQueries.cs (Änderung)

- Konstante `Event` (String) → `EventQuery`-Singleton + dynamisches Property
- Altes `public const string Event = "query={...}"` entfernen

### EasyVereinApiClient.cs (Änderung)

- `GetEventsAsync()` → `ListEventsAsync(...)` mit 11 Filtern + Query-Builder
- `UpdateEventAsync` existiert bereits — keine Änderung nötig

## 3. Server Layer

### EventTools.cs (Komplett-Überarbeitung)

| Tool | Parameter | Änderung |
|------|-----------|----------|
| `list_events` | `name`, `startGte`, `startLte`, `endGte`, `endLte`, `calendar`, `canceled`, `isPublic`, `idIn`, `ordering`, `search` | Vorher keine Filter; jetzt 11 Filter + try-catch |
| `get_event` | `id` | +try-catch |
| `create_event` | `name`, `description`, `locationName` | +try-catch |
| `update_event` | `id`, `name`, `description`, `locationName`, `start`, `end`, `allDay`, `canceled`, `isPublic` | **Neu** — PATCH-Semantik |
| `delete_event` | `id` | +try-catch |

Weitere Änderungen:
- Primary Constructor `EventTools(IEasyVereinApiClient client)` statt Feld-Injection
- Konsistentes Error-Handling auf allen Tools

## 4. Tests

### Domain.Tests — EventEntityTests.cs (Erweiterung)

Neuer Test: `JsonPropertyNames_WithCalendar_AreCorrect` — prüft `calendar{id}` nested Deserialisierung

### Infrastructure.Tests — EasyVereinApiClientTests.cs (Erweiterung)

4 neue Tests:
- `ListEvents_ReturnsEvents` — Basistest
- `GetEvent_WithNotFound_ReturnsNull` — existiert ggf. schon, prüfen
- `ListEvents_WithUnauthorized_ThrowsUnauthorizedAccessException`
- `ListEvents_SendsQueryParameter` — prüft query= und limit=100

## 5. Dateiübersicht

| Schicht | Datei | Aktion |
|---------|-------|--------|
| Domain | `ValueObjects/EventFields.cs` | +8 Filter-Konstanten |
| Domain | `Entities/Event.cs` | +Calendar Property |
| Domain | `Interfaces/IEasyVereinApiClient.cs` | `GetEventsAsync` → `ListEventsAsync` |
| Infrastructure | `ApiClient/EventQuery.cs` | Neu |
| Infrastructure | `ApiClient/ApiQueries.cs` | Event-Konstante → EventQuery |
| Infrastructure | `ApiClient/EasyVereinApiClient.cs` | `GetEventsAsync` → `ListEventsAsync` |
| Server | `Tools/EventTools.cs` | Komplett-Überarbeitung |
| Tests/Domain | `EventEntityTests.cs` | +Calendar-nested-Test |
| Tests/Infrastructure | `EasyVereinApiClientTests.cs` | +Event-Filter-Tests |

## Breaking Changes

- `GetEventsAsync()` wird zu `ListEventsAsync(...)` — betrifft Interface, Client und alle Aufrufer (EventTools)
- `ApiQueries.Event` ändert sich von `const string` zu dynamischem Property — betrifft `GetEventAsync` in EasyVereinApiClient
