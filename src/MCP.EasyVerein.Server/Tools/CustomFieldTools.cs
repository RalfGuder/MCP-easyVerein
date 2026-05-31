using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing custom fields (user-defined fields) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class CustomFieldTools(IEasyVereinApiClient client)
{
    /// <summary>Lists custom fields with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_custom_fields"), Description("List all custom fields (user-defined fields)")]
    public async Task<string> ListCustomFields(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Exact name filter")] string? name,
        [Description("Exact color filter (hex value)")] string? color,
        [Description("Kind-code filter (e.g. 'E','H','J','I')")] string? kind,
        [Description("Field-type filter (e.g. 'T','Z','D','C','S')")] string? settingsType,
        [Description("Comma-separated list of field types filter")] string? settingsTypeIn,
        [Description("Editable-in-member-area filter")] bool? memberEdit,
        [Description("Show-in-member-area filter")] bool? memberShow,
        [Description("Soft-delete filter (true to include deleted fields)")] bool? deleted,
        [Description("Collection (tab/group) ID filter")] string? collection,
        [Description("No-collection filter (true for fields in no tab)")] bool? collectionIsnull,
        [Description("Ordering (e.g. 'name' or '-position')")] string? ordering,
        [Description("Search terms (allowed fields: name, color, short)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var fields = await client.ListCustomFieldsAsync(
                idIn, name, color, kind, settingsType, settingsTypeIn, memberEdit,
                memberShow, deleted, collection, collectionIsnull, ordering, search, ct);
            return JsonSerializer.Serialize(fields, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single custom field by its unique identifier.</summary>
    [McpServerTool(Name = "get_custom_field"), Description("Retrieve a custom field by its ID")]
    public async Task<string> GetCustomField(
        [Description("The ID of the custom field")] long id,
        CancellationToken ct)
    {
        try
        {
            var field = await client.GetCustomFieldAsync(id, ct);
            return field != null
                ? JsonSerializer.Serialize(field, new JsonSerializerOptions { WriteIndented = true })
                : $"Custom field with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new custom field in easyVerein.</summary>
    [McpServerTool(Name = "create_custom_field"), Description("Create a new custom field (user-defined field)")]
    public async Task<string> CreateCustomField(
        [Description("The field name (required, max 200 chars)")] string name,
        [Description("Hex color (max 7 chars; required for groups)")] string? color,
        [Description("Short label (max 4 chars; required for groups, unique)")] string? @short,
        [Description("Field type code (e.g. 'T','F','Z','D','C','R','S','A','B','M'; default 'T')")] string? settingsType,
        [Description("Kind code (custom fields: 'E'=members, 'H'=events, 'J'=contact-details, 'I'=inventory)")] string? kind,
        [Description("Description text (max 124 chars)")] string? description,
        [Description("Additional metadata")] string? additional,
        [Description("Show field in member area (default false)")] bool? memberShow,
        [Description("Make field editable in member area (default false)")] bool? memberEdit,
        [Description("Mark as GDPR consent field (default false)")] bool? memberDsgvo,
        [Description("Position within the tab/group (default 0)")] int? position,
        [Description("Related collection ID (tab/group)")] long? collection,
        [Description("Changes require admin approval (default false)")] bool? needsAdminApproval,
        CancellationToken ct)
    {
        try
        {
            var field = new CustomField { Name = name };
            if (HasValue(color)) field.Color = color;
            if (HasValue(@short)) field.Short = @short;
            if (HasValue(settingsType)) field.SettingsType = settingsType;
            if (HasValue(kind)) field.Kind = kind;
            if (HasValue(description)) field.Description = description;
            if (HasValue(additional)) field.Additional = additional;
            if (memberShow.HasValue) field.MemberShow = memberShow;
            if (memberEdit.HasValue) field.MemberEdit = memberEdit;
            if (memberDsgvo.HasValue) field.MemberDsgvo = memberDsgvo;
            if (position.HasValue) field.Position = position;
            if (collection.HasValue) field.Collection = collection;
            if (needsAdminApproval.HasValue) field.NeedsAdminApproval = needsAdminApproval;

            var created = await client.CreateCustomFieldAsync(field, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing custom field (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_custom_field"), Description("Update a custom field (only provided fields are changed)")]
    public async Task<string> UpdateCustomField(
        [Description("The ID of the custom field to update")] long id,
        [Description("New name")] string? name,
        [Description("New hex color")] string? color,
        [Description("New short label")] string? @short,
        [Description("New field type code")] string? settingsType,
        [Description("New kind code")] string? kind,
        [Description("New description")] string? description,
        [Description("New additional metadata")] string? additional,
        [Description("New show-in-member-area flag")] bool? memberShow,
        [Description("New editable-in-member-area flag")] bool? memberEdit,
        [Description("New GDPR-consent flag")] bool? memberDsgvo,
        [Description("New position")] int? position,
        [Description("New related collection ID")] long? collection,
        [Description("New needs-admin-approval flag")] bool? needsAdminApproval,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[CustomFieldFields.Name] = name!;
            if (HasValue(color)) patch[CustomFieldFields.Color] = color!;
            if (HasValue(@short)) patch[CustomFieldFields.Short] = @short!;
            if (HasValue(settingsType)) patch[CustomFieldFields.SettingsType] = settingsType!;
            if (HasValue(kind)) patch[CustomFieldFields.Kind] = kind!;
            if (HasValue(description)) patch[CustomFieldFields.Description] = description!;
            if (HasValue(additional)) patch[CustomFieldFields.Additional] = additional!;
            if (memberShow.HasValue) patch[CustomFieldFields.MemberShow] = memberShow.Value;
            if (memberEdit.HasValue) patch[CustomFieldFields.MemberEdit] = memberEdit.Value;
            if (memberDsgvo.HasValue) patch[CustomFieldFields.MemberDsgvo] = memberDsgvo.Value;
            if (position.HasValue) patch[CustomFieldFields.Position] = position.Value;
            if (collection.HasValue) patch[CustomFieldFields.Collection] = collection.Value;
            if (needsAdminApproval.HasValue) patch[CustomFieldFields.NeedsAdminApproval] = needsAdminApproval.Value;

            var updated = await client.UpdateCustomFieldAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a custom field by its unique identifier.</summary>
    [McpServerTool(Name = "delete_custom_field"), Description("Delete a custom field. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteCustomField(
        [Description("The ID of the custom field to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteCustomFieldAsync(id, ct);
            return $"Custom field with ID {id} has been deleted.";
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
