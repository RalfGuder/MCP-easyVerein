# US-0011 Bank-Account-Endpoint Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** CRUD-Operationen für den easyVerein Bank-Account-Endpoint als MCP-Tools bereitstellen.

**Architecture:** Clean Architecture (4 Schichten) nach bestehendem Muster: Domain (Entity + Fields) → Infrastructure (ApiClient + Query) → Server (MCP-Tools). TDD mit Red-Green-Refactor.

**Tech Stack:** C# / .NET 8.0, xUnit, System.Text.Json, ModelContextProtocol SDK

---

## API-Felder (easyVerein v1.7 — `/api/v1.7/bank-account`)

Basierend auf `docs/easyverein-api-v1.7.yaml` (Zeilen 3913–4412).

| API-Feld | .NET-Typ | Beschreibung |
|----------|----------|--------------|
| `id` | `long` | Eindeutige ID (bereits vorhanden) |
| `name` | `string?` | Kontoname (max. 200 Zeichen, erforderlich) |
| `color` | `string?` | Hex-Farbe (max. 7 Zeichen) |
| `short` | `string?` | Kürzel (max. 4 Zeichen) |
| `billingAccount` | `long?` | Relation zu BillingAccount |
| `accountHolder` | `string?` | Kontoinhaber (max. 200 Zeichen) |
| `bankName` | `string?` | Name der Bank (max. 200 Zeichen) |
| `IBAN` | `string?` | IBAN (max. 32 Zeichen) |
| `BIC` | `string?` | BIC (max. 11 Zeichen) |
| `startsaldo` | `decimal?` | Startkapital (default 0) |
| `importSaldo` | `decimal?` | Importsaldo |
| `sphere` | `int?` | Sphäre im SKR 42 (default 9) |
| `computeStartsaldoOnImport` | `bool?` | Startsaldo beim Import berechnen |
| `last_imported_date` | `DateTime?` | Datum des letzten Online-Banking-Imports |

### Unterstützte Filter (GET-Liste)

`id__in`, `name`, `IBAN`, `BIC`, `accountHolder`, `bankName`, `ordering`, `search` (sucht in `name`, `short`, `bankName`, `accountHolder`)

## File Structure

| Datei | Aktion | Verantwortung |
|-------|--------|---------------|
| `src/MCP.EasyVerein.Domain/ValueObjects/BankAccountFields.cs` | Modify | Alle API-Feldnamen als Konstanten (nur `Id` vorhanden) |
| `src/MCP.EasyVerein.Domain/Entities/BankAccount.cs` | Create | Entity mit JsonPropertyName |
| `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` | Modify | CRUD-Methoden hinzufügen |
| `src/MCP.EasyVerein.Infrastructure/ApiClient/BankAccountQuery.cs` | Create | Query-Builder mit Feldauswahl und Filtern |
| `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` | Modify | BankAccount registrieren |
| `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` | Modify | CRUD-Implementierung |
| `src/MCP.EasyVerein.Server/Tools/BankAccountTools.cs` | Create | MCP-Tool-Klasse |
| `src/MCP.EasyVerein.Server/Program.cs` | Modify | Tool-Registrierung |
| `tests/MCP.EasyVerein.Domain.Tests/BankAccountEntityTests.cs` | Create | Entity-Deserialisierungs-Tests |
| `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` | Modify | API-Client-Tests |
| `CLAUDE.md` | Modify | Endpoint-Tabelle erweitern |
| `docs/001 User Stories/011-bank-account-endpoint.md` | Modify | Akzeptanzkriterien abhaken |

---

### Task 1: BankAccountFields ValueObject erweitern

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/ValueObjects/BankAccountFields.cs`

- [ ] **Step 1: Replace BankAccountFields.cs contents with full constant set**

```csharp
namespace MCP.EasyVerein.Domain.ValueObjects
{
    /// <summary>Constants for easyVerein Bank Account API field names used in JSON serialization and query building.</summary>
    internal static class BankAccountFields
    {
        /// <summary>API field name for the unique bank account identifier.</summary>
        internal const string Id = "id";

        /// <summary>API field name for the bank account name.</summary>
        internal const string Name = "name";

        /// <summary>API field name for the hex color value.</summary>
        internal const string Color = "color";

        /// <summary>API field name for the short label.</summary>
        internal const string Short = "short";

        /// <summary>API field name for the related billing account.</summary>
        internal const string BillingAccount = "billingAccount";

        /// <summary>API field name for the account holder name.</summary>
        internal const string AccountHolder = "accountHolder";

        /// <summary>API field name for the bank name.</summary>
        internal const string BankName = "bankName";

        /// <summary>API field name for the IBAN.</summary>
        internal const string Iban = "IBAN";

        /// <summary>API field name for the BIC.</summary>
        internal const string Bic = "BIC";

        /// <summary>API field name for the initial balance (Startsaldo).</summary>
        internal const string Startsaldo = "startsaldo";

        /// <summary>API field name for the import balance (Importsaldo).</summary>
        internal const string ImportSaldo = "importSaldo";

        /// <summary>API field name for the accounting sphere (SKR 42).</summary>
        internal const string Sphere = "sphere";

        /// <summary>API field name for the compute-startsaldo-on-import flag.</summary>
        internal const string ComputeStartsaldoOnImport = "computeStartsaldoOnImport";

        /// <summary>API field name for the last online banking import date.</summary>
        internal const string LastImportedDate = "last_imported_date";

        /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
        internal const string IdIn = "id__in";

        /// <summary>API query parameter for ordering results.</summary>
        internal const string Ordering = "ordering";

        /// <summary>API query parameter for full-text search.</summary>
        internal const string Search = "search";
    }
}
```

- [ ] **Step 2: Verify build compiles**

Run: `dotnet build src/MCP.EasyVerein.Domain/ --verbosity quiet`
Expected: Build succeeded, 0 errors

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/BankAccountFields.cs
git commit -m "feat(domain): extend BankAccountFields value object with full API field constants"
```

---

### Task 2: BankAccount Entity + Tests (TDD)

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/BankAccountEntityTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Entities/BankAccount.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class BankAccountEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "name": "Vereinskonto",
                "color": "#3366ff",
                "short": "VK",
                "billingAccount": 1200,
                "accountHolder": "TSV Musterhausen e.V.",
                "bankName": "Sparkasse Musterhausen",
                "IBAN": "DE89370400440532013000",
                "BIC": "COBADEFFXXX",
                "startsaldo": 1250.50,
                "importSaldo": 980.25,
                "sphere": 9,
                "computeStartsaldoOnImport": true,
                "last_imported_date": "2026-04-15T00:00:00"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var account = JsonSerializer.Deserialize<BankAccount>(json, options);

        Assert.NotNull(account);
        Assert.Equal(42L, account.Id);
        Assert.Equal("Vereinskonto", account.Name);
        Assert.Equal("#3366ff", account.Color);
        Assert.Equal("VK", account.Short);
        Assert.Equal(1200L, account.BillingAccount);
        Assert.Equal("TSV Musterhausen e.V.", account.AccountHolder);
        Assert.Equal("Sparkasse Musterhausen", account.BankName);
        Assert.Equal("DE89370400440532013000", account.Iban);
        Assert.Equal("COBADEFFXXX", account.Bic);
        Assert.Equal(1250.50m, account.Startsaldo);
        Assert.Equal(980.25m, account.ImportSaldo);
        Assert.Equal(9, account.Sphere);
        Assert.True(account.ComputeStartsaldoOnImport);
        Assert.Equal(new DateTime(2026, 4, 15), account.LastImportedDate);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "name": "Minimal"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var account = JsonSerializer.Deserialize<BankAccount>(json, options);

        Assert.NotNull(account);
        Assert.Equal(99L, account.Id);
        Assert.Equal("Minimal", account.Name);
        Assert.Null(account.Iban);
        Assert.Null(account.Bic);
        Assert.Null(account.Startsaldo);
        Assert.Null(account.LastImportedDate);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "BankAccountEntityTests" --verbosity quiet`
Expected: FAIL — `BankAccount` type does not exist

- [ ] **Step 3: Write BankAccount entity**

```csharp
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a bank account from the easyVerein API.
/// </summary>
public class BankAccount
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the bank account name. Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color value. Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Color)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label. Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the related billing account ID. Maps to API field '<c>billingAccount</c>'.</summary>
    [JsonPropertyName(BankAccountFields.BillingAccount)]
    public long? BillingAccount { get; set; }

    /// <summary>Gets or sets the account holder name. Maps to API field '<c>accountHolder</c>'.</summary>
    [JsonPropertyName(BankAccountFields.AccountHolder)]
    public string? AccountHolder { get; set; }

    /// <summary>Gets or sets the bank name. Maps to API field '<c>bankName</c>'.</summary>
    [JsonPropertyName(BankAccountFields.BankName)]
    public string? BankName { get; set; }

    /// <summary>Gets or sets the IBAN. Maps to API field '<c>IBAN</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Iban)]
    public string? Iban { get; set; }

    /// <summary>Gets or sets the BIC. Maps to API field '<c>BIC</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Bic)]
    public string? Bic { get; set; }

    /// <summary>Gets or sets the initial balance. Maps to API field '<c>startsaldo</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Startsaldo)]
    public decimal? Startsaldo { get; set; }

    /// <summary>Gets or sets the import balance. Maps to API field '<c>importSaldo</c>'.</summary>
    [JsonPropertyName(BankAccountFields.ImportSaldo)]
    public decimal? ImportSaldo { get; set; }

    /// <summary>Gets or sets the accounting sphere (SKR 42). Maps to API field '<c>sphere</c>'.</summary>
    [JsonPropertyName(BankAccountFields.Sphere)]
    public int? Sphere { get; set; }

    /// <summary>Gets or sets whether to compute startsaldo on import. Maps to API field '<c>computeStartsaldoOnImport</c>'.</summary>
    [JsonPropertyName(BankAccountFields.ComputeStartsaldoOnImport)]
    public bool? ComputeStartsaldoOnImport { get; set; }

    /// <summary>Gets or sets the last online-banking import date. Maps to API field '<c>last_imported_date</c>'.</summary>
    [JsonPropertyName(BankAccountFields.LastImportedDate)]
    public DateTime? LastImportedDate { get; set; }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "BankAccountEntityTests" --verbosity quiet`
Expected: 2 tests PASS

- [ ] **Step 5: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/BankAccountEntityTests.cs src/MCP.EasyVerein.Domain/Entities/BankAccount.cs
git commit -m "feat(domain): add BankAccount entity with TDD tests"
```

---

### Task 3: BankAccountQuery + ApiQueries Registration

**Files:**
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/BankAccountQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Create BankAccountQuery.cs**

```csharp
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the bank-account API endpoint with field selection and filters.
/// </summary>
internal class BankAccountQuery
{
    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional IBAN filter (exact match).</summary>
    internal string? Iban { get; set; }

    /// <summary>Gets or sets an optional BIC filter (exact match).</summary>
    internal string? Bic { get; set; }

    /// <summary>Gets or sets an optional account holder filter (exact match).</summary>
    internal string? AccountHolder { get; set; }

    /// <summary>Gets or sets an optional bank name filter (exact match).</summary>
    internal string? BankName { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all bank-account fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            BankAccountFields.Id + "," +
            BankAccountFields.Name + "," +
            BankAccountFields.Color + "," +
            BankAccountFields.Short + "," +
            BankAccountFields.BillingAccount + "," +
            BankAccountFields.AccountHolder + "," +
            BankAccountFields.BankName + "," +
            BankAccountFields.Iban + "," +
            BankAccountFields.Bic + "," +
            BankAccountFields.Startsaldo + "," +
            BankAccountFields.ImportSaldo + "," +
            BankAccountFields.Sphere + "," +
            BankAccountFields.ComputeStartsaldoOnImport + "," +
            BankAccountFields.LastImportedDate +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{BankAccountFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Iban))
            parts.Add($"{BankAccountFields.Iban}={Uri.EscapeDataString(Iban)}");
        if (!string.IsNullOrEmpty(Bic))
            parts.Add($"{BankAccountFields.Bic}={Uri.EscapeDataString(Bic)}");
        if (!string.IsNullOrEmpty(AccountHolder))
            parts.Add($"{BankAccountFields.AccountHolder}={Uri.EscapeDataString(AccountHolder)}");
        if (!string.IsNullOrEmpty(BankName))
            parts.Add($"{BankAccountFields.BankName}={Uri.EscapeDataString(BankName)}");
        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{BankAccountFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{BankAccountFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{BankAccountFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
```

- [ ] **Step 2: Register in ApiQueries.cs**

Append these two members inside the `ApiQueries` class (same pattern as `AnnouncementQuery`):

```csharp
/// <summary>
/// Shared <see cref="BankAccountQuery"/> instance used to build bank-account query strings with optional filters.
/// </summary>
internal static readonly BankAccountQuery BankAccountQuery = new();

/// <summary>
/// Gets the current bank-account query string including field selection and any active filters.
/// </summary>
internal static string BankAccount => BankAccountQuery.ToString();
```

- [ ] **Step 3: Verify build compiles**

Run: `dotnet build src/MCP.EasyVerein.Infrastructure/ --verbosity quiet`
Expected: Build succeeded, 0 errors

- [ ] **Step 4: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/BankAccountQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
git commit -m "feat(infra): add BankAccountQuery builder and register in ApiQueries"
```

---

### Task 4: IEasyVereinApiClient Interface erweitern

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`

- [ ] **Step 1: Add BankAccount CRUD method signatures**

Insert inside the `IEasyVereinApiClient` interface (anywhere — suggested: after the `Announcement` methods):

```csharp
/// <summary>Lists all bank accounts, optionally filtered.</summary>
/// <param name="name">Optional name filter (exact match).</param>
/// <param name="iban">Optional IBAN filter (exact match).</param>
/// <param name="bic">Optional BIC filter (exact match).</param>
/// <param name="accountHolder">Optional account-holder filter.</param>
/// <param name="bankName">Optional bank-name filter.</param>
/// <param name="idIn">Optional comma-separated list of IDs filter.</param>
/// <param name="ordering">Optional ordering criterion.</param>
/// <param name="search">Optional search terms.</param>
/// <param name="ct">Cancellation token.</param>
Task<IReadOnlyList<BankAccount>> ListBankAccountsAsync(
    string? name = null, string? iban = null, string? bic = null,
    string? accountHolder = null, string? bankName = null,
    string? idIn = null, string? ordering = null, string[]? search = null,
    CancellationToken ct = default);

/// <summary>Gets a single bank account by ID.</summary>
/// <param name="id">The bank account ID.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The bank account, or <c>null</c> if not found.</returns>
Task<BankAccount?> GetBankAccountAsync(long id, CancellationToken ct = default);

/// <summary>Creates a new bank account.</summary>
/// <param name="bankAccount">The bank account to create.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The created bank account.</returns>
Task<BankAccount> CreateBankAccountAsync(BankAccount bankAccount, CancellationToken ct = default);

/// <summary>Partially updates a bank account (PATCH semantics).</summary>
/// <param name="id">The bank account ID to update.</param>
/// <param name="patchData">An object containing the fields to patch.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The updated bank account.</returns>
Task<BankAccount> UpdateBankAccountAsync(long id, object patchData, CancellationToken ct = default);

/// <summary>Deletes a bank account by ID.</summary>
/// <param name="id">The bank account ID to delete.</param>
/// <param name="ct">Cancellation token.</param>
Task DeleteBankAccountAsync(long id, CancellationToken ct = default);
```

- [ ] **Step 2: Verify build compiles (Domain only — Infrastructure will break until Task 5)**

Run: `dotnet build src/MCP.EasyVerein.Domain/ --verbosity quiet`
Expected: Build succeeded, 0 errors

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs
git commit -m "feat(domain): add BankAccount CRUD methods to IEasyVereinApiClient"
```

---

### Task 5: EasyVereinApiClient Implementation + Tests (TDD)

**Files:**
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`

- [ ] **Step 1: Write failing tests**

Append these tests at the end of the test class in `EasyVereinApiClientTests.cs`:

```csharp
// ------------------------------------------------------------------ //
// Bank Accounts
// ------------------------------------------------------------------ //

[Fact]
public async Task ListBankAccounts_ReturnsBankAccounts()
{
    var json = JsonSerializer.Serialize(new
    {
        results = new[]
        {
            new
            {
                id = 1,
                name = "Vereinskonto",
                IBAN = "DE89370400440532013000",
                BIC = "COBADEFFXXX",
                accountHolder = "TSV Musterhausen e.V.",
                bankName = "Sparkasse"
            }
        },
        next = (string?)null
    });
    var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
    var client = CreateClient(handler);

    var result = await client.ListBankAccountsAsync();

    Assert.Single(result);
    Assert.Equal("Vereinskonto", result[0].Name);
    Assert.Equal("DE89370400440532013000", result[0].Iban);
    Assert.Equal("COBADEFFXXX", result[0].Bic);
}

[Fact]
public async Task GetBankAccount_WithNotFound_ReturnsNull()
{
    var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
    var client = CreateClient(handler);

    var result = await client.GetBankAccountAsync(999);

    Assert.Null(result);
}

[Fact]
public async Task ListBankAccounts_WithUnauthorized_ThrowsUnauthorizedAccessException()
{
    var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
    var client = CreateClient(handler);

    await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListBankAccountsAsync());
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --filter "BankAccount" --verbosity quiet`
Expected: FAIL — methods not implemented on `EasyVereinApiClient`

- [ ] **Step 3: Implement BankAccount CRUD in EasyVereinApiClient**

Add these methods to `EasyVereinApiClient.cs` (near the other endpoint regions):

```csharp
// ------------------------------------------------------------------ //
// Bank Accounts
// ------------------------------------------------------------------ //

/// <summary>Creates a new bank account via the API.</summary>
/// <param name="bankAccount">The bank account to create.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The created <see cref="BankAccount"/> as returned by the API.</returns>
public async Task<BankAccount> CreateBankAccountAsync(BankAccount bankAccount, CancellationToken ct = default)
{
    var response = await SendWithErrorHandling(
        () => _httpClient.PostAsJsonAsync(BuildUrl("bank-account"), bankAccount, ct), ct);
    return await HandleResponse<BankAccount>(response, ct);
}

/// <summary>Deletes a bank account by ID.</summary>
/// <param name="id">The bank account ID to delete.</param>
/// <param name="ct">Cancellation token.</param>
public async Task DeleteBankAccountAsync(long id, CancellationToken ct = default)
{
    var response = await SendWithErrorHandling(
        () => _httpClient.DeleteAsync(BuildUrl($"bank-account/{id}"), ct), ct);
    await EnsureSuccessOrThrowAsync(response, ct);
}

/// <summary>Gets a single bank account by ID.</summary>
/// <param name="id">The bank account ID.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The bank account, or <c>null</c> if not found.</returns>
public async Task<BankAccount?> GetBankAccountAsync(long id, CancellationToken ct = default)
{
    var response = await SendWithErrorHandling(
        () => _httpClient.GetAsync(BuildGetUrl($"bank-account/{id}", ApiQueries.BankAccount), ct), ct);
    if (response.StatusCode == HttpStatusCode.NotFound) return null;
    return await HandleResponse<BankAccount>(response, ct);
}

/// <summary>Lists bank accounts with optional filters and automatic pagination.</summary>
public async Task<IReadOnlyList<BankAccount>> ListBankAccountsAsync(
    string? name = null, string? iban = null, string? bic = null,
    string? accountHolder = null, string? bankName = null,
    string? idIn = null, string? ordering = null, string[]? search = null,
    CancellationToken ct = default)
{
    ApiQueries.BankAccountQuery.Name = name;
    ApiQueries.BankAccountQuery.Iban = iban;
    ApiQueries.BankAccountQuery.Bic = bic;
    ApiQueries.BankAccountQuery.AccountHolder = accountHolder;
    ApiQueries.BankAccountQuery.BankName = bankName;
    ApiQueries.BankAccountQuery.IdIn = idIn;
    ApiQueries.BankAccountQuery.Ordering = ordering;
    ApiQueries.BankAccountQuery.Search = search;

    return await HandleListResponseWithPagination<BankAccount>(
        BuildListUrl("bank-account", ApiQueries.BankAccount), ct);
}

/// <summary>Updates a bank account with PATCH semantics.</summary>
/// <param name="id">The bank account ID to update.</param>
/// <param name="patchData">An object containing the fields to patch.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The updated <see cref="BankAccount"/> as returned by the API.</returns>
public async Task<BankAccount> UpdateBankAccountAsync(long id, object patchData, CancellationToken ct = default)
{
    var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var response = await SendWithErrorHandling(
        () => _httpClient.PatchAsync(BuildUrl($"bank-account/{id}"), content, ct), ct);
    return await HandleResponse<BankAccount>(response, ct);
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --filter "BankAccount" --verbosity quiet`
Expected: 3 tests PASS

- [ ] **Step 5: Run all tests to check for regressions**

Run: `dotnet test --configuration Release --verbosity quiet`
Expected: All tests PASS (target: 68 tests total — 28 Domain, 13 Application, 27 Infrastructure)

- [ ] **Step 6: Commit**

```bash
git add tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
git commit -m "feat(infra): implement BankAccount CRUD in EasyVereinApiClient with TDD tests"
```

---

### Task 6: BankAccountTools MCP-Server

**Files:**
- Create: `src/MCP.EasyVerein.Server/Tools/BankAccountTools.cs`
- Modify: `src/MCP.EasyVerein.Server/Program.cs`

- [ ] **Step 1: Create BankAccountTools.cs**

```csharp
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing bank accounts via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class BankAccountTools(IEasyVereinApiClient client)
{
    /// <summary>Lists bank accounts with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_bank_accounts"), Description("List all bank accounts")]
    public async Task<string> ListBankAccounts(
        [Description("Exact name filter")] string? name,
        [Description("Exact IBAN filter")] string? iban,
        [Description("Exact BIC filter")] string? bic,
        [Description("Exact account-holder filter")] string? accountHolder,
        [Description("Exact bank-name filter")] string? bankName,
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Ordering (e.g. 'name' or '-startsaldo')")] string? ordering,
        [Description("Search terms (name, short, bankName, accountHolder)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var accounts = await client.ListBankAccountsAsync(
                name, iban, bic, accountHolder, bankName, idIn, ordering, search, ct);
            return JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single bank account by its unique identifier.</summary>
    [McpServerTool(Name = "get_bank_account"), Description("Retrieve a bank account by its ID")]
    public async Task<string> GetBankAccount(
        [Description("The ID of the bank account")] long id,
        CancellationToken ct)
    {
        try
        {
            var account = await client.GetBankAccountAsync(id, ct);
            return account != null
                ? JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true })
                : $"Bank account with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new bank account in easyVerein.</summary>
    [McpServerTool(Name = "create_bank_account"), Description("Create a new bank account")]
    public async Task<string> CreateBankAccount(
        [Description("The bank account name (required)")] string name,
        [Description("Hex color (max. 7 chars, e.g. #3366ff)")] string? color,
        [Description("Short label (max. 4 chars)")] string? @short,
        [Description("Related billing account ID")] long? billingAccount,
        [Description("Account holder name")] string? accountHolder,
        [Description("Bank name")] string? bankName,
        [Description("IBAN (max. 32 chars)")] string? iban,
        [Description("BIC (max. 11 chars)")] string? bic,
        [Description("Initial balance (decimal)")] decimal? startsaldo,
        [Description("Import balance (decimal)")] decimal? importSaldo,
        [Description("Accounting sphere (SKR 42, default 9)")] int? sphere,
        [Description("Compute startsaldo on import")] bool? computeStartsaldoOnImport,
        [Description("Last online-banking import date (ISO 8601)")] string? lastImportedDate,
        CancellationToken ct)
    {
        try
        {
            var account = new BankAccount { Name = name };

            if (HasValue(color)) account.Color = color;
            if (HasValue(@short)) account.Short = @short;
            if (billingAccount.HasValue) account.BillingAccount = billingAccount.Value;
            if (HasValue(accountHolder)) account.AccountHolder = accountHolder;
            if (HasValue(bankName)) account.BankName = bankName;
            if (HasValue(iban)) account.Iban = iban;
            if (HasValue(bic)) account.Bic = bic;
            if (startsaldo.HasValue) account.Startsaldo = startsaldo.Value;
            if (importSaldo.HasValue) account.ImportSaldo = importSaldo.Value;
            if (sphere.HasValue) account.Sphere = sphere.Value;
            if (computeStartsaldoOnImport.HasValue) account.ComputeStartsaldoOnImport = computeStartsaldoOnImport.Value;
            if (DateTime.TryParse(lastImportedDate, out var lid)) account.LastImportedDate = lid;

            var created = await client.CreateBankAccountAsync(account, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing bank account (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_bank_account"), Description("Update a bank account (only provided fields are changed)")]
    public async Task<string> UpdateBankAccount(
        [Description("The ID of the bank account to update")] long id,
        [Description("New name")] string? name,
        [Description("New hex color")] string? color,
        [Description("New short label")] string? @short,
        [Description("New related billing account ID")] long? billingAccount,
        [Description("New account holder")] string? accountHolder,
        [Description("New bank name")] string? bankName,
        [Description("New IBAN")] string? iban,
        [Description("New BIC")] string? bic,
        [Description("New initial balance")] decimal? startsaldo,
        [Description("New import balance")] decimal? importSaldo,
        [Description("New accounting sphere")] int? sphere,
        [Description("New compute-startsaldo-on-import flag")] bool? computeStartsaldoOnImport,
        [Description("New last import date (ISO 8601)")] string? lastImportedDate,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();

            if (HasValue(name)) patch[BankAccountFields.Name] = name!;
            if (HasValue(color)) patch[BankAccountFields.Color] = color!;
            if (HasValue(@short)) patch[BankAccountFields.Short] = @short!;
            if (billingAccount.HasValue) patch[BankAccountFields.BillingAccount] = billingAccount.Value;
            if (HasValue(accountHolder)) patch[BankAccountFields.AccountHolder] = accountHolder!;
            if (HasValue(bankName)) patch[BankAccountFields.BankName] = bankName!;
            if (HasValue(iban)) patch[BankAccountFields.Iban] = iban!;
            if (HasValue(bic)) patch[BankAccountFields.Bic] = bic!;
            if (startsaldo.HasValue) patch[BankAccountFields.Startsaldo] = startsaldo.Value;
            if (importSaldo.HasValue) patch[BankAccountFields.ImportSaldo] = importSaldo.Value;
            if (sphere.HasValue) patch[BankAccountFields.Sphere] = sphere.Value;
            if (computeStartsaldoOnImport.HasValue) patch[BankAccountFields.ComputeStartsaldoOnImport] = computeStartsaldoOnImport.Value;
            if (DateTime.TryParse(lastImportedDate, out var lid)) patch[BankAccountFields.LastImportedDate] = lid;

            var updated = await client.UpdateBankAccountAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a bank account by its unique identifier.</summary>
    [McpServerTool(Name = "delete_bank_account"), Description("Delete a bank account. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteBankAccount(
        [Description("The ID of the bank account to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteBankAccountAsync(id, ct);
            return $"Bank account with ID {id} has been deleted.";
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

- [ ] **Step 2: Register in Program.cs**

Add `.WithTools<BankAccountTools>()` to the MCP server builder chain at `src/MCP.EasyVerein.Server/Program.cs:61` (directly after `.WithTools<AnnouncementTools>()`):

```csharp
    .WithTools<AnnouncementTools>()
    .WithTools<BankAccountTools>();
```

- [ ] **Step 3: Verify build compiles**

Run: `dotnet build --configuration Release --verbosity quiet`
Expected: Build succeeded, 0 errors

- [ ] **Step 4: Run all tests**

Run: `dotnet test --configuration Release --verbosity quiet`
Expected: All 68 tests PASS

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/BankAccountTools.cs src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat(server): add BankAccountTools MCP tools with CRUD operations and error handling"
```

---

### Task 7: CLAUDE.md und User Story aktualisieren

**Files:**
- Modify: `CLAUDE.md`
- Modify: `docs/001 User Stories/011-bank-account-endpoint.md`

- [ ] **Step 1: Update CLAUDE.md endpoint table**

In the "Implementierte Endpoints" table, add a row after the Announcement row:

```
| BankAccount    | US-0011    | list, get, create, update (PATCH), delete                |
```

Update the counter in the table header from `(7)` to `(8)`, remove `US-0011: Bank Account` from the "Nächste anstehende Endpoints" list, and add US-0016 Contact Details Group to keep the queue at 5 entries.

- [ ] **Step 2: Update CLAUDE.md Teststruktur counts**

Update:
- `**Domain.Tests** — Entity- und Value-Object-Tests (28)` (was 26)
- `**Infrastructure.Tests** — HTTP-Client mit gemocktem HttpMessageHandler (27)` (was 26)
- `**Gesamt: 68 Tests**` (was 65)

- [ ] **Step 3: Check all acceptance criteria in the user story**

In `docs/001 User Stories/011-bank-account-endpoint.md`, replace every `- [ ]` with `- [x]` in the Akzeptanzkriterien list, and add a final status line after the list:

```
**Status:** Implementiert mit PR #<NR> am 2026-04-20.
```

- [ ] **Step 4: Run final full test suite**

Run: `dotnet test --configuration Release --verbosity quiet`
Expected: All 68 tests PASS

- [ ] **Step 5: Commit**

```bash
git add CLAUDE.md "docs/001 User Stories/011-bank-account-endpoint.md"
git commit -m "docs: update CLAUDE.md and user story with BankAccount endpoint status"
```

---

## Self-Review Notes

- **Spec coverage:** Alle 8 Akzeptanzkriterien aus US-0011 sind abgedeckt — Entity (Task 2), Fields (Task 1), Query (Task 3), API-Client (Tasks 4–5), MCP-Tools (Task 6), PATCH-Dictionary (Task 6 Update-Methode), Pagination (Task 5 via `HandleListResponseWithPagination`), TDD-Tests (Tasks 2 + 5).
- **No placeholders:** Alle Code-Blöcke vollständig, alle Feldnamen aus OpenAPI verifiziert.
- **Type consistency:** `BankAccount`-Properties stimmen zwischen Entity, Interface-Signaturen, Client-Implementierung und Tools überein. `BankAccountFields.Iban` mappt korrekt auf `IBAN` (API-Groß­schreibung), `BankAccountFields.LastImportedDate` auf `last_imported_date` (snake_case).
- **Bekannte Abweichungen:** `@short` als C#-Parameter-Name (Keyword-Escape), da `short` reserviert ist. Range-Filter (`startsaldo__gte` etc.) sind bewusst weggelassen — können bei Bedarf nachgezogen werden.
