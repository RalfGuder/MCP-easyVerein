using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing invoices via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class InvoiceTools
{
    private readonly IEasyVereinApiClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceTools"/> class.
    /// </summary>
    /// <param name="client">The easyVerein API client.</param>
    public InvoiceTools(IEasyVereinApiClient client) { _client = client; }

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
            var invoices = await _client.GetInvoicesAsync(ct);
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
        var invoice = await _client.GetInvoiceAsync(id, ct);
        return invoice != null
            ? JsonSerializer.Serialize(invoice, new JsonSerializerOptions { WriteIndented = true })
            : $"Invoice with ID {id} not found.";
    }

    /// <summary>
    /// Creates a new invoice in easyVerein.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="totalPrice">The total price of the invoice.</param>
    /// <param name="description">An optional description.</param>
    /// <param name="kind">The invoice type (e.g. "invoice", "credit").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created invoice.</returns>
    [McpServerTool(Name = "create_invoice"), Description("Creates a new invoice and retuns the new entry, or responds an error if something fails")]
    public async Task<string> CreateInvoice(
        string? invoiceNumber, decimal totalPrice, string? description, string? kind, CancellationToken ct)
    {
        try
        {
            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                TotalPrice = totalPrice,
                Description = description,
                Kind = kind
            };
            var created = await _client.CreateInvoiceAsync(invoice, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
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
        await _client.DeleteInvoiceAsync(id, ct);
        return $"Invoice with ID {id} has been deleted.";
    }
}
