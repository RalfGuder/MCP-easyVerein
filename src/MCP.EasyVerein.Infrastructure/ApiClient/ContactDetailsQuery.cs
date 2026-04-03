using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient
{
    internal class ContactDetailsQuery
    {
        public long? Id { get; set; }
        public string? FirstName { get; set; } = string.Empty;

        public string? FamilyName { get; set; } = string.Empty;
        public string? Name { get; set; }

        private const string FieldQuery =
            "query=" + 
            "{" +
                ContactDetailsFields.Id + "," +
                ContactDetailsFields.FirstName + "," +
                ContactDetailsFields.FamilyName + "," +
                ContactDetailsFields.Salutation + "," +
                ContactDetailsFields.NameAffix + "," +
                ContactDetailsFields.DateOfBirth + "," +
                ContactDetailsFields.PrivateEmail + "," +
                ContactDetailsFields.CompanyEmail + "," +
                ContactDetailsFields.PrimaryEmail + "," +
                ContactDetailsFields.PreferredEmailField + "," +
                ContactDetailsFields.PreferredCommunicationWay + "," +
                ContactDetailsFields.PrivatePhone + "," +
                ContactDetailsFields.CompanyPhone + "," +
                ContactDetailsFields.MobilePhone + "," +
                ContactDetailsFields.Street + "," +
                ContactDetailsFields.City + "," +
                ContactDetailsFields.State + "," +
                ContactDetailsFields.Zip + "," +
                ContactDetailsFields.Country + "," +
                ContactDetailsFields.IsCompany + "," +
                ContactDetailsFields.CompanyName + "," +
                ContactDetailsFields.CompanyStreet + "," +
                ContactDetailsFields.CompanyCity + "," +
                ContactDetailsFields.CompanyState + "," +
                ContactDetailsFields.CompanyZip + "," +
                ContactDetailsFields.CompanyCountry + "," +
                ContactDetailsFields.CompanyNameInvoice + "," +
                ContactDetailsFields.CompanyStreetInvoice + "," +
                ContactDetailsFields.CompanyCityInvoice + "," +
                ContactDetailsFields.CompanyStateInvoice + "," +
                ContactDetailsFields.CompanyZipInvoice + "," +
                ContactDetailsFields.CompanyCountryInvoice + "," +
                ContactDetailsFields.CompanyPhoneInvoice + "," +
                ContactDetailsFields.CompanyEmailInvoice + "," +
                ContactDetailsFields.ProfessionalRole + "," +
                ContactDetailsFields.Balance + "," +
                ContactDetailsFields.Iban + "," +
                ContactDetailsFields.Bic + "," +
                ContactDetailsFields.BankAccountOwner + "," +
                ContactDetailsFields.SepaMandate + "," +
                ContactDetailsFields.SepaDate + "," +
                ContactDetailsFields.MethodOfPayment + "," +
                ContactDetailsFields.DatevAccountNumber + "," +
                ContactDetailsFields.InternalNote + "," +
                ContactDetailsFields.InvoiceCompany + "," +
                ContactDetailsFields.SendInvoiceCompanyMail + "," +
                ContactDetailsFields.AddressCompany +
            "}";

        public override string ToString()
        {
            var parts = new List<string> { FieldQuery };

            if (Id != null)
                parts.Add($"id={Id}");

            if (!string.IsNullOrEmpty(FirstName))
                parts.Add($"firstName={Uri.EscapeDataString(FirstName)}");

            if (!string.IsNullOrEmpty(FamilyName))
                parts.Add($"familyName={Uri.EscapeDataString(FamilyName)}");
            
            if (!string.IsNullOrEmpty(Name))
                parts.Add($"familyName={Uri.EscapeDataString(Name)}");

            return string.Join("&", parts);
        }
    }
}
