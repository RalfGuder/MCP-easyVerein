# Event-Endpoint Modernisierung Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Modernize the existing Event endpoint with filter parameters, error handling, query builder, and update tool — matching the Calendar/Booking pattern.

**Architecture:** Refactor existing Event code: add filter constants to EventFields, extend Event entity with Calendar reference, replace const query string with dynamic EventQuery builder, rewrite EventTools with filters and try-catch, add UpdateEvent tool. Breaking change: `GetEventsAsync` → `ListEventsAsync`.

**Tech Stack:** C# / .NET 8.0, ModelContextProtocol v1.2.0, xUnit 2.4.2

---

## File Structure

| Action | File | Responsibility |
|--------|------|----------------|
| Modify | `src/MCP.EasyVerein.Domain/ValueObjects/EventFields.cs` | +8 filter constants |
| Modify | `src/MCP.EasyVerein.Domain/Entities/Event.cs` | +Calendar property |
| Modify | `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` | `GetEventsAsync` → `ListEventsAsync` |
| Create | `src/MCP.EasyVerein.Infrastructure/ApiClient/EventQuery.cs` | Query builder with all filters |
| Modify | `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` | Event const → EventQuery |
| Modify | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` | `GetEventsAsync` → `ListEventsAsync` |
| Modify | `src/MCP.EasyVerein.Server/Tools/EventTools.cs` | Complete rewrite with 5 tools |
| Modify | `tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs` | +Calendar nested test |
| Modify | `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` | +Event filter tests |

---

### Task 1: EventFields Filter Constants + Event.Calendar Property (Domain)

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/ValueObjects/EventFields.cs`
- Modify: `src/MCP.EasyVerein.Domain/Entities/Event.cs`
- Modify: `tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs`

- [ ] **Step 1: Write the failing test for Calendar nested field**

Add a new test method to `tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs`:

```csharp
    [Fact]
    public void JsonPropertyNames_WithCalendar_AreCorrect()
    {
        var json = """
            {
                "id": 335703210,
                "name": "Mitgliederversammlung",
                "start": "2025-11-03T19:00:00",
                "end": "2025-11-03T21:00:00",
                "allDay": false,
                "calendar": {"id": 335702286},
                "canceled": false,
                "isPublic": false,
                "sendMailCheck": false,
                "showMemberarea": false,
                "massParticipations": false,
                "isReservation": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var ev = JsonSerializer.Deserialize<Event>(json, options);

        Assert.NotNull(ev);
        Assert.Equal(335703210L, ev.Id);
        Assert.Equal("Mitgliederversammlung", ev.Name);
        Assert.NotNull(ev.Calendar);
        Assert.Equal(335702286L, ev.Calendar.Id);
    }
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "WithCalendar" -v n`
Expected: FAIL — `Calendar` property does not exist on `Event`.

- [ ] **Step 3: Add filter constants to EventFields**

Add the following constants at the end of `src/MCP.EasyVerein.Domain/ValueObjects/EventFields.cs`, before the closing brace:

```csharp
        /// <summary>API field name for the calendar reference.</summary>
        public const string Calendar = "calendar";

        /// <summary>API filter field name for start date greater than or equal.</summary>
        public const string StartGte = "start__gte";

        /// <summary>API filter field name for start date less than or equal.</summary>
        public const string StartLte = "start__lte";

        /// <summary>API filter field name for end date greater than or equal.</summary>
        public const string EndGte = "end__gte";

        /// <summary>API filter field name for end date less than or equal.</summary>
        public const string EndLte = "end__lte";

        /// <summary>API filter field name for filtering by multiple IDs.</summary>
        public const string IdIn = "id__in";

        /// <summary>API query parameter for result ordering.</summary>
        public const string Ordering = "ordering";

        /// <summary>API query parameter for full-text search.</summary>
        public const string Search = "search";
```

- [ ] **Step 4: Add Calendar property to Event entity**

Add the following property to `src/MCP.EasyVerein.Domain/Entities/Event.cs`, after the `ReservationParentEvent` property:

```csharp
    /// <summary>
    /// Gets or sets the calendar reference. Maps to API field '<c>calendar</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Calendar)]
    public Calendar? Calendar { get; set; }
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter EventEntityTests -v n`
Expected: PASS (2 tests)

- [ ] **Step 6: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/EventFields.cs src/MCP.EasyVerein.Domain/Entities/Event.cs tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs
git commit -m "feat(domain): add Calendar property and filter constants to Event"
```

---

### Task 2: IEasyVereinApiClient — GetEventsAsync → ListEventsAsync (Domain)

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`

- [ ] **Step 1: Replace GetEventsAsync with ListEventsAsync**

In `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`, replace the existing `GetEventsAsync` method:

```csharp
    /// <summary>Gets all events.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all events.</returns>
    Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default);
```

with:

```csharp
    /// <summary>Lists events, optionally filtered by name, date range, calendar, and other criteria.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="startGte">Optional start date greater than or equal filter (ISO 8601).</param>
    /// <param name="startLte">Optional start date less than or equal filter (ISO 8601).</param>
    /// <param name="endGte">Optional end date greater than or equal filter (ISO 8601).</param>
    /// <param name="endLte">Optional end date less than or equal filter (ISO 8601).</param>
    /// <param name="calendar">Optional calendar ID filter.</param>
    /// <param name="canceled">Optional canceled filter.</param>
    /// <param name="isPublic">Optional public visibility filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching events.</returns>
    Task<IReadOnlyList<Event>> ListEventsAsync(string? name = null, string? startGte = null,
        string? startLte = null, string? endGte = null, string? endLte = null,
        string? calendar = null, string? canceled = null, string? isPublic = null,
        string? idIn = null, string? ordering = null, string[]? search = null,
        CancellationToken ct = default);
```

- [ ] **Step 2: Verify Domain builds**

Run: `dotnet build src/MCP.EasyVerein.Domain`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs
git commit -m "feat(domain): replace GetEventsAsync with ListEventsAsync in interface"
```

---

### Task 3: EventQuery Builder + ApiQueries (Infrastructure)

**Files:**
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/EventQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Create EventQuery**

Create `src/MCP.EasyVerein.Infrastructure/ApiClient/EventQuery.cs`:

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the event API endpoint, including field selection and optional filters.
/// </summary>
internal class EventQuery
{
    /// <summary>Gets or sets an optional name filter.</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional start date greater than or equal filter.</summary>
    internal string? StartGte { get; set; }

    /// <summary>Gets or sets an optional start date less than or equal filter.</summary>
    internal string? StartLte { get; set; }

    /// <summary>Gets or sets an optional end date greater than or equal filter.</summary>
    internal string? EndGte { get; set; }

    /// <summary>Gets or sets an optional end date less than or equal filter.</summary>
    internal string? EndLte { get; set; }

    /// <summary>Gets or sets an optional calendar ID filter.</summary>
    internal string? Calendar { get; set; }

    /// <summary>Gets or sets an optional canceled filter.</summary>
    internal string? Canceled { get; set; }

    /// <summary>Gets or sets an optional public visibility filter.</summary>
    internal string? IsPublic { get; set; }

    /// <summary>Gets or sets an optional comma-separated IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional ordering criterion for the results.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets optional search terms to filter events.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The base field selection query requesting all event fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            EventFields.Id + "," +
            EventFields.Name + "," +
            EventFields.Description + "," +
            EventFields.Prologue + "," +
            EventFields.Note + "," +
            EventFields.Start + "," +
            EventFields.End + "," +
            EventFields.AllDay + "," +
            EventFields.LocationName + "," +
            EventFields.LocationObject + "," +
            EventFields.Parent + "," +
            EventFields.MinParticipators + "," +
            EventFields.MaxParticipators + "," +
            EventFields.StartParticipation + "," +
            EventFields.EndParticipation + "," +
            EventFields.Access + "," +
            EventFields.Weekdays + "," +
            EventFields.SendMailCheck + "," +
            EventFields.ShowMemberArea + "," +
            EventFields.IsPublic + "," +
            EventFields.MassParticipations + "," +
            EventFields.Canceled + "," +
            EventFields.IsReservation + "," +
            EventFields.Creator + "," +
            EventFields.ReservationParentEvent + "," +
            EventFields.Calendar +
            "{" +
                EventFields.Id +
            "}" +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the event endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{EventFields.Name}={Name}");

        if (!string.IsNullOrEmpty(StartGte))
            parts.Add($"{EventFields.StartGte}={StartGte}");

        if (!string.IsNullOrEmpty(StartLte))
            parts.Add($"{EventFields.StartLte}={StartLte}");

        if (!string.IsNullOrEmpty(EndGte))
            parts.Add($"{EventFields.EndGte}={EndGte}");

        if (!string.IsNullOrEmpty(EndLte))
            parts.Add($"{EventFields.EndLte}={EndLte}");

        if (!string.IsNullOrEmpty(Calendar))
            parts.Add($"{EventFields.Calendar}={Calendar}");

        if (!string.IsNullOrEmpty(Canceled))
            parts.Add($"{EventFields.Canceled}={Canceled}");

        if (!string.IsNullOrEmpty(IsPublic))
            parts.Add($"{EventFields.IsPublic}={IsPublic}");

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{EventFields.IdIn}={IdIn}");

        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{EventFields.Ordering}={Ordering}");

        if (Search != null && Search.Length != 0)
            parts.Add($"{EventFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
```

- [ ] **Step 2: Replace Event constant with EventQuery in ApiQueries**

In `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`, replace:

```csharp
    /// <summary>
    /// Query string for event endpoints specifying all requested fields.
    /// </summary>
    public const string Event =
        "query={id,name,description,prologue,note,start,end,allDay," +
        "locationName,locationObject,parent," +
        "minParticipators,maxParticipators," +
        "startParticipation,endParticipation,access,weekdays," +
        "sendMailCheck,showMemberarea,isPublic,massParticipations," +
        "canceled,isReservation,creator,reservationParentEvent}";
```

with:

```csharp
    /// <summary>
    /// Shared <see cref="EventQuery"/> instance used to build event query strings with optional filters.
    /// </summary>
    internal static readonly EventQuery EventQuery = new();

    /// <summary>
    /// Gets the current event query string including field selection and any active filters.
    /// </summary>
    internal static string Event => EventQuery.ToString();
```

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/EventQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
git commit -m "feat(infra): add EventQuery builder and replace Event constant in ApiQueries"
```

---

### Task 4: EasyVereinApiClient — GetEventsAsync → ListEventsAsync (Infrastructure)

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

- [ ] **Step 1: Write failing infrastructure tests**

Add the following tests to the `EasyVereinApiClientTests` class in `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`. Also rename the existing `GetEvents_ReturnsEvents` test.

Replace the existing Events section:

```csharp
    // ------------------------------------------------------------------ //
    // Events
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetEvents_ReturnsEvents()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, name = "Jahresversammlung" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.GetEventsAsync();

        Assert.Single(result);
        Assert.Equal("Jahresversammlung", result[0].Name);
    }
```

with:

```csharp
    // ------------------------------------------------------------------ //
    // Events
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListEvents_ReturnsEvents()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, name = "Jahresversammlung" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListEventsAsync();

        Assert.Single(result);
        Assert.Equal("Jahresversammlung", result[0].Name);
    }

    [Fact]
    public async Task ListEvents_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListEventsAsync());
    }

    [Fact]
    public async Task ListEvents_SendsQueryParameter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListEventsAsync();

        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("query=", handler.LastRequestUri!.Query);
        Assert.Contains("limit=100", handler.LastRequestUri!.Query);
    }
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --filter "ListEvents" -v n`
Expected: FAIL — `ListEventsAsync` does not exist.

- [ ] **Step 3: Replace GetEventsAsync with ListEventsAsync in EasyVereinApiClient**

In `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`, replace:

```csharp
    /// <summary>
    /// Retrieves all events with automatic pagination.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all <see cref="Event"/> records.</returns>
    public async Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Event>(
            BuildListUrl("event", ApiQueries.Event), ct);
    }
```

with:

```csharp
    /// <summary>Lists events with optional filters and automatic pagination.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="startGte">Optional start date greater than or equal filter.</param>
    /// <param name="startLte">Optional start date less than or equal filter.</param>
    /// <param name="endGte">Optional end date greater than or equal filter.</param>
    /// <param name="endLte">Optional end date less than or equal filter.</param>
    /// <param name="calendar">Optional calendar ID filter.</param>
    /// <param name="canceled">Optional canceled filter.</param>
    /// <param name="isPublic">Optional public visibility filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="Event"/> records.</returns>
    public async Task<IReadOnlyList<Event>> ListEventsAsync(string? name = null, string? startGte = null,
        string? startLte = null, string? endGte = null, string? endLte = null,
        string? calendar = null, string? canceled = null, string? isPublic = null,
        string? idIn = null, string? ordering = null, string[]? search = null,
        CancellationToken ct = default)
    {
        ApiQueries.EventQuery.Name = name;
        ApiQueries.EventQuery.StartGte = startGte;
        ApiQueries.EventQuery.StartLte = startLte;
        ApiQueries.EventQuery.EndGte = endGte;
        ApiQueries.EventQuery.EndLte = endLte;
        ApiQueries.EventQuery.Calendar = calendar;
        ApiQueries.EventQuery.Canceled = canceled;
        ApiQueries.EventQuery.IsPublic = isPublic;
        ApiQueries.EventQuery.IdIn = idIn;
        ApiQueries.EventQuery.Ordering = ordering;
        ApiQueries.EventQuery.Search = search;

        return await HandleListResponseWithPagination<Event>(
            BuildListUrl("event", ApiQueries.Event), ct);
    }
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --filter "ListEvents" -v n`
Expected: PASS (3 tests)

- [ ] **Step 5: Run all tests to check for regressions**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests && dotnet test tests/MCP.EasyVerein.Application.Tests && dotnet test tests/MCP.EasyVerein.Infrastructure.Tests`
Expected: All tests pass.

- [ ] **Step 6: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
git commit -m "feat(infra): replace GetEventsAsync with ListEventsAsync and add filter tests"
```

---

### Task 5: EventTools Rewrite (Server)

**Files:**
- Modify: `src/MCP.EasyVerein.Server/Tools/EventTools.cs`

- [ ] **Step 1: Rewrite EventTools completely**

Replace the entire content of `src/MCP.EasyVerein.Server/Tools/EventTools.cs` with:

```csharp
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing events via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class EventTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists events with optional filters and automatic pagination.
    /// </summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="startGte">Optional start date greater than or equal filter.</param>
    /// <param name="startLte">Optional start date less than or equal filter.</param>
    /// <param name="endGte">Optional end date greater than or equal filter.</param>
    /// <param name="endLte">Optional end date less than or equal filter.</param>
    /// <param name="calendar">Optional calendar ID filter.</param>
    /// <param name="canceled">Optional canceled filter.</param>
    /// <param name="isPublic">Optional public visibility filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching events, or an error message.</returns>
    [McpServerTool(Name = "list_events"), Description("List all events")]
    public async Task<string> ListEvents(
        [Description("Filter by event name")] string? name,
        [Description("Start date greater than or equal (ISO 8601)")] string? startGte,
        [Description("Start date less than or equal (ISO 8601)")] string? startLte,
        [Description("End date greater than or equal (ISO 8601)")] string? endGte,
        [Description("End date less than or equal (ISO 8601)")] string? endLte,
        [Description("Filter by calendar ID")] string? calendar,
        [Description("Filter by canceled status (true/false)")] string? canceled,
        [Description("Filter by public visibility (true/false)")] string? isPublic,
        [Description("Filter by comma-separated IDs")] string? idIn,
        [Description("Ordering criteria (e.g. 'start' or '-start')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var events = await client.ListEventsAsync(name, startGte, startLte, endGte, endLte,
                calendar, canceled, isPublic, idIn, ordering, search, ct);
            return JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the event, or a not-found message.</returns>
    [McpServerTool(Name = "get_event"), Description("Retrieve an event by its ID")]
    public async Task<string> GetEvent(
        [Description("The ID of the event")] long id, CancellationToken ct)
    {
        try
        {
            var ev = await client.GetEventAsync(id, ct);
            return ev != null
                ? JsonSerializer.Serialize(ev, new JsonSerializerOptions { WriteIndented = true })
                : $"Event with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new event in easyVerein.
    /// </summary>
    /// <param name="name">The name of the event (required).</param>
    /// <param name="description">An optional description.</param>
    /// <param name="locationName">An optional location name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created event, or an error message.</returns>
    [McpServerTool(Name = "create_event"), Description("Create a new event")]
    public async Task<string> CreateEvent(
        [Description("The event name (required)")] string name,
        [Description("An optional description")] string? description,
        [Description("An optional location name")] string? locationName,
        CancellationToken ct)
    {
        try
        {
            var ev = new Event { Name = name, Description = description, LocationName = locationName };
            var created = await client.CreateEventAsync(ev, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing event. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the event to update.</param>
    /// <param name="name">Optional new name.</param>
    /// <param name="description">Optional new description.</param>
    /// <param name="locationName">Optional new location name.</param>
    /// <param name="start">Optional new start date (ISO 8601).</param>
    /// <param name="end">Optional new end date (ISO 8601).</param>
    /// <param name="allDay">Optional new all-day flag.</param>
    /// <param name="canceled">Optional new canceled flag.</param>
    /// <param name="isPublic">Optional new public visibility flag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated event, or an error message.</returns>
    [McpServerTool(Name = "update_event"), Description("Update an event (only provided fields are changed)")]
    public async Task<string> UpdateEvent(
        [Description("The ID of the event")] long id,
        [Description("The new name")] string? name,
        [Description("The new description")] string? description,
        [Description("The new location name")] string? locationName,
        [Description("The new start date (ISO 8601)")] string? start,
        [Description("The new end date (ISO 8601)")] string? end,
        [Description("Whether this is an all-day event")] bool? allDay,
        [Description("Whether the event is canceled")] bool? canceled,
        [Description("Whether the event is publicly visible")] bool? isPublic,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (name != null) patch[EventFields.Name] = name;
            if (description != null) patch[EventFields.Description] = description;
            if (locationName != null) patch[EventFields.LocationName] = locationName;
            if (start != null) patch[EventFields.Start] = start;
            if (end != null) patch[EventFields.End] = end;
            if (allDay != null) patch[EventFields.AllDay] = allDay;
            if (canceled != null) patch[EventFields.Canceled] = canceled;
            if (isPublic != null) patch[EventFields.IsPublic] = isPublic;

            var updated = await client.UpdateEventAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes an event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_event"), Description("Delete an event. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteEvent(
        [Description("The ID of the event")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteEventAsync(id, ct);
            return $"Event with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
```

- [ ] **Step 2: Verify build succeeds**

Run: `dotnet build src/MCP.EasyVerein.Server`
Expected: Build succeeded (may fail due to DLL lock — verify with `dotnet build src/MCP.EasyVerein.Server -o /tmp/build-test` as fallback).

- [ ] **Step 3: Run all tests**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests && dotnet test tests/MCP.EasyVerein.Application.Tests && dotnet test tests/MCP.EasyVerein.Infrastructure.Tests`
Expected: All tests pass.

- [ ] **Step 4: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/EventTools.cs
git commit -m "feat(server): rewrite EventTools with filters, error handling, and update tool"
```

---

### Task 6: Final Verification

- [ ] **Step 1: Run full test suite**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests && dotnet test tests/MCP.EasyVerein.Application.Tests && dotnet test tests/MCP.EasyVerein.Infrastructure.Tests`
Expected: All tests pass (24 Domain + 13 Application + 23 Infrastructure = 60 total).

- [ ] **Step 2: Verify event list with date filter against live API (curl)**

```bash
curl -s "https://easyverein.com/api/v1.7/event?start__gte=2026-05-01&start__lte=2026-05-31&ordering=start&limit=5&query=%7Bid,name,start,calendar%7Bid%7D%7D" \
  -H "Authorization: Bearer $EASYVEREIN_API_KEY"
```

Expected: JSON response with events in May 2026 including nested calendar objects.

- [ ] **Step 3: Commit if any final adjustments were needed**

Only if changes were made during verification.
