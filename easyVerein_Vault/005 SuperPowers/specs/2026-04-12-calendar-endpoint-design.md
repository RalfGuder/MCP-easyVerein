# Design-Spezifikation: US-0014 Calendar-Endpoint

> **GitHub Issue:** [#21 – US-0014 Calendar-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/21)
> **Datum:** 2026-04-12

## Überblick

Implementierung des Calendar-Endpoints als MCP-Tools mit vollständigen CRUD-Operationen und allen verfügbaren API-Filtern. Folgt dem etablierten Booking-Muster.

## API-Verifizierung (curl)

Die folgenden Felder wurden am 2026-04-12 gegen die easyVerein API v1.7 verifiziert:

**Verfügbare Felder:**
- `id` (long) — Primary Key
- `name` (string, max. 200 Zeichen, erforderlich)
- `color` (string, max. 7 Zeichen, Hex-Farbwert)
- `short` (string, max. 4 Zeichen, muss eindeutig sein in v1.7)
- `allowedGroups` (MemberGroup[]) — Nested Query `allowedGroups{id}` liefert Objekte mit `id`
- `linkedItems` (int) — Anzahl aktiver Relationen (read-only)
- `deleteEventsAfterDeletion` (bool, Default: false)

**Bewusst weggelassen (existieren nicht in der tatsächlichen API):**
- `calendarImportURL` — 400-Fehler bei Query
- `deleted` — 400-Fehler bei Query

**Filter-Parameter (verifiziert):**
- `name`, `color`, `short` — exakte Filterung
- `name__not`, `color__not`, `short__not` — Negation
- `id__in` — Mehrfach-ID-Filter
- `allowedGroups` — Gruppen-Filter
- `ordering` — Sortierung (auch mit `-` für absteigend)
- `search` — Suchfelder: `name`, `short`, `color`

**Testdaten im Verein:**
- Kulturverein (ID: 335702286), Dorfscheune (ID: 344715981), Feiertage (ID: 400324380)

## 1. Domain Layer

### MemberGroup.cs (neue Entity)

Minimale Entity als Referenz für `allowedGroups`:

```csharp
public class MemberGroup
{
    [JsonPropertyName("id")] public long Id { get; set; }
}
```

### CalendarFields.cs (ValueObject)

Konstanten für alle API-Feldnamen:

| Konstante | Wert | Verwendung |
|-----------|------|------------|
| `Id` | `"id"` | Entity + Filter |
| `Name` | `"name"` | Entity + Filter |
| `Color` | `"color"` | Entity + Filter |
| `Short` | `"short"` | Entity + Filter |
| `AllowedGroups` | `"allowedGroups"` | Entity + Filter |
| `LinkedItems` | `"linkedItems"` | Entity (read-only) |
| `DeleteEventsAfterDeletion` | `"deleteEventsAfterDeletion"` | Entity |
| `NameNot` | `"name__not"` | Filter |
| `ColorNot` | `"color__not"` | Filter |
| `ShortNot` | `"short__not"` | Filter |
| `IdIn` | `"id__in"` | Filter |
| `Ordering` | `"ordering"` | Filter |
| `Search` | `"search"` | Filter |

### Calendar.cs (Entity)

```
Properties:
- Id (long)
- Name (string?)
- Color (string?)
- Short (string?)
- AllowedGroups (MemberGroup[]?)
- LinkedItems (int?)
- DeleteEventsAfterDeletion (bool?)
```

Alle Properties mit `[JsonPropertyName(CalendarFields.Xxx)]`-Attributen.

### IEasyVereinApiClient.cs (Interface-Erweiterung)

5 neue Methoden:

```
ListCalendarsAsync(long? id, string? name, string? color, string? short_,
    string? nameNot, string? colorNot, string? shortNot, string? idIn,
    string? allowedGroups, string? ordering, string[]? search,
    CancellationToken ct) → IReadOnlyList<Calendar>

GetCalendarAsync(long id, CancellationToken ct) → Calendar?

CreateCalendarAsync(Calendar calendar, CancellationToken ct) → Calendar

UpdateCalendarAsync(long id, object patchData, CancellationToken ct) → Calendar

DeleteCalendarAsync(long id, CancellationToken ct) → Task
```

## 2. Infrastructure Layer

### CalendarQuery.cs (Query-Builder)

- `FieldQuery`: `query={id,name,color,short,allowedGroups{id},linkedItems,deleteEventsAfterDeletion}`
- 10 Filter-Properties: `Id`, `Name`, `Color`, `Short`, `NameNot`, `ColorNot`, `ShortNot`, `IdIn`, `AllowedGroups`, `Ordering`, `Search`
- `ToString()` baut dynamisch den Query-String mit allen gesetzten Filtern

### ApiQueries.cs (Erweiterung)

- Neues `CalendarQuery`-Singleton
- Neues `Calendar`-Property mit `ToString()`-Aufruf

### EasyVereinApiClient.cs (Implementierung)

5 Methoden nach Booking-Muster:

| Methode | HTTP | Muster |
|---------|------|--------|
| `ListCalendarsAsync` | GET /calendar | Filter auf Query-Objekt setzen, `HandleListResponseWithPagination<Calendar>` |
| `GetCalendarAsync` | GET /calendar/{id} | `BuildGetUrl`, NotFound → null |
| `CreateCalendarAsync` | POST /calendar | `PostAsJsonAsync` |
| `UpdateCalendarAsync` | PATCH /calendar/{id} | Dictionary serialisieren, `PatchAsync` |
| `DeleteCalendarAsync` | DELETE /calendar/{id} | `EnsureSuccessOrThrowAsync` |

## 3. Server Layer

### CalendarTools.cs (MCP-Tools)

| Tool | Parameter | Beschreibung |
|------|-----------|--------------|
| `list_calendars` | `name`, `color`, `short`, `nameNot`, `colorNot`, `shortNot`, `idIn`, `allowedGroups`, `ordering`, `search` | Alle Kalender mit vollständigen Filtern |
| `get_calendar` | `id` | Einzelnen Kalender per ID |
| `create_calendar` | `name` (required), `color`, `short`, `allowedGroups` (long[]), `deleteEventsAfterDeletion` | Neuen Kalender erstellen |
| `update_calendar` | `id` (required), `name`, `color`, `short`, `allowedGroups` (long[]), `deleteEventsAfterDeletion` | PATCH-Update (nur übergebene Felder) |
| `delete_calendar` | `id` | Kalender löschen |

**Besonderheit `allowedGroups`:**
- `list_calendars`: Filter-Parameter `allowedGroups` ist ein `string` (Gruppen-ID als String, wie von der API erwartet)
- `create_calendar` / `update_calendar`: Parameter `allowedGroups` ist `long[]` (IDs). Das Tool konstruiert intern `MemberGroup`-Objekte daraus, bevor sie an den API-Client übergeben werden.

### Program.cs (Registrierung)

```csharp
.WithTools<CalendarTools>()
```

## 4. Tests (TDD)

### Domain.Tests

- **CalendarEntityTests:** JSON-Deserialisierung aller Felder inkl. nested `MemberGroup` in `allowedGroups`
- **CalendarFieldsTests:** Validierung aller Feldkonstanten

### Infrastructure.Tests

- **CalendarApiClientTests:** CRUD-Operationen mit gemocktem `HttpMessageHandler`
  - List mit Pagination
  - Get (found + not found)
  - Create
  - Update (PATCH)
  - Delete

## 5. Dateiübersicht

| Schicht | Datei | Aktion |
|---------|-------|--------|
| Domain | `Entities/MemberGroup.cs` | Neu |
| Domain | `ValueObjects/CalendarFields.cs` | Neu |
| Domain | `Entities/Calendar.cs` | Neu |
| Domain | `Interfaces/IEasyVereinApiClient.cs` | Erweitern |
| Infrastructure | `QueryBuilder/CalendarQuery.cs` | Neu |
| Infrastructure | `QueryBuilder/ApiQueries.cs` | Erweitern |
| Infrastructure | `ApiClient/EasyVereinApiClient.cs` | Erweitern |
| Server | `Tools/CalendarTools.cs` | Neu |
| Server | `Program.cs` | Erweitern |
| Tests/Domain | `CalendarEntityTests.cs` | Neu |
| Tests/Domain | `CalendarFieldsTests.cs` | Neu |
| Tests/Infrastructure | `CalendarApiClientTests.cs` | Neu |
