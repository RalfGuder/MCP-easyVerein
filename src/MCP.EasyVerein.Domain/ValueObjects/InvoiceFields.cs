namespace MCP.EasyVerein.Domain.ValueObjects
{
    /// <summary>Constants for easyVerein Invoice API field names used in JSON serialization.</summary>
    public static class InvoiceFields
    {
        /// <summary>API field name for the account number.</summary>
        public const string AccountNumber = "accnumber";
        /// <summary>API field name for the actual call state name.</summary>
        public const string ActualCallStateName = "actualCallStateName";
        /// <summary>API field name for whether the invoice was approved by an admin.</summary>
        public const string ApprovedFromAdmin = "approvedFromAdmin";
        /// <summary>API field name for the bank account reference.</summary>
        public const string BankAccount = "bankAccount";
        /// <summary>API field name for the call state delay in days.</summary>
        public const string CallStateDelayDays = "callStateDelayDays";
        /// <summary>API field name for the cancel-invoice reference.</summary>
        public const string CancelInvoice = "cancelInvoice";
        /// <summary>API field name for the canceled invoice reference.</summary>
        public const string CanceledInvoice = "canceledInvoice";
        /// <summary>API field name for the cancellation description.</summary>
        public const string CancellationDescription = "cancellationDescription";
        /// <summary>API field name for the payment-processor charges object.</summary>
        public const string Charges = "charges";
        /// <summary>API field name for the closing description.</summary>
        public const string ClosingDescription = "closingDescription";
        /// <summary>API field name for the creation date of recurring invoices.</summary>
        public const string CreationDateForRecurringInvoices = "creationDateForRecurringInvoices";
        /// <summary>API field name for the custom payment method.</summary>
        public const string CustomPaymentMethod = "customPaymentMethod";
        /// <summary>API field name for the invoice date.</summary>
        public const string Date = "date";
        /// <summary>API field name for the date the invoice was sent.</summary>
        public const string DateSent = "dateSent";
        /// <summary>API field name for the scheduled deletion date.</summary>
        public const string DeleteAfterDate = "_deleteAfterDate";
        /// <summary>API field name for the deleting user reference.</summary>
        public const string DeletedBy = "_deletedBy";
        /// <summary>API field name for the invoice description.</summary>
        public const string Description = "description";
        /// <summary>API field name for the due date.</summary>
        public const string DueDate = "dateItHappend";
        /// <summary>API field name for the gross amount.</summary>
        public const string Gross = "gross";
        /// <summary>API field name for the globally unique identifier.</summary>
        public const string Guid = "guid";
        /// <summary>API field name for the unique invoice identifier.</summary>
        public const string Id = "id";
        /// <summary>API field name for the list of invoice-item references.</summary>
        public const string InvoiceItems = "invoiceItems";
        /// <summary>API field name for the invoice number.</summary>
        public const string InvoiceNumber = "invNumber";
        /// <summary>API field name for whether the invoice is a draft.</summary>
        public const string IsDraft = "isDraft";
        /// <summary>API field name for whether the invoice is a receipt.</summary>
        public const string IsReceipt = "isReceipt";
        /// <summary>API field name for whether the invoice is a request.</summary>
        public const string IsRequest = "isRequest";
        /// <summary>API field name for the flag marking VAT liability.</summary>
        public const string IsSubjectToTax = "_isSubjectToTax";
        /// <summary>API field name for the flag marking per-item tax rates.</summary>
        public const string IsTaxRatePerInvoiceItem = "_isTaxRatePerInvoiceItem";
        /// <summary>API field name for whether the invoice is a template.</summary>
        public const string IsTemplate = "isTemplate";
        /// <summary>API field name for the invoice kind.</summary>
        public const string Kind = "kind";
        /// <summary>API field name for the invoice mode.</summary>
        public const string Mode = "mode";
        /// <summary>API field name for the offer number.</summary>
        public const string OfferNumber = "offerNumber";
        /// <summary>API field name for the offer status.</summary>
        public const string OfferStatus = "offerStatus";
        /// <summary>API field name for the offer validity end date.</summary>
        public const string OfferValidUntil = "offerValidUntil";
        /// <summary>API field name for the organisation reference.</summary>
        public const string Org = "org";
        /// <summary>API field name for the file path / download URL.</summary>
        public const string Path = "path";
        /// <summary>API field name for whether the invoice was paid by the user.</summary>
        public const string PayedFromUser = "payedFromUser";
        /// <summary>API field name for the payment difference amount.</summary>
        public const string PaymentDifference = "paymentDifference";
        /// <summary>API field name for the payment information.</summary>
        public const string PaymentInformation = "paymentInformation";
        /// <summary>API field name for the invoice receiver.</summary>
        public const string Receiver = "receiver";
        /// <summary>API field name for the recurring invoices interval.</summary>
        public const string RecurringInvoicesInterval = "recurringInvoicesInterval";
        /// <summary>API field name for the reference number.</summary>
        public const string RefNumber = "refNumber";
        /// <summary>API field name for the related address.</summary>
        public const string RelatedAddress = "relatedAddress";
        /// <summary>API field name for the related bookings.</summary>
        public const string RelatedBookings = "relatedBookings";
        /// <summary>API field name for the related offer.</summary>
        public const string RelatedOffer = "relatedOffer";
        /// <summary>API field name for removing the file on delete.</summary>
        public const string RemoveFileOnDelete = "removeFileOnDelete";
        /// <summary>API field name for the search filter.</summary>
        public const string Search = "search";
        /// <summary>API field name for the selection account.</summary>
        public const string SelectionAccount = "selectionAcc";
        /// <summary>API field name for the tax amount.</summary>
        public const string Tax = "tax";
        /// <summary>API field name for the tax name.</summary>
        public const string TaxName = "taxName";
        /// <summary>API field name for the tax rate.</summary>
        public const string TaxRate = "taxRate";
        /// <summary>API field name for the template name.</summary>
        public const string TemplateName = "templateName";
        /// <summary>API field name for the total price.</summary>
        public const string TotalPrice = "totalPrice";
        /// <summary>API field name for using the address balance.</summary>
        public const string UseAddressBalance = "useAddressBalance";
    }
}
