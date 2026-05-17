using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing invoice items (Rechnungspositionen) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class InvoiceItemTools(IEasyVereinApiClient client, EasyVereinConfiguration config)
{
    /// <summary>Lists invoice items with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_invoice_items"), Description("List all invoice items")]
    public async Task<string> ListInvoiceItems(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Parent invoice ID filter (numeric)")] string? relatedInvoice,
        [Description("Ordering (e.g. 'id' or '-id')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var items = await client.ListInvoiceItemsAsync(idIn, relatedInvoice, ordering, search, ct);
            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single invoice item by its unique identifier.</summary>
    [McpServerTool(Name = "get_invoice_item"), Description("Retrieve an invoice item by its ID")]
    public async Task<string> GetInvoiceItem(
        [Description("The ID of the invoice item")] long id,
        CancellationToken ct)
    {
        try
        {
            var item = await client.GetInvoiceItemAsync(id, ct);
            return item != null
                ? JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true })
                : $"Invoice item with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new invoice item.</summary>
    [McpServerTool(Name = "create_invoice_item"), Description("Create a new invoice item")]
    public async Task<string> CreateInvoiceItem(
        [Description("Parent invoice ID (numeric, required)")] long relatedInvoiceId,
        [Description("Item title (required)")] string title,
        [Description("Quantity (decimal, default 1.00)")] decimal? quantity,
        [Description("Unit price (decimal, default 0)")] decimal? unitPrice,
        [Description("Item description (optional)")] string? description,
        [Description("Tax rate (decimal, default 0)")] decimal? taxRate,
        [Description("Tax name (optional)")] string? taxName,
        [Description("Gross-pricing flag (default false)")] bool? gross,
        [Description("Billing-account ID (numeric, optional)")] long? billingAccountId,
        [Description("SKR42 sphere (1=ideell, 2=Vermögensverwaltung, 3=Zweckbetrieb, 4=wGB, 9=unkategorisiert default)")] int? sphere,
        [Description("Cost centre / Kostenstelle (optional)")] string? costCentre,
        CancellationToken ct)
    {
        try
        {
            var item = new InvoiceItem
            {
                RelatedInvoice = $"{config.GetVersionedBaseUrl()}/invoice/{relatedInvoiceId}",
                Title = title,
                Quantity = quantity ?? 1m,
                UnitPrice = unitPrice ?? 0m,
                Description = description,
                TaxRate = taxRate ?? 0m,
                TaxName = taxName,
                Gross = gross ?? false,
                Sphere = sphere ?? 9,
                CostCentre = costCentre ?? string.Empty
            };
            if (billingAccountId.HasValue)
                item.BillingAccount = $"{config.GetVersionedBaseUrl()}/billing-account/{billingAccountId.Value}";

            var created = await client.CreateInvoiceItemAsync(item, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing invoice item (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_invoice_item"), Description("Update an invoice item (only provided fields are changed)")]
    public async Task<string> UpdateInvoiceItem(
        [Description("The ID of the invoice item to update")] long id,
        [Description("New title")] string? title,
        [Description("New description")] string? description,
        [Description("New quantity")] decimal? quantity,
        [Description("New unit price")] decimal? unitPrice,
        [Description("New tax rate")] decimal? taxRate,
        [Description("New billing-account ID (numeric)")] long? billingAccountId,
        [Description("New SKR42 sphere")] int? sphere,
        [Description("New cost centre")] string? costCentre,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(title)) patch[InvoiceItemFields.Title] = title!;
            if (HasValue(description)) patch[InvoiceItemFields.Description] = description!;
            if (quantity.HasValue) patch[InvoiceItemFields.Quantity] = quantity.Value;
            if (unitPrice.HasValue) patch[InvoiceItemFields.UnitPrice] = unitPrice.Value;
            if (taxRate.HasValue) patch[InvoiceItemFields.TaxRate] = taxRate.Value;
            if (billingAccountId.HasValue)
                patch[InvoiceItemFields.BillingAccount] = $"{config.GetVersionedBaseUrl()}/billing-account/{billingAccountId.Value}";
            if (sphere.HasValue) patch[InvoiceItemFields.Sphere] = sphere.Value;
            if (HasValue(costCentre)) patch[InvoiceItemFields.CostCentre] = costCentre!;

            var updated = await client.UpdateInvoiceItemAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes an invoice item by its unique identifier.</summary>
    [McpServerTool(Name = "delete_invoice_item"), Description("Delete an invoice item. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteInvoiceItem(
        [Description("The ID of the invoice item to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteInvoiceItemAsync(id, ct);
            return $"Invoice item with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
