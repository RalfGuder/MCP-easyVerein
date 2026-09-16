# US-0014 Calendar-Endpoint Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement full CRUD MCP tools for the easyVerein Calendar endpoint with all API filters.

**Architecture:** Follows existing Booking endpoint pattern — Domain entity + Fields ValueObject, Infrastructure query builder + API client methods, Server MCP tools. New `MemberGroup` entity as reference type for `allowedGroups`.

**Tech Stack:** C# / .NET 8.0, ModelContextProtocol v1.2.0, xUnit 2.4.2, Moq 4.20.72

---

## File Structure

| Action | File | Responsibility |
|--------|------|----------------|
| Create | `src/MCP.EasyVerein.Domain/Entities/MemberGroup.cs` | Minimal entity for group references |
| Create | `src/MCP.EasyVerein.Domain/ValueObjects/CalendarFields.cs` | API field name constants |
| Create | `src/MCP.EasyVerein.Domain/Entities/Calendar.cs` | Calendar entity with JSON mapping |
| Modify | `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` | +5 Calendar CRUD methods |
| Create | `src/MCP.EasyVerein.Infrastructure/ApiClient/CalendarQuery.cs` | Query builder with all filters |
| Modify | `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` | +CalendarQuery singleton |
| Modify | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` | +5 Calendar implementations |
| Create | `src/MCP.EasyVerein.Server/Tools/CalendarTools.cs` | 5 MCP tools |
| Modify | `src/MCP.EasyVerein.Server/Program.cs` | Register CalendarTools |
| Create | `tests/MCP.EasyVerein.Domain.Tests/MemberGroupEntityTests.cs` | MemberGroup JSON test |
| Create | `tests/MCP.EasyVerein.Domain.Tests/CalendarEntityTests.cs` | Calendar JSON test |
| Modify | `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` | +Calendar API client tests |

---

### Task 1: MemberGroup Entity (Domain)

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/MemberGroupEntityTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Entities/MemberGroup.cs`

- [ ] **Step 1: Write the failing test**

Create `tests/MCP.EasyVerein.Domain.Tests/MemberGroupEntityTests.cs`:

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberGroupEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 335646249
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var group = JsonSerializer.Deserialize<MemberGroup>(json, options);

        Assert.NotNull(group);
        Assert.Equal(335646249L, group.Id);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter MemberGroupEntityTests -v n`
Expected: FAIL — `MemberGroup` type does not exist.

- [ ] **Step 3: Write minimal implementation**

Create `src/MCP.EasyVerein.Domain/Entities/MemberGroup.cs`:

```csharp
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a member group reference from the easyVerein API.
/// </summary>
public class MemberGroup
{
    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
    /// </summary>
    [JsonPropertyName("id")] public long Id { get; set; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter MemberGroupEntityTests -v n`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/MemberGroupEntityTests.cs src/MCP.EasyVerein.Domain/Entities/MemberGroup.cs
git commit -m "feat(domain): add MemberGroup entity with TDD test"
```

---

### Task 2: CalendarFields ValueObject (Domain)

**Files:**
- Create: `src/MCP.EasyVerein.Domain/ValueObjects/CalendarFields.cs`

- [ ] **Step 1: Create CalendarFields**

Create `src/MCP.EasyVerein.Domain/ValueObjects/CalendarFields.cs`:

```csharp
namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Constants for easyVerein Calendar API field names used in JSON serialization and query building.
/// </summary>
internal static class CalendarFields
{
    /// <summary>API field name for the unique calendar identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the calendar name.</summary>
    internal const string Name = "name";

    /// <summary>API field name for the calendar color (hex value).</summary>
    internal const string Color = "color";

    /// <summary>API field name for the calendar short name (abbreviation).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the allowed member groups.</summary>
    internal const string AllowedGroups = "allowedGroups";

    /// <summary>API field name for the count of linked items.</summary>
    internal const string LinkedItems = "linkedItems";

    /// <summary>API field name for whether events are deleted after calendar deletion.</summary>
    internal const string DeleteEventsAfterDeletion = "deleteEventsAfterDeletion";

    /// <summary>API filter field name for name negation.</summary>
    internal const string NameNot = "name__not";

    /// <summary>API filter field name for color negation.</summary>
    internal const string ColorNot = "color__not";

    /// <summary>API filter field name for short name negation.</summary>
    internal const string ShortNot = "short__not";

    /// <summary>API filter field name for filtering by multiple IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for result ordering.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
```

- [ ] **Step 2: Verify build succeeds**

Run: `dotnet build src/MCP.EasyVerein.Domain`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/CalendarFields.cs
git commit -m "feat(domain): add CalendarFields value object with API field constants"
```

---

### Task 3: Calendar Entity (Domain)

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/CalendarEntityTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Entities/Calendar.cs`

- [ ] **Step 1: Write the failing test**

Create `tests/MCP.EasyVerein.Domain.Tests/CalendarEntityTests.cs`:

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class CalendarEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 335702286,
                "name": "Kulturverein",
                "color": "#f9e4c6",
                "short": "KVMi",
                "allowedGroups": [{"id": 335646249}],
                "linkedItems": 96,
                "deleteEventsAfterDeletion": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var calendar = JsonSerializer.Deserialize<Calendar>(json, options);

        Assert.NotNull(calendar);
        Assert.Equal(335702286L, calendar.Id);
        Assert.Equal("Kulturverein", calendar.Name);
        Assert.Equal("#f9e4c6", calendar.Color);
        Assert.Equal("KVMi", calendar.Short);
        Assert.NotNull(calendar.AllowedGroups);
        Assert.Single(calendar.AllowedGroups);
        Assert.Equal(335646249L, calendar.AllowedGroups[0].Id);
        Assert.Equal(96, calendar.LinkedItems);
        Assert.False(calendar.DeleteEventsAfterDeletion);
    }

    [Fact]
    public void JsonPropertyNames_WithEmptyAllowedGroups_AreCorrect()
    {
        var json = """
            {
                "id": 400324380,
                "name": "Feiertage",
                "color": "#5aaf17",
                "short": "FT",
                "allowedGroups": [],
                "linkedItems": 24,
                "deleteEventsAfterDeletion": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var calendar = JsonSerializer.Deserialize<Calendar>(json, options);

        Assert.NotNull(calendar);
        Assert.Equal(400324380L, calendar.Id);
        Assert.Equal("Feiertage", calendar.Name);
        Assert.NotNull(calendar.AllowedGroups);
        Assert.Empty(calendar.AllowedGroups);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter CalendarEntityTests -v n`
Expected: FAIL — `Calendar` type does not exist.

- [ ] **Step 3: Write minimal implementation**

Create `src/MCP.EasyVerein.Domain/Entities/Calendar.cs`:

```csharp
using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a calendar from the easyVerein API.
/// </summary>
public class Calendar
{
    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Id)] public long Id { get; set; }

    /// <summary>
    /// Gets or sets the calendar name. Maps to API field '<c>name</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Name)] public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the calendar color as hex value. Maps to API field '<c>color</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Color)] public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the calendar short name (abbreviation). Maps to API field '<c>short</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Short)] public string? Short { get; set; }

    /// <summary>
    /// Gets or sets the allowed member groups. Maps to API field '<c>allowedGroups</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.AllowedGroups)] public MemberGroup[]? AllowedGroups { get; set; }

    /// <summary>
    /// Gets or sets the count of linked items. Maps to API field '<c>linkedItems</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.LinkedItems)] public int? LinkedItems { get; set; }

    /// <summary>
    /// Gets or sets whether events are deleted after calendar deletion. Maps to API field '<c>deleteEventsAfterDeletion</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.DeleteEventsAfterDeletion)] public bool? DeleteEventsAfterDeletion { get; set; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter CalendarEntityTests -v n`
Expected: PASS (2 tests)

- [ ] **Step 5: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/CalendarEntityTests.cs src/MCP.EasyVerein.Domain/Entities/Calendar.cs
git commit -m "feat(domain): add Calendar entity with TDD tests"
```

---

### Task 4: IEasyVereinApiClient Interface Extension (Domain)

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`

- [ ] **Step 1: Add Calendar methods to the interface**

Add the following 5 methods to `IEasyVereinApiClient.cs` (alphabetically sorted, after the existing `CreateBookingAsync`):

```csharp
    /// <summary>Creates a new calendar.</summary>
    /// <param name="calendar">The calendar to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created calendar.</returns>
    Task<Calendar> CreateCalendarAsync(Calendar calendar, CancellationToken ct = default);

    /// <summary>Deletes a calendar by ID.</summary>
    /// <param name="id">The calendar ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteCalendarAsync(long id, CancellationToken ct = default);

    /// <summary>Gets a single calendar by ID.</summary>
    /// <param name="id">The calendar ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The calendar, or <c>null</c> if not found.</returns>
    Task<Calendar?> GetCalendarAsync(long id, CancellationToken ct = default);

    /// <summary>Lists calendars, optionally filtered by name, color, short, ordering, or search terms.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="color">Optional color filter.</param>
    /// <param name="short_">Optional short name filter.</param>
    /// <param name="nameNot">Optional name negation filter.</param>
    /// <param name="colorNot">Optional color negation filter.</param>
    /// <param name="shortNot">Optional short name negation filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="allowedGroups">Optional allowed groups filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching calendars.</returns>
    Task<IReadOnlyList<Calendar>> ListCalendarsAsync(string? name = null, string? color = null,
        string? short_ = null, string? nameNot = null, string? colorNot = null,
        string? shortNot = null, string? idIn = null, string? allowedGroups = null,
        string? ordering = null, string[]? search = null, CancellationToken ct = default);

    /// <summary>Partially updates a calendar.</summary>
    /// <param name="id">The calendar ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated calendar.</returns>
    Task<Calendar> UpdateCalendarAsync(long id, object patchData, CancellationToken ct = default);
```

- [ ] **Step 2: Verify build succeeds (interface only, implementations will fail — expected)**

Run: `dotnet build src/MCP.EasyVerein.Domain`
Expected: Build succeeded. (The Infrastructure and Server projects will not build yet — that is expected.)

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs
git commit -m "feat(domain): add Calendar CRUD methods to IEasyVereinApiClient"
```

---

### Task 5: CalendarQuery and ApiQueries (Infrastructure)

**Files:**
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/CalendarQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Create CalendarQuery**

Create `src/MCP.EasyVerein.Infrastructure/ApiClient/CalendarQuery.cs`:

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the calendar API endpoint, including field selection and optional filters.
/// </summary>
internal class CalendarQuery
{
    /// <summary>Gets or sets an optional name filter.</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional color filter.</summary>
    internal string? Color { get; set; }

    /// <summary>Gets or sets an optional short name filter.</summary>
    internal string? Short { get; set; }

    /// <summary>Gets or sets an optional name negation filter.</summary>
    internal string? NameNot { get; set; }

    /// <summary>Gets or sets an optional color negation filter.</summary>
    internal string? ColorNot { get; set; }

    /// <summary>Gets or sets an optional short name negation filter.</summary>
    internal string? ShortNot { get; set; }

    /// <summary>Gets or sets an optional comma-separated IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional allowed groups filter.</summary>
    internal string? AllowedGroups { get; set; }

    /// <summary>Gets or sets an optional ordering criterion for the results.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets optional search terms to filter calendars.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The base field selection query requesting all calendar fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            CalendarFields.Id + "," +
            CalendarFields.Name + "," +
            CalendarFields.Color + "," +
            CalendarFields.Short + "," +
            CalendarFields.AllowedGroups +
            "{" +
                CalendarFields.Id +
            "}," +
            CalendarFields.LinkedItems + "," +
            CalendarFields.DeleteEventsAfterDeletion +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the calendar endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{CalendarFields.Name}={Name}");

        if (!string.IsNullOrEmpty(Color))
            parts.Add($"{CalendarFields.Color}={Color}");

        if (!string.IsNullOrEmpty(Short))
            parts.Add($"{CalendarFields.Short}={Short}");

        if (!string.IsNullOrEmpty(NameNot))
            parts.Add($"{CalendarFields.NameNot}={NameNot}");

        if (!string.IsNullOrEmpty(ColorNot))
            parts.Add($"{CalendarFields.ColorNot}={ColorNot}");

        if (!string.IsNullOrEmpty(ShortNot))
            parts.Add($"{CalendarFields.ShortNot}={ShortNot}");

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{CalendarFields.IdIn}={IdIn}");

        if (!string.IsNullOrEmpty(AllowedGroups))
            parts.Add($"{CalendarFields.AllowedGroups}={AllowedGroups}");

        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{CalendarFields.Ordering}={Ordering}");

        if (Search != null && Search.Length != 0)
            parts.Add($"{CalendarFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
```

- [ ] **Step 2: Add CalendarQuery to ApiQueries**

Add to `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` after the existing BookingQuery entries:

```csharp
    /// <summary>
    /// Shared <see cref="CalendarQuery"/> instance used to build calendar query strings with optional filters.
    /// </summary>
    internal static readonly CalendarQuery CalendarQuery = new();

    /// <summary>
    /// Gets the current calendar query string including field selection and any active filters.
    /// </summary>
    internal static string Calendar => CalendarQuery.ToString();
```

- [ ] **Step 3: Verify build succeeds**

Run: `dotnet build src/MCP.EasyVerein.Infrastructure`
Expected: FAIL — `EasyVereinApiClient` does not implement the new Calendar interface methods yet. That is expected; the query classes themselves should compile.

- [ ] **Step 4: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/CalendarQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
git commit -m "feat(infra): add CalendarQuery builder and register in ApiQueries"
```

---

### Task 6: EasyVereinApiClient Calendar Implementation (Infrastructure)

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

- [ ] **Step 1: Write failing infrastructure tests**

Add the following tests to the end of the `EasyVereinApiClientTests` class in `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` (before the closing brace of the class, after the Bookings section):

```csharp
    // ------------------------------------------------------------------ //
    // Calendars
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListCalendars_ReturnsCalendars()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, name = "Kulturverein", color = "#f9e4c6", @short = "KVMi" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListCalendarsAsync();

        Assert.Single(result);
        Assert.Equal("Kulturverein", result[0].Name);
        Assert.Equal("#f9e4c6", result[0].Color);
    }

    [Fact]
    public async Task GetCalendar_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetCalendarAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListCalendars_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListCalendarsAsync());
    }

    [Fact]
    public async Task ListCalendars_SendsQueryParameter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCalendarsAsync();

        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("query=", handler.LastRequestUri!.Query);
        Assert.Contains("limit=100", handler.LastRequestUri!.Query);
    }
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --filter "ListCalendars|GetCalendar" -v n`
Expected: FAIL — `EasyVereinApiClient` does not implement Calendar methods.

- [ ] **Step 3: Implement Calendar methods in EasyVereinApiClient**

Add the following methods to `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`. Insert them alphabetically among the existing methods:

After `CreateBookingAsync`:
```csharp
    /// <summary>Creates a new calendar via the API.</summary>
    /// <param name="calendar">The calendar to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Calendar"/> as returned by the API.</returns>
    public async Task<Calendar> CreateCalendarAsync(Calendar calendar, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("calendar"), calendar, ct), ct);
        return await HandleResponse<Calendar>(response, ct);
    }
```

After `DeleteBookingAsync`:
```csharp
    /// <summary>Deletes a calendar by its identifier.</summary>
    /// <param name="id">The unique identifier of the calendar to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteCalendarAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"calendar/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }
```

After `GetBookingAsync`:
```csharp
    /// <summary>Retrieves a single calendar by its identifier.</summary>
    /// <param name="id">The unique identifier of the calendar.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Calendar"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Calendar?> GetCalendarAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"calendar/{id}", ApiQueries.Calendar), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Calendar>(response, ct);
    }
```

After `ListBookingsAsync`:
```csharp
    /// <summary>Lists calendars with optional filters and automatic pagination.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="color">Optional color filter.</param>
    /// <param name="short_">Optional short name filter.</param>
    /// <param name="nameNot">Optional name negation filter.</param>
    /// <param name="colorNot">Optional color negation filter.</param>
    /// <param name="shortNot">Optional short name negation filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="allowedGroups">Optional allowed groups filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="Calendar"/> records.</returns>
    public async Task<IReadOnlyList<Calendar>> ListCalendarsAsync(string? name = null, string? color = null,
        string? short_ = null, string? nameNot = null, string? colorNot = null,
        string? shortNot = null, string? idIn = null, string? allowedGroups = null,
        string? ordering = null, string[]? search = null, CancellationToken ct = default)
    {
        ApiQueries.CalendarQuery.Name = name;
        ApiQueries.CalendarQuery.Color = color;
        ApiQueries.CalendarQuery.Short = short_;
        ApiQueries.CalendarQuery.NameNot = nameNot;
        ApiQueries.CalendarQuery.ColorNot = colorNot;
        ApiQueries.CalendarQuery.ShortNot = shortNot;
        ApiQueries.CalendarQuery.IdIn = idIn;
        ApiQueries.CalendarQuery.AllowedGroups = allowedGroups;
        ApiQueries.CalendarQuery.Ordering = ordering;
        ApiQueries.CalendarQuery.Search = search;

        return await HandleListResponseWithPagination<Calendar>(
            BuildListUrl("calendar", ApiQueries.Calendar), ct);
    }
```

After `UpdateBookingAsync`:
```csharp
    /// <summary>Partially updates a calendar with a patch dictionary.</summary>
    /// <param name="id">The unique identifier of the calendar to update.</param>
    /// <param name="patchData">An object containing only the fields to update.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Calendar"/> as returned by the API.</returns>
    public async Task<Calendar> UpdateCalendarAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"calendar/{id}"), content, ct), ct);
        return await HandleResponse<Calendar>(response, ct);
    }
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --filter "ListCalendars|GetCalendar" -v n`
Expected: PASS (4 tests)

- [ ] **Step 5: Run all tests to check for regressions**

Run: `dotnet test`
Expected: All existing tests still pass.

- [ ] **Step 6: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
git commit -m "feat(infra): implement Calendar CRUD in EasyVereinApiClient with TDD tests"
```

---

### Task 7: CalendarTools MCP Tools (Server)

**Files:**
- Create: `src/MCP.EasyVerein.Server/Tools/CalendarTools.cs`
- Modify: `src/MCP.EasyVerein.Server/Program.cs`

- [ ] **Step 1: Create CalendarTools**

Create `src/MCP.EasyVerein.Server/Tools/CalendarTools.cs`:

```csharp
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing calendars via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class CalendarTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists calendars with optional filters and automatic pagination.
    /// </summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="color">Optional color filter.</param>
    /// <param name="short_">Optional short name filter.</param>
    /// <param name="nameNot">Optional name negation filter.</param>
    /// <param name="colorNot">Optional color negation filter.</param>
    /// <param name="shortNot">Optional short name negation filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="allowedGroups">Optional allowed groups filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching calendars, or an error message.</returns>
    [McpServerTool(Name = "list_calendars"), Description("List all calendars")]
    public async Task<string> ListCalendars(
        [Description("Filter by calendar name")] string? name,
        [Description("Filter by color (hex value)")] string? color,
        [Description("Filter by short name")] string? short_,
        [Description("Exclude calendars with this name")] string? nameNot,
        [Description("Exclude calendars with this color")] string? colorNot,
        [Description("Exclude calendars with this short name")] string? shortNot,
        [Description("Filter by comma-separated IDs")] string? idIn,
        [Description("Filter by allowed group ID")] string? allowedGroups,
        [Description("Ordering criteria (e.g. 'name' or '-name')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var calendars = await client.ListCalendarsAsync(name, color, short_, nameNot, colorNot,
                shortNot, idIn, allowedGroups, ordering, search, ct);
            return JsonSerializer.Serialize(calendars, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single calendar by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the calendar.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the calendar, or a not-found message.</returns>
    [McpServerTool(Name = "get_calendar"), Description("Retrieve a calendar by its ID")]
    public async Task<string> GetCalendar(
        [Description("The ID of the calendar")] long id, CancellationToken ct)
    {
        try
        {
            var calendar = await client.GetCalendarAsync(id, ct);
            return calendar != null
                ? JsonSerializer.Serialize(calendar, new JsonSerializerOptions { WriteIndented = true })
                : $"Calendar with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new calendar in easyVerein.
    /// </summary>
    /// <param name="name">The calendar name (required, max 200 characters).</param>
    /// <param name="color">Optional hex color value (max 7 characters, e.g. '#FF5733').</param>
    /// <param name="short_">Optional short name/abbreviation (max 4 characters, must be unique).</param>
    /// <param name="allowedGroupIds">Optional array of member group IDs that have access.</param>
    /// <param name="deleteEventsAfterDeletion">Optional flag whether to delete events when calendar is deleted.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created calendar, or an error message.</returns>
    [McpServerTool(Name = "create_calendar"), Description("Create a new calendar")]
    public async Task<string> CreateCalendar(
        [Description("The calendar name (required)")] string name,
        [Description("Hex color value (e.g. '#FF5733')")] string? color,
        [Description("Short name / abbreviation (max 4 chars, must be unique)")] string? short_,
        [Description("Array of member group IDs with access")] long[]? allowedGroupIds,
        [Description("Delete events when calendar is deleted (default: false)")] bool? deleteEventsAfterDeletion,
        CancellationToken ct)
    {
        try
        {
            var calendar = new Calendar
            {
                Name = name,
                Color = color,
                Short = short_,
                AllowedGroups = allowedGroupIds?.Select(id => new MemberGroup { Id = id }).ToArray(),
                DeleteEventsAfterDeletion = deleteEventsAfterDeletion
            };
            var created = await client.CreateCalendarAsync(calendar, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing calendar. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the calendar to update.</param>
    /// <param name="name">Optional new name.</param>
    /// <param name="color">Optional new hex color value.</param>
    /// <param name="short_">Optional new short name.</param>
    /// <param name="allowedGroupIds">Optional new array of member group IDs.</param>
    /// <param name="deleteEventsAfterDeletion">Optional new value for delete-events flag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated calendar, or an error message.</returns>
    [McpServerTool(Name = "update_calendar"), Description("Update a calendar (only provided fields are changed)")]
    public async Task<string> UpdateCalendar(
        [Description("The ID of the calendar")] long id,
        [Description("The new name")] string? name,
        [Description("The new hex color value")] string? color,
        [Description("The new short name")] string? short_,
        [Description("New array of member group IDs")] long[]? allowedGroupIds,
        [Description("New value for delete-events-after-deletion flag")] bool? deleteEventsAfterDeletion,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (name != null) patch[CalendarFields.Name] = name;
            if (color != null) patch[CalendarFields.Color] = color;
            if (short_ != null) patch[CalendarFields.Short] = short_;
            if (allowedGroupIds != null)
                patch[CalendarFields.AllowedGroups] = allowedGroupIds.Select(gid => new MemberGroup { Id = gid }).ToArray();
            if (deleteEventsAfterDeletion != null) patch[CalendarFields.DeleteEventsAfterDeletion] = deleteEventsAfterDeletion;

            var updated = await client.UpdateCalendarAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes a calendar by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the calendar to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_calendar"), Description("Delete a calendar. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteCalendar(
        [Description("The ID of the calendar")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteCalendarAsync(id, ct);
            return $"Calendar with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
```

- [ ] **Step 2: Register CalendarTools in Program.cs**

In `src/MCP.EasyVerein.Server/Program.cs`, add `.WithTools<CalendarTools>()` after the existing `.WithTools<BookingTools>()` line:

```csharp
    .WithTools<BookingTools>()
    .WithTools<CalendarTools>();
```

- [ ] **Step 3: Verify full build succeeds**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 4: Run all tests to check for regressions**

Run: `dotnet test`
Expected: All tests pass (previous 50 + 7 new = 57 total).

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/CalendarTools.cs src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat(server): add CalendarTools MCP tools with CRUD operations and error handling"
```

---

### Task 8: Final Verification

- [ ] **Step 1: Run full test suite**

Run: `dotnet test --verbosity normal`
Expected: 57 tests passed, 0 failed.

- [ ] **Step 2: Verify calendar tools against live API (curl)**

```bash
curl -s "https://easyverein.com/api/v1.7/calendar?query=%7Bid,name,color,short,allowedGroups%7Bid%7D,linkedItems,deleteEventsAfterDeletion%7D" \
  -H "Authorization: Bearer $EASYVEREIN_API_KEY" | python -m json.tool
```

Expected: JSON response with calendar data matching the entity structure.

- [ ] **Step 3: Commit if any final adjustments were needed**

Only if changes were made during verification.
