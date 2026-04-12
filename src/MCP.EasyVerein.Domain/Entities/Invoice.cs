using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an invoice from the easyVerein API.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Id)] 
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the invoice number. Maps to API field '<c>invNumber</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.InvoiceNumber)] 
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Gets or sets the total price. Maps to API field '<c>totalPrice</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.TotalPrice)] 
    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the invoice date. Maps to API field '<c>date</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Date)] 
    public DateTime? Date { get; set; }

    /// <summary>
    /// Gets or sets the due date. Maps to API field '<c>dateItHappend</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.DueDate)] 
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the date the invoice was sent. Maps to API field '<c>dateSent</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.DateSent)] 
    public DateTime? DateSent { get; set; }

    /// <summary>
    /// Gets or sets the invoice kind/type. Maps to API field '<c>kind</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Kind)] 
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets the invoice description. Maps to API field '<c>description</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Description)] 
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the receiver name. Maps to API field '<c>receiver</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Receiver)] 
    public string? Receiver { get; set; }

    /// <summary>
    /// Gets or sets the related address ID. Maps to API field '<c>relatedAddress</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.RelatedAddress)] 
    public long? RelatedAddress { get; set; }

    /// <summary>
    /// Gets or sets the list of related booking IDs. Maps to API field '<c>relatedBookings</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.RelatedBookings)] 
    public List<long>? RelatedBookings { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who paid. Maps to API field '<c>payedFromUser</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.PayedFromUser)] 
    public long? PayedFromUser { get; set; }

    /// <summary>
    /// Gets or sets the ID of the admin who approved. Maps to API field '<c>approvedFromAdmin</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.ApprovedFromAdmin)] 
    public long? ApprovedFromAdmin { get; set; }

    /// <summary>
    /// Gets or sets the ID of the canceled invoice. Maps to API field '<c>canceledInvoice</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.CanceledInvoice)] 
    public long? CanceledInvoice { get; set; }

    /// <summary>
    /// Gets or sets the bank account ID. Maps to API field '<c>bankAccount</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.BankAccount)] 
    public long? BankAccount { get; set; }

    /// <summary>
    /// Gets or sets whether the invoice amount is gross. Maps to API field '<c>gross</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Gross)] 
    public bool Gross { get; set; }

    /// <summary>
    /// Gets or sets the cancellation description. Maps to API field '<c>cancellationDescription</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.CancellationDescription)] 
    public string? CancellationDescription { get; set; }

    /// <summary>
    /// Gets or sets the template name. Maps to API field '<c>templateName</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.TemplateName)] 
    public string? TemplateName { get; set; }

    /// <summary>
    /// Gets or sets the reference number. Maps to API field '<c>refNumber</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.RefNumber)] 
    public string? RefNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the invoice is a draft. Maps to API field '<c>isDraft</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.IsDraft)] 
    public bool IsDraft { get; set; }

    /// <summary>
    /// Gets or sets whether this is a template. Maps to API field '<c>isTemplate</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.IsTemplate)] 
    public bool IsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the creation date for recurring invoices. Maps to API field '<c>creationDateForRecurringInvoices</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.CreationDateForRecurringInvoices)] 
    public DateTime? CreationDateForRecurringInvoices { get; set; }

    /// <summary>
    /// Gets or sets the recurring invoices interval. Maps to API field '<c>recurringInvoicesInterval</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.RecurringInvoicesInterval)] 
    public int? RecurringInvoicesInterval { get; set; }

    /// <summary>
    /// Gets or sets the payment information text. Maps to API field '<c>paymentInformation</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.PaymentInformation)] 
    public string? PaymentInformation { get; set; }

    /// <summary>
    /// Gets or sets whether this is a request/expense report. Maps to API field '<c>isRequest</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.IsRequest)] 
    public bool IsRequest { get; set; }

    /// <summary>
    /// Gets or sets the tax rate. Maps to API field '<c>taxRate</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.TaxRate)] 
    public decimal? TaxRate { get; set; }

    /// <summary>
    /// Gets or sets the tax name. Maps to API field '<c>taxName</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.TaxName)] 
    public string? TaxName { get; set; }

    /// <summary>
    /// Gets or sets the current call state name. Maps to API field '<c>actualCallStateName</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.ActualCallStateName)] 
    public string? ActualCallStateName { get; set; }

    /// <summary>
    /// Gets or sets the delay in days for the call state. Maps to API field '<c>callStateDelayDays</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.CallStateDelayDays)] 
    public int? CallStateDelayDays { get; set; }

    /// <summary>
    /// Gets or sets the account number. Maps to API field '<c>accnumber</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.AccountNumber)] 
    public int? AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the globally unique identifier. Maps to API field '<c>guid</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Guid)] 
    public string? Guid { get; set; }

    /// <summary>
    /// Gets or sets the selection account number. Maps to API field '<c>selectionAcc</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.SelectionAccount)] 
    public int? SelectionAccount { get; set; }

    /// <summary>
    /// Gets or sets whether the file is removed on delete. Maps to API field '<c>removeFileOnDelete</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.RemoveFileOnDelete)] 
    public bool RemoveFileOnDelete { get; set; }

    /// <summary>
    /// Gets or sets the custom payment method ID. Maps to API field '<c>customPaymentMethod</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.CustomPaymentMethod)] 
    public int? CustomPaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets whether this is a receipt. Maps to API field '<c>isReceipt</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.IsReceipt)] 
    public bool IsReceipt { get; set; }

    /// <summary>
    /// Gets or sets the invoice mode. Maps to API field '<c>mode</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.Mode)] 
    public string? Mode { get; set; }

    /// <summary>
    /// Gets or sets the offer status. Maps to API field '<c>offerStatus</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.OfferStatus)] 
    public string? OfferStatus { get; set; }

    /// <summary>
    /// Gets or sets the date until which the offer is valid. Maps to API field '<c>offerValidUntil</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.OfferValidUntil)] 
    public DateTime? OfferValidUntil { get; set; }

    /// <summary>
    /// Gets or sets the offer number. Maps to API field '<c>offerNumber</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.OfferNumber)] 
    public string? OfferNumber { get; set; }

    /// <summary>
    /// Gets or sets the related offer ID. Maps to API field '<c>relatedOffer</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.RelatedOffer)] 
    public long? RelatedOffer { get; set; }

    /// <summary>
    /// Gets or sets the closing description text. Maps to API field '<c>closingDescription</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.ClosingDescription)] 
    public string? ClosingDescription { get; set; }

    /// <summary>
    /// Gets or sets whether the address balance is used for payment. Maps to API field '<c>useAddressBalance</c>'.
    /// </summary>
    [JsonPropertyName(InvoiceFields.UseAddressBalance)] 
    public bool UseAddressBalance { get; set; }
}
