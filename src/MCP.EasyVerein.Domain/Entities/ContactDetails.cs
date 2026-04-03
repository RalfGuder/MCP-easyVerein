using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class ContactDetails
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("familyName")]
    public string FamilyName { get; init; } = string.Empty;

    [JsonPropertyName("salutation")]
    public string? Salutation { get; init; }

    [JsonPropertyName("nameAffix")]
    public string? NameAffix { get; init; }

    [JsonPropertyName("dateOfBirth")]
    public string? DateOfBirth { get; init; }

    // Email fields
    [JsonPropertyName("privateEmail")]
    public string? PrivateEmail { get; init; }

    [JsonPropertyName("companyEmail")]
    public string? CompanyEmail { get; init; }

    [JsonPropertyName("primaryEmail")]
    public string? PrimaryEmail { get; init; }

    [JsonPropertyName("_preferredEmailField")]
    public string? PreferredEmailField { get; init; }

    [JsonPropertyName("preferredCommunicationWay")]
    public string? PreferredCommunicationWay { get; init; }

    // Phone fields
    [JsonPropertyName("privatePhone")]
    public string? PrivatePhone { get; init; }

    [JsonPropertyName("companyPhone")]
    public string? CompanyPhone { get; init; }

    [JsonPropertyName("mobilePhone")]
    public string? MobilePhone { get; init; }

    // Private address
    [JsonPropertyName("street")]
    public string? Street { get; init; }

    [JsonPropertyName("addressSuffix")]
    public string? AddressSuffix { get; init; }

    [JsonPropertyName("city")]
    public string? City { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("zip")]
    public string? Zip { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    // Company flags and address
    [JsonPropertyName("_isCompany")]
    public bool IsCompany { get; init; }

    [JsonPropertyName("companyName")]
    public string? CompanyName { get; init; }

    [JsonPropertyName("companyStreet")]
    public string? CompanyStreet { get; init; }

    [JsonPropertyName("companyCity")]
    public string? CompanyCity { get; init; }

    [JsonPropertyName("companyState")]
    public string? CompanyState { get; init; }

    [JsonPropertyName("companyZip")]
    public string? CompanyZip { get; init; }

    [JsonPropertyName("companyCountry")]
    public string? CompanyCountry { get; init; }

    [JsonPropertyName("companyAddressSuffix")]
    public string? CompanyAddressSuffix { get; init; }

    // Company invoice address
    [JsonPropertyName("companyNameInvoice")]
    public string? CompanyNameInvoice { get; init; }

    [JsonPropertyName("companyStreetInvoice")]
    public string? CompanyStreetInvoice { get; init; }

    [JsonPropertyName("companyCityInvoice")]
    public string? CompanyCityInvoice { get; init; }

    [JsonPropertyName("companyStateInvoice")]
    public string? CompanyStateInvoice { get; init; }

    [JsonPropertyName("companyZipInvoice")]
    public string? CompanyZipInvoice { get; init; }

    [JsonPropertyName("companyCountryInvoice")]
    public string? CompanyCountryInvoice { get; init; }

    [JsonPropertyName("companyAddressSuffixInvoice")]
    public string? CompanyAddressSuffixInvoice { get; init; }

    [JsonPropertyName("companyPhoneInvoice")]
    public string? CompanyPhoneInvoice { get; init; }

    [JsonPropertyName("companyEmailInvoice")]
    public string? CompanyEmailInvoice { get; init; }

    // Financial and professional fields
    [JsonPropertyName("professionalRole")]
    public string? ProfessionalRole { get; init; }

    [JsonPropertyName("balance")]
    public string? Balance { get; init; }

    [JsonPropertyName("iban")]
    public string? Iban { get; init; }

    [JsonPropertyName("bic")]
    public string? Bic { get; init; }

    [JsonPropertyName("bankAccountOwner")]
    public string? BankAccountOwner { get; init; }

    [JsonPropertyName("sepaMandate")]
    public string? SepaMandate { get; init; }

    [JsonPropertyName("sepaDate")]
    public string? SepaDate { get; init; }

    [JsonPropertyName("methodOfPayment")]
    public string? MethodOfPayment { get; init; }

    [JsonPropertyName("datevAccountNumber")]
    public string? DatevAccountNumber { get; init; }

    [JsonPropertyName("internalNote")]
    public string? InternalNote { get; init; }

    // Invoice and address preferences
    [JsonPropertyName("invoiceCompany")]
    public bool? InvoiceCompany { get; init; }

    [JsonPropertyName("sendInvoiceCompanyMail")]
    public bool? SendInvoiceCompanyMail { get; init; }

    [JsonPropertyName("addressCompany")]
    public bool? AddressCompany { get; init; }

    [JsonPropertyName("customPaymentMethod")]
    public string? CustomPaymentMethod { get; init; }

    // Computed property
    [JsonIgnore]
    public string FullName => $"{FirstName} {FamilyName}".Trim();
}
