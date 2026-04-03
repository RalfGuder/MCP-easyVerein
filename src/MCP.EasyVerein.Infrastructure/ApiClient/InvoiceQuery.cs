using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient
{
    internal class InvoiceQuery
    {
        internal long? Id { get; set; }

        public string? MembershipNumber { get; set; }
        public string[]? Search { get; set; }

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

        public override string ToString()
        {
            var parts = new List<string> { FieldQuery };

            if (Id != null)
                parts.Add($"{InvoiceFields.Id}={Id}");
            

            return string.Join("&", parts);
        }
    }
}
