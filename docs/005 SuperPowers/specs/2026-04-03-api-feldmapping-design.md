# Design-Spec: API-Feldmapping und Query-Parameter korrigieren

> **User Story:** [US-0008 / Issue #14](https://github.com/RalfGuder/MCP-easyVerein/issues/14)
> **Datum:** 2026-04-03
> **Status:** Genehmigt

## Zusammenfassung

Die easyVerein API trennt Daten in separate Ressourcen mit eigenen Feldnamen und erfordert einen `query`-Parameter zur Feldauswahl. Die aktuelle Implementierung verwendet falsche Feldnamen, fehlende `query`-Parameter und keine Pagination. Dieses Design korrigiert alle vier Endpunkte (Member, ContactDetails, Invoice, Event) mit einem Direct-Mapping-Ansatz.

## Architekturentscheidung: Direct Mapping (Ansatz A)

Domain-Entities bilden die API-Ressourcen 1:1 ab. `[JsonPropertyName]`-Attribute auf den Entities sorgen für korrektes JSON-Mapping in beide Richtungen (API-Deserialisierung und MCP-Ausgabe). Kein separater DTO-Layer, kein Mapper.

**Begründung:** Der MCP-Server fungiert als API-Proxy. Eine zusätzliche Abstraktionsschicht wäre Over-Engineering.

```
┌─────────────────────────────────────────┐
│  MCP.EasyVerein.Server                  │
│  (MCP-Tools: MemberTools, etc.)         │
├─────────────────────────────────────────┤
│  MCP.EasyVerein.Domain                  │
│  ├── Entities/   (mit [JsonPropertyName])│
│  └── Interfaces/ (IEasyVereinApiClient) │
├─────────────────────────────────────────┤
│  MCP.EasyVerein.Infrastructure          │
│  └── ApiClient/  (query, Pagination)    │
│       → JSON ↔ Domain-Entity direkt     │
└─────────────────────────────────────────┘
```

## Domain-Entities

### Member (`/api/v1.7/member`)

Nur mitgliedschaftsbezogene Felder. Persönliche Daten kommen aus dem verschachtelten `ContactDetails`-Objekt.

```csharp
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

    // Berechnete Properties
    [JsonIgnore]
    public bool IsActive => ResignationDate == null && !IsBlocked;

    [JsonIgnore]
    public string FullName => ContactDetails != null
        ? $"{ContactDetails.FirstName} {ContactDetails.FamilyName}"
        : EmailOrUserName;
}
```

### ContactDetails (`/api/v1.7/contact-details`)

Umbenannt von `Contact`. Enthält alle persönlichen Daten, Adress-, Firmen- und Bankdaten.

```csharp
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

    // Berechnetes Property
    [JsonIgnore]
    public string FullName => $"{FirstName} {FamilyName}".Trim();
}
```

### Invoice (`/api/v1.7/invoice`)

```csharp
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

### Event (`/api/v1.7/event`)

```csharp
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

## API-Client

### Query-Konstanten

```csharp
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

### Pagination

```csharp
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
                $"Authentifizierung fehlgeschlagen (HTTP {(int)response.StatusCode}).");
        }

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);
        var page = JsonSerializer.Deserialize<ApiListResponse<T>>(content, _jsonOptions);

        if (page?.Results != null)
            allResults.AddRange(page.Results);

        url = page?.Next; // null auf letzter Seite
    }

    return allResults.AsReadOnly();
}
```

### Zweistufige Member-Erstellung

```csharp
public async Task<Member> CreateMemberAsync(
    string emailOrUserName, ContactDetails contactDetails, CancellationToken ct = default)
{
    // Schritt 1: ContactDetails anlegen
    var createdContact = await CreateContactDetailsAsync(contactDetails, ct);

    // Schritt 2: Member mit Referenz anlegen
    var payload = new { emailOrUserName, contactDetails = createdContact.Id };
    var response = await SendWithErrorHandling(
        () => _httpClient.PostAsJsonAsync(BuildUrl("member"), payload, ct), ct);
    return await HandleResponse<Member>(response, ct);
}
```

## Interface

```csharp
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

## MCP-Tools

- `ContactTools` wird zu `ContactDetailsTools` umbenannt
- `MemberTools.CreateMember` nimmt `emailOrUserName`, `firstName`, `familyName`, `privateEmail` entgegen
- MCP-Ausgabe ist verschachtelt (Member enthält ContactDetails-Objekt)
- Alle Tools serialisieren Domain-Entities direkt (JSON-Feldnamen via `[JsonPropertyName]`)

## Teststrategie

| Testprojekt | Testfokus |
|---|---|
| Domain.Tests | `IsActive`-Logik, `FullName`, Entity-Konstruktion |
| Infrastructure.Tests | Query-Parameter, Auth-Header (`Bearer`), Pagination (multi-page), JSON-Deserialisierung mit echten API-Samples, zweistufiges Create |
| Server.Tests | MCP-Tool-Integration |

Bestehende Tests werden an neue Feldnamen angepasst (inkl. Auth-Test `Token` -> `Bearer`).
