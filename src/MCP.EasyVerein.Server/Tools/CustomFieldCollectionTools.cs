using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing custom field collections (groups/tabs of user-defined fields) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class CustomFieldCollectionTools(IEasyVereinApiClient client)
{
    /// <summary>Lists custom field collections with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_custom_field_collections"), Description("List all custom field collections (groups/tabs of user-defined fields)")]
    public async Task<string> ListCustomFieldCollections(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Position filter")] int? position,
        [Description("Ordering (e.g. 'name' or '-position')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var collections = await client.ListCustomFieldCollectionsAsync(idIn, position, ordering, search, ct);
            return JsonSerializer.Serialize(collections, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single custom field collection by its unique identifier.</summary>
    [McpServerTool(Name = "get_custom_field_collection"), Description("Retrieve a custom field collection by its ID")]
    public async Task<string> GetCustomFieldCollection(
        [Description("The ID of the custom field collection")] long id,
        CancellationToken ct)
    {
        try
        {
            var collection = await client.GetCustomFieldCollectionAsync(id, ct);
            return collection != null
                ? JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true })
                : $"Custom field collection with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new custom field collection in easyVerein.</summary>
    [McpServerTool(Name = "create_custom_field_collection"), Description("Create a new custom field collection (group/tab of user-defined fields)")]
    public async Task<string> CreateCustomFieldCollection(
        [Description("The collection name (required, max 200 chars)")] string name,
        [Description("Sort order of the collection")] int? orderSequence,
        [Description("Position within the member profile tab")] int? position,
        CancellationToken ct)
    {
        try
        {
            var collection = new CustomFieldCollection { Name = name };
            if (orderSequence.HasValue) collection.OrderSequence = orderSequence;
            if (position.HasValue) collection.Position = position;

            var created = await client.CreateCustomFieldCollectionAsync(collection, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing custom field collection (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_custom_field_collection"), Description("Update a custom field collection (only provided fields are changed)")]
    public async Task<string> UpdateCustomFieldCollection(
        [Description("The ID of the custom field collection to update")] long id,
        [Description("New name")] string? name,
        [Description("New sort order")] int? orderSequence,
        [Description("New position")] int? position,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[CustomFieldCollectionFields.Name] = name!;
            if (orderSequence.HasValue) patch[CustomFieldCollectionFields.OrderSequence] = orderSequence.Value;
            if (position.HasValue) patch[CustomFieldCollectionFields.Position] = position.Value;

            var updated = await client.UpdateCustomFieldCollectionAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a custom field collection by its unique identifier.</summary>
    [McpServerTool(Name = "delete_custom_field_collection"), Description("Delete a custom field collection. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteCustomFieldCollection(
        [Description("The ID of the custom field collection to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteCustomFieldCollectionAsync(id, ct);
            return $"Custom field collection with ID {id} has been deleted.";
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
