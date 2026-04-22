using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>Represents the contact details of a member from the easyVerein API.</summary>
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class ContactDetails : IHasId
{
    /// <summary>Gets whether the address is a company address. Maps to API field '<c>addressCompany</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.AddressCompany)]
    public bool AddressCompany { get; init; }

    /// <summary>Gets the address suffix. Maps to API field '<c>addressSuffix</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.AddressSuffix)]
    public string? AddressSuffix { get; init; }

    /// <summary>Gets the account balance. Maps to API field '<c>balance</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Balance)]
    public string? Balance { get; init; }

    /// <summary>Gets the bank account owner name. Maps to API field '<c>bankAccountOwner</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.BankAccountOwner)]
    public string? BankAccountOwner { get; init; }

    /// <summary>Gets the BIC (Bank Identifier Code). Maps to API field '<c>bic</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Bic)]
    public string? Bic { get; init; }

    /// <summary>Gets the city. Maps to API field '<c>city</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.City)]
    public string? City { get; init; }

    /// <summary>Gets the company address suffix. Maps to API field '<c>companyAddressSuffix</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyAddressSuffix)]
    public string? CompanyAddressSuffix { get; init; }

    /// <summary>Gets the company address suffix for invoices. Maps to API field '<c>companyAddressSuffixInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyAddressSuffixInvoice)]
    public string? CompanyAddressSuffixInvoice { get; init; }

    /// <summary>Gets the company city. Maps to API field '<c>companyCity</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyCity)]
    public string? CompanyCity { get; init; }

    /// <summary>Gets the company city for invoices. Maps to API field '<c>companyCityInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyCityInvoice)]
    public string? CompanyCityInvoice { get; init; }

    /// <summary>Gets the company country. Maps to API field '<c>companyCountry</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyCountry)]
    public string? CompanyCountry { get; init; }

    /// <summary>Gets the company country for invoices. Maps to API field '<c>companyCountryInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyCountryInvoice)]
    public string? CompanyCountryInvoice { get; init; }

    /// <summary>Gets the company email address. Maps to API field '<c>companyEmail</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyEmail)]
    public string? CompanyEmail { get; init; }

    /// <summary>Gets the company email address for invoices. Maps to API field '<c>companyEmailInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyEmailInvoice)]
    public string? CompanyEmailInvoice { get; init; }

    /// <summary>Gets the company name. Maps to API field '<c>companyName</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyName)]
    public string? CompanyName { get; init; }

    /// <summary>Gets the company name for invoices. Maps to API field '<c>companyNameInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyNameInvoice)]
    public string? CompanyNameInvoice { get; init; }

    /// <summary>Gets the company phone number. Maps to API field '<c>companyPhone</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyPhone)]
    public string? CompanyPhone { get; init; }

    /// <summary>Gets the company phone number for invoices. Maps to API field '<c>companyPhoneInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyPhoneInvoice)]
    public string? CompanyPhoneInvoice { get; init; }

    /// <summary>Gets the company state or region. Maps to API field '<c>companyState</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyState)]
    public string? CompanyState { get; init; }

    /// <summary>Gets the company state or region for invoices. Maps to API field '<c>companyStateInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyStateInvoice)]
    public string? CompanyStateInvoice { get; init; }

    /// <summary>Gets the company street address. Maps to API field '<c>companyStreet</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyStreet)]
    public string? CompanyStreet { get; init; }

    /// <summary>Gets the company street address for invoices. Maps to API field '<c>companyStreetInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyStreetInvoice)]
    public string? CompanyStreetInvoice { get; init; }

    /// <summary>Gets the company ZIP code. Maps to API field '<c>companyZip</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyZip)]
    public string? CompanyZip { get; init; }

    /// <summary>Gets the company ZIP code for invoices. Maps to API field '<c>companyZipInvoice</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.CompanyZipInvoice)]
    public string? CompanyZipInvoice { get; init; }

    /// <summary>Gets the country. Maps to API field '<c>country</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Country)]
    public string? Country { get; init; }

    /// <summary>Gets the date of birth. Maps to API field '<c>dateOfBirth</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.DateOfBirth)]
    public string? DateOfBirth { get; init; }

    /// <summary>Gets the DATEV account number. Maps to API field '<c>datevAccountNumber</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.DatevAccountNumber)]
    public int? DatevAccountNumber { get; init; }

    /// <summary>Gets the family name. Maps to API field '<c>familyName</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.FamilyName)]
    public string FamilyName { get; init; } = string.Empty;

    /// <summary>Gets the first name. Maps to API field '<c>firstName</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.FirstName)]
    public string FirstName { get; init; } = string.Empty;

    /// <summary>Gets the combined first and family name.</summary>
    [JsonIgnore]
    public string FullName => $"{FirstName} {FamilyName}".Trim();

    /// <summary>Gets the IBAN (International Bank Account Number). Maps to API field '<c>iban</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Iban)]
    public string? Iban { get; init; }

    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets the internal note. Maps to API field '<c>internalNote</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.InternalNote)]
    public string? InternalNote { get; init; }

    /// <summary>Gets whether the invoice address is a company address. Maps to API field '<c>invoiceCompany</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.InvoiceCompany)]
    public bool? InvoiceCompany { get; init; }

    /// <summary>Gets whether this contact is a company. Maps to API field '<c>_isCompany</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.IsCompany)]
    public bool? IsCompany { get; init; }

    /// <summary>Gets the method of payment. Maps to API field '<c>methodOfPayment</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.MethodOfPayment)]
    public int? MethodOfPayment { get; init; }

    /// <summary>Gets the mobile phone number. Maps to API field '<c>mobilePhone</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.MobilePhone)]
    public string? MobilePhone { get; init; }

    /// <summary>Gets the name affix (e.g. title or suffix). Maps to API field '<c>nameAffix</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.NameAffix)]
    public string? NameAffix { get; init; }

    /// <summary>Gets the preferred communication way. Maps to API field '<c>preferredCommunicationWay</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.PreferredCommunicationWay)]
    public int? PreferredCommunicationWay { get; init; }

    /// <summary>Gets the preferred email field. Maps to API field '<c>_preferredEmailField</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.PreferredEmailField)]
    public int? PreferredEmailField { get; init; }

    /// <summary>Gets the primary email address. Maps to API field '<c>primaryEmail</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.PrimaryEmail)]
    public string? PrimaryEmail { get; init; }

    /// <summary>Gets the private email address. Maps to API field '<c>privateEmail</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.PrivateEmail)]
    public string? PrivateEmail { get; init; }

    /// <summary>Gets the private phone number. Maps to API field '<c>privatePhone</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.PrivatePhone)]
    public string? PrivatePhone { get; init; }

    /// <summary>Gets the professional role or job title. Maps to API field '<c>professionalRole</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.ProfessionalRole)]
    public string? ProfessionalRole { get; init; }

    /// <summary>Gets the salutation. Maps to API field '<c>salutation</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Salutation)]
    public string? Salutation { get; init; }

    /// <summary>Gets whether invoices are sent to the company email. Maps to API field '<c>sendInvoiceCompanyMail</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.SendInvoiceCompanyMail)]
    public bool? SendInvoiceCompanyMail { get; init; }

    /// <summary>Gets the SEPA mandate date. Maps to API field '<c>sepaDate</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.SepaDate)]
    public string? SepaDate { get; init; }

    /// <summary>Gets the SEPA mandate reference. Maps to API field '<c>sepaMandate</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.SepaMandate)]
    public string? SepaMandate { get; init; }

    /// <summary>Gets the state or region. Maps to API field '<c>state</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.State)]
    public string? State { get; init; }

    /// <summary>Gets the street address. Maps to API field '<c>street</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Street)]
    public string? Street { get; init; }

    /// <summary>Gets the ZIP code. Maps to API field '<c>zip</c>'.</summary>
    [JsonPropertyName(ContactDetailsFields.Zip)]
    public string? Zip { get; init; }
}
