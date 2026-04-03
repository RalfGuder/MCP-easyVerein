using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

public class ContactDetails
{
    [JsonPropertyName(ContactDetailsFields.AddressCompany)]
    public bool AddressCompany { get; init; }

    [JsonPropertyName(ContactDetailsFields.AddressSuffix)]
    public string? AddressSuffix { get; init; }

    [JsonPropertyName(ContactDetailsFields.Balance)]
    public string? Balance { get; init; }

    [JsonPropertyName(ContactDetailsFields.BankAccountOwner)]
    public string? BankAccountOwner { get; init; }

    [JsonPropertyName(ContactDetailsFields.Bic)]
    public string? Bic { get; init; }

    [JsonPropertyName(ContactDetailsFields.City)]
    public string? City { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyAddressSuffix)]
    public string? CompanyAddressSuffix { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyAddressSuffixInvoice)]
    public string? CompanyAddressSuffixInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyCity)]
    public string? CompanyCity { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyCityInvoice)]
    public string? CompanyCityInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyCountry)]
    public string? CompanyCountry { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyCountryInvoice)]
    public string? CompanyCountryInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyEmail)]
    public string? CompanyEmail { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyEmailInvoice)]
    public string? CompanyEmailInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyName)]
    public string? CompanyName { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyNameInvoice)]
    public string? CompanyNameInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyPhone)]
    public string? CompanyPhone { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyPhoneInvoice)]
    public string? CompanyPhoneInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyState)]
    public string? CompanyState { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyStateInvoice)]
    public string? CompanyStateInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyStreet)]
    public string? CompanyStreet { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyStreetInvoice)]
    public string? CompanyStreetInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyZip)]
    public string? CompanyZip { get; init; }

    [JsonPropertyName(ContactDetailsFields.CompanyZipInvoice)]
    public string? CompanyZipInvoice { get; init; }

    [JsonPropertyName(ContactDetailsFields.Country)]
    public string? Country { get; init; }

    [JsonPropertyName(ContactDetailsFields.DateOfBirth)]
    public string? DateOfBirth { get; init; }

    [JsonPropertyName(ContactDetailsFields.DatevAccountNumber)]
    public int? DatevAccountNumber { get; init; }

    [JsonPropertyName(ContactDetailsFields.FamilyName)]
    public string FamilyName { get; init; } = string.Empty;

    [JsonPropertyName(ContactDetailsFields.FirstName)]
    public string FirstName { get; init; } = string.Empty;

    [JsonIgnore]
    public string FullName => $"{FirstName} {FamilyName}".Trim();

    [JsonPropertyName(ContactDetailsFields.Iban)]
    public string? Iban { get; init; }

    [JsonPropertyName(ContactDetailsFields.Id)]
    public long Id { get; init; }

    [JsonPropertyName(ContactDetailsFields.InternalNote)]
    public string? InternalNote { get; init; }

    [JsonPropertyName(ContactDetailsFields.InvoiceCompany)]
    public bool InvoiceCompany { get; init; }

    [JsonPropertyName(ContactDetailsFields.IsCompany)]
    public bool IsCompany { get; init; }

    [JsonPropertyName(ContactDetailsFields.MethodOfPayment)]
    public int? MethodOfPayment { get; init; }

    [JsonPropertyName(ContactDetailsFields.MobilePhone)]
    public string? MobilePhone { get; init; }

    [JsonPropertyName(ContactDetailsFields.NameAffix)]
    public string? NameAffix { get; init; }

    [JsonPropertyName(ContactDetailsFields.PreferredCommunicationWay)]
    public int? PreferredCommunicationWay { get; init; }

    [JsonPropertyName(ContactDetailsFields.PreferredEmailField)]
    public int? PreferredEmailField { get; init; }

    [JsonPropertyName(ContactDetailsFields.PrimaryEmail)]
    public string? PrimaryEmail { get; init; }

    [JsonPropertyName(ContactDetailsFields.PrivateEmail)]
    public string? PrivateEmail { get; init; }

    [JsonPropertyName(ContactDetailsFields.PrivatePhone)]
    public string? PrivatePhone { get; init; }

    [JsonPropertyName(ContactDetailsFields.ProfessionalRole)]
    public string? ProfessionalRole { get; init; }

    [JsonPropertyName(ContactDetailsFields.Salutation)]
    public string? Salutation { get; init; }

    [JsonPropertyName(ContactDetailsFields.SendInvoiceCompanyMail)]
    public bool SendInvoiceCompanyMail { get; init; }

    [JsonPropertyName(ContactDetailsFields.SepaDate)]
    public string? SepaDate { get; init; }

    [JsonPropertyName(ContactDetailsFields.SepaMandate)]
    public string? SepaMandate { get; init; }

    [JsonPropertyName(ContactDetailsFields.State)]
    public string? State { get; init; }

    [JsonPropertyName(ContactDetailsFields.Street)]
    public string? Street { get; init; }

    [JsonPropertyName(ContactDetailsFields.Zip)]
    public string? Zip { get; init; }
}
