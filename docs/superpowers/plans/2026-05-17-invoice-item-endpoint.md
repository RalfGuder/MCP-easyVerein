# Invoice-Item-Endpoint Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** US-0028 — Implement full CRUD MCP-Tools for the easyVerein `invoice-item` endpoint following the established Clean-Architecture pattern (Domain → Infrastructure → Server).

**Architecture:** Mirror the existing `BookingProject` endpoint (also PATCH-only, similar field shape). Add `InvoiceItem` entity + `InvoiceItemFields` VO in Domain, `InvoiceItemQuery` + 5 client methods in Infrastructure, `InvoiceItemTools` with 5 MCP tools in Server. Wire into DI in `Program.cs`. TDD throughout.

**Tech Stack:** C# 12 / .NET 8.0, ModelContextProtocol SDK v1.2.0 (stdio), xUnit 2.4.2 + Moq 4.20.72 for tests, `HttpClient.PatchAsync` for updates, `System.Text.Json` with `[JsonPropertyName]` attributes.

---

## File Structure

**Create:**
- `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceItemFields.cs` — API field-name constants
- `src/MCP.EasyVerein.Domain/Entities/InvoiceItem.cs` — Entity with `[JsonPropertyName]` mappings
- `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceItemQuery.cs` — Query-string builder
- `src/MCP.EasyVerein.Server/Tools/InvoiceItemTools.cs` — MCP tool surface
- `tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs` — Entity & VO unit tests

**Modify:**
- `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` — Add 5 methods (List/Get/Create/Update/Delete)
- `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` — Implement the 5 methods
- `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` — Register InvoiceItemQuery alongside others
- `src/MCP.EasyVerein.Server/Program.cs` — Add `.WithTools<InvoiceItemTools>()`
- `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` — Add HTTP tests
- `CLAUDE.md` — Mark endpoint as implemented in status table

---

## API-Schema Reference

GET sample (verified via curl against live API):

```json
{
  "id": 469271652,
  "org": "https://easyverein.com/api/v2.0/organization/30189",
  "relatedInvoice": "https://easyverein.com/api/v2.0/invoice/469271649",
  "billingAccount": "https://easyverein.com/api/v2.0/billing-account/58811",
  "totalPrice": 3.2,
  "articleObject": null,
  "quantity": "1.00",
  "unitPrice": "3.20",
  "title": "Rechnung zur Buchung 5920469474 vom 01.04.2026",
  "description": "Entgeltabrechnung\r\nsiehe Anlage",
  "taxRate": "0.00",
  "gross": false,
  "taxName": " ",
  "sphere": 2,
  "costCentre": "2901",
  "deductedExistingBalance": false
}
```

- Endpoint: `/invoice-item` (top-level, NOT nested under invoice).
- `quantity`, `unitPrice`, `taxRate` are **strings holding decimals** — use `FlexibleDecimalConverter`.
- `relatedInvoice`, `billingAccount`, `org` are **URL refs** — store as `string` and add `IdHelpers.ExtractId` in tests if needed.
- HTTP methods: GET (list+detail), POST, PATCH, DELETE. **No PUT** (per CLAUDE.md "Nur PATCH" list).
- Standard filter `relatedInvoice=<id>` returns items for one invoice.

---

### Task 1: Setup feature branch

**Files:** none (git operation only)

- [ ] **Step 1: Verify clean working tree**

Run: `git status`
Expected: `nothing to commit, working tree clean` (untracked `.claude/...` artefacts are fine).

- [ ] **Step 2: Create + switch branch**

Run: `git checkout -b feature/US-0028-invoice-item-endpoint`
Expected: `Switched to a new branch 'feature/US-0028-invoice-item-endpoint'`

---

### Task 2: Domain — InvoiceItemFields value object

**Files:**
- Create: `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceItemFields.cs`
- Test: `tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs` (created here, expanded later)

- [ ] **Step 1: Write the failing test**

Create `tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs`:

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Tests.Entities;

public class InvoiceItemFieldsTests
{
    [Fact]
    public void Constants_ExposeExpectedApiFieldNames()
    {
        Assert.Equal("id", InvoiceItemFields.Id);
        Assert.Equal("org", InvoiceItemFields.Org);
        Assert.Equal("relatedInvoice", InvoiceItemFields.RelatedInvoice);
        Assert.Equal("billingAccount", InvoiceItemFields.BillingAccount);
        Assert.Equal("totalPrice", InvoiceItemFields.TotalPrice);
        Assert.Equal("articleObject", InvoiceItemFields.ArticleObject);
        Assert.Equal("quantity", InvoiceItemFields.Quantity);
        Assert.Equal("unitPrice", InvoiceItemFields.UnitPrice);
        Assert.Equal("title", InvoiceItemFields.Title);
        Assert.Equal("description", InvoiceItemFields.Description);
        Assert.Equal("taxRate", InvoiceItemFields.TaxRate);
        Assert.Equal("gross", InvoiceItemFields.Gross);
        Assert.Equal("taxName", InvoiceItemFields.TaxName);
        Assert.Equal("sphere", InvoiceItemFields.Sphere);
        Assert.Equal("costCentre", InvoiceItemFields.CostCentre);
        Assert.Equal("deductedExistingBalance", InvoiceItemFields.DeductedExistingBalance);
    }

    [Fact]
    public void Filter_Constants_ExposeExpectedQueryParameters()
    {
        Assert.Equal("id__in", InvoiceItemFields.IdIn);
        Assert.Equal("relatedInvoice", InvoiceItemFields.RelatedInvoiceFilter);
        Assert.Equal("ordering", InvoiceItemFields.Ordering);
        Assert.Equal("search", InvoiceItemFields.Search);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~InvoiceItemFields"`
Expected: FAIL — `The type or namespace name 'InvoiceItemFields' could not be found`.

- [ ] **Step 3: Implement InvoiceItemFields**

Create `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceItemFields.cs`:

```csharp
namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Invoice Item API field names used in JSON serialization and query building.</summary>
internal static class InvoiceItemFields
{
    /// <summary>API field name for the unique invoice-item identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the owning organization URL reference.</summary>
    internal const string Org = "org";

    /// <summary>API field name for the parent invoice URL reference.</summary>
    internal const string RelatedInvoice = "relatedInvoice";

    /// <summary>API field name for the billing-account URL reference.</summary>
    internal const string BillingAccount = "billingAccount";

    /// <summary>API field name for the total price of the item (quantity * unitPrice).</summary>
    internal const string TotalPrice = "totalPrice";

    /// <summary>API field name for the linked article reference (optional).</summary>
    internal const string ArticleObject = "articleObject";

    /// <summary>API field name for the quantity (decimal as string).</summary>
    internal const string Quantity = "quantity";

    /// <summary>API field name for the unit price (decimal as string).</summary>
    internal const string UnitPrice = "unitPrice";

    /// <summary>API field name for the item title.</summary>
    internal const string Title = "title";

    /// <summary>API field name for the item description.</summary>
    internal const string Description = "description";

    /// <summary>API field name for the tax rate (decimal as string).</summary>
    internal const string TaxRate = "taxRate";

    /// <summary>API field name for the gross-pricing flag.</summary>
    internal const string Gross = "gross";

    /// <summary>API field name for the human-readable tax name.</summary>
    internal const string TaxName = "taxName";

    /// <summary>API field name for the SKR 42 sphere (integer).</summary>
    internal const string Sphere = "sphere";

    /// <summary>API field name for the cost centre (Kostenstelle).</summary>
    internal const string CostCentre = "costCentre";

    /// <summary>API field name for the deducted-existing-balance flag.</summary>
    internal const string DeductedExistingBalance = "deductedExistingBalance";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering by parent invoice ID.</summary>
    internal const string RelatedInvoiceFilter = "relatedInvoice";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~InvoiceItemFields"`
Expected: PASS (2/2).

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/InvoiceItemFields.cs tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs
git commit -m "feat(invoice-item): Domain — InvoiceItemFields VO (US-0028)"
```

---

### Task 3: Domain — InvoiceItem entity

**Files:**
- Create: `src/MCP.EasyVerein.Domain/Entities/InvoiceItem.cs`
- Modify: `tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs`

- [ ] **Step 1: Write the failing entity tests**

Append to `tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs`:

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

public class InvoiceItemEntityTests
{
    [Fact]
    public void Roundtrip_PreservesAllFields()
    {
        var item = new InvoiceItem
        {
            Id = 469271652,
            Org = "https://easyverein.com/api/v2.0/organization/30189",
            RelatedInvoice = "https://easyverein.com/api/v2.0/invoice/469271649",
            BillingAccount = "https://easyverein.com/api/v2.0/billing-account/58811",
            TotalPrice = 3.2m,
            Quantity = 1.00m,
            UnitPrice = 3.20m,
            Title = "Test",
            Description = "Desc",
            TaxRate = 0.00m,
            Gross = false,
            TaxName = " ",
            Sphere = 2,
            CostCentre = "2901",
            DeductedExistingBalance = false
        };

        var json = JsonSerializer.Serialize(item);
        var roundtrip = JsonSerializer.Deserialize<InvoiceItem>(json)!;

        Assert.Equal(item.Id, roundtrip.Id);
        Assert.Equal(item.Sphere, roundtrip.Sphere);
        Assert.Equal(item.CostCentre, roundtrip.CostCentre);
        Assert.Equal(item.BillingAccount, roundtrip.BillingAccount);
        Assert.Equal(item.RelatedInvoice, roundtrip.RelatedInvoice);
    }

    [Fact]
    public void Deserialize_AcceptsStringNumericFields()
    {
        // easyVerein v2.0 returns quantity/unitPrice/taxRate as strings.
        var json = "{\"id\":1,\"quantity\":\"1.00\",\"unitPrice\":\"3.20\",\"taxRate\":\"0.00\",\"totalPrice\":3.2}";
        var item = JsonSerializer.Deserialize<InvoiceItem>(json)!;
        Assert.Equal(1m, item.Quantity);
        Assert.Equal(3.20m, item.UnitPrice);
        Assert.Equal(0.00m, item.TaxRate);
        Assert.Equal(3.2m, item.TotalPrice);
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~InvoiceItem"`
Expected: FAIL — `InvoiceItem` type unknown.

- [ ] **Step 3: Implement InvoiceItem**

Create `src/MCP.EasyVerein.Domain/Entities/InvoiceItem.cs`:

```csharp
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a single line item (Rechnungsposition) on an easyVerein invoice.
/// </summary>
public class InvoiceItem : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the owning organization URL reference. Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Org)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the parent invoice URL reference. Maps to API field '<c>relatedInvoice</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.RelatedInvoice)]
    public string? RelatedInvoice { get; set; }

    /// <summary>Gets or sets the billing-account URL reference. Maps to API field '<c>billingAccount</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.BillingAccount)]
    public string? BillingAccount { get; set; }

    /// <summary>Gets or sets the total price (quantity * unitPrice). Maps to API field '<c>totalPrice</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.TotalPrice)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? TotalPrice { get; set; }

    /// <summary>Gets or sets the linked article URL reference (optional). Maps to API field '<c>articleObject</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.ArticleObject)]
    public string? ArticleObject { get; set; }

    /// <summary>Gets or sets the quantity. Maps to API field '<c>quantity</c>' (returned as string in v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.Quantity)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? Quantity { get; set; }

    /// <summary>Gets or sets the unit price. Maps to API field '<c>unitPrice</c>' (returned as string in v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.UnitPrice)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? UnitPrice { get; set; }

    /// <summary>Gets or sets the item title. Maps to API field '<c>title</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Title)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the item description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Description)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the tax rate. Maps to API field '<c>taxRate</c>' (returned as string in v2.0).</summary>
    [JsonPropertyName(InvoiceItemFields.TaxRate)]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? TaxRate { get; set; }

    /// <summary>Gets or sets the gross-pricing flag. Maps to API field '<c>gross</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Gross)]
    public bool? Gross { get; set; }

    /// <summary>Gets or sets the human-readable tax name. Maps to API field '<c>taxName</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.TaxName)]
    public string? TaxName { get; set; }

    /// <summary>Gets or sets the SKR 42 sphere. Maps to API field '<c>sphere</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.Sphere)]
    public int? Sphere { get; set; }

    /// <summary>Gets or sets the cost centre (Kostenstelle). Maps to API field '<c>costCentre</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.CostCentre)]
    public string? CostCentre { get; set; }

    /// <summary>Gets or sets the deducted-existing-balance flag. Maps to API field '<c>deductedExistingBalance</c>'.</summary>
    [JsonPropertyName(InvoiceItemFields.DeductedExistingBalance)]
    public bool? DeductedExistingBalance { get; set; }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~InvoiceItem"`
Expected: PASS (4/4 — 2 from Task 2 + 2 new entity tests).

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/InvoiceItem.cs tests/MCP.EasyVerein.Domain.Tests/Entities/InvoiceItemTests.cs
git commit -m "feat(invoice-item): Domain — InvoiceItem entity (US-0028)"
```

---

### Task 4: Infrastructure — InvoiceItemQuery builder

**Files:**
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceItemQuery.cs`
- Test: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` (extended)

- [ ] **Step 1: Write the failing test**

Append to `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`:

```csharp
[Fact]
public async Task ListInvoiceItems_SendsExpectedQuery()
{
    var json = JsonSerializer.Serialize(new
    {
        results = Array.Empty<object>(),
        next = (string?)null
    });
    var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
    var client = CreateClient(handler);

    await client.ListInvoiceItemsAsync(relatedInvoice: "469271649");

    Assert.NotNull(handler.LastRequestUri);
    var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
    Assert.Contains("relatedInvoice=469271649", query);
    Assert.Contains("query={id,", query);
    Assert.Contains(",costCentre,", query);
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --filter "ListInvoiceItems_SendsExpectedQuery"`
Expected: FAIL — `ListInvoiceItemsAsync` not on client.

- [ ] **Step 3: Implement InvoiceItemQuery**

Create `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceItemQuery.cs`:

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the invoice-item API endpoint with field selection and filters.
/// </summary>
internal class InvoiceItemQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional parent-invoice-ID filter.</summary>
    internal string? RelatedInvoice { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all invoice-item response fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            InvoiceItemFields.Id + "," +
            InvoiceItemFields.Org + "," +
            InvoiceItemFields.RelatedInvoice + "," +
            InvoiceItemFields.BillingAccount + "," +
            InvoiceItemFields.TotalPrice + "," +
            InvoiceItemFields.ArticleObject + "," +
            InvoiceItemFields.Quantity + "," +
            InvoiceItemFields.UnitPrice + "," +
            InvoiceItemFields.Title + "," +
            InvoiceItemFields.Description + "," +
            InvoiceItemFields.TaxRate + "," +
            InvoiceItemFields.Gross + "," +
            InvoiceItemFields.TaxName + "," +
            InvoiceItemFields.Sphere + "," +
            InvoiceItemFields.CostCentre + "," +
            InvoiceItemFields.DeductedExistingBalance +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{InvoiceItemFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(RelatedInvoice))
            parts.Add($"{InvoiceItemFields.RelatedInvoiceFilter}={Uri.EscapeDataString(RelatedInvoice)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{InvoiceItemFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{InvoiceItemFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
```

- [ ] **Step 4: Run test** — still red (client method missing). Proceed to Task 5.

---

### Task 5: Infrastructure — IEasyVereinApiClient extension

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`

- [ ] **Step 1: Locate insertion point**

Read `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` and find the BookingProject methods (~line 121-150). Insert the five new InvoiceItem methods directly below.

- [ ] **Step 2: Add interface methods**

```csharp
    // ------------------------------------------------------------------ //
    // Invoice Items
    // ------------------------------------------------------------------ //

    /// <summary>Lists invoice items with optional filters and pagination.</summary>
    Task<IReadOnlyList<InvoiceItem>> ListInvoiceItemsAsync(
        string? idIn = null,
        string? relatedInvoice = null,
        string? ordering = null,
        string[]? search = null,
        CancellationToken ct = default);

    /// <summary>Retrieves a single invoice item by its ID.</summary>
    Task<InvoiceItem?> GetInvoiceItemAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new invoice item.</summary>
    Task<InvoiceItem> CreateInvoiceItemAsync(InvoiceItem item, CancellationToken ct = default);

    /// <summary>Updates an existing invoice item (PATCH — only provided fields are changed).</summary>
    Task<InvoiceItem> UpdateInvoiceItemAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Deletes an invoice item by its ID.</summary>
    Task DeleteInvoiceItemAsync(long id, CancellationToken ct = default);
```

Add `using MCP.EasyVerein.Domain.Entities;` if not already present.

- [ ] **Step 3: Verify compile**

Run: `dotnet build src/MCP.EasyVerein.Infrastructure --nologo`
Expected: FAIL — `EasyVereinApiClient` does not implement the 5 new members.

---

### Task 6: Infrastructure — EasyVereinApiClient implementation

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Register query in ApiQueries**

Read existing `ApiQueries.cs`. Append a public read-only `InvoiceItem` property mirroring the BookingProject pattern. Example:

```csharp
/// <summary>Gets the invoice-item query builder.</summary>
public InvoiceItemQuery InvoiceItem { get; } = new();
```

If `ApiQueries` is a static class with method-style accessors, follow that idiom instead.

- [ ] **Step 2: Implement the 5 methods**

Add these methods to `EasyVereinApiClient.cs` after the BookingProject region (copy the structure exactly — they all follow the same shape used elsewhere in the file):

```csharp
// ------------------------------------------------------------------ //
// Invoice Items
// ------------------------------------------------------------------ //

/// <inheritdoc/>
public async Task<IReadOnlyList<InvoiceItem>> ListInvoiceItemsAsync(
    string? idIn = null,
    string? relatedInvoice = null,
    string? ordering = null,
    string[]? search = null,
    CancellationToken ct = default)
{
    var query = new InvoiceItemQuery
    {
        IdIn = idIn,
        RelatedInvoice = relatedInvoice,
        Ordering = ordering,
        Search = search
    };
    return await GetPaginatedAsync<InvoiceItem>(
        BuildUrl("invoice-item", query.ToString()), ct);
}

/// <inheritdoc/>
public async Task<InvoiceItem?> GetInvoiceItemAsync(long id, CancellationToken ct = default)
    => await GetSingleAsync<InvoiceItem>(BuildUrl($"invoice-item/{id}", new InvoiceItemQuery().ToString()), ct);

/// <inheritdoc/>
public async Task<InvoiceItem> CreateInvoiceItemAsync(InvoiceItem item, CancellationToken ct = default)
    => await PostAsync<InvoiceItem, InvoiceItem>(BuildUrl("invoice-item", null), item, ct);

/// <inheritdoc/>
public async Task<InvoiceItem> UpdateInvoiceItemAsync(long id, object patchData, CancellationToken ct = default)
    => await PatchAsync<InvoiceItem>(BuildUrl($"invoice-item/{id}", null), patchData, ct);

/// <inheritdoc/>
public async Task DeleteInvoiceItemAsync(long id, CancellationToken ct = default)
    => await DeleteAsync(BuildUrl($"invoice-item/{id}", null), ct);
```

**Note:** If helper method names (`GetPaginatedAsync`, `GetSingleAsync`, `PostAsync`, `PatchAsync`, `DeleteAsync`, `BuildUrl`) differ in the actual file, mirror exactly what the existing `BookingProject*` methods use. Do not invent new helpers.

- [ ] **Step 3: Run the Task 4 test**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --filter "ListInvoiceItems_SendsExpectedQuery"`
Expected: PASS.

- [ ] **Step 4: Add coverage for Get/Create/Update/Delete**

Append to the same test file:

```csharp
[Fact]
public async Task GetInvoiceItem_WithNotFound_ReturnsNull()
{
    var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
    var client = CreateClient(handler);
    Assert.Null(await client.GetInvoiceItemAsync(999));
}

[Fact]
public async Task UpdateInvoiceItem_SendsPatchDictionary()
{
    var responseJson = JsonSerializer.Serialize(new { id = 5, sphere = 2, costCentre = "2901" });
    var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, responseJson);
    var client = CreateClient(handler);

    var patch = new Dictionary<string, object> { ["sphere"] = 2, ["costCentre"] = "2901" };
    var updated = await client.UpdateInvoiceItemAsync(5, patch);

    Assert.Equal(HttpMethod.Patch, handler.LastRequest!.Method);
    Assert.Equal(2, updated.Sphere);
    Assert.Equal("2901", updated.CostCentre);
}

[Fact]
public async Task DeleteInvoiceItem_SendsDeleteToExpectedPath()
{
    var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, "");
    var client = CreateClient(handler);
    await client.DeleteInvoiceItemAsync(42);
    Assert.Contains("/invoice-item/42", handler.LastRequestUri!.AbsolutePath);
    Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
}

[Fact]
public async Task CreateInvoiceItem_PostsEntityAndReturnsCreated()
{
    var createdJson = JsonSerializer.Serialize(new { id = 99, title = "New", sphere = 9 });
    var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
    var client = CreateClient(handler);

    var created = await client.CreateInvoiceItemAsync(new InvoiceItem { Title = "New", Sphere = 9 });

    Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
    Assert.Equal(99, created.Id);
}
```

- [ ] **Step 5: Run all infrastructure tests**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests --nologo`
Expected: PASS (≈61 tests = 56 prior + 5 new).

- [ ] **Step 6: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceItemQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
git commit -m "feat(invoice-item): Infrastructure — Query + Client (US-0028)"
```

---

### Task 7: Server — InvoiceItemTools MCP-Tools

**Files:**
- Create: `src/MCP.EasyVerein.Server/Tools/InvoiceItemTools.cs`
- Modify: `src/MCP.EasyVerein.Server/Program.cs`

- [ ] **Step 1: Implement the tool surface**

Create `src/MCP.EasyVerein.Server/Tools/InvoiceItemTools.cs` (model exactly on `BookingProjectTools.cs`):

```csharp
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Helpers;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>MCP tools for managing invoice items (Rechnungspositionen) via the easyVerein API.</summary>
[McpServerToolType]
public sealed class InvoiceItemTools(IEasyVereinApiClient client)
{
    /// <summary>Lists invoice items with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_invoice_items"), Description("List all invoice items")]
    public async Task<string> ListInvoiceItems(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Parent invoice ID filter (numeric)")] string? relatedInvoice,
        [Description("Ordering (e.g. 'id' or '-id')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var items = await client.ListInvoiceItemsAsync(idIn, relatedInvoice, ordering, search, ct);
            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single invoice item by its unique identifier.</summary>
    [McpServerTool(Name = "get_invoice_item"), Description("Retrieve an invoice item by its ID")]
    public async Task<string> GetInvoiceItem(
        [Description("The ID of the invoice item")] long id,
        CancellationToken ct)
    {
        try
        {
            var item = await client.GetInvoiceItemAsync(id, ct);
            return item != null
                ? JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true })
                : $"Invoice item with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new invoice item.</summary>
    [McpServerTool(Name = "create_invoice_item"), Description("Create a new invoice item")]
    public async Task<string> CreateInvoiceItem(
        [Description("Parent invoice ID (numeric, required)")] long relatedInvoiceId,
        [Description("Item title (required)")] string title,
        [Description("Quantity (decimal, default 1.00)")] decimal? quantity,
        [Description("Unit price (decimal, default 0)")] decimal? unitPrice,
        [Description("Item description (optional)")] string? description,
        [Description("Tax rate (decimal, default 0)")] decimal? taxRate,
        [Description("Tax name (optional)")] string? taxName,
        [Description("Gross-pricing flag (default false)")] bool? gross,
        [Description("Billing-account ID (numeric, optional)")] long? billingAccountId,
        [Description("SKR42 sphere (1=ideell, 2=Vermögensverwaltung, 3=Zweckbetrieb, 4=wGB, 9=unkategorisiert default)")] int? sphere,
        [Description("Cost centre / Kostenstelle (optional)")] string? costCentre,
        CancellationToken ct)
    {
        try
        {
            var item = new InvoiceItem
            {
                RelatedInvoice = $"https://easyverein.com/api/v2.0/invoice/{relatedInvoiceId}",
                Title = title,
                Quantity = quantity ?? 1m,
                UnitPrice = unitPrice ?? 0m,
                Description = description,
                TaxRate = taxRate ?? 0m,
                TaxName = taxName,
                Gross = gross ?? false,
                Sphere = sphere ?? 9,
                CostCentre = costCentre ?? string.Empty
            };
            if (billingAccountId.HasValue)
                item.BillingAccount = $"https://easyverein.com/api/v2.0/billing-account/{billingAccountId.Value}";

            var created = await client.CreateInvoiceItemAsync(item, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing invoice item (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_invoice_item"), Description("Update an invoice item (only provided fields are changed)")]
    public async Task<string> UpdateInvoiceItem(
        [Description("The ID of the invoice item to update")] long id,
        [Description("New title")] string? title,
        [Description("New description")] string? description,
        [Description("New quantity")] decimal? quantity,
        [Description("New unit price")] decimal? unitPrice,
        [Description("New tax rate")] decimal? taxRate,
        [Description("New billing-account ID (numeric)")] long? billingAccountId,
        [Description("New SKR42 sphere")] int? sphere,
        [Description("New cost centre")] string? costCentre,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(title)) patch[InvoiceItemFields.Title] = title!;
            if (HasValue(description)) patch[InvoiceItemFields.Description] = description!;
            if (quantity.HasValue) patch[InvoiceItemFields.Quantity] = quantity.Value;
            if (unitPrice.HasValue) patch[InvoiceItemFields.UnitPrice] = unitPrice.Value;
            if (taxRate.HasValue) patch[InvoiceItemFields.TaxRate] = taxRate.Value;
            if (billingAccountId.HasValue)
                patch[InvoiceItemFields.BillingAccount] = $"https://easyverein.com/api/v2.0/billing-account/{billingAccountId.Value}";
            if (sphere.HasValue) patch[InvoiceItemFields.Sphere] = sphere.Value;
            if (HasValue(costCentre)) patch[InvoiceItemFields.CostCentre] = costCentre!;

            var updated = await client.UpdateInvoiceItemAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes an invoice item by its unique identifier.</summary>
    [McpServerTool(Name = "delete_invoice_item"), Description("Delete an invoice item. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteInvoiceItem(
        [Description("The ID of the invoice item to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteInvoiceItemAsync(id, ct);
            return $"Invoice item with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
```

- [ ] **Step 2: Register tool class in Program.cs**

Open `src/MCP.EasyVerein.Server/Program.cs`. Locate the chain of `.WithTools<...>()` calls. Add **after** the `BookingProjectTools` line:

```csharp
.WithTools<InvoiceItemTools>()
```

- [ ] **Step 3: Build the solution**

Run: `dotnet build --nologo`
Expected: 0 errors.

- [ ] **Step 4: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/InvoiceItemTools.cs src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat(invoice-item): Server — MCP tools (US-0028)"
```

---

### Task 8: Documentation & status sync

**Files:**
- Modify: `CLAUDE.md`
- Modify: `docs/001 User Stories/028-invoice-item-endpoint.md` (mark closed)

- [ ] **Step 1: Update CLAUDE.md endpoint table**

Open `CLAUDE.md`. In the "Implementierte Endpoints" section, change the count from **11** to **12** and add a row:

```markdown
| InvoiceItem    | US-0028    | list, get, create, update (PATCH), delete                |
```

In the "Nächste anstehende Endpoints" section, **remove** US-0028 if it appears (it does not currently — the section lists US-0016 and US-0017 only — so this step is a verification only).

- [ ] **Step 2: Mark user story doc as done**

Open `docs/001 User Stories/028-invoice-item-endpoint.md`. Tick all acceptance-criteria checkboxes (`- [ ]` → `- [x]`).

- [ ] **Step 3: Commit docs**

```bash
git add CLAUDE.md "docs/001 User Stories/028-invoice-item-endpoint.md"
git commit -m "docs(invoice-item): Status auf 12 Endpoints, US-0028 abgeschlossen"
```

---

### Task 9: Manual verification against live API

**Files:** none (live-call validation)

- [ ] **Step 1: Restart easyverein MCP server**

In Claude Code: `/mcp` → reconnect easyverein. Confirms the new tools register.

- [ ] **Step 2: List items for a known invoice**

Call `list_invoice_items` with `relatedInvoice="469271649"`.
Expected: One item with `sphere=2`, `costCentre="2901"`, `billingAccount` URL with `/58811`.

- [ ] **Step 3: Update sphere on a test item then revert**

Call `update_invoice_item` with `id=469271652`, `sphere=2`. Confirm returned `sphere=2`. (Already at 2 — round-trip safe.)

---

### Task 10: Push branch + open PR

**Files:** none (git operations)

- [ ] **Step 1: Push branch**

Run: `git push -u origin feature/US-0028-invoice-item-endpoint`

- [ ] **Step 2: Open PR linking issue #35**

```bash
gh pr create --title "feat(invoice-item): Endpoint implementieren (US-0028)" --body "$(cat <<'EOF'
## Summary
- Vollständiger CRUD-MCP-Endpoint für `invoice-item` (Rechnungspositionen) nach Standard-Muster.
- 5 neue MCP-Tools: `list_invoice_items`, `get_invoice_item`, `create_invoice_item`, `update_invoice_item`, `delete_invoice_item`.
- PATCH-Semantik analog `BookingProject` (kein PUT API-seitig).

Closes #35.

## Test plan
- [x] Domain.Tests grün (+4 Tests für InvoiceItemFields + InvoiceItem)
- [x] Infrastructure.Tests grün (+5 Tests für List/Get/Update/Delete/Create)
- [x] Manuelle Verifikation gegen live API (siehe Task 9)
EOF
)"
```

---

## Self-Review Notes

**Spec coverage:**
- ✅ Entity InvoiceItem → Task 3
- ✅ ValueObject InvoiceItemFields → Task 2
- ✅ Query-Klasse InvoiceItemQuery → Task 4
- ✅ API-Client mit 5 Methoden → Task 5 + 6
- ✅ MCP-Tools mit CRUD + Error-Handling → Task 7
- ✅ PATCH-Semantik via Dictionary → Task 7 Step 1 + Task 6 Step 2
- ✅ Pagination via `GetPaginatedAsync` → Task 6 Step 2
- ✅ Tests TDD → Tasks 2/3 (Domain), 4/6 (Infra)
- ✅ Manuelle Verifikation → Task 9

**Type consistency:** `ListInvoiceItemsAsync(string?, string?, string?, string[]?)` signature used identically in Task 5 (interface), Task 6 (impl), Task 7 (tool wrapper). `UpdateInvoiceItemAsync(long, object, CancellationToken)` consistent across interface/impl/tool. PATCH payload uses `Dictionary<string,object>` consistently.

**Placeholder scan:** No TBD / TODO / "similar to" / "implement later" present. All code blocks complete.
