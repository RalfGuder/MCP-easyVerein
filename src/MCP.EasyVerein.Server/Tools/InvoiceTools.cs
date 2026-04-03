using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class InvoiceTools
{
    private readonly IEasyVereinApiClient _client;

    public InvoiceTools(IEasyVereinApiClient client) { _client = client; }

    [McpServerTool, Description("Alle Rechnungen auflisten")]
    public async Task<string> ListInvoices(CancellationToken ct)
    {
        var invoices = await _client.GetInvoicesAsync(ct);
        return JsonSerializer.Serialize(invoices, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Eine Rechnung anhand der ID abrufen")]
    public async Task<string> GetInvoice(long id, CancellationToken ct)
    {
        var invoice = await _client.GetInvoiceAsync(id, ct);
        return invoice != null
            ? JsonSerializer.Serialize(invoice, new JsonSerializerOptions { WriteIndented = true })
            : $"Rechnung mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neue Rechnung anlegen")]
    public async Task<string> CreateInvoice(
        string? invoiceNumber, decimal totalPrice, string? description, string? kind, CancellationToken ct)
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

    [McpServerTool, Description("Rechnung löschen")]
    public async Task<string> DeleteInvoice(long id, CancellationToken ct)
    {
        await _client.DeleteInvoiceAsync(id, ct);
        return $"Rechnung mit ID {id} wurde gelöscht.";
    }
}
