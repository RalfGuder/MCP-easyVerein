# Booking-Endpoint Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement full CRUD operations for the easyVerein `/booking` endpoint as MCP tools.

**Architecture:** New Domain entity + fields + query following the established Invoice/ContactDetails pattern. API client extended with 5 CRUD methods. MCP tools with consistent try/catch error handling and PATCH-dictionary update semantics.

**Tech Stack:** C# / .NET 8, xUnit, System.Text.Json, ModelContextProtocol SDK

**Spec:** `docs/005 SuperPowers/specs/2026-04-04-booking-endpoint-design.md`

---

### Task 1: BookingFields ValueObject

**Files:**
- Create: `src/MCP.EasyVerein.Domain/ValueObjects/BookingFields.cs`

- [ ] **Step 1: Create BookingFields.cs**

```csharp
namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Booking API field names used in JSON serialization.</summary>
public static class BookingFields
{
    /// <summary>API field name for the unique booking identifier.</summary>
    public const string Id = "id";
    /// <summary>API field name for the booking amount.</summary>
    public const string Amount = "amount";
    /// <summary>API field name for the bank account reference.</summary>
    public const string BankAccount = "bankAccount";
    /// <summary>API field name for the billing account reference.</summary>
    public const string BillingAccount = "billingAccount";
    /// <summary>API field name for the booking description.</summary>
    public const string Description = "description";
    /// <summary>API field name for the booking date.</summary>
    public const string Date = "date";
    /// <summary>API field name for the receiver.</summary>
    public const string Receiver = "receiver";
    /// <summary>API field name for the billing ID.</summary>
    public const string BillingId = "billingId";
    /// <summary>API field name for whether the booking is blocked.</summary>
    public const string Blocked = "blocked";
    /// <summary>API field name for the payment difference.</summary>
    public const string PaymentDifference = "paymentDifference";
    /// <summary>API field name for the counterpart IBAN.</summary>
    public const string CounterpartIban = "counterpartIban";
    /// <summary>API field name for the counterpart BIC.</summary>
    public const string CounterpartBic = "counterpartBic";
    /// <summary>API field name for whether this is a Twingle donation.</summary>
    public const string TwingleDonation = "twingleDonation";
    /// <summary>API field name for the booking project reference.</summary>
    public const string BookingProject = "bookingProject";
    /// <summary>API field name for the sphere (area).</summary>
    public const string Sphere = "sphere";
    /// <summary>API field name for the related invoice references.</summary>
    public const string RelatedInvoice = "relatedInvoice";
}
```

- [ ] **Step 2: Verify build**

Run: `dotnet build src/MCP.EasyVerein.Domain/MCP.EasyVerein.Domain.csproj --configuration Release`
Expected: Build succeeded, 0 errors

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/BookingFields.cs
git commit -m "feat(domain): add BookingFields value object with API field name constants"
```

---

### Task 2: Booking Entity + Domain Tests (TDD)

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/BookingEntityTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Entities/Booking.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class BookingEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "amount": 150.50,
                "bankAccount": 7,
                "billingAccount": 3,
                "description": "Jahresbeitrag 2026",
                "date": "2026-03-15T00:00:00",
                "receiver": "Max Mustermann",
                "billingId": "BIL-001",
                "blocked": false,
                "paymentDifference": 0.00,
                "counterpartIban": "DE89370400440532013000",
                "counterpartBic": "COBADEFFXXX",
                "twingleDonation": false,
                "bookingProject": "Projekt A",
                "sphere": "ideell",
                "relatedInvoice": [1, 2, 3]
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var booking = JsonSerializer.Deserialize<Booking>(json, options);

        Assert.NotNull(booking);
        Assert.Equal(42, booking.Id);
        Assert.Equal(150.50m, booking.Amount);
        Assert.Equal(7L, booking.BankAccount);
        Assert.Equal(3L, booking.BillingAccount);
        Assert.Equal("Jahresbeitrag 2026", booking.Description);
        Assert.Equal(new DateTime(2026, 3, 15), booking.Date);
        Assert.Equal("Max Mustermann", booking.Receiver);
        Assert.Equal("BIL-001", booking.BillingId);
        Assert.False(booking.Blocked);
        Assert.Equal(0.00m, booking.PaymentDifference);
        Assert.Equal("DE89370400440532013000", booking.CounterpartIban);
        Assert.Equal("COBADEFFXXX", booking.CounterpartBic);
        Assert.False(booking.TwingleDonation);
        Assert.Equal("Projekt A", booking.BookingProject);
        Assert.Equal("ideell", booking.Sphere);
        Assert.Equal(new List<long> { 1, 2, 3 }, booking.RelatedInvoice);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "BookingEntityTests" --verbosity normal`
Expected: FAIL — `Booking` type does not exist

- [ ] **Step 3: Write minimal implementation**

```csharp
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>Represents a booking from the easyVerein API.</summary>
public class Booking
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(BookingFields.Id)] public long Id { get; set; }

    /// <summary>Gets or sets the booking amount. Maps to API field '<c>amount</c>'.</summary>
    [JsonPropertyName(BookingFields.Amount)] public decimal? Amount { get; set; }

    /// <summary>Gets or sets the bank account reference. Maps to API field '<c>bankAccount</c>'.</summary>
    [JsonPropertyName(BookingFields.BankAccount)] public long? BankAccount { get; set; }

    /// <summary>Gets or sets the billing account reference. Maps to API field '<c>billingAccount</c>'.</summary>
    [JsonPropertyName(BookingFields.BillingAccount)] public long? BillingAccount { get; set; }

    /// <summary>Gets or sets the description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(BookingFields.Description)] public string? Description { get; set; }

    /// <summary>Gets or sets the booking date. Maps to API field '<c>date</c>'.</summary>
    [JsonPropertyName(BookingFields.Date)] public DateTime? Date { get; set; }

    /// <summary>Gets or sets the receiver. Maps to API field '<c>receiver</c>'.</summary>
    [JsonPropertyName(BookingFields.Receiver)] public string? Receiver { get; set; }

    /// <summary>Gets or sets the billing ID. Maps to API field '<c>billingId</c>'.</summary>
    [JsonPropertyName(BookingFields.BillingId)] public string? BillingId { get; set; }

    /// <summary>Gets or sets whether the booking is blocked. Maps to API field '<c>blocked</c>'.</summary>
    [JsonPropertyName(BookingFields.Blocked)] public bool Blocked { get; set; }

    /// <summary>Gets or sets the payment difference. Maps to API field '<c>paymentDifference</c>'.</summary>
    [JsonPropertyName(BookingFields.PaymentDifference)] public decimal? PaymentDifference { get; set; }

    /// <summary>Gets or sets the counterpart IBAN. Maps to API field '<c>counterpartIban</c>'.</summary>
    [JsonPropertyName(BookingFields.CounterpartIban)] public string? CounterpartIban { get; set; }

    /// <summary>Gets or sets the counterpart BIC. Maps to API field '<c>counterpartBic</c>'.</summary>
    [JsonPropertyName(BookingFields.CounterpartBic)] public string? CounterpartBic { get; set; }

    /// <summary>Gets or sets whether this is a Twingle donation. Maps to API field '<c>twingleDonation</c>'.</summary>
    [JsonPropertyName(BookingFields.TwingleDonation)] public bool TwingleDonation { get; set; }

    /// <summary>Gets or sets the booking project reference. Maps to API field '<c>bookingProject</c>'.</summary>
    [JsonPropertyName(BookingFields.BookingProject)] public string? BookingProject { get; set; }

    /// <summary>Gets or sets the sphere (area). Maps to API field '<c>sphere</c>'.</summary>
    [JsonPropertyName(BookingFields.Sphere)] public string? Sphere { get; set; }

    /// <summary>Gets or sets the related invoice IDs. Maps to API field '<c>relatedInvoice</c>'.</summary>
    [JsonPropertyName(BookingFields.RelatedInvoice)] public List<long>? RelatedInvoice { get; set; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "BookingEntityTests" --verbosity normal`
Expected: PASS — 1 test passed

- [ ] **Step 5: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/BookingEntityTests.cs src/MCP.EasyVerein.Domain/Entities/Booking.cs
git commit -m "feat(domain): add Booking entity with JSON mapping and TDD test"
```

---

### Task 3: Interface Extension

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`

- [ ] **Step 1: Add Booking methods to the interface**

Add at the end of the interface, before the closing `}`:

```csharp
    // Bookings
    /// <summary>Lists bookings, optionally filtered by ID.</summary>
    /// <param name="id">Optional booking ID to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching bookings.</returns>
    Task<IReadOnlyList<Booking>> ListBookingsAsync(long? id = null, CancellationToken ct = default);

    /// <summary>Gets a single booking by ID.</summary>
    /// <param name="id">The booking ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The booking, or <c>null</c> if not found.</returns>
    Task<Booking?> GetBookingAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new booking.</summary>
    /// <param name="booking">The booking to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created booking.</returns>
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct = default);

    /// <summary>Partially updates a booking.</summary>
    /// <param name="id">The booking ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated booking.</returns>
    Task<Booking> UpdateBookingAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Deletes a booking by ID.</summary>
    /// <param name="id">The booking ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteBookingAsync(long id, CancellationToken ct = default);
```

- [ ] **Step 2: Verify build (expect failure — implementation missing)**

Run: `dotnet build src/MCP.EasyVerein.Domain/MCP.EasyVerein.Domain.csproj --configuration Release`
Expected: Build succeeded (interface only, no implementation needed)

Run: `dotnet build src/MCP.EasyVerein.Infrastructure/MCP.EasyVerein.Infrastructure.csproj --configuration Release`
Expected: FAIL — `EasyVereinApiClient` does not implement the new methods

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs
git commit -m "feat(domain): add Booking CRUD methods to IEasyVereinApiClient interface"
```

---

### Task 4: BookingQuery + ApiQueries Extension

**Files:**
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/BookingQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Create BookingQuery.cs**

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the booking API endpoint, including field selection and optional filters.
/// </summary>
internal class BookingQuery
{
    /// <summary>Gets or sets an optional booking identifier filter.</summary>
    internal long? Id { get; set; }

    /// <summary>The base field selection query requesting all booking fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            BookingFields.Id + "," +
            BookingFields.Amount + "," +
            BookingFields.BankAccount + "," +
            BookingFields.BillingAccount + "," +
            BookingFields.Description + "," +
            BookingFields.Date + "," +
            BookingFields.Receiver + "," +
            BookingFields.BillingId + "," +
            BookingFields.Blocked + "," +
            BookingFields.PaymentDifference + "," +
            BookingFields.CounterpartIban + "," +
            BookingFields.CounterpartBic + "," +
            BookingFields.TwingleDonation + "," +
            BookingFields.BookingProject + "," +
            BookingFields.Sphere + "," +
            BookingFields.RelatedInvoice +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the booking endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (Id != null)
            parts.Add($"{BookingFields.Id}={Id}");

        return string.Join("&", parts);
    }
}
```

- [ ] **Step 2: Extend ApiQueries.cs**

Add after the existing `ContactDetailsQuery` field and `ContactDetails` property:

```csharp
    /// <summary>
    /// Shared <see cref="BookingQuery"/> instance used to build booking query strings with optional filters.
    /// </summary>
    internal static readonly BookingQuery BookingQuery = new();

    /// <summary>
    /// Gets the current booking query string including field selection and any active filters.
    /// </summary>
    internal static string Booking => BookingQuery.ToString();
```

- [ ] **Step 3: Verify build**

Run: `dotnet build src/MCP.EasyVerein.Infrastructure/MCP.EasyVerein.Infrastructure.csproj --configuration Release`
Expected: FAIL — `EasyVereinApiClient` still doesn't implement the interface (expected at this stage)

- [ ] **Step 4: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/BookingQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
git commit -m "feat(infra): add BookingQuery and extend ApiQueries for booking endpoint"
```

---

### Task 5: API Client Implementation + Infrastructure Tests (TDD)

**Files:**
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`

- [ ] **Step 1: Write the failing tests**

Add at the end of `EasyVereinApiClientTests` class, before the closing `}`:

```csharp
    // ------------------------------------------------------------------ //
    // Bookings
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetBookings_ReturnsBookings()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, amount = 150.50, receiver = "Max Mustermann" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListBookingsAsync();

        Assert.Single(result);
        Assert.Equal("Max Mustermann", result[0].Receiver);
        Assert.Equal(150.50m, result[0].Amount);
    }

    [Fact]
    public async Task GetBooking_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetBookingAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookings_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListBookingsAsync());
    }
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --filter "Booking" --verbosity normal`
Expected: FAIL — `ListBookingsAsync` and `GetBookingAsync` methods not found

- [ ] **Step 3: Implement the 5 CRUD methods in EasyVereinApiClient.cs**

Add these methods to the `EasyVereinApiClient` class (after the existing `UpdateMemberAsync` method):

```csharp
    // --- Bookings (FR-045) ---

    /// <summary>Lists bookings with optional ID filter and automatic pagination.</summary>
    /// <param name="id">Optional filter by booking identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="Booking"/> records.</returns>
    public async Task<IReadOnlyList<Booking>> ListBookingsAsync(long? id = null, CancellationToken ct = default)
    {
        ApiQueries.BookingQuery.Id = id;
        return await HandleListResponseWithPagination<Booking>(
            BuildListUrl("booking", ApiQueries.Booking), ct);
    }

    /// <summary>Retrieves a single booking by its identifier.</summary>
    /// <param name="id">The unique identifier of the booking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Booking"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Booking?> GetBookingAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"booking/{id}", ApiQueries.Booking), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Booking>(response, ct);
    }

    /// <summary>Creates a new booking via the API.</summary>
    /// <param name="booking">The booking to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Booking"/> as returned by the API.</returns>
    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("booking"), booking, ct), ct);
        return await HandleResponse<Booking>(response, ct);
    }

    /// <summary>Partially updates a booking with a patch dictionary.</summary>
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="patchData">An object containing only the fields to update.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Booking"/> as returned by the API.</returns>
    public async Task<Booking> UpdateBookingAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"booking/{id}"), content, ct), ct);
        return await HandleResponse<Booking>(response, ct);
    }

    /// <summary>Deletes a booking by its identifier.</summary>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteBookingAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"booking/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --filter "Booking" --verbosity normal`
Expected: PASS — 3 tests passed

- [ ] **Step 5: Run all tests to verify no regressions**

Run: `dotnet test --configuration Release --verbosity normal`
Expected: All tests pass (previous 46 + 3 new = 49)

- [ ] **Step 6: Commit**

```bash
git add tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
git commit -m "feat(infra): implement Booking CRUD in EasyVereinApiClient with TDD tests"
```

---

### Task 6: BookingTools MCP Server + Registration

**Files:**
- Create: `src/MCP.EasyVerein.Server/Tools/BookingTools.cs`
- Modify: `src/MCP.EasyVerein.Server/Program.cs`

- [ ] **Step 1: Create BookingTools.cs**

```csharp
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing bookings via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class BookingTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists bookings with an optional ID filter and automatic pagination.
    /// </summary>
    /// <param name="id">Optional booking ID filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching bookings, or an error message.</returns>
    [McpServerTool(Name = "list_bookings"), Description("List all bookings")]
    public async Task<string> ListBookings(
        [Description("The ID of a booking")] long? id, CancellationToken ct)
    {
        try
        {
            var bookings = await client.ListBookingsAsync(id, ct);
            return JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single booking by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the booking, or a not-found message.</returns>
    [McpServerTool(Name = "get_booking"), Description("Retrieve a booking by its ID")]
    public async Task<string> GetBooking(
        [Description("The ID of the booking")] long id, CancellationToken ct)
    {
        try
        {
            var booking = await client.GetBookingAsync(id, ct);
            return booking != null
                ? JsonSerializer.Serialize(booking, new JsonSerializerOptions { WriteIndented = true })
                : $"Booking with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new booking in easyVerein.
    /// </summary>
    /// <param name="amount">The booking amount.</param>
    /// <param name="receiver">The receiver of the booking.</param>
    /// <param name="description">An optional description.</param>
    /// <param name="date">An optional booking date (ISO 8601 format).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created booking, or an error message.</returns>
    [McpServerTool(Name = "create_booking"), Description("Create a new booking")]
    public async Task<string> CreateBooking(
        [Description("The booking amount")] decimal amount,
        [Description("The receiver of the booking")] string receiver,
        [Description("An optional description")] string? description,
        [Description("The booking date (ISO 8601)")] string? date,
        CancellationToken ct)
    {
        try
        {
            var booking = new Booking
            {
                Amount = amount,
                Receiver = receiver,
                Description = description,
                Date = date != null ? DateTime.Parse(date) : null
            };
            var created = await client.CreateBookingAsync(booking, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing booking. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="amount">An optional new amount.</param>
    /// <param name="description">An optional new description.</param>
    /// <param name="date">An optional new date (ISO 8601 format).</param>
    /// <param name="receiver">An optional new receiver.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated booking, or an error message.</returns>
    [McpServerTool(Name = "update_booking"), Description("Update a booking (only provided fields are changed)")]
    public async Task<string> UpdateBooking(
        [Description("The ID of the booking")] long id,
        [Description("The new amount")] decimal? amount,
        [Description("The new description")] string? description,
        [Description("The new date (ISO 8601)")] string? date,
        [Description("The new receiver")] string? receiver,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (amount != null) patch["amount"] = amount;
            if (description != null) patch["description"] = description;
            if (date != null) patch["date"] = date;
            if (receiver != null) patch["receiver"] = receiver;

            var updated = await client.UpdateBookingAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes a booking by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_booking"), Description("Delete a booking. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteBooking(
        [Description("The ID of the booking")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteBookingAsync(id, ct);
            return $"Booking with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
```

- [ ] **Step 2: Register BookingTools in Program.cs**

Add `.WithTools<BookingTools>()` after the existing tool registrations in `Program.cs`. Change:

```csharp
    .WithTools<ContactDetailsTools>();
```

to:

```csharp
    .WithTools<ContactDetailsTools>()
    .WithTools<BookingTools>();
```

Also add the `using` if not already present (it should be covered by the existing `using MCP.EasyVerein.Server.Tools;`).

- [ ] **Step 3: Verify full build**

Run: `dotnet build --configuration Release`
Expected: Build succeeded, 0 errors

- [ ] **Step 4: Run all tests**

Run: `dotnet test --configuration Release --verbosity normal`
Expected: All 50 tests pass (46 original + 1 BookingEntity + 3 Booking Infrastructure)

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/BookingTools.cs src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat(server): add BookingTools MCP tools with CRUD operations and error handling"
```

---

### Task 7: Final Verification

- [ ] **Step 1: Run full test suite**

Run: `dotnet test --configuration Release --verbosity normal`
Expected: 50 tests pass, 0 failures

- [ ] **Step 2: Verify all new files exist**

```bash
ls src/MCP.EasyVerein.Domain/ValueObjects/BookingFields.cs
ls src/MCP.EasyVerein.Domain/Entities/Booking.cs
ls src/MCP.EasyVerein.Infrastructure/ApiClient/BookingQuery.cs
ls src/MCP.EasyVerein.Server/Tools/BookingTools.cs
ls tests/MCP.EasyVerein.Domain.Tests/BookingEntityTests.cs
```

All 5 new files should exist.

- [ ] **Step 3: Verify git log shows all commits**

Run: `git log --oneline -7`
Expected: 5 feature commits for booking implementation
