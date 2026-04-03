using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class Invoice
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("invNumber")] public string? InvoiceNumber { get; set; }
    [JsonPropertyName("totalPrice")] public decimal? TotalPrice { get; set; }
    [JsonPropertyName("date")] public DateTime? Date { get; set; }
    [JsonPropertyName("dateItHappend")] public DateTime? DueDate { get; set; }
    [JsonPropertyName("dateSent")] public DateTime? DateSent { get; set; }
    [JsonPropertyName("kind")] public string? Kind { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("receiver")] public string? Receiver { get; set; }
    [JsonPropertyName("relatedAddress")] public long? RelatedAddress { get; set; }
    [JsonPropertyName("relatedBookings")] public List<long>? RelatedBookings { get; set; }
    [JsonPropertyName("payedFromUser")] public long? PayedFromUser { get; set; }
    [JsonPropertyName("approvedFromAdmin")] public long? ApprovedFromAdmin { get; set; }
    [JsonPropertyName("canceledInvoice")] public long? CanceledInvoice { get; set; }
    [JsonPropertyName("bankAccount")] public long? BankAccount { get; set; }
    [JsonPropertyName("gross")] public bool Gross { get; set; }
    [JsonPropertyName("cancellationDescription")] public string? CancellationDescription { get; set; }
    [JsonPropertyName("templateName")] public string? TemplateName { get; set; }
    [JsonPropertyName("refNumber")] public string? RefNumber { get; set; }
    [JsonPropertyName("isDraft")] public bool IsDraft { get; set; }
    [JsonPropertyName("isTemplate")] public bool IsTemplate { get; set; }
    [JsonPropertyName("creationDateForRecurringInvoices")] public DateTime? CreationDateForRecurringInvoices { get; set; }
    [JsonPropertyName("recurringInvoicesInterval")] public int? RecurringInvoicesInterval { get; set; }
    [JsonPropertyName("paymentInformation")] public string? PaymentInformation { get; set; }
    [JsonPropertyName("isRequest")] public bool IsRequest { get; set; }
    [JsonPropertyName("taxRate")] public decimal? TaxRate { get; set; }
    [JsonPropertyName("taxName")] public string? TaxName { get; set; }
    [JsonPropertyName("actualCallStateName")] public string? ActualCallStateName { get; set; }
    [JsonPropertyName("callStateDelayDays")] public int? CallStateDelayDays { get; set; }
    [JsonPropertyName("accnumber")] public int? AccountNumber { get; set; }
    [JsonPropertyName("guid")] public string? Guid { get; set; }
    [JsonPropertyName("selectionAcc")] public int? SelectionAccount { get; set; }
    [JsonPropertyName("removeFileOnDelete")] public bool RemoveFileOnDelete { get; set; }
    [JsonPropertyName("customPaymentMethod")] public int? CustomPaymentMethod { get; set; }
    [JsonPropertyName("isReceipt")] public bool IsReceipt { get; set; }
    [JsonPropertyName("mode")] public string? Mode { get; set; }
    [JsonPropertyName("offerStatus")] public string? OfferStatus { get; set; }
    [JsonPropertyName("offerValidUntil")] public DateTime? OfferValidUntil { get; set; }
    [JsonPropertyName("offerNumber")] public string? OfferNumber { get; set; }
    [JsonPropertyName("relatedOffer")] public long? RelatedOffer { get; set; }
    [JsonPropertyName("closingDescription")] public string? ClosingDescription { get; set; }
    [JsonPropertyName("useAddressBalance")] public bool UseAddressBalance { get; set; }
}
