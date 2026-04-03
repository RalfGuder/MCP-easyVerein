using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

public class Invoice
{
    [JsonPropertyName(InvoiceFields.Id)] public long Id { get; set; }
    [JsonPropertyName(InvoiceFields.InvoiceNumber)] public string? InvoiceNumber { get; set; }
    [JsonPropertyName(InvoiceFields.TotalPrice)] public decimal? TotalPrice { get; set; }
    [JsonPropertyName(InvoiceFields.Date)] public DateTime? Date { get; set; }
    [JsonPropertyName(InvoiceFields.DueDate)] public DateTime? DueDate { get; set; }
    [JsonPropertyName(InvoiceFields.DateSent)] public DateTime? DateSent { get; set; }
    [JsonPropertyName(InvoiceFields.Kind)] public string? Kind { get; set; }
    [JsonPropertyName(InvoiceFields.Description)] public string? Description { get; set; }
    [JsonPropertyName(InvoiceFields.Receiver)] public string? Receiver { get; set; }
    [JsonPropertyName(InvoiceFields.RelatedAddress)] public long? RelatedAddress { get; set; }
    [JsonPropertyName(InvoiceFields.RelatedBookings)] public List<long>? RelatedBookings { get; set; }
    [JsonPropertyName(InvoiceFields.PayedFromUser)] public long? PayedFromUser { get; set; }
    [JsonPropertyName(InvoiceFields.ApprovedFromAdmin)] public long? ApprovedFromAdmin { get; set; }
    [JsonPropertyName(InvoiceFields.CanceledInvoice)] public long? CanceledInvoice { get; set; }
    [JsonPropertyName(InvoiceFields.BankAccount)] public long? BankAccount { get; set; }
    [JsonPropertyName(InvoiceFields.Gross)] public bool Gross { get; set; }
    [JsonPropertyName(InvoiceFields.CancellationDescription)] public string? CancellationDescription { get; set; }
    [JsonPropertyName(InvoiceFields.TemplateName)] public string? TemplateName { get; set; }
    [JsonPropertyName(InvoiceFields.RefNumber)] public string? RefNumber { get; set; }
    [JsonPropertyName(InvoiceFields.IsDraft)] public bool IsDraft { get; set; }
    [JsonPropertyName(InvoiceFields.IsTemplate)] public bool IsTemplate { get; set; }
    [JsonPropertyName(InvoiceFields.CreationDateForRecurringInvoices)] public DateTime? CreationDateForRecurringInvoices { get; set; }
    [JsonPropertyName(InvoiceFields.RecurringInvoicesInterval)] public int? RecurringInvoicesInterval { get; set; }
    [JsonPropertyName(InvoiceFields.PaymentInformation)] public string? PaymentInformation { get; set; }
    [JsonPropertyName(InvoiceFields.IsRequest)] public bool IsRequest { get; set; }
    [JsonPropertyName(InvoiceFields.TaxRate)] public decimal? TaxRate { get; set; }
    [JsonPropertyName(InvoiceFields.TaxName)] public string? TaxName { get; set; }
    [JsonPropertyName(InvoiceFields.ActualCallStateName)] public string? ActualCallStateName { get; set; }
    [JsonPropertyName(InvoiceFields.CallStateDelayDays)] public int? CallStateDelayDays { get; set; }
    [JsonPropertyName(InvoiceFields.AccountNumber)] public int? AccountNumber { get; set; }
    [JsonPropertyName(InvoiceFields.Guid)] public string? Guid { get; set; }
    [JsonPropertyName(InvoiceFields.SelectionAccount)] public int? SelectionAccount { get; set; }
    [JsonPropertyName(InvoiceFields.RemoveFileOnDelete)] public bool RemoveFileOnDelete { get; set; }
    [JsonPropertyName(InvoiceFields.CustomPaymentMethod)] public int? CustomPaymentMethod { get; set; }
    [JsonPropertyName(InvoiceFields.IsReceipt)] public bool IsReceipt { get; set; }
    [JsonPropertyName(InvoiceFields.Mode)] public string? Mode { get; set; }
    [JsonPropertyName(InvoiceFields.OfferStatus)] public string? OfferStatus { get; set; }
    [JsonPropertyName(InvoiceFields.OfferValidUntil)] public DateTime? OfferValidUntil { get; set; }
    [JsonPropertyName(InvoiceFields.OfferNumber)] public string? OfferNumber { get; set; }
    [JsonPropertyName(InvoiceFields.RelatedOffer)] public long? RelatedOffer { get; set; }
    [JsonPropertyName(InvoiceFields.ClosingDescription)] public string? ClosingDescription { get; set; }
    [JsonPropertyName(InvoiceFields.UseAddressBalance)] public bool UseAddressBalance { get; set; }
}
