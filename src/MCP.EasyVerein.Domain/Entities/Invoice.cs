using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Helpers;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an invoice from the easyVerein API.
/// </summary>
public class Invoice
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Id)] public long Id { get; set; }

    /// <summary>Gets or sets the invoice number. Maps to API field '<c>invNumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.InvoiceNumber)] public string? InvoiceNumber { get; set; }

    /// <summary>Gets or sets the total price. Maps to API field '<c>totalPrice</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TotalPrice)] public decimal? TotalPrice { get; set; }

    /// <summary>Gets or sets the tax amount. Maps to API field '<c>tax</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Tax)] public decimal? Tax { get; set; }

    /// <summary>Gets or sets the difference between invoiced and paid amount. Maps to API field '<c>paymentDifference</c>'.</summary>
    [JsonPropertyName(InvoiceFields.PaymentDifference)] public decimal? PaymentDifference { get; set; }

    /// <summary>Gets or sets the invoice date. Maps to API field '<c>date</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Date)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? Date { get; set; }

    /// <summary>Gets or sets the due date. Maps to API field '<c>dateItHappend</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DueDate)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? DueDate { get; set; }

    /// <summary>Gets or sets the date the invoice was sent. Maps to API field '<c>dateSent</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DateSent)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? DateSent { get; set; }

    /// <summary>Gets or sets the invoice kind/type. Maps to API field '<c>kind</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Kind)] public string? Kind { get; set; }

    /// <summary>Gets or sets the invoice description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Description)] public string? Description { get; set; }

    /// <summary>Gets or sets the receiver address text. Maps to API field '<c>receiver</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Receiver)] public string? Receiver { get; set; }

    /// <summary>Gets or sets the related address URL reference. Maps to API field '<c>relatedAddress</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RelatedAddress)] public string? RelatedAddress { get; set; }

    /// <summary>Gets the contact-details ID extracted from <see cref="RelatedAddress"/>.</summary>
    [JsonIgnore] public long? RelatedAddressId => UrlReference.ExtractId(RelatedAddress);

    /// <summary>Gets or sets the related booking URL references. Maps to API field '<c>relatedBookings</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RelatedBookings)] public List<string>? RelatedBookings { get; set; }

    /// <summary>Gets or sets the URL reference of the user who paid. Maps to API field '<c>payedFromUser</c>'.</summary>
    [JsonPropertyName(InvoiceFields.PayedFromUser)] public string? PayedFromUser { get; set; }

    /// <summary>Gets the user ID extracted from <see cref="PayedFromUser"/>.</summary>
    [JsonIgnore] public long? PayedFromUserId => UrlReference.ExtractId(PayedFromUser);

    /// <summary>Gets or sets the URL reference of the approving admin. Maps to API field '<c>approvedFromAdmin</c>'.</summary>
    [JsonPropertyName(InvoiceFields.ApprovedFromAdmin)] public string? ApprovedFromAdmin { get; set; }

    /// <summary>Gets the admin ID extracted from <see cref="ApprovedFromAdmin"/>.</summary>
    [JsonIgnore] public long? ApprovedFromAdminId => UrlReference.ExtractId(ApprovedFromAdmin);

    /// <summary>Gets or sets the URL reference of the canceled invoice. Maps to API field '<c>canceledInvoice</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CanceledInvoice)] public string? CanceledInvoice { get; set; }

    /// <summary>Gets the invoice ID extracted from <see cref="CanceledInvoice"/>.</summary>
    [JsonIgnore] public long? CanceledInvoiceId => UrlReference.ExtractId(CanceledInvoice);

    /// <summary>Gets or sets the URL reference of a storno-target invoice. Maps to API field '<c>cancelInvoice</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CancelInvoice)] public string? CancelInvoice { get; set; }

    /// <summary>Gets the invoice ID extracted from <see cref="CancelInvoice"/>.</summary>
    [JsonIgnore] public long? CancelInvoiceId => UrlReference.ExtractId(CancelInvoice);

    /// <summary>Gets or sets the URL reference of the bank account. Maps to API field '<c>bankAccount</c>'.</summary>
    [JsonPropertyName(InvoiceFields.BankAccount)] public string? BankAccount { get; set; }

    /// <summary>Gets the bank-account ID extracted from <see cref="BankAccount"/>.</summary>
    [JsonIgnore] public long? BankAccountId => UrlReference.ExtractId(BankAccount);

    /// <summary>Gets or sets the URL reference of the organisation. Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Org)] public string? Org { get; set; }

    /// <summary>Gets the organisation ID extracted from <see cref="Org"/>.</summary>
    [JsonIgnore] public long? OrgId => UrlReference.ExtractId(Org);

    /// <summary>Gets or sets the file/download URL of the invoice PDF. Maps to API field '<c>path</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Path)] public string? Path { get; set; }

    /// <summary>Gets or sets the list of invoice-item URL references. Maps to API field '<c>invoiceItems</c>'.</summary>
    [JsonPropertyName(InvoiceFields.InvoiceItems)] public List<string>? InvoiceItems { get; set; }

    /// <summary>Gets or sets the payment-processor charges summary. Maps to API field '<c>charges</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Charges)] public InvoiceCharges? Charges { get; set; }

    /// <summary>Gets or sets whether the invoice amount is gross. Maps to API field '<c>gross</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Gross)] public bool Gross { get; set; }

    /// <summary>Gets or sets the cancellation description. Maps to API field '<c>cancellationDescription</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CancellationDescription)] public string? CancellationDescription { get; set; }

    /// <summary>Gets or sets the template name. Maps to API field '<c>templateName</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TemplateName)] public string? TemplateName { get; set; }

    /// <summary>Gets or sets the reference number. Maps to API field '<c>refNumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RefNumber)] public string? RefNumber { get; set; }

    /// <summary>Gets or sets whether the invoice is a draft. Maps to API field '<c>isDraft</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsDraft)] public bool IsDraft { get; set; }

    /// <summary>Gets or sets whether this is a template. Maps to API field '<c>isTemplate</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsTemplate)] public bool IsTemplate { get; set; }

    /// <summary>Gets or sets the creation date for recurring invoices. Maps to API field '<c>creationDateForRecurringInvoices</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CreationDateForRecurringInvoices)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? CreationDateForRecurringInvoices { get; set; }

    /// <summary>Gets or sets the recurring-invoices interval in months. Maps to API field '<c>recurringInvoicesInterval</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RecurringInvoicesInterval)] public int? RecurringInvoicesInterval { get; set; }

    /// <summary>Gets or sets the payment information text. Maps to API field '<c>paymentInformation</c>'.</summary>
    [JsonPropertyName(InvoiceFields.PaymentInformation)] public string? PaymentInformation { get; set; }

    /// <summary>Gets or sets whether this is a request/expense report. Maps to API field '<c>isRequest</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsRequest)] public bool IsRequest { get; set; }

    /// <summary>Gets or sets the tax rate. Maps to API field '<c>taxRate</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TaxRate)] public decimal? TaxRate { get; set; }

    /// <summary>Gets or sets the tax name. Maps to API field '<c>taxName</c>'.</summary>
    [JsonPropertyName(InvoiceFields.TaxName)] public string? TaxName { get; set; }

    /// <summary>Gets or sets the current call-state name. Maps to API field '<c>actualCallStateName</c>'.</summary>
    [JsonPropertyName(InvoiceFields.ActualCallStateName)] public string? ActualCallStateName { get; set; }

    /// <summary>Gets or sets the delay in days for the call state. Maps to API field '<c>callStateDelayDays</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CallStateDelayDays)] public int? CallStateDelayDays { get; set; }

    /// <summary>Gets or sets the account number. Maps to API field '<c>accnumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.AccountNumber)] public int? AccountNumber { get; set; }

    /// <summary>Gets or sets the globally unique identifier. Maps to API field '<c>guid</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Guid)] public string? Guid { get; set; }

    /// <summary>Gets or sets the selection account number. Maps to API field '<c>selectionAcc</c>'.</summary>
    [JsonPropertyName(InvoiceFields.SelectionAccount)] public int? SelectionAccount { get; set; }

    /// <summary>Gets or sets whether the file is removed on delete. Maps to API field '<c>removeFileOnDelete</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RemoveFileOnDelete)] public bool RemoveFileOnDelete { get; set; }

    /// <summary>Gets or sets the custom payment method ID. Maps to API field '<c>customPaymentMethod</c>'.</summary>
    [JsonPropertyName(InvoiceFields.CustomPaymentMethod)] public int? CustomPaymentMethod { get; set; }

    /// <summary>Gets or sets whether this is a receipt. Maps to API field '<c>isReceipt</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsReceipt)] public bool IsReceipt { get; set; }

    /// <summary>Gets or sets whether tax rates are per invoice item. Maps to API field '<c>_isTaxRatePerInvoiceItem</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsTaxRatePerInvoiceItem)] public bool IsTaxRatePerInvoiceItem { get; set; }

    /// <summary>Gets or sets whether this invoice is subject to tax. Maps to API field '<c>_isSubjectToTax</c>'.</summary>
    [JsonPropertyName(InvoiceFields.IsSubjectToTax)] public bool IsSubjectToTax { get; set; }

    /// <summary>Gets or sets the invoice mode. Maps to API field '<c>mode</c>'.</summary>
    [JsonPropertyName(InvoiceFields.Mode)] public string? Mode { get; set; }

    /// <summary>Gets or sets the offer status. Maps to API field '<c>offerStatus</c>'.</summary>
    [JsonPropertyName(InvoiceFields.OfferStatus)] public string? OfferStatus { get; set; }

    /// <summary>Gets or sets the date until which the offer is valid. Maps to API field '<c>offerValidUntil</c>'.</summary>
    [JsonPropertyName(InvoiceFields.OfferValidUntil)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? OfferValidUntil { get; set; }

    /// <summary>Gets or sets the offer number. Maps to API field '<c>offerNumber</c>'.</summary>
    [JsonPropertyName(InvoiceFields.OfferNumber)] public string? OfferNumber { get; set; }

    /// <summary>Gets or sets the URL reference of the related offer. Maps to API field '<c>relatedOffer</c>'.</summary>
    [JsonPropertyName(InvoiceFields.RelatedOffer)] public string? RelatedOffer { get; set; }

    /// <summary>Gets the offer ID extracted from <see cref="RelatedOffer"/>.</summary>
    [JsonIgnore] public long? RelatedOfferId => UrlReference.ExtractId(RelatedOffer);

    /// <summary>Gets or sets the closing description text. Maps to API field '<c>closingDescription</c>'.</summary>
    [JsonPropertyName(InvoiceFields.ClosingDescription)] public string? ClosingDescription { get; set; }

    /// <summary>Gets or sets whether the address balance is used for payment. Maps to API field '<c>useAddressBalance</c>'.</summary>
    [JsonPropertyName(InvoiceFields.UseAddressBalance)] public bool UseAddressBalance { get; set; }

    /// <summary>Gets or sets the scheduled deletion date. Maps to API field '<c>_deleteAfterDate</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DeleteAfterDate)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? DeleteAfterDate { get; set; }

    /// <summary>Gets or sets the URL reference of the deleting user. Maps to API field '<c>_deletedBy</c>'.</summary>
    [JsonPropertyName(InvoiceFields.DeletedBy)] public string? DeletedBy { get; set; }

    /// <summary>Gets the user ID extracted from <see cref="DeletedBy"/>.</summary>
    [JsonIgnore] public long? DeletedById => UrlReference.ExtractId(DeletedBy);
}
