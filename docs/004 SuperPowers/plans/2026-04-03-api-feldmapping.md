# US-0008 API-Feldmapping Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Alle Domain-Entities an die tatsächliche easyVerein API-Struktur anpassen, Query-Parameter und Pagination implementieren.

**Architecture:** Direct Mapping – Domain-Entities bilden API-Ressourcen 1:1 ab mit `[JsonPropertyName]`-Attributen. Kein DTO-Layer. Pagination wird zentral über `next`-Link gelöst. Member-Erstellung ist zweistufig (ContactDetails → Member).

**Tech Stack:** C# / .NET 8, System.Text.Json, ModelContextProtocol.Server, xUnit

**Spec:** `docs/004 SuperPowers/specs/2026-04-03-api-feldmapping-design.md`

---

## File Structure

| Action | File | Verantwortung |
|--------|------|---------------|
| Rewrite | `src/MCP.EasyVerein.Domain/Entities/Member.cs` | Member-Entity mit API-Feldnamen |
| Create | `src/MCP.EasyVerein.Domain/Entities/ContactDetails.cs` | ContactDetails-Entity (ersetzt Contact.cs) |
| Delete | `src/MCP.EasyVerein.Domain/Entities/Contact.cs` | Wird durch ContactDetails.cs ersetzt |
| Rewrite | `src/MCP.EasyVerein.Domain/Entities/Invoice.cs` | Invoice-Entity mit API-Feldnamen |
| Rewrite | `src/MCP.EasyVerein.Domain/Entities/Event.cs` | Event-Entity mit API-Feldnamen |
| Rewrite | `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` | Interface mit ContactDetails + neuem CreateMember |
| Create | `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` | Query-Konstanten pro Entity |
| Rewrite | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` | Query-Parameter, Pagination, zweistufiges Create |
| Rewrite | `src/MCP.EasyVerein.Server/Tools/MemberTools.cs` | Angepasste Parameter |
| Create | `src/MCP.EasyVerein.Server/Tools/ContactDetailsTools.cs` | Ersetzt ContactTools.cs |
| Delete | `src/MCP.EasyVerein.Server/Tools/ContactTools.cs` | Wird durch ContactDetailsTools.cs ersetzt |
| Rewrite | `src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs` | Angepasste Feldnamen |
| Rewrite | `src/MCP.EasyVerein.Server/Tools/EventTools.cs` | Angepasste Feldnamen |
| Modify | `src/MCP.EasyVerein.Server/Program.cs:58` | ContactTools → ContactDetailsTools |
| Rewrite | `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs` | Tests für IsActive, FullName |
| Create | `tests/MCP.EasyVerein.Domain.Tests/ContactDetailsEntityTests.cs` | Tests für FullName |
| Rewrite | `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` | Query-Parameter, Auth, Pagination, JSON-Mapping |

---

### Task 1: ContactDetails Entity

**Files:**
- Create: `src/MCP.EasyVerein.Domain/Entities/ContactDetails.cs`
- Test: `tests/MCP.EasyVerein.Domain.Tests/ContactDetailsEntityTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// tests/MCP.EasyVerein.Domain.Tests/ContactDetailsEntityTests.cs
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class ContactDetailsEntityTests
{
    [Fact]
    public void FullName_ReturnsCombinedName()
    {
        var cd = new ContactDetails
        {
            FirstName = "Max",
            FamilyName = "Mustermann"
        };

        Assert.Equal("Max Mustermann", cd.FullName);
    }

    [Fact]
    public void FullName_TrimsWhitespace_WhenFirstNameEmpty()
    {
        var cd = new ContactDetails { FamilyName = "Mustermann" };

        Assert.Equal("Mustermann", cd.FullName);
    }

    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """{"id":1,"firstName":"Anna","familyName":"Schmidt","zip":"10115"}""";
        var cd = System.Text.Json.JsonSerializer.Deserialize<ContactDetails>(json);

        Assert.NotNull(cd);
        Assert.Equal(1, cd!.Id);
        Assert.Equal("Anna", cd.FirstName);
        Assert.Equal("Schmidt", cd.FamilyName);
        Assert.Equal("10115", cd.Zip);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "ContactDetailsEntityTests" --no-restore`
Expected: FAIL – `ContactDetails` does not exist

- [ ] **Step 3: Write ContactDetails entity**

```csharp
// src/MCP.EasyVerein.Domain/Entities/ContactDetails.cs
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class ContactDetails
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("familyName")]
    public string FamilyName { get; set; } = string.Empty;

    [JsonPropertyName("salutation")]
    public string? Salutation { get; set; }

    [JsonPropertyName("nameAffix")]
    public string? NameAffix { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }

    [JsonPropertyName("privateEmail")]
    public string? PrivateEmail { get; set; }

    [JsonPropertyName("companyEmail")]
    public string? CompanyEmail { get; set; }

    [JsonPropertyName("primaryEmail")]
    public string? PrimaryEmail { get; set; }

    [JsonPropertyName("_preferredEmailField")]
    public int? PreferredEmailField { get; set; }

    [JsonPropertyName("preferredCommunicationWay")]
    public int? PreferredCommunicationWay { get; set; }

    [JsonPropertyName("privatePhone")]
    public string? PrivatePhone { get; set; }

    [JsonPropertyName("companyPhone")]
    public string? CompanyPhone { get; set; }

    [JsonPropertyName("mobilePhone")]
    public string? MobilePhone { get; set; }

    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("addressSuffix")]
    public string? AddressSuffix { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("_isCompany")]
    public bool IsCompany { get; set; }

    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonPropertyName("companyStreet")]
    public string? CompanyStreet { get; set; }

    [JsonPropertyName("companyCity")]
    public string? CompanyCity { get; set; }

    [JsonPropertyName("companyState")]
    public string? CompanyState { get; set; }

    [JsonPropertyName("companyZip")]
    public string? CompanyZip { get; set; }

    [JsonPropertyName("companyCountry")]
    public string? CompanyCountry { get; set; }

    [JsonPropertyName("companyAddressSuffix")]
    public string? CompanyAddressSuffix { get; set; }

    [JsonPropertyName("companyNameInvoice")]
    public string? CompanyNameInvoice { get; set; }

    [JsonPropertyName("companyStreetInvoice")]
    public string? CompanyStreetInvoice { get; set; }

    [JsonPropertyName("companyCityInvoice")]
    public string? CompanyCityInvoice { get; set; }

    [JsonPropertyName("companyStateInvoice")]
    public string? CompanyStateInvoice { get; set; }

    [JsonPropertyName("companyZipInvoice")]
    public string? CompanyZipInvoice { get; set; }

    [JsonPropertyName("companyCountryInvoice")]
    public string? CompanyCountryInvoice { get; set; }

    [JsonPropertyName("companyAddressSuffixInvoice")]
    public string? CompanyAddressSuffixInvoice { get; set; }

    [JsonPropertyName("companyPhoneInvoice")]
    public string? CompanyPhoneInvoice { get; set; }

    [JsonPropertyName("companyEmailInvoice")]
    public string? CompanyEmailInvoice { get; set; }

    [JsonPropertyName("professionalRole")]
    public string? ProfessionalRole { get; set; }

    [JsonPropertyName("balance")]
    public decimal? Balance { get; set; }

    [JsonPropertyName("iban")]
    public string? Iban { get; set; }

    [JsonPropertyName("bic")]
    public string? Bic { get; set; }

    [JsonPropertyName("bankAccountOwner")]
    public string? BankAccountOwner { get; set; }

    [JsonPropertyName("sepaMandate")]
    public string? SepaMandate { get; set; }

    [JsonPropertyName("sepaDate")]
    public DateTime? SepaDate { get; set; }

    [JsonPropertyName("methodOfPayment")]
    public int? MethodOfPayment { get; set; }

    [JsonPropertyName("datevAccountNumber")]
    public int? DatevAccountNumber { get; set; }

    [JsonPropertyName("internalNote")]
    public string? InternalNote { get; set; }

    [JsonPropertyName("invoiceCompany")]
    public bool InvoiceCompany { get; set; }

    [JsonPropertyName("sendInvoiceCompanyMail")]
    public bool SendInvoiceCompanyMail { get; set; }

    [JsonPropertyName("addressCompany")]
    public bool AddressCompany { get; set; }

    [JsonPropertyName("customPaymentMethod")]
    public int? CustomPaymentMethod { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName} {FamilyName}".Trim();
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "ContactDetailsEntityTests" --no-restore`
Expected: PASS (3 tests)

- [ ] **Step 5: Delete old Contact.cs**

```bash
git rm src/MCP.EasyVerein.Domain/Entities/Contact.cs
```

- [ ] **Step 6: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/ContactDetails.cs tests/MCP.EasyVerein.Domain.Tests/ContactDetailsEntityTests.cs
git commit -m "feat: ContactDetails Entity mit korrekten API-Feldnamen (US-0008)"
```

---

### Task 2: Member Entity

**Files:**
- Rewrite: `src/MCP.EasyVerein.Domain/Entities/Member.cs`
- Rewrite: `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs`

- [ ] **Step 1: Write the failing tests**

```csharp
// tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberEntityTests
{
    [Fact]
    public void IsActive_True_WhenNoResignationAndNotBlocked()
    {
        var member = new Member { EmailOrUserName = "test@test.de" };
        Assert.True(member.IsActive);
    }

    [Fact]
    public void IsActive_False_WhenResignationDateSet()
    {
        var member = new Member
        {
            EmailOrUserName = "test@test.de",
            ResignationDate = DateTime.Now
        };
        Assert.False(member.IsActive);
    }

    [Fact]
    public void IsActive_False_WhenBlocked()
    {
        var member = new Member
        {
            EmailOrUserName = "test@test.de",
            IsBlocked = true
        };
        Assert.False(member.IsActive);
    }

    [Fact]
    public void FullName_UsesContactDetails_WhenPresent()
    {
        var member = new Member
        {
            EmailOrUserName = "test@test.de",
            ContactDetails = new ContactDetails
            {
                FirstName = "Max",
                FamilyName = "Mustermann"
            }
        };
        Assert.Equal("Max Mustermann", member.FullName);
    }

    [Fact]
    public void FullName_FallsBackToEmail_WhenNoContactDetails()
    {
        var member = new Member { EmailOrUserName = "test@test.de" };
        Assert.Equal("test@test.de", member.FullName);
    }

    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
        {
            "id": 42,
            "emailOrUserName": "max@verein.de",
            "membershipNumber": "M-001",
            "joinDate": "2024-09-19T12:00:00+02:00",
            "_isBlocked": false,
            "_isApplication": false,
            "contactDetails": {
                "id": 99,
                "firstName": "Max",
                "familyName": "Mustermann"
            }
        }
        """;
        var member = System.Text.Json.JsonSerializer.Deserialize<Member>(json);

        Assert.NotNull(member);
        Assert.Equal(42, member!.Id);
        Assert.Equal("max@verein.de", member.EmailOrUserName);
        Assert.Equal("M-001", member.MembershipNumber);
        Assert.NotNull(member.ContactDetails);
        Assert.Equal("Max", member.ContactDetails!.FirstName);
        Assert.Equal("Mustermann", member.ContactDetails.FamilyName);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "MemberEntityTests" --no-restore`
Expected: FAIL – `EmailOrUserName` property does not exist

- [ ] **Step 3: Rewrite Member entity**

```csharp
// src/MCP.EasyVerein.Domain/Entities/Member.cs
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class Member
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("emailOrUserName")]
    public string EmailOrUserName { get; set; } = string.Empty;

    [JsonPropertyName("contactDetails")]
    public ContactDetails? ContactDetails { get; set; }

    [JsonPropertyName("membershipNumber")]
    public string? MembershipNumber { get; set; }

    [JsonPropertyName("joinDate")]
    public DateTime? JoinDate { get; set; }

    [JsonPropertyName("resignationDate")]
    public DateTime? ResignationDate { get; set; }

    [JsonPropertyName("resignationNoticeDate")]
    public DateTime? ResignationNoticeDate { get; set; }

    [JsonPropertyName("paymentAmount")]
    public decimal? PaymentAmount { get; set; }

    [JsonPropertyName("paymentIntervallMonths")]
    public int? PaymentIntervalMonths { get; set; }

    [JsonPropertyName("_paymentStartDate")]
    public DateTime? PaymentStartDate { get; set; }

    [JsonPropertyName("_isApplication")]
    public bool IsApplication { get; set; }

    [JsonPropertyName("_applicationDate")]
    public DateTime? ApplicationDate { get; set; }

    [JsonPropertyName("_applicationWasAcceptedAt")]
    public DateTime? ApplicationWasAcceptedAt { get; set; }

    [JsonPropertyName("_isChairman")]
    public bool IsChairman { get; set; }

    [JsonPropertyName("_chairmanPermissionGroup")]
    public int? ChairmanPermissionGroup { get; set; }

    [JsonPropertyName("_isBlocked")]
    public bool IsBlocked { get; set; }

    [JsonPropertyName("blockReason")]
    public string? BlockReason { get; set; }

    [JsonPropertyName("_profilePicture")]
    public string? ProfilePicture { get; set; }

    [JsonPropertyName("_relatedMember")]
    public long? RelatedMember { get; set; }

    [JsonPropertyName("_editableByRelatedMembers")]
    public bool EditableByRelatedMembers { get; set; }

    [JsonPropertyName("useBalanceForMembershipFee")]
    public bool UseBalanceForMembershipFee { get; set; }

    [JsonPropertyName("bulletinBoardNewPostNotification")]
    public bool BulletinBoardNewPostNotification { get; set; }

    [JsonPropertyName("declarationOfApplication")]
    public string? DeclarationOfApplication { get; set; }

    [JsonPropertyName("sepaMandateFile")]
    public string? SepaMandateFile { get; set; }

    [JsonPropertyName("requirePasswordChange")]
    public bool RequirePasswordChange { get; set; }

    [JsonPropertyName("_isMatrixSearchable")]
    public bool IsMatrixSearchable { get; set; }

    [JsonPropertyName("matrixBlockReason")]
    public string? MatrixBlockReason { get; set; }

    [JsonPropertyName("blockedFromMatrix")]
    public bool BlockedFromMatrix { get; set; }

    [JsonPropertyName("_matrixCommunicationPermission")]
    public int? MatrixCommunicationPermission { get; set; }

    [JsonPropertyName("useMatrixGroupSettings")]
    public bool UseMatrixGroupSettings { get; set; }

    [JsonPropertyName("showWarningsAndNotesToAdminsInProfile")]
    public bool ShowWarningsAndNotesToAdminsInProfile { get; set; }

    [JsonPropertyName("signatureText")]
    public string? SignatureText { get; set; }

    [JsonIgnore]
    public bool IsActive => ResignationDate == null && !IsBlocked;

    [JsonIgnore]
    public string FullName => ContactDetails != null
        ? $"{ContactDetails.FirstName} {ContactDetails.FamilyName}"
        : EmailOrUserName;
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "MemberEntityTests" --no-restore`
Expected: PASS (6 tests)

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/Member.cs tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs
git commit -m "feat: Member Entity mit korrekten API-Feldnamen und ContactDetails-Referenz (US-0008)"
```

---

### Task 3: Invoice Entity

**Files:**
- Rewrite: `src/MCP.EasyVerein.Domain/Entities/Invoice.cs`
- Create: `tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class InvoiceEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
        {
            "id": 1,
            "invNumber": "R-2026-001",
            "totalPrice": 120.50,
            "date": "2026-01-15",
            "dateItHappend": "2026-02-15",
            "kind": "Membership",
            "isDraft": false,
            "relatedAddress": 42
        }
        """;
        var invoice = System.Text.Json.JsonSerializer.Deserialize<Invoice>(json);

        Assert.NotNull(invoice);
        Assert.Equal(1, invoice!.Id);
        Assert.Equal("R-2026-001", invoice.InvoiceNumber);
        Assert.Equal(120.50m, invoice.TotalPrice);
        Assert.Equal("Membership", invoice.Kind);
        Assert.Equal(42, invoice.RelatedAddress);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "InvoiceEntityTests" --no-restore`
Expected: FAIL – `InvoiceNumber` not found / wrong type

- [ ] **Step 3: Rewrite Invoice entity**

```csharp
// src/MCP.EasyVerein.Domain/Entities/Invoice.cs
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class Invoice
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("invNumber")]
    public string? InvoiceNumber { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal? TotalPrice { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("dateItHappend")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("dateSent")]
    public DateTime? DateSent { get; set; }

    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("receiver")]
    public string? Receiver { get; set; }

    [JsonPropertyName("relatedAddress")]
    public long? RelatedAddress { get; set; }

    [JsonPropertyName("relatedBookings")]
    public List<long>? RelatedBookings { get; set; }

    [JsonPropertyName("payedFromUser")]
    public long? PayedFromUser { get; set; }

    [JsonPropertyName("approvedFromAdmin")]
    public long? ApprovedFromAdmin { get; set; }

    [JsonPropertyName("canceledInvoice")]
    public long? CanceledInvoice { get; set; }

    [JsonPropertyName("bankAccount")]
    public long? BankAccount { get; set; }

    [JsonPropertyName("gross")]
    public bool Gross { get; set; }

    [JsonPropertyName("cancellationDescription")]
    public string? CancellationDescription { get; set; }

    [JsonPropertyName("templateName")]
    public string? TemplateName { get; set; }

    [JsonPropertyName("refNumber")]
    public string? RefNumber { get; set; }

    [JsonPropertyName("isDraft")]
    public bool IsDraft { get; set; }

    [JsonPropertyName("isTemplate")]
    public bool IsTemplate { get; set; }

    [JsonPropertyName("creationDateForRecurringInvoices")]
    public DateTime? CreationDateForRecurringInvoices { get; set; }

    [JsonPropertyName("recurringInvoicesInterval")]
    public int? RecurringInvoicesInterval { get; set; }

    [JsonPropertyName("paymentInformation")]
    public string? PaymentInformation { get; set; }

    [JsonPropertyName("isRequest")]
    public bool IsRequest { get; set; }

    [JsonPropertyName("taxRate")]
    public decimal? TaxRate { get; set; }

    [JsonPropertyName("taxName")]
    public string? TaxName { get; set; }

    [JsonPropertyName("actualCallStateName")]
    public string? ActualCallStateName { get; set; }

    [JsonPropertyName("callStateDelayDays")]
    public int? CallStateDelayDays { get; set; }

    [JsonPropertyName("accnumber")]
    public int? AccountNumber { get; set; }

    [JsonPropertyName("guid")]
    public string? Guid { get; set; }

    [JsonPropertyName("selectionAcc")]
    public int? SelectionAccount { get; set; }

    [JsonPropertyName("removeFileOnDelete")]
    public bool RemoveFileOnDelete { get; set; }

    [JsonPropertyName("customPaymentMethod")]
    public int? CustomPaymentMethod { get; set; }

    [JsonPropertyName("isReceipt")]
    public bool IsReceipt { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("offerStatus")]
    public string? OfferStatus { get; set; }

    [JsonPropertyName("offerValidUntil")]
    public DateTime? OfferValidUntil { get; set; }

    [JsonPropertyName("offerNumber")]
    public string? OfferNumber { get; set; }

    [JsonPropertyName("relatedOffer")]
    public long? RelatedOffer { get; set; }

    [JsonPropertyName("closingDescription")]
    public string? ClosingDescription { get; set; }

    [JsonPropertyName("useAddressBalance")]
    public bool UseAddressBalance { get; set; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "InvoiceEntityTests" --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/Invoice.cs tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs
git commit -m "feat: Invoice Entity mit korrekten API-Feldnamen (US-0008)"
```

---

### Task 4: Event Entity

**Files:**
- Rewrite: `src/MCP.EasyVerein.Domain/Entities/Event.cs`
- Create: `tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class EventEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
        {
            "id": 1,
            "name": "Jahresversammlung",
            "start": "2026-06-15T10:00:00",
            "end": "2026-06-15T18:00:00",
            "locationName": "Vereinsheim",
            "maxParticipators": 50,
            "isPublic": true,
            "canceled": false
        }
        """;
        var ev = System.Text.Json.JsonSerializer.Deserialize<Event>(json);

        Assert.NotNull(ev);
        Assert.Equal(1, ev!.Id);
        Assert.Equal("Jahresversammlung", ev.Name);
        Assert.Equal("Vereinsheim", ev.LocationName);
        Assert.Equal(50, ev.MaxParticipants);
        Assert.True(ev.IsPublic);
        Assert.False(ev.Canceled);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "EventEntityTests" --no-restore`
Expected: FAIL – `LocationName`, `MaxParticipants` not found

- [ ] **Step 3: Rewrite Event entity**

```csharp
// src/MCP.EasyVerein.Domain/Entities/Event.cs
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class Event
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("prologue")]
    public string? Prologue { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("start")]
    public DateTime? Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime? End { get; set; }

    [JsonPropertyName("allDay")]
    public bool AllDay { get; set; }

    [JsonPropertyName("locationName")]
    public string? LocationName { get; set; }

    [JsonPropertyName("locationObject")]
    public long? LocationObject { get; set; }

    [JsonPropertyName("parent")]
    public long? Parent { get; set; }

    [JsonPropertyName("minParticipators")]
    public int? MinParticipants { get; set; }

    [JsonPropertyName("maxParticipators")]
    public int? MaxParticipants { get; set; }

    [JsonPropertyName("startParticipation")]
    public DateTime? StartParticipation { get; set; }

    [JsonPropertyName("endParticipation")]
    public DateTime? EndParticipation { get; set; }

    [JsonPropertyName("access")]
    public int? Access { get; set; }

    [JsonPropertyName("weekdays")]
    public string? Weekdays { get; set; }

    [JsonPropertyName("sendMailCheck")]
    public bool SendMailCheck { get; set; }

    [JsonPropertyName("showMemberarea")]
    public bool ShowMemberArea { get; set; }

    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("massParticipations")]
    public bool MassParticipations { get; set; }

    [JsonPropertyName("canceled")]
    public bool Canceled { get; set; }

    [JsonPropertyName("isReservation")]
    public bool IsReservation { get; set; }

    [JsonPropertyName("creator")]
    public long? Creator { get; set; }

    [JsonPropertyName("reservationParentEvent")]
    public long? ReservationParentEvent { get; set; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/MCP.EasyVerein.Domain.Tests/ --filter "EventEntityTests" --no-restore`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/Event.cs tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs
git commit -m "feat: Event Entity mit korrekten API-Feldnamen (US-0008)"
```

---

### Task 5: Interface und ApiQueries

**Files:**
- Rewrite: `src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs`
- Create: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Rewrite IEasyVereinApiClient**

```csharp
// src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Interfaces;

public interface IEasyVereinApiClient
{
    // Mitglieder
    Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken ct = default);
    Task<Member?> GetMemberAsync(long id, CancellationToken ct = default);
    Task<Member> CreateMemberAsync(string emailOrUserName,
        ContactDetails contactDetails, CancellationToken ct = default);
    Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default);
    Task DeleteMemberAsync(long id, CancellationToken ct = default);

    // Kontaktdaten
    Task<IReadOnlyList<ContactDetails>> GetContactDetailsAsync(CancellationToken ct = default);
    Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default);
    Task<ContactDetails> CreateContactDetailsAsync(ContactDetails contact, CancellationToken ct = default);
    Task<ContactDetails> UpdateContactDetailsAsync(long id, ContactDetails contact, CancellationToken ct = default);
    Task DeleteContactDetailsAsync(long id, CancellationToken ct = default);

    // Rechnungen
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default);
    Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default);
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default);
    Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default);
    Task DeleteInvoiceAsync(long id, CancellationToken ct = default);

    // Veranstaltungen
    Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default);
    Task<Event?> GetEventAsync(long id, CancellationToken ct = default);
    Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default);
    Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default);
    Task DeleteEventAsync(long id, CancellationToken ct = default);
}
```

- [ ] **Step 2: Create ApiQueries**

```csharp
// src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
namespace MCP.EasyVerein.Infrastructure.ApiClient;

internal static class ApiQueries
{
    public const string Member =
        "{id,emailOrUserName,membershipNumber,joinDate,resignationDate," +
        "resignationNoticeDate,paymentAmount,paymentIntervallMonths," +
        "_paymentStartDate,_isApplication,_applicationDate," +
        "_applicationWasAcceptedAt,_isChairman,_chairmanPermissionGroup," +
        "_isBlocked,blockReason,_profilePicture,_relatedMember," +
        "_editableByRelatedMembers,useBalanceForMembershipFee," +
        "bulletinBoardNewPostNotification,declarationOfApplication," +
        "sepaMandateFile,requirePasswordChange,_isMatrixSearchable," +
        "matrixBlockReason,blockedFromMatrix,_matrixCommunicationPermission," +
        "useMatrixGroupSettings,showWarningsAndNotesToAdminsInProfile," +
        "signatureText," +
        "contactDetails{id,firstName,familyName,salutation,nameAffix," +
        "dateOfBirth,privateEmail,companyEmail,primaryEmail," +
        "_preferredEmailField,preferredCommunicationWay," +
        "privatePhone,companyPhone,mobilePhone," +
        "street,addressSuffix,city,state,zip,country," +
        "_isCompany,companyName,companyStreet,companyCity,companyState," +
        "companyZip,companyCountry,companyAddressSuffix," +
        "companyNameInvoice,companyStreetInvoice,companyCityInvoice," +
        "companyStateInvoice,companyZipInvoice,companyCountryInvoice," +
        "companyAddressSuffixInvoice,companyPhoneInvoice,companyEmailInvoice," +
        "professionalRole,balance,iban,bic,bankAccountOwner," +
        "sepaMandate,sepaDate,methodOfPayment,datevAccountNumber," +
        "internalNote,invoiceCompany,sendInvoiceCompanyMail," +
        "addressCompany,customPaymentMethod}}";

    public const string ContactDetails =
        "{id,firstName,familyName,salutation,nameAffix,dateOfBirth," +
        "privateEmail,companyEmail,primaryEmail,_preferredEmailField," +
        "preferredCommunicationWay,privatePhone,companyPhone,mobilePhone," +
        "street,addressSuffix,city,state,zip,country," +
        "_isCompany,companyName,companyStreet,companyCity,companyState," +
        "companyZip,companyCountry,companyAddressSuffix," +
        "companyNameInvoice,companyStreetInvoice,companyCityInvoice," +
        "companyStateInvoice,companyZipInvoice,companyCountryInvoice," +
        "companyAddressSuffixInvoice,companyPhoneInvoice,companyEmailInvoice," +
        "professionalRole,balance,iban,bic,bankAccountOwner," +
        "sepaMandate,sepaDate,methodOfPayment,datevAccountNumber," +
        "internalNote,invoiceCompany,sendInvoiceCompanyMail," +
        "addressCompany,customPaymentMethod}";

    public const string Invoice =
        "{id,invNumber,totalPrice,date,dateItHappend,dateSent,kind," +
        "description,receiver,relatedAddress,relatedBookings," +
        "payedFromUser,approvedFromAdmin,canceledInvoice,bankAccount," +
        "gross,cancellationDescription,templateName,refNumber," +
        "isDraft,isTemplate,creationDateForRecurringInvoices," +
        "recurringInvoicesInterval,paymentInformation,isRequest," +
        "taxRate,taxName,actualCallStateName,callStateDelayDays," +
        "accnumber,guid,selectionAcc,removeFileOnDelete," +
        "customPaymentMethod,isReceipt,mode,offerStatus," +
        "offerValidUntil,offerNumber,relatedOffer," +
        "closingDescription,useAddressBalance}";

    public const string Event =
        "{id,name,description,prologue,note,start,end,allDay," +
        "locationName,locationObject,parent," +
        "minParticipators,maxParticipators," +
        "startParticipation,endParticipation,access,weekdays," +
        "sendMailCheck,showMemberarea,isPublic,massParticipations," +
        "canceled,isReservation,creator,reservationParentEvent}";
}
```

- [ ] **Step 3: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs \
        src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
git commit -m "feat: Interface mit ContactDetails und ApiQueries-Konstanten (US-0008)"
```

---

### Task 6: API Client – Pagination und Query-Parameter

**Files:**
- Rewrite: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`
- Rewrite: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

- [ ] **Step 1: Write pagination test**

```csharp
// Zu tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs hinzufügen

[Fact]
public async Task GetMembers_FollowsPagination_ReturnsAllPages()
{
    var page1 = new
    {
        results = new[] { new { id = 1L, emailOrUserName = "a@test.de" } },
        next = "https://easyverein.com/api/v1.7/member?page=2"
    };
    var page2 = new
    {
        results = new[] { new { id = 2L, emailOrUserName = "b@test.de" } },
        next = (string?)null
    };
    var handler = new MultiPageFakeHttpHandler(new[]
    {
        (HttpStatusCode.OK, JsonSerializer.Serialize(page1)),
        (HttpStatusCode.OK, JsonSerializer.Serialize(page2))
    });

    var client = CreateClient(handler);
    var result = await client.GetMembersAsync();

    Assert.Equal(2, result.Count);
    Assert.Equal(1, result[0].Id);
    Assert.Equal(2, result[1].Id);
}
```

Dazu `MultiPageFakeHttpHandler` am Ende der Testdatei:

```csharp
public class MultiPageFakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<(HttpStatusCode Status, string Content)> _responses;

    public MultiPageFakeHttpHandler(IEnumerable<(HttpStatusCode, string)> responses)
    {
        _responses = new Queue<(HttpStatusCode, string)>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (status, content) = _responses.Dequeue();
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = status,
            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}
```

- [ ] **Step 2: Write query-parameter test**

```csharp
[Fact]
public async Task GetMembers_SendsQueryParameter()
{
    string? capturedUrl = null;
    var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK,
        JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null }),
        url => capturedUrl = url);

    var client = CreateClient(handler);
    await client.GetMembersAsync();

    Assert.NotNull(capturedUrl);
    Assert.Contains("query=", capturedUrl);
    Assert.Contains("limit=100", capturedUrl);
}
```

Dazu `CapturingFakeHttpHandler`:

```csharp
public class CapturingFakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;
    private readonly Action<string> _captureUrl;

    public CapturingFakeHttpHandler(HttpStatusCode statusCode, string content, Action<string> captureUrl)
    {
        _statusCode = statusCode;
        _content = content;
        _captureUrl = captureUrl;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _captureUrl(request.RequestUri?.ToString() ?? "");
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}
```

- [ ] **Step 3: Write auth-header test (fix Token → Bearer)**

```csharp
[Fact]
public void Constructor_SetsAuthorizationHeader()
{
    var handler = new FakeHttpHandler(HttpStatusCode.OK, "{}");
    var httpClient = new HttpClient(handler);
    var config = new EasyVereinConfiguration { ApiKey = "my-secret-token" };

    _ = new EasyVereinApiClient(httpClient, config);

    Assert.Contains(httpClient.DefaultRequestHeaders.GetValues("Authorization"),
        v => v == "Bearer my-secret-token");
}
```

- [ ] **Step 4: Run tests to verify they fail**

Run: `dotnet test tests/MCP.EasyVerein.Infrastructure.Tests/ --no-restore`
Expected: FAIL – compilation errors (Contact → ContactDetails, pagination method missing)

- [ ] **Step 5: Rewrite EasyVereinApiClient**

Komplette Datei `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`:

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

public class EasyVereinApiClient : IEasyVereinApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EasyVereinConfiguration _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public EasyVereinApiClient(HttpClient httpClient, EasyVereinConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");
    }

    private string BuildUrl(string resource, string? apiVersionOverride = null)
    {
        return $"{_config.GetVersionedBaseUrl(apiVersionOverride)}/{resource}";
    }

    private string BuildListUrl(string resource, string query)
    {
        return $"{BuildUrl(resource)}?query={query}&limit=100";
    }

    private string BuildGetUrl(string resource, string query)
    {
        return $"{BuildUrl(resource)}?query={query}";
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException(
                $"Authentifizierung fehlgeschlagen (HTTP {(int)response.StatusCode}). " +
                "Bitte prüfen Sie Ihren API-Token.");
        }

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);

        // easyVerein wraps single results in { "results": [...] } too
        try
        {
            var listResponse = JsonSerializer.Deserialize<ApiListResponse<T>>(content, _jsonOptions);
            if (listResponse?.Results != null && listResponse.Results.Any())
                return listResponse.Results.First();
        }
        catch (JsonException) { }

        var single = JsonSerializer.Deserialize<T>(content, _jsonOptions);
        return single ?? throw new InvalidOperationException("Leere API-Antwort.");
    }

    private async Task<IReadOnlyList<T>> HandleListResponseWithPagination<T>(
        string initialUrl, CancellationToken ct)
    {
        var allResults = new List<T>();
        string? url = initialUrl;

        while (url != null)
        {
            var response = await SendWithErrorHandling(
                () => _httpClient.GetAsync(url, ct), ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException(
                    $"Authentifizierung fehlgeschlagen (HTTP {(int)response.StatusCode}). " +
                    "Bitte prüfen Sie Ihren API-Token.");
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            var page = JsonSerializer.Deserialize<ApiListResponse<T>>(content, _jsonOptions);

            if (page?.Results != null)
                allResults.AddRange(page.Results);

            url = page?.Next;
        }

        return allResults.AsReadOnly();
    }

    private async Task<HttpResponseMessage> SendWithErrorHandling(
        Func<Task<HttpResponseMessage>> request, CancellationToken ct)
    {
        try
        {
            return await request();
        }
        catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
        {
            throw new InvalidOperationException(
                "Netzwerkfehler: Verbindung zum easyVerein-Server konnte nicht hergestellt werden. " +
                "Bitte prüfen Sie Ihre Internetverbindung.", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                "Zeitüberschreitung: Die easyVerein-API hat nicht rechtzeitig geantwortet.", ex);
        }
    }

    // --- Mitglieder ---

    public async Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Member>(
            BuildListUrl("member", ApiQueries.Member), ct);
    }

    public async Task<Member?> GetMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"member/{id}", ApiQueries.Member), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<Member>(response, ct);
    }

    public async Task<Member> CreateMemberAsync(
        string emailOrUserName, ContactDetails contactDetails, CancellationToken ct = default)
    {
        var createdContact = await CreateContactDetailsAsync(contactDetails, ct);
        var payload = new { emailOrUserName, contactDetails = createdContact.Id };
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("member"), payload, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    public async Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"member/{id}"), member, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    public async Task DeleteMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"member/{id}"), ct), ct);
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException("Authentifizierung fehlgeschlagen.");
        response.EnsureSuccessStatusCode();
    }

    // --- Kontaktdaten ---

    public async Task<IReadOnlyList<ContactDetails>> GetContactDetailsAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<ContactDetails>(
            BuildListUrl("contact-details", ApiQueries.ContactDetails), ct);
    }

    public async Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"contact-details/{id}", ApiQueries.ContactDetails), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<ContactDetails>(response, ct);
    }

    public async Task<ContactDetails> CreateContactDetailsAsync(ContactDetails contact, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("contact-details"), contact, ct), ct);
        return await HandleResponse<ContactDetails>(response, ct);
    }

    public async Task<ContactDetails> UpdateContactDetailsAsync(long id, ContactDetails contact, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"contact-details/{id}"), contact, ct), ct);
        return await HandleResponse<ContactDetails>(response, ct);
    }

    public async Task DeleteContactDetailsAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"contact-details/{id}"), ct), ct);
        response.EnsureSuccessStatusCode();
    }

    // --- Rechnungen ---

    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Invoice>(
            BuildListUrl("invoice", ApiQueries.Invoice), ct);
    }

    public async Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"invoice/{id}", ApiQueries.Invoice), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("invoice"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"invoice/{id}"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task DeleteInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"invoice/{id}"), ct), ct);
        response.EnsureSuccessStatusCode();
    }

    // --- Veranstaltungen ---

    public async Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Event>(
            BuildListUrl("event", ApiQueries.Event), ct);
    }

    public async Task<Event?> GetEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"event/{id}", ApiQueries.Event), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Event>(response, ct);
    }

    public async Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("event"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    public async Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"event/{id}"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    public async Task DeleteEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"event/{id}"), ct), ct);
        response.EnsureSuccessStatusCode();
    }
}

internal class ApiListResponse<T>
{
    public List<T> Results { get; set; } = new();
    public int? Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
}
```

- [ ] **Step 6: Rewrite test file**

Komplette Datei `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`:

```csharp
using System.Net;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Infrastructure.ApiClient;

namespace MCP.EasyVerein.Infrastructure.Tests;

public class EasyVereinApiClientTests
{
    private static EasyVereinApiClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://easyverein.com/api")
        };
        var config = new EasyVereinConfiguration
        {
            ApiKey = "test-token",
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };
        return new EasyVereinApiClient(httpClient, config);
    }

    [Fact]
    public void Constructor_SetsAuthorizationHeader()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, "{}");
        var httpClient = new HttpClient(handler);
        var config = new EasyVereinConfiguration { ApiKey = "my-secret-token" };

        _ = new EasyVereinApiClient(httpClient, config);

        Assert.Contains(httpClient.DefaultRequestHeaders.GetValues("Authorization"),
            v => v == "Bearer my-secret-token");
    }

    [Fact]
    public async Task GetMembers_ReturnsMembers()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1L, emailOrUserName = "max@test.de", membershipNumber = "M-001" } },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);

        var client = CreateClient(handler);
        var result = await client.GetMembersAsync();

        Assert.Single(result);
        Assert.Equal("max@test.de", result[0].EmailOrUserName);
    }

    [Fact]
    public async Task GetMembers_FollowsPagination_ReturnsAllPages()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1L, emailOrUserName = "a@test.de" } },
            next = "https://easyverein.com/api/v1.7/member?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2L, emailOrUserName = "b@test.de" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[] {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });

        var client = CreateClient(handler);
        var result = await client.GetMembersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetMembers_SendsQueryParameter()
    {
        string? capturedUrl = null;
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null }),
            url => capturedUrl = url);

        var client = CreateClient(handler);
        await client.GetMembersAsync();

        Assert.NotNull(capturedUrl);
        Assert.Contains("query=", capturedUrl);
        Assert.Contains("limit=100", capturedUrl);
    }

    [Fact]
    public async Task GetMembers_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.GetMembersAsync());
    }

    [Fact]
    public async Task GetMember_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetMemberAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteMember_WithForbidden_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Forbidden, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.DeleteMemberAsync(1));
    }

    [Fact]
    public async Task GetInvoices_ReturnsInvoices()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1L, invNumber = "R-001", totalPrice = 50.00m } },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);

        var client = CreateClient(handler);
        var result = await client.GetInvoicesAsync();

        Assert.Single(result);
        Assert.Equal("R-001", result[0].InvoiceNumber);
    }

    [Fact]
    public async Task GetEvents_ReturnsEvents()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1L, name = "Jahresversammlung" } },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);

        var client = CreateClient(handler);
        var result = await client.GetEventsAsync();

        Assert.Single(result);
        Assert.Equal("Jahresversammlung", result[0].Name);
    }

    [Fact]
    public async Task GetContactDetails_ReturnsContactDetails()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1L, firstName = "Anna", familyName = "Schmidt" } },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);

        var client = CreateClient(handler);
        var result = await client.GetContactDetailsAsync();

        Assert.Single(result);
        Assert.Equal("Anna", result[0].FirstName);
        Assert.Equal("Schmidt", result[0].FamilyName);
    }
}

public class FakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    public FakeHttpHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}

public class MultiPageFakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<(HttpStatusCode Status, string Content)> _responses;

    public MultiPageFakeHttpHandler(IEnumerable<(HttpStatusCode, string)> responses)
    {
        _responses = new Queue<(HttpStatusCode, string)>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (status, content) = _responses.Dequeue();
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = status,
            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}

public class CapturingFakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;
    private readonly Action<string> _captureUrl;

    public CapturingFakeHttpHandler(HttpStatusCode statusCode, string content, Action<string> captureUrl)
    {
        _statusCode = statusCode;
        _content = content;
        _captureUrl = captureUrl;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _captureUrl(request.RequestUri?.ToString() ?? "");
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}
```

- [ ] **Step 7: Run all tests**

Run: `dotnet test --no-restore`
Expected: FAIL – MCP-Tools und Program.cs kompilieren noch nicht (Contact → ContactDetails)

- [ ] **Step 8: Commit (Infrastructure done)**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs \
        tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
git commit -m "feat: API-Client mit Query-Parameter, Pagination und Bearer-Auth (US-0008)"
```

---

### Task 7: MCP-Tools und Program.cs

**Files:**
- Rewrite: `src/MCP.EasyVerein.Server/Tools/MemberTools.cs`
- Create: `src/MCP.EasyVerein.Server/Tools/ContactDetailsTools.cs`
- Delete: `src/MCP.EasyVerein.Server/Tools/ContactTools.cs`
- Rewrite: `src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs`
- Rewrite: `src/MCP.EasyVerein.Server/Tools/EventTools.cs`
- Modify: `src/MCP.EasyVerein.Server/Program.cs:58`

- [ ] **Step 1: Rewrite MemberTools**

```csharp
// src/MCP.EasyVerein.Server/Tools/MemberTools.cs
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class MemberTools
{
    private readonly IEasyVereinApiClient _client;

    public MemberTools(IEasyVereinApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Alle Mitglieder auflisten")]
    public async Task<string> ListMembers(CancellationToken ct)
    {
        var members = await _client.GetMembersAsync(ct);
        return JsonSerializer.Serialize(members, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Ein Mitglied anhand der ID abrufen")]
    public async Task<string> GetMember(long id, CancellationToken ct)
    {
        var member = await _client.GetMemberAsync(id, ct);
        return member != null
            ? JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true })
            : $"Mitglied mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neues Mitglied anlegen (erstellt ContactDetails automatisch)")]
    public async Task<string> CreateMember(
        string emailOrUserName, string firstName, string familyName,
        string? privateEmail, CancellationToken ct)
    {
        var contactDetails = new ContactDetails
        {
            FirstName = firstName,
            FamilyName = familyName,
            PrivateEmail = privateEmail
        };
        var created = await _client.CreateMemberAsync(emailOrUserName, contactDetails, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Mitglied aktualisieren")]
    public async Task<string> UpdateMember(
        long id, string? emailOrUserName, string? membershipNumber, CancellationToken ct)
    {
        var member = new Member { Id = id };
        if (emailOrUserName != null) member.EmailOrUserName = emailOrUserName;
        if (membershipNumber != null) member.MembershipNumber = membershipNumber;

        var updated = await _client.UpdateMemberAsync(id, member, ct);
        return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Mitglied löschen")]
    public async Task<string> DeleteMember(long id, CancellationToken ct)
    {
        await _client.DeleteMemberAsync(id, ct);
        return $"Mitglied mit ID {id} wurde gelöscht.";
    }
}
```

- [ ] **Step 2: Create ContactDetailsTools (replace ContactTools)**

```csharp
// src/MCP.EasyVerein.Server/Tools/ContactDetailsTools.cs
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class ContactDetailsTools
{
    private readonly IEasyVereinApiClient _client;

    public ContactDetailsTools(IEasyVereinApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Alle Kontaktdaten auflisten")]
    public async Task<string> ListContactDetails(CancellationToken ct)
    {
        var contacts = await _client.GetContactDetailsAsync(ct);
        return JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Kontaktdaten anhand der ID abrufen")]
    public async Task<string> GetContactDetails(long id, CancellationToken ct)
    {
        var contact = await _client.GetContactDetailsAsync(id, ct);
        return contact != null
            ? JsonSerializer.Serialize(contact, new JsonSerializerOptions { WriteIndented = true })
            : $"Kontaktdaten mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neue Kontaktdaten anlegen")]
    public async Task<string> CreateContactDetails(
        string firstName, string familyName, string? privateEmail, CancellationToken ct)
    {
        var contact = new ContactDetails
        {
            FirstName = firstName,
            FamilyName = familyName,
            PrivateEmail = privateEmail
        };
        var created = await _client.CreateContactDetailsAsync(contact, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Kontaktdaten löschen")]
    public async Task<string> DeleteContactDetails(long id, CancellationToken ct)
    {
        await _client.DeleteContactDetailsAsync(id, ct);
        return $"Kontaktdaten mit ID {id} wurden gelöscht.";
    }
}
```

- [ ] **Step 3: Rewrite InvoiceTools**

```csharp
// src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class InvoiceTools
{
    private readonly IEasyVereinApiClient _client;

    public InvoiceTools(IEasyVereinApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Alle Rechnungen auflisten")]
    public async Task<string> ListInvoices(CancellationToken ct)
    {
        var invoices = await _client.GetInvoicesAsync(ct);
        return JsonSerializer.Serialize(invoices, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Eine Rechnung anhand der ID abrufen")]
    public async Task<string> GetInvoice(long id, CancellationToken ct)
    {
        var invoice = await _client.GetInvoiceAsync(id, ct);
        return invoice != null
            ? JsonSerializer.Serialize(invoice, new JsonSerializerOptions { WriteIndented = true })
            : $"Rechnung mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neue Rechnung anlegen")]
    public async Task<string> CreateInvoice(
        string? invoiceNumber, decimal totalPrice, string? description, string? kind, CancellationToken ct)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            TotalPrice = totalPrice,
            Description = description,
            Kind = kind
        };
        var created = await _client.CreateInvoiceAsync(invoice, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Rechnung löschen")]
    public async Task<string> DeleteInvoice(long id, CancellationToken ct)
    {
        await _client.DeleteInvoiceAsync(id, ct);
        return $"Rechnung mit ID {id} wurde gelöscht.";
    }
}
```

- [ ] **Step 4: Rewrite EventTools**

```csharp
// src/MCP.EasyVerein.Server/Tools/EventTools.cs
using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class EventTools
{
    private readonly IEasyVereinApiClient _client;

    public EventTools(IEasyVereinApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Alle Veranstaltungen auflisten")]
    public async Task<string> ListEvents(CancellationToken ct)
    {
        var events = await _client.GetEventsAsync(ct);
        return JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Eine Veranstaltung anhand der ID abrufen")]
    public async Task<string> GetEvent(long id, CancellationToken ct)
    {
        var ev = await _client.GetEventAsync(id, ct);
        return ev != null
            ? JsonSerializer.Serialize(ev, new JsonSerializerOptions { WriteIndented = true })
            : $"Veranstaltung mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neue Veranstaltung anlegen")]
    public async Task<string> CreateEvent(
        string name, string? description, string? locationName, CancellationToken ct)
    {
        var ev = new Event { Name = name, Description = description, LocationName = locationName };
        var created = await _client.CreateEventAsync(ev, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Veranstaltung löschen")]
    public async Task<string> DeleteEvent(long id, CancellationToken ct)
    {
        await _client.DeleteEventAsync(id, ct);
        return $"Veranstaltung mit ID {id} wurde gelöscht.";
    }
}
```

- [ ] **Step 5: Delete old ContactTools.cs**

```bash
git rm src/MCP.EasyVerein.Server/Tools/ContactTools.cs
```

- [ ] **Step 6: Update Program.cs – ContactTools → ContactDetailsTools**

In `src/MCP.EasyVerein.Server/Program.cs`, Zeile 58 ändern:

```csharp
// Alt:
    .WithTools<ContactTools>();
// Neu:
    .WithTools<ContactDetailsTools>();
```

- [ ] **Step 7: Build and run all tests**

Run: `dotnet build && dotnet test --no-restore`
Expected: PASS (alle Tests grün)

- [ ] **Step 8: Commit**

```bash
git add src/MCP.EasyVerein.Server/Tools/ src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat: MCP-Tools an neue Entity-Struktur anpassen (US-0008)"
```

---

### Task 8: Abschluss und Verifikation

- [ ] **Step 1: Run complete test suite**

Run: `dotnet test --verbosity normal`
Expected: Alle Tests PASS

- [ ] **Step 2: Verify build is clean**

Run: `dotnet build --no-restore 2>&1 | grep -i "warning\|error" || echo "Clean build"`
Expected: Keine Fehler, keine relevanten Warnungen

- [ ] **Step 3: Final commit (if any remaining changes)**

```bash
git status
# Falls noch Änderungen offen:
git add -A && git commit -m "chore: Bereinigung nach US-0008 Refactoring"
```
