# US-0010 Announcement-Endpoint Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** CRUD-Operationen für den easyVerein Announcement-Endpoint als MCP-Tools bereitstellen.

**Architecture:** Clean Architecture (4 Schichten) nach bestehendem Muster: Domain (Entity + Fields) → Infrastructure (ApiClient + Query) → Server (MCP-Tools). TDD mit Red-Green-Refactor.

**Tech Stack:** C# / .NET 8.0, xUnit, System.Text.Json, ModelContextProtocol SDK

---

## API-Felder (easyVerein v1.7)

| Feld | Typ | Beschreibung |
|------|-----|-------------|
| `id` | long | Eindeutige ID |
| `text` | string | HTML-Inhalt der Ankündigung |
| `start` | DateTime | Startdatum/-zeit |
| `end` | DateTime | Enddatum/-zeit |
| `showBanner` | bool | Banner anzeigen |
| `isDismissible` | bool | Kann geschlossen werden |
| `isPublic` | bool | Öffentlich sichtbar |
| `showForNormalMembers` | bool | Für normale Mitglieder sichtbar |
| `platform` | int | Plattform (Web/App) |
| `bannerLevel` | string | Banner-Level (success, warning, etc.) |
| `accountTypeVisibility` | int | Sichtbarkeit nach Kontotyp |

## File Structure

| Datei | Aktion | Verantwortung |
|-------|--------|---------------|
| `src/MCP.EasyVerein.Domain/ValueObjects/AnnouncementFields.cs` | Create | API-Feldnamen als Konstanten |
| `src/MCP.EasyVerein.Domain/Entities/Announcement.cs` | Create | Entity mit JsonPropertyName |
| `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` | Modify | CRUD-Methoden hinzufügen |
| `src/MCP.EasyVerein.Infrastructure/ApiClient/AnnouncementQuery.cs` | Create | Query-Builder mit Feldauswahl |
| `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` | Modify | Announcement registrieren |
| `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` | Modify | CRUD implementieren |
| `src/MCP.EasyVerein.Server/Tools/AnnouncementTools.cs` | Create | MCP-Tool-Klasse |
| `src/MCP.EasyVerein.Server/Program.cs` | Modify | Tools registrieren |
| `tests/MCP.EasyVerein.Domain.Tests/AnnouncementEntityTests.cs` | Create | Entity-Deserialisierung |
| `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` | Modify | API-Client-Tests |

---

### Task 1: AnnouncementFields ValueObject

**Files:**
- Create: `src/MCP.EasyVerein.Domain/ValueObjects/AnnouncementFields.cs`

- [ ] **Step 1: Create AnnouncementFields.cs**

```csharp
namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Contains constant field names for the easyVerein Announcement API resource.
/// </summary>
internal static class AnnouncementFields
{
    /// <summary>The unique identifier field.</summary>
    internal const string Id = "id";

    /// <summary>The HTML text content field.</summary>
    internal const string Text = "text";

    /// <summary>The start date/time field.</summary>
    internal const string Start = "start";

    /// <summary>The end date/time field.</summary>
    internal const string End = "end";

    /// <summary>The show banner flag field.</summary>
    internal const string ShowBanner = "showBanner";

    /// <summary>The dismissible flag field.</summary>
    internal const string IsDismissible = "isDismissible";

    /// <summary>The public visibility flag field.</summary>
    internal const string IsPublic = "isPublic";

    /// <summary>The normal members visibility flag field.</summary>
    internal const string ShowForNormalMembers = "showForNormalMembers";

    /// <summary>The platform identifier field.</summary>
    internal const string Platform = "platform";

    /// <summary>The banner level field (e.g. success, warning).</summary>
    internal const string BannerLevel = "bannerLevel";

    /// <summary>The account type visibility field.</summary>
    internal const string AccountTypeVisibility = "accountTypeVisibility";

    /// <summary>The ordering query parameter.</summary>
    internal const string Ordering = "ordering";

    /// <summary>The search query parameter.</summary>
    internal const string Search = "search";
}
```

- [ ] **Step 2: Verify build compiles**

Run: `dotnet build src/MCP.EasyVerein.Domain/ --verbosity quiet`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/AnnouncementFields.cs
git commit -m "feat(domain): add AnnouncementFields value object with API field constants"
```

---

### Task 2: Announcement Entity + Tests (TDD)

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/AnnouncementEntityTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Entities/Announcement.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class AnnouncementEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "text": "<p>Wichtige Ankündigung</p>",
                "start": "2026-05-01T08:00:00",
                "end": "2026-05-31T23:59:59",
                "showBanner": true,
                "isDismissible": false,
                "isPublic": true,
                "showForNormalMembers": true,
                "platform": 1,
                "bannerLevel": "success",
                "accountTypeVisibility": 0
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var announcement = JsonSerializer.Deserialize<Announcement>(json, options);

        Assert.NotNull(announcement);
        Assert.Equal(42L, announcement.Id);
        Assert.Equal("<p>Wichtige Ankündigung</p>", announcement.Text);
        Assert.Equal(new DateTime(2026, 5, 1, 8, 0, 0), announcement.Start);
        Assert.Equal(new DateTime(2026, 5, 31, 23, 59, 59), announcement.End);
        Assert.True(announcement.ShowBanner);
        Assert.False(announcement.IsDismissible);
        Assert.True(announcement.IsPublic);
        Assert.True(announcement.ShowForNormalMembers);
        Assert.Equal(1, announcement.Platform);
        Assert.Equal("success", announcement.BannerLevel);
        Assert.Equal(0, announcement.AccountTypeVisibility);
    }

    [Fact]
    public void JsonPropertyNames_WithNullOptionalFields_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "text": "Minimal",
                "showBanner": false,
                "isDismissible": true,
                "isPublic": false,
                "showForNormalMembers": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var announcement = JsonSerializer.Deserialize<Announcement>(json, options);

        Assert.NotNull(announcement);
        Assert.Equal(99L, announcement.Id);
        Assert.Equal("Minimal", announcement.Text);
        Assert.Null(announcement.Start);
        Assert.Null(announcement.End);
        Assert.False(announcement.ShowBanner);
        Assert.True(announcement.IsDismissible);
        Assert.Null(announcement.Platform);
        Assert.Null(announcement.BannerLevel);
        Assert.Null(announcement.AccountTypeVisibility);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "AnnouncementEntityTests" --verbosity quiet`
Expected: FAIL — `Announcement` type does not exist

- [ ] **Step 3: Write Announcement entity**

```csharp
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an announcement from the easyVerein API.
/// </summary>
public class Announcement
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the HTML text content. Maps to API field '<c>text</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Text)]
    public string? Text { get; set; }

    /// <summary>Gets or sets the start date/time. Maps to API field '<c>start</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Start)]
    public DateTime? Start { get; set; }

    /// <summary>Gets or sets the end date/time. Maps to API field '<c>end</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.End)]
    public DateTime? End { get; set; }

    /// <summary>Gets or sets whether to show the banner. Maps to API field '<c>showBanner</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.ShowBanner)]
    public bool ShowBanner { get; set; }

    /// <summary>Gets or sets whether the announcement is dismissible. Maps to API field '<c>isDismissible</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.IsDismissible)]
    public bool IsDismissible { get; set; }

    /// <summary>Gets or sets whether the announcement is public. Maps to API field '<c>isPublic</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.IsPublic)]
    public bool IsPublic { get; set; }

    /// <summary>Gets or sets whether to show for normal members. Maps to API field '<c>showForNormalMembers</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.ShowForNormalMembers)]
    public bool ShowForNormalMembers { get; set; }

    /// <summary>Gets or sets the platform identifier. Maps to API field '<c>platform</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Platform)]
    public int? Platform { get; set; }

    /// <summary>Gets or sets the banner level (e.g. success, warning). Maps to API field '<c>bannerLevel</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.BannerLevel)]
    public string? BannerLevel { get; set; }

    /// <summary>Gets or sets the account type visibility. Maps to API field '<c>accountTypeVisibility</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.AccountTypeVisibility)]
    public int? AccountTypeVisibility { get; set; }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "AnnouncementEntityTests" --verbosity quiet`
Expected: 2 tests PASS

- [ ] **Step 5: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/AnnouncementEntityTests.cs src/MCP.EasyVerein.Domain/Entities/Announcement.cs
git commit -m "feat(domain): add Announcement entity with TDD tests"
```

---

### Task 3: AnnouncementQuery + ApiQueries Registration

**Files:**
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/AnnouncementQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Create AnnouncementQuery.cs**

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the announcement API endpoint with field selection and filters.
/// </summary>
internal class AnnouncementQuery
{
    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all announcement fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            AnnouncementFields.Id + "," +
            AnnouncementFields.Text + "," +
            AnnouncementFields.Start + "," +
            AnnouncementFields.End + "," +
            AnnouncementFields.ShowBanner + "," +
            AnnouncementFields.IsDismissible + "," +
            AnnouncementFields.IsPublic + "," +
            AnnouncementFields.ShowForNormalMembers + "," +
            AnnouncementFields.Platform + "," +
            AnnouncementFields.BannerLevel + "," +
            AnnouncementFields.AccountTypeVisibility +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{AnnouncementFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{AnnouncementFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
```

- [ ] **Step 2: Register in ApiQueries.cs**

Add these two lines to the `ApiQueries` class:

```csharp
internal static readonly AnnouncementQuery AnnouncementQuery = new();
internal static string Announcement => AnnouncementQuery.ToString();
```

- [ ] **Step 3: Verify build compiles**

Run: `dotnet build src/MCP.EasyVerein.Infrastructure/ --verbosity quiet`
Expected: Build succeeded

- [ ] **Step 4: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/AnnouncementQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
git commit -m "feat(infra): add AnnouncementQuery builder and register in ApiQueries"
```

---

### Task 4: IEasyVereinApiClient Interface erweitern

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`

- [ ] **Step 1: Add Announcement CRUD methods to the interface**

Add these method signatures:

```csharp
/// <summary>Lists all announcements, optionally filtered.</summary>
Task<IReadOnlyList<Announcement>> ListAnnouncementsAsync(
    string? ordering = null, string[]? search = null,
    CancellationToken ct = default);

/// <summary>Gets a single announcement by ID.</summary>
Task<Announcement?> GetAnnouncementAsync(long id, CancellationToken ct = default);

/// <summary>Creates a new announcement.</summary>
Task<Announcement> CreateAnnouncementAsync(Announcement announcement, CancellationToken ct = default);

/// <summary>Updates an announcement with PATCH semantics.</summary>
Task<Announcement> UpdateAnnouncementAsync(long id, object patchData, CancellationToken ct = default);

/// <summary>Deletes an announcement.</summary>
Task DeleteAnnouncementAsync(long id, CancellationToken ct = default);
```

- [ ] **Step 2: Verify build compiles (Domain only)**

Run: `dotnet build src/MCP.EasyVerein.Domain/ --verbosity quiet`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs
git commit -m "feat(domain): add Announcement CRUD methods to IEasyVereinApiClient"
```

---

### Task 5: EasyVereinApiClient Implementation + Tests (TDD)

**Files:**
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`

- [ ] **Step 1: Write failing tests**

Add these tests to `EasyVereinApiClientTests.cs`:

```csharp
// ------------------------------------------------------------------ //
// Announcements
// ------------------------------------------------------------------ //

[Fact]
public async Task ListAnnouncements_ReturnsAnnouncements()
{
    var json = JsonSerializer.Serialize(new
    {
        results = new[]
        {
            new { id = 1, text = "<p>Willkommen</p>", showBanner = true, isDismissible = false, isPublic = true, showForNormalMembers = true }
        },
        next = (string?)null
    });
    var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
    var client = CreateClient(handler);

    var result = await client.ListAnnouncementsAsync();

    Assert.Single(result);
    Assert.Equal("<p>Willkommen</p>", result[0].Text);
    Assert.True(result[0].ShowBanner);
}

[Fact]
public async Task GetAnnouncement_WithNotFound_ReturnsNull()
{
    var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
    var client = CreateClient(handler);

    var result = await client.GetAnnouncementAsync(999);

    Assert.Null(result);
}

[Fact]
public async Task ListAnnouncements_WithUnauthorized_ThrowsUnauthorizedAccessException()
{
    var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
    var client = CreateClient(handler);

    await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListAnnouncementsAsync());
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --filter "Announcement" --verbosity quiet`
Expected: FAIL — methods not implemented

- [ ] **Step 3: Implement Announcement CRUD in EasyVereinApiClient**

Add these methods to `EasyVereinApiClient.cs`:

```csharp
// ------------------------------------------------------------------ //
// Announcements
// ------------------------------------------------------------------ //

/// <summary>Creates a new announcement via the API.</summary>
public async Task<Announcement> CreateAnnouncementAsync(Announcement announcement, CancellationToken ct = default)
{
    var response = await SendWithErrorHandling(
        () => _httpClient.PostAsJsonAsync(BuildUrl("announcement"), announcement, ct), ct);
    return await HandleResponse<Announcement>(response, ct);
}

/// <summary>Deletes an announcement by ID.</summary>
public async Task DeleteAnnouncementAsync(long id, CancellationToken ct = default)
{
    var response = await SendWithErrorHandling(
        () => _httpClient.DeleteAsync(BuildUrl($"announcement/{id}"), ct), ct);
    await EnsureSuccessOrThrowAsync(response, ct);
}

/// <summary>Gets a single announcement by ID.</summary>
public async Task<Announcement?> GetAnnouncementAsync(long id, CancellationToken ct = default)
{
    var response = await SendWithErrorHandling(
        () => _httpClient.GetAsync(BuildGetUrl($"announcement/{id}", ApiQueries.Announcement), ct), ct);
    if (response.StatusCode == HttpStatusCode.NotFound) return null;
    return await HandleResponse<Announcement>(response, ct);
}

/// <summary>Lists announcements with optional filters.</summary>
public async Task<IReadOnlyList<Announcement>> ListAnnouncementsAsync(
    string? ordering = null, string[]? search = null,
    CancellationToken ct = default)
{
    ApiQueries.AnnouncementQuery.Ordering = ordering;
    ApiQueries.AnnouncementQuery.Search = search;

    return await HandleListResponseWithPagination<Announcement>(
        BuildListUrl("announcement", ApiQueries.Announcement), ct);
}

/// <summary>Updates an announcement with PATCH semantics.</summary>
public async Task<Announcement> UpdateAnnouncementAsync(long id, object patchData, CancellationToken ct = default)
{
    var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var response = await SendWithErrorHandling(
        () => _httpClient.PatchAsync(BuildUrl($"announcement/{id}"), content, ct), ct);
    return await HandleResponse<Announcement>(response, ct);
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --filter "Announcement" --verbosity quiet`
Expected: 3 tests PASS

- [ ] **Step 5: Run all tests to check for regressions**

Run: `dotnet test --configuration Release --verbosity quiet`
Expected: All tests PASS

- [ ] **Step 6: Commit**

```bash
git add tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
git commit -m "feat(infra): implement Announcement CRUD in EasyVereinApiClient with TDD tests"
```

---

### Task 6: AnnouncementTools MCP-Server

**Files:**
- Create: `src/MCP.EasyVerein.Server/Tools/AnnouncementTools.cs`
- Modify: `src/MCP.EasyVerein.Server/Program.cs`

- [ ] **Step 1: Create AnnouncementTools.cs**

```csharp
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing announcements in easyVerein.
/// </summary>
[McpServerToolType]
public sealed class AnnouncementTools(IEasyVereinApiClient client)
{
    /// <summary>Lists all announcements with optional filters.</summary>
    [McpServerTool(Name = "list_announcements"), Description("List all announcements")]
    public async Task<string> ListAnnouncements(
        [Description("Ordering (e.g. 'start' or '-start')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var announcements = await client.ListAnnouncementsAsync(ordering, search, ct);
            return JsonSerializer.Serialize(announcements, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Gets a single announcement by ID.</summary>
    [McpServerTool(Name = "get_announcement"), Description("Retrieve an announcement by its ID")]
    public async Task<string> GetAnnouncement(
        [Description("The ID of the announcement")] long id,
        CancellationToken ct)
    {
        try
        {
            var announcement = await client.GetAnnouncementAsync(id, ct);
            return announcement != null
                ? JsonSerializer.Serialize(announcement, new JsonSerializerOptions { WriteIndented = true })
                : $"Announcement with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new announcement.</summary>
    [McpServerTool(Name = "create_announcement"), Description("Create a new announcement")]
    public async Task<string> CreateAnnouncement(
        [Description("The HTML text content (required)")] string text,
        [Description("Start date/time (ISO 8601)")] string? start,
        [Description("End date/time (ISO 8601)")] string? end,
        [Description("Show as banner")] bool? showBanner,
        [Description("Can be dismissed by users")] bool? isDismissible,
        [Description("Publicly visible")] bool? isPublic,
        [Description("Show for normal members")] bool? showForNormalMembers,
        [Description("Platform (integer)")] int? platform,
        [Description("Banner level (e.g. success, warning, info, danger)")] string? bannerLevel,
        [Description("Account type visibility (integer)")] int? accountTypeVisibility,
        CancellationToken ct)
    {
        try
        {
            var announcement = new Announcement { Text = text };

            if (DateTime.TryParse(start, out var s)) announcement.Start = s;
            if (DateTime.TryParse(end, out var e)) announcement.End = e;
            if (showBanner.HasValue) announcement.ShowBanner = showBanner.Value;
            if (isDismissible.HasValue) announcement.IsDismissible = isDismissible.Value;
            if (isPublic.HasValue) announcement.IsPublic = isPublic.Value;
            if (showForNormalMembers.HasValue) announcement.ShowForNormalMembers = showForNormalMembers.Value;
            if (platform.HasValue) announcement.Platform = platform.Value;
            if (!string.IsNullOrEmpty(bannerLevel)) announcement.BannerLevel = bannerLevel;
            if (accountTypeVisibility.HasValue) announcement.AccountTypeVisibility = accountTypeVisibility.Value;

            var created = await client.CreateAnnouncementAsync(announcement, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing announcement with PATCH semantics.</summary>
    [McpServerTool(Name = "update_announcement"), Description("Update an announcement (only provided fields are changed)")]
    public async Task<string> UpdateAnnouncement(
        [Description("The ID of the announcement to update")] long id,
        [Description("The new HTML text content")] string? text,
        [Description("New start date/time (ISO 8601)")] string? start,
        [Description("New end date/time (ISO 8601)")] string? end,
        [Description("Show as banner")] bool? showBanner,
        [Description("Can be dismissed by users")] bool? isDismissible,
        [Description("Publicly visible")] bool? isPublic,
        [Description("Show for normal members")] bool? showForNormalMembers,
        [Description("Platform (integer)")] int? platform,
        [Description("Banner level (e.g. success, warning, info, danger)")] string? bannerLevel,
        [Description("Account type visibility (integer)")] int? accountTypeVisibility,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();

            if (HasValue(text)) patch[AnnouncementFields.Text] = text!;
            if (DateTime.TryParse(start, out var s)) patch[AnnouncementFields.Start] = s;
            if (DateTime.TryParse(end, out var e)) patch[AnnouncementFields.End] = e;
            if (showBanner.HasValue) patch[AnnouncementFields.ShowBanner] = showBanner.Value;
            if (isDismissible.HasValue) patch[AnnouncementFields.IsDismissible] = isDismissible.Value;
            if (isPublic.HasValue) patch[AnnouncementFields.IsPublic] = isPublic.Value;
            if (showForNormalMembers.HasValue) patch[AnnouncementFields.ShowForNormalMembers] = showForNormalMembers.Value;
            if (platform.HasValue) patch[AnnouncementFields.Platform] = platform.Value;
            if (HasValue(bannerLevel)) patch[AnnouncementFields.BannerLevel] = bannerLevel!;
            if (accountTypeVisibility.HasValue) patch[AnnouncementFields.AccountTypeVisibility] = accountTypeVisibility.Value;

            var updated = await client.UpdateAnnouncementAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes an announcement.</summary>
    [McpServerTool(Name = "delete_announcement"), Description("Delete an announcement. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteAnnouncement(
        [Description("The ID of the announcement to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteAnnouncementAsync(id, ct);
            return $"Announcement with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string value is non-null and not the literal "null".</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
```

- [ ] **Step 2: Register in Program.cs**

Add `.WithTools<AnnouncementTools>()` to the MCP server builder chain.

- [ ] **Step 3: Verify build compiles**

Run: `dotnet build --configuration Release --verbosity quiet`
Expected: Build succeeded, 0 errors

- [ ] **Step 4: Run all tests**

Run: `dotnet test --configuration Release --verbosity quiet`
Expected: All tests PASS

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/AnnouncementTools.cs src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat(server): add AnnouncementTools MCP tools with CRUD operations and error handling"
```

---

### Task 7: CLAUDE.md und User Story aktualisieren

**Files:**
- Modify: `CLAUDE.md`
- Modify: `docs/001 User Stories/010-announcement-endpoint.md`

- [ ] **Step 1: Update CLAUDE.md**

Add Announcement to the implemented endpoints table:

```
| Announcement   | US-0010    | list, get, create, update (PATCH), delete                |
```

Update test counts, remove US-0010 from "Nächste anstehende Endpoints".

- [ ] **Step 2: Update User Story — check all acceptance criteria**

Mark all Akzeptanzkriterien as `[x]` completed.

- [ ] **Step 3: Run final full test suite**

Run: `dotnet test --configuration Release --verbosity quiet`
Expected: All tests PASS

- [ ] **Step 4: Commit**

```bash
git add CLAUDE.md docs/001\ User\ Stories/010-announcement-endpoint.md
git commit -m "docs: update CLAUDE.md and user story with Announcement endpoint status"
```
