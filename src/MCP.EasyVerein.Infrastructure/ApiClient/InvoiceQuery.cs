using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient
{
    /// <summary>
    /// Builds the query string for the invoice API endpoint, including field selection and optional filters.
    /// </summary>
    internal class InvoiceQuery
    {
        /// <summary>
        /// Gets or sets an optional invoice identifier filter.
        /// </summary>
        internal long? Id { get; set; }

        /// <summary>
        /// Gets or sets an optional membership number filter.
        /// </summary>
        public string? MembershipNumber { get; set; }

        /// <summary>
        /// Gets or sets optional search terms to filter invoices.
        /// </summary>
        public string[]? Search { get; set; }

        /// <summary>
        /// The base field selection query requesting all invoice fields.
        /// </summary>
        private const string FieldQuery =
            "query=" +
            "{" +
                InvoiceFields.Id + "," +
                InvoiceFields.InvoiceNumber + "," +
                InvoiceFields.TotalPrice + "," +
                InvoiceFields.Date + "," +
                InvoiceFields.DueDate + "," +
                InvoiceFields.DateSent + "," +
                InvoiceFields.Kind + "," +
                InvoiceFields.Description + "," +
                InvoiceFields.Receiver + "," +
                InvoiceFields.RelatedAddress + "," +
                InvoiceFields.RelatedBookings + "," +
                InvoiceFields.PayedFromUser + "," +
                InvoiceFields.ApprovedFromAdmin + "," +
                InvoiceFields.CanceledInvoice + "," +
                InvoiceFields.BankAccount + "," +
                InvoiceFields.Gross + "," +
                InvoiceFields.CancellationDescription + "," +
                InvoiceFields.TemplateName + "," +
                InvoiceFields.RefNumber + "," +
                InvoiceFields.IsDraft + "," +
                InvoiceFields.IsTemplate + "," +
                InvoiceFields.CreationDateForRecurringInvoices + "," +
                InvoiceFields.RecurringInvoicesInterval + "," +
                InvoiceFields.PaymentInformation + "," +
                InvoiceFields.IsRequest + "," +
                InvoiceFields.TaxRate + "," +
                InvoiceFields.TaxName + "," +
                InvoiceFields.ActualCallStateName + "," +
                InvoiceFields.CallStateDelayDays + "," +
                InvoiceFields.AccountNumber + "," +
                InvoiceFields.Guid + "," +
                InvoiceFields.SelectionAccount + "," +
                InvoiceFields.RemoveFileOnDelete + "," +
                InvoiceFields.CustomPaymentMethod + "," +
                InvoiceFields.IsReceipt +
            "}";

        /// <summary>
        /// Returns the complete query string with field selection and any active filters.
        /// </summary>
        /// <returns>A URL query string for the invoice endpoint.</returns>
        public override string ToString()
        {
            var parts = new List<string> { FieldQuery };

            if (Id != null)
                parts.Add($"{InvoiceFields.Id}={Id}");


            return string.Join("&", parts);
        }
    }
}
