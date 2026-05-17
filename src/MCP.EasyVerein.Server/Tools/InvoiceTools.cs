using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing invoices via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class InvoiceTools(IEasyVereinApiClient client, EasyVereinConfiguration config)
{
    /// <summary>
    /// Lists all invoices from the easyVerein API with automatic pagination.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing all invoices, or an error message.</returns>
    [McpServerTool(Name = "list_invoices"), Description("List all invoices")]
    public async Task<string> ListInvoices(CancellationToken ct)
    {
        try
        {
            var invoices = await client.GetInvoicesAsync(ct);
            return JsonSerializer.Serialize(invoices, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single invoice by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the invoice, or a not-found message.</returns>
    [McpServerTool(Name = "get_invoice"), Description("Retrieve an invoice by its ID")]
    public async Task<string> GetInvoice(long id, CancellationToken ct)
    {
        var invoice = await client.GetInvoiceAsync(id, ct);
        return invoice != null
            ? JsonSerializer.Serialize(invoice, new JsonSerializerOptions { WriteIndented = true })
            : $"Invoice with ID {id} not found.";
    }

    /// <summary>
    /// Creates a new invoice in easyVerein.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="totalPrice">The total price of the invoice.</param>
    /// <param name="description">Payment description / statement text.</param>
    /// <param name="kind">Invoice kind. Allowed: 'Balance', 'Donation', 'Membership', 'Revenue', 'Expense', 'Cancel', 'Credit', 'Selfissuedreceipt'.</param>
    /// <param name="refNumber">Invoice-specific reference number.</param>
    /// <param name="paymentInformation">Payment information. Allowed: 'Nothing', 'Account', 'Debit', 'Cash'.</param>
    /// <param name="actualCallStateName">Name of the current dunning/call state.</param>
    /// <param name="callStateDelayDays">Delay in days for the current dunning state.</param>
    /// <param name="accnumber">DATEV account number.</param>
    /// <param name="guid">DATEV invoice GUID.</param>
    /// <param name="mode">Invoice mode (e.g. 'invoice', 'offer', 'offer_template').</param>
    /// <param name="offerStatus">Offer status (only relevant when mode is an offer variant).</param>
    /// <param name="receiver">Free-text receiver/payee for the invoice.</param>
    /// <param name="relatedAddressId">Numeric ID of the related contact-details entry; the tool builds the full resource URL.</param>
    /// <param name="relatedBookingIds">Optional numeric IDs of related bookings; the tool builds the full resource URLs.</param>
    /// <param name="isReceipt">If <c>true</c>, mark this invoice as a receipt (Beleg).</param>
    /// <param name="isDraft">If <c>true</c>, create the invoice as a draft.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created invoice.</returns>
    [McpServerTool(Name = "create_invoice"), Description("Creates a new invoice and returns the new entry, or responds an error if something fails. Supported kinds: Balance, Donation, Membership, Revenue, Expense, Cancel, Credit, Selfissuedreceipt. Supported paymentInformation: Nothing, Account, Debit, Cash. Supported modes: invoice, offer, offer_template. Supports optional receiver, relatedAddressId, relatedBookingIds, isReceipt, isDraft.")]
    public async Task<string> CreateInvoice(
        string? invoiceNumber,
        decimal totalPrice,
        string? description,
        string? kind,
        string? refNumber,
        string? paymentInformation,
        string? actualCallStateName,
        int? callStateDelayDays,
        int? accnumber,
        string? guid,
        string? mode,
        string? offerStatus,
        string? receiver,
        long? relatedAddressId,
        long[]? relatedBookingIds,
        bool? isReceipt,
        bool? isDraft,
        CancellationToken ct)
    {
        try
        {
            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                TotalPrice = totalPrice,
                Description = description,
                Kind = kind,
                RefNumber = refNumber,
                PaymentInformation = paymentInformation,
                ActualCallStateName = actualCallStateName,
                CallStateDelayDays = callStateDelayDays,
                AccountNumber = accnumber,
                Guid = guid,
                Mode = mode,
                OfferStatus = offerStatus,
                Receiver = receiver,
                RelatedAddress = relatedAddressId.HasValue
                    ? $"{config.GetVersionedBaseUrl()}/contact-details/{relatedAddressId.Value}"
                    : null,
                RelatedBookings = relatedBookingIds?
                    .Select(id => $"{config.GetVersionedBaseUrl()}/booking/{id}")
                    .ToList(),
                IsReceipt = isReceipt ?? false,
                IsDraft = isDraft ?? false
            };
            var created = await client.CreateInvoiceAsync(invoice, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }

    }

    /// <summary>
    /// Updates an existing invoice. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to update.</param>
    /// <param name="description">New description.</param>
    /// <param name="totalPrice">New total price.</param>
    /// <param name="receiver">New receiver text.</param>
    /// <param name="relatedAddressId">New related-address ID; the tool builds the full resource URL.</param>
    /// <param name="relatedBookingIds">New related bookings (numeric IDs); the tool builds the full resource URLs.</param>
    /// <param name="kind">New invoice kind.</param>
    /// <param name="paymentInformation">New payment information.</param>
    /// <param name="isReceipt">New isReceipt flag.</param>
    /// <param name="isDraft">New isDraft flag (set to <c>false</c> to finalize a draft).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated invoice, or an error message.</returns>
    [McpServerTool(Name = "update_invoice"), Description("Update an invoice (only provided fields are changed). Use this to finalize drafts (isDraft=false) or link to bookings (relatedBookingIds).")]
    public async Task<string> UpdateInvoice(
        [Description("The ID of the invoice to update")] long id,
        [Description("New description")] string? description,
        [Description("New total price")] decimal? totalPrice,
        [Description("New receiver text")] string? receiver,
        [Description("New related-address ID (numeric, optional)")] long? relatedAddressId,
        [Description("New related bookings (numeric IDs, optional)")] long[]? relatedBookingIds,
        [Description("New invoice kind")] string? kind,
        [Description("New payment information")] string? paymentInformation,
        [Description("New isReceipt flag")] bool? isReceipt,
        [Description("New isDraft flag (set to false to finalize a draft)")] bool? isDraft,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(description)) patch[InvoiceFields.Description] = description!;
            if (totalPrice.HasValue) patch[InvoiceFields.TotalPrice] = totalPrice.Value;
            if (HasValue(receiver)) patch[InvoiceFields.Receiver] = receiver!;
            if (relatedAddressId.HasValue)
                patch[InvoiceFields.RelatedAddress] = $"{config.GetVersionedBaseUrl()}/contact-details/{relatedAddressId.Value}";
            if (relatedBookingIds is not null)
                patch[InvoiceFields.RelatedBookings] = relatedBookingIds
                    .Select(b => $"{config.GetVersionedBaseUrl()}/booking/{b}")
                    .ToList();
            if (HasValue(kind)) patch[InvoiceFields.Kind] = kind!;
            if (HasValue(paymentInformation)) patch[InvoiceFields.PaymentInformation] = paymentInformation!;
            if (isReceipt.HasValue) patch[InvoiceFields.IsReceipt] = isReceipt.Value;
            if (isDraft.HasValue) patch[InvoiceFields.IsDraft] = isDraft.Value;

            var updated = await client.UpdateInvoiceAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes an invoice by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message.</returns>
    [McpServerTool(Name = "delete_invoice"), Description("Delete a invoice. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteInvoice(long id, CancellationToken ct)
    {
        await client.DeleteInvoiceAsync(id, ct);
        return $"Invoice with ID {id} has been deleted.";
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    /// <param name="value">The string value to test.</param>
    /// <returns><c>true</c> if the value is a non-empty string different from "null"; otherwise <c>false</c>.</returns>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
