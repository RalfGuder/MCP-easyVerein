# Invoice Entity API Alignment Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Align the `Invoice` entity with the actual easyVerein API v1.7 response so that `list_invoices` / `get_invoice` deserialize successfully for real data (including URL-typed foreign keys, date-only fields, nested `charges`, and previously-missing fields).

**Architecture:** The easyVerein API returns foreign-key references as absolute URL strings (`"https://easyverein.com/api/v1.7/contact-details/345175845"`), date fields as `YYYY-MM-DD` (not ISO-DateTime), and numeric money fields as strings. We keep URL references as `string?`/`List<string>?`, add `JsonIgnore` helper properties that extract the numeric ID, introduce a `UrlReferenceConverter` only if complex cases appear, add a `JsonConverter` that accepts both `YYYY-MM-DD` and full ISO datetimes for `DateTime?` fields, and complete the entity with the missing fields. `JsonNumberHandling.AllowReadingFromString` is already configured in `EasyVereinApiClient`, so `decimal?` string values already deserialize.

**Tech Stack:** C# / .NET 8, System.Text.Json, xUnit, Moq.

---

## File Structure

### Create
- `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceCharges.cs` — nested object (`charge`, `chargeBack`, `total`).
- `src/MCP.EasyVerein.Domain/Converters/FlexibleDateTimeConverter.cs` — `JsonConverter<DateTime?>` accepting `yyyy-MM-dd` and full ISO.
- `src/MCP.EasyVerein.Domain/Helpers/UrlReference.cs` — static helper `ExtractId(string? url)` returning `long?`.
- `tests/MCP.EasyVerein.Domain.Tests/InvoiceRealResponseTests.cs` — integration-style JSON test using a captured real API response.
- `tests/MCP.EasyVerein.Domain.Tests/FlexibleDateTimeConverterTests.cs`
- `tests/MCP.EasyVerein.Domain.Tests/UrlReferenceTests.cs`

### Modify
- `src/MCP.EasyVerein.Domain/Entities/Invoice.cs` — retype URL refs as `string?`/`List<string>?`, add missing fields, add `JsonIgnore` id-helper properties, apply `FlexibleDateTimeConverter` to date fields.
- `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceFields.cs` — add constants for new fields (`Org`, `Path`, `InvoiceItems`, `Charges`, `Tax`, `PaymentDifference`, `CancelInvoice`, `DeleteAfterDate`, `DeletedBy`, `IsTaxRatePerInvoiceItem`, `IsSubjectToTax`).
- `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs` — include new fields in the `FieldQuery` selection.
- `tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs` — update expected types: URL strings instead of `long`, list of URL strings instead of list of long.
- `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientInvoiceTests.cs` (if exists) — adjust mock responses to use URL-form references.

### Not touched in this plan
- `src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs` — no signature change expected (create/delete payload still uses numeric IDs on *write*; we only fix *read*). Revisit if tests expose a problem.

---

## Task 1: Capture a real API response as a test fixture

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/Fixtures/invoice-real-response.json`

- [ ] **Step 1: Save the captured JSON as a fixture**

Use the response we captured (single invoice, field-filtered). Redact nothing — the record is already sanitized (no PII beyond a business address that is part of a receipt test).

```json
{
  "id": 348583191,
  "relatedBookings": ["https://easyverein.com/api/v1.7/booking/234717573"],
  "relatedAddress": "https://easyverein.com/api/v1.7/contact-details/345175845",
  "payedFromUser": null,
  "approvedFromAdmin": null,
  "canceledInvoice": null,
  "bankAccount": null,
  "gross": false,
  "date": "2025-03-26",
  "dateItHappend": "2025-03-26",
  "dateSent": null,
  "invNumber": "1",
  "receiver": "Kaufland Rathenow\r\nSchwedendamm 3\r\n14712 Rathenow\r\nDeutschland",
  "description": "",
  "totalPrice": "18.88",
  "kind": "expense",
  "refNumber": "94KW7ABAR",
  "isDraft": false,
  "isTemplate": false,
  "isRequest": false,
  "taxRate": "0.00",
  "taxName": null,
  "accnumber": 0,
  "isReceipt": true
}
```

- [ ] **Step 2: Mark fixture to be copied to output**

Add to `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj` (inside an `<ItemGroup>`):

```xml
<None Update="Fixtures\invoice-real-response.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

- [ ] **Step 3: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/Fixtures/invoice-real-response.json tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj
git commit -m "test(domain): add real invoice API response fixture"
```

---

## Task 2: Add `UrlReference.ExtractId` helper with tests

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/UrlReferenceTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Helpers/UrlReference.cs`

- [ ] **Step 1: Write failing tests**

```csharp
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Tests;

public class UrlReferenceTests
{
    [Theory]
    [InlineData("https://easyverein.com/api/v1.7/contact-details/345175845", 345175845L)]
    [InlineData("https://easyverein.com/api/v1.7/booking/234717573/", 234717573L)]
    [InlineData("https://easyverein.com/api/v1.4/invoice/1", 1L)]
    public void ExtractId_ReturnsTrailingNumber(string url, long expected)
    {
        Assert.Equal(expected, UrlReference.ExtractId(url));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("https://easyverein.com/api/v1.7/contact-details/")]
    public void ExtractId_ReturnsNullForInvalidInput(string? url)
    {
        Assert.Null(UrlReference.ExtractId(url));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~UrlReferenceTests"`
Expected: FAIL (compiler error — `UrlReference` does not exist).

- [ ] **Step 3: Implement the helper**

```csharp
namespace MCP.EasyVerein.Domain.Helpers;

/// <summary>
/// Helpers for working with easyVerein API URL-form foreign-key references such as
/// <c>https://easyverein.com/api/v1.7/contact-details/345175845</c>.
/// </summary>
public static class UrlReference
{
    /// <summary>
    /// Extracts the trailing numeric identifier from an easyVerein resource URL.
    /// </summary>
    /// <param name="url">The URL or null.</param>
    /// <returns>The parsed identifier, or <c>null</c> if the URL is null, empty, or has no trailing number.</returns>
    public static long? ExtractId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var trimmed = url.TrimEnd('/');
        var slash = trimmed.LastIndexOf('/');
        if (slash < 0 || slash == trimmed.Length - 1) return null;
        var tail = trimmed[(slash + 1)..];
        return long.TryParse(tail, out var id) ? id : null;
    }
}
```

- [ ] **Step 4: Run tests — expect PASS**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~UrlReferenceTests"`
Expected: PASS (4 test cases + 4 test cases).

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Helpers/UrlReference.cs tests/MCP.EasyVerein.Domain.Tests/UrlReferenceTests.cs
git commit -m "feat(domain): add UrlReference.ExtractId helper for API URL refs"
```

---

## Task 3: Add `FlexibleDateTimeConverter`

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/FlexibleDateTimeConverterTests.cs`
- Create: `src/MCP.EasyVerein.Domain/Converters/FlexibleDateTimeConverter.cs`

- [ ] **Step 1: Write failing tests**

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleDateTimeConverterTests
{
    private sealed class Probe
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime? Value { get; set; }
    }

    [Theory]
    [InlineData("\"2025-03-26\"", 2025, 3, 26)]
    [InlineData("\"2025-03-26T00:00:00\"", 2025, 3, 26)]
    [InlineData("\"2025-03-26T12:34:56Z\"", 2025, 3, 26)]
    public void Read_ParsesDateOnlyAndIsoDateTime(string json, int y, int m, int d)
    {
        var probe = JsonSerializer.Deserialize<Probe>($"{{\"Value\":{json}}}");
        Assert.NotNull(probe);
        Assert.Equal(new DateTime(y, m, d), probe!.Value!.Value.Date);
    }

    [Fact]
    public void Read_ReturnsNullForJsonNull()
    {
        var probe = JsonSerializer.Deserialize<Probe>("{\"Value\":null}");
        Assert.Null(probe!.Value);
    }

    [Fact]
    public void Write_EmitsIsoRoundTrip()
    {
        var probe = new Probe { Value = new DateTime(2025, 3, 26) };
        var json = JsonSerializer.Serialize(probe);
        Assert.Contains("\"2025-03-26", json);
    }
}
```

- [ ] **Step 2: Run test — expect FAIL (type missing)**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~FlexibleDateTimeConverterTests"`
Expected: FAIL.

- [ ] **Step 3: Implement the converter**

```csharp
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for nullable <see cref="DateTime"/> that accepts either a date-only
/// string (<c>yyyy-MM-dd</c>) as returned by the easyVerein API for plain date fields,
/// or a full ISO-8601 datetime.
/// </summary>
public sealed class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    /// <summary>Reads a nullable <see cref="DateTime"/> accepting date-only or ISO datetimes.</summary>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        var s = reader.GetString();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out var date))
            return date;
        return DateTime.Parse(s!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>Writes a nullable <see cref="DateTime"/> as ISO-8601 or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
    }
}
```

- [ ] **Step 4: Run tests — expect PASS**

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Converters/FlexibleDateTimeConverter.cs tests/MCP.EasyVerein.Domain.Tests/FlexibleDateTimeConverterTests.cs
git commit -m "feat(domain): add FlexibleDateTimeConverter for date-only API fields"
```

---

## Task 4: Add `InvoiceCharges` value object

**Files:**
- Create: `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceCharges.cs`
- Create: `tests/MCP.EasyVerein.Domain.Tests/InvoiceChargesTests.cs`

- [ ] **Step 1: Write failing test**

```csharp
using System.Text.Json;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceChargesTests
{
    [Fact]
    public void Deserialize_MapsAllThreeFields()
    {
        var json = """{"charge":1.23,"chargeBack":0.50,"total":18.88}""";
        var options = new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };
        var c = JsonSerializer.Deserialize<InvoiceCharges>(json, options);
        Assert.NotNull(c);
        Assert.Equal(1.23m, c!.Charge);
        Assert.Equal(0.50m, c.ChargeBack);
        Assert.Equal(18.88m, c.Total);
    }
}
```

- [ ] **Step 2: Run — expect FAIL (type missing)**

- [ ] **Step 3: Implement**

```csharp
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Payment-processor charge summary returned by the easyVerein API.</summary>
public class InvoiceCharges
{
    /// <summary>Gets or sets the charge amount. Maps to API field <c>charge</c>.</summary>
    [JsonPropertyName("charge")] public decimal Charge { get; set; }

    /// <summary>Gets or sets the charge-back amount. Maps to API field <c>chargeBack</c>.</summary>
    [JsonPropertyName("chargeBack")] public decimal ChargeBack { get; set; }

    /// <summary>Gets or sets the total amount (after charges). Maps to API field <c>total</c>.</summary>
    [JsonPropertyName("total")] public decimal Total { get; set; }
}
```

- [ ] **Step 4: Run test — expect PASS**

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/InvoiceCharges.cs tests/MCP.EasyVerein.Domain.Tests/InvoiceChargesTests.cs
git commit -m "feat(domain): add InvoiceCharges value object"
```

---

## Task 5: Extend `InvoiceFields` constants

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceFields.cs`

- [ ] **Step 1: Add the missing constants**

Add inside the class (alphabetised to match the existing style):

```csharp
/// <summary>API field name for the cancel-invoice reference.</summary>
public const string CancelInvoice = "cancelInvoice";
/// <summary>API field name for the payment-processor charges object.</summary>
public const string Charges = "charges";
/// <summary>API field name for the scheduled deletion date.</summary>
public const string DeleteAfterDate = "_deleteAfterDate";
/// <summary>API field name for the deleting user reference.</summary>
public const string DeletedBy = "_deletedBy";
/// <summary>API field name for the list of invoice-item references.</summary>
public const string InvoiceItems = "invoiceItems";
/// <summary>API field name for the flag marking per-item tax rates.</summary>
public const string IsTaxRatePerInvoiceItem = "_isTaxRatePerInvoiceItem";
/// <summary>API field name for the flag marking VAT liability.</summary>
public const string IsSubjectToTax = "_isSubjectToTax";
/// <summary>API field name for the organisation reference.</summary>
public const string Org = "org";
/// <summary>API field name for the file path / download URL.</summary>
public const string Path = "path";
/// <summary>API field name for the payment difference amount.</summary>
public const string PaymentDifference = "paymentDifference";
/// <summary>API field name for the tax amount.</summary>
public const string Tax = "tax";
```

- [ ] **Step 2: Verify compile**

Run: `dotnet build src/MCP.EasyVerein.Domain`
Expected: succeeds.

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/InvoiceFields.cs
git commit -m "feat(domain): add missing Invoice API field constants"
```

---

## Task 6: Retype URL-reference fields and add missing fields on `Invoice`

**Files:**
- Modify: `src/MCP.EasyVerein.Domain/Entities/Invoice.cs`
- Create: `tests/MCP.EasyVerein.Domain.Tests/InvoiceRealResponseTests.cs`

- [ ] **Step 1: Write failing integration-style test against the captured fixture**

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceRealResponseTests
{
    private static JsonSerializerOptions Options() => new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = false
    };

    [Fact]
    public void Deserialize_RealResponse_Succeeds()
    {
        var json = File.ReadAllText(Path.Combine("Fixtures", "invoice-real-response.json"));
        var invoice = JsonSerializer.Deserialize<Invoice>(json, Options());

        Assert.NotNull(invoice);
        Assert.Equal(348583191L, invoice!.Id);
        Assert.Equal("https://easyverein.com/api/v1.7/contact-details/345175845", invoice.RelatedAddress);
        Assert.Equal(345175845L, invoice.RelatedAddressId);
        Assert.NotNull(invoice.RelatedBookings);
        Assert.Single(invoice.RelatedBookings!);
        Assert.Equal(234717573L, UrlReference.ExtractId(invoice.RelatedBookings![0]));
        Assert.Equal(18.88m, invoice.TotalPrice);
        Assert.Equal(new DateTime(2025, 3, 26), invoice.Date);
        Assert.Equal(new DateTime(2025, 3, 26), invoice.DueDate);
        Assert.Null(invoice.DateSent);
        Assert.Equal("1", invoice.InvoiceNumber);
        Assert.Equal("expense", invoice.Kind);
        Assert.Equal("94KW7ABAR", invoice.RefNumber);
        Assert.True(invoice.IsReceipt);
    }
}
```

- [ ] **Step 2: Run — expect FAIL (compile error: properties not yet in expected types)**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests --filter "FullyQualifiedName~InvoiceRealResponseTests"`
Expected: FAIL.

- [ ] **Step 3: Rewrite `Invoice.cs` with the aligned shape**

Retype the URL-form foreign keys, apply `FlexibleDateTimeConverter` to every date field, add the new fields, and expose `*Id` helper properties (`JsonIgnore`). Replace the existing class body with:

```csharp
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Helpers;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an invoice from the easyVerein API.
/// </summary>
public class Invoice
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Id)] public long Id { get; set; }

    /// <summary>Gets or sets the invoice number. Maps to API field '<c>invNumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.InvoiceNumber)] public string? InvoiceNumber { get; set; }

    /// <summary>Gets or sets the total price. Maps to API field '<c>totalPrice</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TotalPrice)] public decimal? TotalPrice { get; set; }

    /// <summary>Gets or sets the tax amount. Maps to API field '<c>tax</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Tax)] public decimal? Tax { get; set; }

    /// <summary>Gets or sets the difference between invoiced and paid amount. Maps to API field '<c>paymentDifference</c>'.</summary>
    [JsonPropertyName(InvoiceFields.PaymentDifference)] public decimal? PaymentDifference { get; set; }

    /// <summary>Gets or sets the invoice date. Maps to API field '<c>date</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Date)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? Date { get; set; }

    /// <summary>Gets or sets the due date. Maps to API field '<c>dateItHappend</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DueDate)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? DueDate { get; set; }

    /// <summary>Gets or sets the date the invoice was sent. Maps to API field '<c>dateSent</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DateSent)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? DateSent { get; set; }

    /// <summary>Gets or sets the invoice kind/type. Maps to API field '<c>kind</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Kind)] public string? Kind { get; set; }

    /// <summary>Gets or sets the invoice description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Description)] public string? Description { get; set; }

    /// <summary>Gets or sets the receiver address text. Maps to API field '<c>receiver</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Receiver)] public string? Receiver { get; set; }

    /// <summary>Gets or sets the related address URL reference. Maps to API field '<c>relatedAddress</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RelatedAddress)] public string? RelatedAddress { get; set; }

    /// <summary>Gets the contact-details ID extracted from <see cref="RelatedAddress"/>.</summary>
    [JsonIgnore] public long? RelatedAddressId => UrlReference.ExtractId(RelatedAddress);

    /// <summary>Gets or sets the related booking URL references. Maps to API field '<c>relatedBookings</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RelatedBookings)] public List<string>? RelatedBookings { get; set; }

    /// <summary>Gets or sets the URL reference of the user who paid. Maps to API field '<c>payedFromUser</c>'.</summary>
    [JsonPropertyName(InvoiceFields.PayedFromUser)] public string? PayedFromUser { get; set; }

    /// <summary>Gets the user ID extracted from <see cref="PayedFromUser"/>.</summary>
    [JsonIgnore] public long? PayedFromUserId => UrlReference.ExtractId(PayedFromUser);

    /// <summary>Gets or sets the URL reference of the approving admin. Maps to API field '<c>approvedFromAdmin</c>'.</summary>
    [JsonPropertyName(InvoiceFields.ApprovedFromAdmin)] public string? ApprovedFromAdmin { get; set; }

    /// <summary>Gets the admin ID extracted from <see cref="ApprovedFromAdmin"/>.</summary>
    [JsonIgnore] public long? ApprovedFromAdminId => UrlReference.ExtractId(ApprovedFromAdmin);

    /// <summary>Gets or sets the URL reference of the canceled invoice. Maps to API field '<c>canceledInvoice</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CanceledInvoice)] public string? CanceledInvoice { get; set; }

    /// <summary>Gets the invoice ID extracted from <see cref="CanceledInvoice"/>.</summary>
    [JsonIgnore] public long? CanceledInvoiceId => UrlReference.ExtractId(CanceledInvoice);

    /// <summary>Gets or sets the URL reference of a storno-target invoice. Maps to API field '<c>cancelInvoice</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CancelInvoice)] public string? CancelInvoice { get; set; }

    /// <summary>Gets the invoice ID extracted from <see cref="CancelInvoice"/>.</summary>
    [JsonIgnore] public long? CancelInvoiceId => UrlReference.ExtractId(CancelInvoice);

    /// <summary>Gets or sets the URL reference of the bank account. Maps to API field '<c>bankAccount</c>'.</summary>
    [JsonPropertyName(InvoiceFields.BankAccount)] public string? BankAccount { get; set; }

    /// <summary>Gets the bank-account ID extracted from <see cref="BankAccount"/>.</summary>
    [JsonIgnore] public long? BankAccountId => UrlReference.ExtractId(BankAccount);

    /// <summary>Gets or sets the URL reference of the organisation. Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Org)] public string? Org { get; set; }

    /// <summary>Gets the organisation ID extracted from <see cref="Org"/>.</summary>
    [JsonIgnore] public long? OrgId => UrlReference.ExtractId(Org);

    /// <summary>Gets or sets the file/download URL of the invoice PDF. Maps to API field '<c>path</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Path)] public string? Path { get; set; }

    /// <summary>Gets or sets the list of invoice-item URL references. Maps to API field '<c>invoiceItems</c>'.</summary>
    [JsonPropertyName(InvoiceFields.InvoiceItems)] public List<string>? InvoiceItems { get; set; }

    /// <summary>Gets or sets the payment-processor charges summary. Maps to API field '<c>charges</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Charges)] public InvoiceCharges? Charges { get; set; }

    /// <summary>Gets or sets whether the invoice amount is gross. Maps to API field '<c>gross</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Gross)] public bool Gross { get; set; }

    /// <summary>Gets or sets the cancellation description. Maps to API field '<c>cancellationDescription</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CancellationDescription)] public string? CancellationDescription { get; set; }

    /// <summary>Gets or sets the template name. Maps to API field '<c>templateName</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TemplateName)] public string? TemplateName { get; set; }

    /// <summary>Gets or sets the reference number. Maps to API field '<c>refNumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RefNumber)] public string? RefNumber { get; set; }

    /// <summary>Gets or sets whether the invoice is a draft. Maps to API field '<c>isDraft</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsDraft)] public bool IsDraft { get; set; }

    /// <summary>Gets or sets whether this is a template. Maps to API field '<c>isTemplate</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsTemplate)] public bool IsTemplate { get; set; }

    /// <summary>Gets or sets the creation date for recurring invoices. Maps to API field '<c>creationDateForRecurringInvoices</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CreationDateForRecurringInvoices)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? CreationDateForRecurringInvoices { get; set; }

    /// <summary>Gets or sets the recurring-invoices interval in months. Maps to API field '<c>recurringInvoicesInterval</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RecurringInvoicesInterval)] public int? RecurringInvoicesInterval { get; set; }

    /// <summary>Gets or sets the payment information text. Maps to API field '<c>paymentInformation</c>'.</summary>
    [JsonPropertyName(InvoiceFields.PaymentInformation)] public string? PaymentInformation { get; set; }

    /// <summary>Gets or sets whether this is a request/expense report. Maps to API field '<c>isRequest</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsRequest)] public bool IsRequest { get; set; }

    /// <summary>Gets or sets the tax rate. Maps to API field '<c>taxRate</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TaxRate)] public decimal? TaxRate { get; set; }

    /// <summary>Gets or sets the tax name. Maps to API field '<c>taxName</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TaxName)] public string? TaxName { get; set; }

    /// <summary>Gets or sets the current call-state name. Maps to API field '<c>actualCallStateName</c>'.</summary>
    [JsonPropertyName(InvoiceFields.ActualCallStateName)] public string? ActualCallStateName { get; set; }

    /// <summary>Gets or sets the delay in days for the call state. Maps to API field '<c>callStateDelayDays</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CallStateDelayDays)] public int? CallStateDelayDays { get; set; }

    /// <summary>Gets or sets the account number. Maps to API field '<c>accnumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.AccountNumber)] public int? AccountNumber { get; set; }

    /// <summary>Gets or sets the globally unique identifier. Maps to API field '<c>guid</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Guid)] public string? Guid { get; set; }

    /// <summary>Gets or sets the selection account number. Maps to API field '<c>selectionAcc</c>'.</summary>
    [JsonPropertyName(InvoiceFields.SelectionAccount)] public int? SelectionAccount { get; set; }

    /// <summary>Gets or sets whether the file is removed on delete. Maps to API field '<c>removeFileOnDelete</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RemoveFileOnDelete)] public bool RemoveFileOnDelete { get; set; }

    /// <summary>Gets or sets the custom payment method ID. Maps to API field '<c>customPaymentMethod</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CustomPaymentMethod)] public int? CustomPaymentMethod { get; set; }

    /// <summary>Gets or sets whether this is a receipt. Maps to API field '<c>isReceipt</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsReceipt)] public bool IsReceipt { get; set; }

    /// <summary>Gets or sets whether tax rates are per invoice item. Maps to API field '<c>_isTaxRatePerInvoiceItem</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsTaxRatePerInvoiceItem)] public bool IsTaxRatePerInvoiceItem { get; set; }

    /// <summary>Gets or sets whether this invoice is subject to tax. Maps to API field '<c>_isSubjectToTax</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsSubjectToTax)] public bool IsSubjectToTax { get; set; }

    /// <summary>Gets or sets the invoice mode. Maps to API field '<c>mode</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Mode)] public string? Mode { get; set; }

    /// <summary>Gets or sets the offer status. Maps to API field '<c>offerStatus</c>'.</summary>
    [JsonPropertyName(InvoiceFields.OfferStatus)] public string? OfferStatus { get; set; }

    /// <summary>Gets or sets the date until which the offer is valid. Maps to API field '<c>offerValidUntil</c>'.</summary>
    [JsonPropertyName(InvoiceFields.OfferValidUntil)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? OfferValidUntil { get; set; }

    /// <summary>Gets or sets the offer number. Maps to API field '<c>offerNumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.OfferNumber)] public string? OfferNumber { get; set; }

    /// <summary>Gets or sets the URL reference of the related offer. Maps to API field '<c>relatedOffer</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RelatedOffer)] public string? RelatedOffer { get; set; }

    /// <summary>Gets the offer ID extracted from <see cref="RelatedOffer"/>.</summary>
    [JsonIgnore] public long? RelatedOfferId => UrlReference.ExtractId(RelatedOffer);

    /// <summary>Gets or sets the closing description text. Maps to API field '<c>closingDescription</c>'.</summary>
    [JsonPropertyName(InvoiceFields.ClosingDescription)] public string? ClosingDescription { get; set; }

    /// <summary>Gets or sets whether the address balance is used for payment. Maps to API field '<c>useAddressBalance</c>'.</summary>
    [JsonPropertyName(InvoiceFields.UseAddressBalance)] public bool UseAddressBalance { get; set; }

    /// <summary>Gets or sets the scheduled deletion date. Maps to API field '<c>_deleteAfterDate</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DeleteAfterDate)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? DeleteAfterDate { get; set; }

    /// <summary>Gets or sets the URL reference of the deleting user. Maps to API field '<c>_deletedBy</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DeletedBy)] public string? DeletedBy { get; set; }

    /// <summary>Gets the user ID extracted from <see cref="DeletedBy"/>.</summary>
    [JsonIgnore] public long? DeletedById => UrlReference.ExtractId(DeletedBy);
}
```

- [ ] **Step 4: Run the new test and the whole Domain suite — expect PASS**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests`
Expected: `InvoiceRealResponseTests` passes. `InvoiceEntityTests` **will fail** because its expectations still use the old `long` types. That's handled in Task 7.

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/Invoice.cs tests/MCP.EasyVerein.Domain.Tests/InvoiceRealResponseTests.cs
git commit -m "feat(domain): align Invoice entity with real easyVerein API v1.7 response"
```

---

## Task 7: Update the synthetic `InvoiceEntityTests`

**Files:**
- Modify: `tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs`

- [ ] **Step 1: Replace the test so the synthetic JSON and asserts match the new types**

Overwrite the file with:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "invNumber": "RE-2024-001",
                "totalPrice": "119.00",
                "date": "2024-01-15",
                "dateItHappend": "2024-02-15",
                "dateSent": "2024-01-16",
                "kind": "invoice",
                "description": "Jahresbeitrag",
                "receiver": "Max Mustermann",
                "relatedAddress": "https://easyverein.com/api/v1.7/contact-details/42",
                "relatedBookings": [
                    "https://easyverein.com/api/v1.7/booking/1",
                    "https://easyverein.com/api/v1.7/booking/2",
                    "https://easyverein.com/api/v1.7/booking/3"
                ],
                "payedFromUser": "https://easyverein.com/api/v1.7/user/7",
                "approvedFromAdmin": "https://easyverein.com/api/v1.7/user/3",
                "canceledInvoice": "https://easyverein.com/api/v1.7/invoice/5",
                "bankAccount": "https://easyverein.com/api/v1.7/bank-account/11",
                "org": "https://easyverein.com/api/v1.7/organization/30189",
                "path": "https://easyverein.com/app/file?category=invoice&path=2024/01/test.pdf",
                "invoiceItems": ["https://easyverein.com/api/v1.7/invoice-item/100"],
                "charges": { "charge": 1.0, "chargeBack": 0.5, "total": 119.00 },
                "tax": "19.00",
                "paymentDifference": "0.00",
                "gross": true,
                "cancellationDescription": "Storno wegen Fehler",
                "templateName": "Standard",
                "refNumber": "REF-001",
                "isDraft": false,
                "isTemplate": false,
                "creationDateForRecurringInvoices": "2024-01-01",
                "recurringInvoicesInterval": 12,
                "paymentInformation": "Bitte bis 15.02. überweisen",
                "isRequest": false,
                "taxRate": "19.00",
                "taxName": "MwSt.",
                "actualCallStateName": "Erste Mahnung",
                "callStateDelayDays": 14,
                "accnumber": 4711,
                "guid": "abc-123-def-456",
                "selectionAcc": 8,
                "removeFileOnDelete": true,
                "customPaymentMethod": 2,
                "isReceipt": false,
                "_isTaxRatePerInvoiceItem": true,
                "_isSubjectToTax": true,
                "mode": "default",
                "offerStatus": "accepted",
                "offerValidUntil": "2024-03-01",
                "offerNumber": "ANG-2024-001",
                "relatedOffer": "https://easyverein.com/api/v1.7/invoice/55",
                "closingDescription": "Mit freundlichen Grüßen",
                "useAddressBalance": false,
                "_deleteAfterDate": null,
                "_deletedBy": null
            }
            """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        var invoice = JsonSerializer.Deserialize<Invoice>(json, options);

        Assert.NotNull(invoice);
        Assert.Equal(99, invoice!.Id);
        Assert.Equal("RE-2024-001", invoice.InvoiceNumber);
        Assert.Equal(119.00m, invoice.TotalPrice);
        Assert.Equal(new DateTime(2024, 1, 15), invoice.Date);
        Assert.Equal(new DateTime(2024, 2, 15), invoice.DueDate);
        Assert.Equal(new DateTime(2024, 1, 16), invoice.DateSent);
        Assert.Equal("invoice", invoice.Kind);
        Assert.Equal("Jahresbeitrag", invoice.Description);
        Assert.Equal("Max Mustermann", invoice.Receiver);
        Assert.Equal("https://easyverein.com/api/v1.7/contact-details/42", invoice.RelatedAddress);
        Assert.Equal(42L, invoice.RelatedAddressId);
        Assert.NotNull(invoice.RelatedBookings);
        Assert.Equal(3, invoice.RelatedBookings!.Count);
        Assert.Equal("https://easyverein.com/api/v1.7/user/7", invoice.PayedFromUser);
        Assert.Equal(7L, invoice.PayedFromUserId);
        Assert.Equal(3L, invoice.ApprovedFromAdminId);
        Assert.Equal(5L, invoice.CanceledInvoiceId);
        Assert.Equal(11L, invoice.BankAccountId);
        Assert.Equal(30189L, invoice.OrgId);
        Assert.Equal("https://easyverein.com/app/file?category=invoice&path=2024/01/test.pdf", invoice.Path);
        Assert.Single(invoice.InvoiceItems!);
        Assert.NotNull(invoice.Charges);
        Assert.Equal(119.00m, invoice.Charges!.Total);
        Assert.Equal(19.00m, invoice.Tax);
        Assert.Equal(0m, invoice.PaymentDifference);
        Assert.True(invoice.Gross);
        Assert.Equal("Storno wegen Fehler", invoice.CancellationDescription);
        Assert.Equal("Standard", invoice.TemplateName);
        Assert.Equal("REF-001", invoice.RefNumber);
        Assert.False(invoice.IsDraft);
        Assert.False(invoice.IsTemplate);
        Assert.Equal(new DateTime(2024, 1, 1), invoice.CreationDateForRecurringInvoices);
        Assert.Equal(12, invoice.RecurringInvoicesInterval);
        Assert.Equal("Bitte bis 15.02. überweisen", invoice.PaymentInformation);
        Assert.False(invoice.IsRequest);
        Assert.Equal(19.00m, invoice.TaxRate);
        Assert.Equal("MwSt.", invoice.TaxName);
        Assert.Equal("Erste Mahnung", invoice.ActualCallStateName);
        Assert.Equal(14, invoice.CallStateDelayDays);
        Assert.Equal(4711, invoice.AccountNumber);
        Assert.Equal("abc-123-def-456", invoice.Guid);
        Assert.Equal(8, invoice.SelectionAccount);
        Assert.True(invoice.RemoveFileOnDelete);
        Assert.Equal(2, invoice.CustomPaymentMethod);
        Assert.False(invoice.IsReceipt);
        Assert.True(invoice.IsTaxRatePerInvoiceItem);
        Assert.True(invoice.IsSubjectToTax);
        Assert.Equal("default", invoice.Mode);
        Assert.Equal("accepted", invoice.OfferStatus);
        Assert.Equal(new DateTime(2024, 3, 1), invoice.OfferValidUntil);
        Assert.Equal("ANG-2024-001", invoice.OfferNumber);
        Assert.Equal(55L, invoice.RelatedOfferId);
        Assert.Equal("Mit freundlichen Grüßen", invoice.ClosingDescription);
        Assert.False(invoice.UseAddressBalance);
        Assert.Null(invoice.DeleteAfterDate);
        Assert.Null(invoice.DeletedBy);
    }
}
```

- [ ] **Step 2: Run the whole Domain test suite — expect PASS**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests`
Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs
git commit -m "test(domain): update Invoice entity tests for URL refs and new fields"
```

---

## Task 8: Update `InvoiceQuery` field selection

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs`

- [ ] **Step 1: Extend the `FieldQuery` string**

Replace the `FieldQuery` constant with one that also requests the new fields. Insert after `InvoiceFields.IsReceipt + "," +`:

```csharp
                InvoiceFields.Org + "," +
                InvoiceFields.Path + "," +
                InvoiceFields.InvoiceItems + "," +
                InvoiceFields.Charges + "," +
                InvoiceFields.Tax + "," +
                InvoiceFields.PaymentDifference + "," +
                InvoiceFields.CancelInvoice + "," +
                InvoiceFields.IsTaxRatePerInvoiceItem + "," +
                InvoiceFields.IsSubjectToTax + "," +
                InvoiceFields.Mode + "," +
                InvoiceFields.OfferStatus + "," +
                InvoiceFields.OfferValidUntil + "," +
                InvoiceFields.OfferNumber + "," +
                InvoiceFields.RelatedOffer + "," +
                InvoiceFields.ClosingDescription + "," +
                InvoiceFields.UseAddressBalance +
```

Remove the dangling `+` on the previous `InvoiceFields.IsReceipt` line (make it end with a comma, not a closing brace). The final `}` stays after the new last field `UseAddressBalance`.

- [ ] **Step 2: Build infra to verify**

Run: `dotnet build src/MCP.EasyVerein.Infrastructure`
Expected: succeeds.

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs
git commit -m "feat(infra): include new Invoice fields in field-selection query"
```

---

## Task 9: Sync Infrastructure tests for Invoice list/get

**Files:**
- Modify (or create if absent): `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientInvoiceTests.cs`

- [ ] **Step 1: Locate and read the current Invoice-related infra tests**

Run: `grep -rln --include=*.cs "Invoice" tests/MCP.EasyVerein.Infrastructure.Tests/`

If tests mock a numeric `relatedAddress` (e.g. `"relatedAddress": 42`), replace with the URL form: `"relatedAddress": "https://easyverein.com/api/v1.7/contact-details/42"`. Similarly for `relatedBookings`, `payedFromUser`, `approvedFromAdmin`, `canceledInvoice`, `bankAccount`, `relatedOffer`.

- [ ] **Step 2: Assert on the helper `*Id` properties where the test previously asserted `RelatedAddress == 42L`**

Example rewrite: `Assert.Equal(42L, result.RelatedAddress);` → `Assert.Equal(42L, result.RelatedAddressId);`

- [ ] **Step 3: Run the Infrastructure test suite — expect PASS**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests`
Expected: all pass.

- [ ] **Step 4: Commit**

```bash
git add tests/MCP.EasyVerein.Infrastructure.Tests
git commit -m "test(infra): update Invoice client tests for URL-form references"
```

---

## Task 10: Full test run + manual MCP sanity check

- [ ] **Step 1: Run the whole test suite**

Run: `dotnet test`
Expected: all pass. If any Server tests fail because they call the Invoice entity with old types, fix them the same way (reference `*Id` helper, URL form in JSON).

- [ ] **Step 2: Manual sanity check against the live API**

Reconnect the MCP server in the running Claude session (`/mcp`) and call `mcp__easyverein__list_invoices`. Expected: no `JsonException`, receives a list of invoices.

- [ ] **Step 3: Commit (only if Step 1 required follow-ups)**

```bash
git add <any-touched-files>
git commit -m "test: align Server Invoice tests with new entity shape"
```

- [ ] **Step 4: Open PR**

```bash
gh pr create --title "Align Invoice entity with easyVerein API v1.7 response" --body "..."
```

PR body should list:
- What was broken (JsonException on `relatedAddress`)
- What now works (list/get invoices against the live API)
- Follow-ups (same URL-reference pattern likely applies to Booking, ContactDetails, etc. — track as new issue)

---

## Self-Review (performed)

1. **Spec coverage:** The user asked for "full alignment". Covered: URL-form refs, missing fields, date-only parsing, charges object, ID-helper properties, field-selection query, test suite. Not covered in this plan (intentionally, to keep scope bounded): applying the same pattern to other entities (Booking, ContactDetails, Event, Member sub-refs) — noted as follow-up.
2. **Placeholder scan:** No `TODO`/`TBD`. All code blocks present. `gh pr create` body is left to the engineer at PR time, which is fine since it is summarising the actual diff.
3. **Type consistency:** `UrlReference.ExtractId` signature `(string?) → long?` is used consistently. Property names (`RelatedAddressId`, `PayedFromUserId`, …) are consistent between Task 6 definition and Task 7 asserts.
