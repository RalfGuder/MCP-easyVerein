using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class ContactDetailsEntityTests
{
    [Fact]
    public void FullName_ReturnsCombinedName()
    {
        var contact = new ContactDetails
        {
            FirstName = "Max",
            FamilyName = "Mustermann"
        };

        Assert.Equal("Max Mustermann", contact.FullName);
    }

    [Fact]
    public void FullName_TrimsWhitespace_WhenFirstNameEmpty()
    {
        var contact = new ContactDetails
        {
            FirstName = "",
            FamilyName = "Mustermann"
        };

        Assert.Equal("Mustermann", contact.FullName);
    }

    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "firstName": "Anna",
                "familyName": "Schmidt",
                "salutation": "Frau",
                "nameAffix": "Dr.",
                "dateOfBirth": "1990-06-15",
                "privateEmail": "anna@private.de",
                "companyEmail": "anna@company.de",
                "primaryEmail": "anna@primary.de",
                "_preferredEmailField": "privateEmail",
                "preferredCommunicationWay": "email",
                "privatePhone": "030-123456",
                "companyPhone": "030-654321",
                "mobilePhone": "0160-111222",
                "street": "Musterstraße 1",
                "addressSuffix": "c/o Beispiel",
                "city": "Berlin",
                "state": "Berlin",
                "zip": "10115",
                "country": "DE",
                "_isCompany": true,
                "companyName": "Muster GmbH",
                "companyStreet": "Firmenstraße 5",
                "companyCity": "Hamburg",
                "companyState": "Hamburg",
                "companyZip": "20095",
                "companyCountry": "DE",
                "companyAddressSuffix": "Etage 3",
                "companyNameInvoice": "Muster GmbH Rechnung",
                "companyStreetInvoice": "Rechnungsstraße 1",
                "companyCityInvoice": "München",
                "companyStateInvoice": "Bayern",
                "companyZipInvoice": "80331",
                "companyCountryInvoice": "DE",
                "companyAddressSuffixInvoice": "Büro 42",
                "companyPhoneInvoice": "089-999888",
                "companyEmailInvoice": "rechnung@muster.de",
                "professionalRole": "Manager",
                "balance": "100.50",
                "iban": "DE89370400440532013000",
                "bic": "COBADEFFXXX",
                "bankAccountOwner": "Anna Schmidt",
                "sepaMandate": "MANDAT-001",
                "sepaDate": "2023-01-15",
                "methodOfPayment": "sepa",
                "datevAccountNumber": "10000",
                "internalNote": "VIP-Mitglied",
                "invoiceCompany": true,
                "sendInvoiceCompanyMail": false,
                "addressCompany": true,
                "customPaymentMethod": "Überweisung"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var contact = JsonSerializer.Deserialize<ContactDetails>(json, options);

        Assert.NotNull(contact);
        Assert.Equal(42, contact.Id);
        Assert.Equal("Anna", contact.FirstName);
        Assert.Equal("Schmidt", contact.FamilyName);
        Assert.Equal("Frau", contact.Salutation);
        Assert.Equal("Dr.", contact.NameAffix);
        Assert.Equal("1990-06-15", contact.DateOfBirth);
        Assert.Equal("anna@private.de", contact.PrivateEmail);
        Assert.Equal("anna@company.de", contact.CompanyEmail);
        Assert.Equal("anna@primary.de", contact.PrimaryEmail);
        Assert.Equal("privateEmail", contact.PreferredEmailField);
        Assert.Equal("email", contact.PreferredCommunicationWay);
        Assert.Equal("030-123456", contact.PrivatePhone);
        Assert.Equal("030-654321", contact.CompanyPhone);
        Assert.Equal("0160-111222", contact.MobilePhone);
        Assert.Equal("Musterstraße 1", contact.Street);
        Assert.Equal("c/o Beispiel", contact.AddressSuffix);
        Assert.Equal("Berlin", contact.City);
        Assert.Equal("Berlin", contact.State);
        Assert.Equal("10115", contact.Zip);
        Assert.Equal("DE", contact.Country);
        Assert.True(contact.IsCompany);
        Assert.Equal("Muster GmbH", contact.CompanyName);
        Assert.Equal("Firmenstraße 5", contact.CompanyStreet);
        Assert.Equal("Hamburg", contact.CompanyCity);
        Assert.Equal("Hamburg", contact.CompanyState);
        Assert.Equal("20095", contact.CompanyZip);
        Assert.Equal("DE", contact.CompanyCountry);
        Assert.Equal("Etage 3", contact.CompanyAddressSuffix);
        Assert.Equal("Muster GmbH Rechnung", contact.CompanyNameInvoice);
        Assert.Equal("Rechnungsstraße 1", contact.CompanyStreetInvoice);
        Assert.Equal("München", contact.CompanyCityInvoice);
        Assert.Equal("Bayern", contact.CompanyStateInvoice);
        Assert.Equal("80331", contact.CompanyZipInvoice);
        Assert.Equal("DE", contact.CompanyCountryInvoice);
        Assert.Equal("Büro 42", contact.CompanyAddressSuffixInvoice);
        Assert.Equal("089-999888", contact.CompanyPhoneInvoice);
        Assert.Equal("rechnung@muster.de", contact.CompanyEmailInvoice);
        Assert.Equal("Manager", contact.ProfessionalRole);
        Assert.Equal("100.50", contact.Balance);
        Assert.Equal("DE89370400440532013000", contact.Iban);
        Assert.Equal("COBADEFFXXX", contact.Bic);
        Assert.Equal("Anna Schmidt", contact.BankAccountOwner);
        Assert.Equal("MANDAT-001", contact.SepaMandate);
        Assert.Equal("2023-01-15", contact.SepaDate);
        Assert.Equal("sepa", contact.MethodOfPayment);
        Assert.Equal("10000", contact.DatevAccountNumber);
        Assert.Equal("VIP-Mitglied", contact.InternalNote);
        Assert.True(contact.InvoiceCompany);
        Assert.False(contact.SendInvoiceCompanyMail);
        Assert.True(contact.AddressCompany);
        Assert.Equal("Überweisung", contact.CustomPaymentMethod);
    }
}
