using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient
{
    /// <summary>
    /// Builds the query string for the contact details API endpoint, including field selection and optional filters.
    /// </summary>
    internal class ContactDetailsQuery
    {
        /// <summary>
        /// Gets or sets an optional contact details identifier filter.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets an optional first name filter.
        /// </summary>
        public string? FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional family name filter.
        /// </summary>
        public string? FamilyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional name filter.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The base field selection query requesting all contact details fields.
        /// </summary>
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

        /// <summary>
        /// Returns the complete query string with field selection and any active filters.
        /// </summary>
        /// <returns>A URL query string for the contact details endpoint.</returns>
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
