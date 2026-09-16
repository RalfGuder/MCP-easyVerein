using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing tax rates (standard and organization-specific) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class CustomTaxRateTools(IEasyVereinApiClient client)
{
    /// <summary>Lowest tax-rate percentage accepted by the API.</summary>
    private const decimal MinRate = 0m;

    /// <summary>Highest tax-rate percentage accepted by the API.</summary>
    private const decimal MaxRate = 100m;

    /// <summary>Lists tax rates with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_custom_tax_rates"), Description("List tax rates (standard rates per country and organization-specific custom rates)")]
    public async Task<string> ListCustomTaxRates(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Exact tax-rate label filter")] string? taxName,
        [Description("Tax-rate label to exclude")] string? taxNameNe,
        [Description("Percentage filter (e.g. '19')")] string? customTaxRate,
        [Description("Percentage to exclude (e.g. '0')")] string? customTaxRateNe,
        [Description("true = only standard rates, false = only organization-specific rates")] bool? orgIsnull,
        [Description("Soft-delete filter (true to list deleted rates)")] bool? deleted,
        [Description("true = only rates the organization is allowed to use")] bool? showAllowedToUse,
        [Description("Ordering (e.g. 'customTaxRate' or '-customTaxRate')")] string? ordering,
        [Description("Search terms (allowed field: taxName)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var rates = await client.ListCustomTaxRatesAsync(
                idIn, taxName, taxNameNe, customTaxRate, customTaxRateNe,
                orgIsnull, deleted, showAllowedToUse, ordering, search, ct);
            return JsonSerializer.Serialize(rates, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single tax rate by its unique identifier.</summary>
    [McpServerTool(Name = "get_custom_tax_rate"), Description("Retrieve a tax rate by its ID")]
    public async Task<string> GetCustomTaxRate(
        [Description("The ID of the tax rate")] long id,
        CancellationToken ct)
    {
        try
        {
            var rate = await client.GetCustomTaxRateAsync(id, ct);
            return rate != null
                ? JsonSerializer.Serialize(rate, new JsonSerializerOptions { WriteIndented = true })
                : $"Tax rate with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new organization-specific tax rate in easyVerein.</summary>
    [McpServerTool(Name = "create_custom_tax_rate"), Description("Create a new organization-specific tax rate")]
    public async Task<string> CreateCustomTaxRate(
        [Description("The tax-rate label (required, max 600 chars)")] string taxName,
        [Description("The tax-rate percentage (required, 0–100, e.g. 7 or 19)")] decimal customTaxRate,
        CancellationToken ct)
    {
        try
        {
            if (!IsValidRate(customTaxRate))
                return $"ERROR: Tax rate {customTaxRate} is out of range; allowed are values from {MinRate} to {MaxRate}.";

            var rate = new CustomTaxRate { TaxName = taxName, CustomTaxRateValue = customTaxRate };
            var created = await client.CreateCustomTaxRateAsync(rate, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an organization-specific tax rate (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_custom_tax_rate"), Description("Update an organization-specific tax rate (only provided fields are changed)")]
    public async Task<string> UpdateCustomTaxRate(
        [Description("The ID of the tax rate to update")] long id,
        [Description("New tax-rate label")] string? taxName,
        [Description("New tax-rate percentage (0–100)")] decimal? customTaxRate,
        CancellationToken ct)
    {
        try
        {
            if (customTaxRate.HasValue && !IsValidRate(customTaxRate.Value))
                return $"ERROR: Tax rate {customTaxRate} is out of range; allowed are values from {MinRate} to {MaxRate}.";

            var patch = new Dictionary<string, object>();
            if (HasValue(taxName)) patch[CustomTaxRateFields.TaxName] = taxName!;
            if (customTaxRate.HasValue) patch[CustomTaxRateFields.CustomTaxRate] = customTaxRate.Value;

            var updated = await client.UpdateCustomTaxRateAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes an organization-specific tax rate by its unique identifier.</summary>
    [McpServerTool(Name = "delete_custom_tax_rate"), Description("Delete an organization-specific tax rate (moves it to the wastebasket). Only authorized users are able to perform this action!")]
    public async Task<string> DeleteCustomTaxRate(
        [Description("The ID of the tax rate to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteCustomTaxRateAsync(id, ct);
            return $"Tax rate with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a percentage lies within the range accepted by the API.</summary>
    private static bool IsValidRate(decimal rate) => rate is >= MinRate and <= MaxRate;

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
