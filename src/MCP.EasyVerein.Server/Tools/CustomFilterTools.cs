using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing saved custom filters via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class CustomFilterTools(IEasyVereinApiClient client)
{
    /// <summary>Description of the allowed filter models, shared by several tool parameters.</summary>
    private const string ModelDescription =
        "Filter model defining the filtered table: 'userFilter', 'userStatisticsFilter', 'addressStatisticsFilter', " +
        "'inventoryStatisticsFilter', 'bookingFilter', 'addressFilter', 'invoiceFilter', 'eventFilter', 'inventoryFilter', " +
        "'oposFilter', 'protocolFilter', 'addressLogFilter', 'taskFilter', 'mailingsFilter', 'changeLogsFilter', " +
        "'emailLogsFilter', 'loginLogsFilter'";

    /// <summary>Description of the rules JSON format, shared by the create and update tools.</summary>
    private const string RulesDescription =
        "Filter rules as JSON object text with the keys 'condition' ('AND'/'OR') and 'rules', e.g. " +
        "{\"condition\":\"AND\",\"rules\":[{\"id\":\"date\",\"field\":\"date\",\"type\":\"date\",\"input\":\"text\",\"operator\":\"greater_or_equal\",\"value\":\"2024-01-01\"}]}";

    /// <summary>Lists custom filters with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_custom_filters"), Description("List all saved custom filters")]
    public async Task<string> ListCustomFilters(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Exact name filter")] string? name,
        [Description("Filter-model filter (e.g. 'bookingFilter', 'userFilter')")] string? model,
        [Description("Comma-separated list of filter models")] string? modelIn,
        [Description("Ordering (e.g. 'name' or '-created_at')")] string? ordering,
        [Description("Search terms (allowed field: name)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var filters = await client.ListCustomFiltersAsync(idIn, name, model, modelIn, ordering, search, ct);
            return JsonSerializer.Serialize(filters, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single custom filter by its unique identifier.</summary>
    [McpServerTool(Name = "get_custom_filter"), Description("Retrieve a saved custom filter by its ID")]
    public async Task<string> GetCustomFilter(
        [Description("The ID of the custom filter")] long id,
        CancellationToken ct)
    {
        try
        {
            var filter = await client.GetCustomFilterAsync(id, ct);
            return filter != null
                ? JsonSerializer.Serialize(filter, new JsonSerializerOptions { WriteIndented = true })
                : $"Custom filter with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new custom filter in easyVerein.</summary>
    [McpServerTool(Name = "create_custom_filter"), Description("Create a new saved custom filter (name, model and rules are required)")]
    public async Task<string> CreateCustomFilter(
        [Description("The filter name (required, max 64 chars)")] string name,
        [Description(ModelDescription)] string model,
        [Description(RulesDescription)] string rules,
        CancellationToken ct)
    {
        try
        {
            var filter = new CustomFilter { Name = name, Model = model, Rules = ParseRules(rules) };
            var created = await client.CreateCustomFilterAsync(filter, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing custom filter (PATCH — only provided fields are changed).
    /// When rules are changed without a model, the current model is looked up and sent along,
    /// because the API rejects a rules change without the model.
    /// </summary>
    [McpServerTool(Name = "update_custom_filter"), Description("Update a saved custom filter (only provided fields are changed)")]
    public async Task<string> UpdateCustomFilter(
        [Description("The ID of the custom filter to update")] long id,
        [Description("New name")] string? name,
        [Description("New filter model. " + ModelDescription)] string? model,
        [Description("New rules. " + RulesDescription + ". The current model is sent along automatically if no model is given.")] string? rules,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[CustomFilterFields.Name] = name!;
            if (HasValue(model)) patch[CustomFilterFields.Model] = model!;
            if (HasValue(rules))
            {
                patch[CustomFilterFields.Rules] = ParseRules(rules!);
                if (!patch.ContainsKey(CustomFilterFields.Model))
                {
                    var current = await client.GetCustomFilterAsync(id, ct);
                    if (current?.Model == null)
                        return $"ERROR: Custom filter with ID {id} not found or has no model; cannot update rules.";
                    patch[CustomFilterFields.Model] = current.Model;
                }
            }

            var updated = await client.UpdateCustomFilterAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a custom filter by its unique identifier.</summary>
    [McpServerTool(Name = "delete_custom_filter"), Description("Delete a saved custom filter permanently. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteCustomFilter(
        [Description("The ID of the custom filter to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteCustomFilterAsync(id, ct);
            return $"Custom filter with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Parses the rules JSON text into a detached JSON object element.</summary>
    /// <exception cref="ArgumentException">Thrown when the text is not a JSON object.</exception>
    private static JsonElement ParseRules(string rules)
    {
        using var doc = JsonDocument.Parse(rules);
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
            throw new ArgumentException("Rules must be a JSON object with the keys 'condition' and 'rules'.", nameof(rules));
        return doc.RootElement.Clone();
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
