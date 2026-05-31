using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing contact-details groups (address groups) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class ContactDetailsGroupTools(IEasyVereinApiClient client)
{
    /// <summary>Lists contact-details groups with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_contact_details_groups"), Description("List all contact-details groups (address groups)")]
    public async Task<string> ListContactDetailsGroups(
        [Description("Exact name filter")] string? name,
        [Description("Exact color filter (hex value, e.g. '#aabbcc')")] string? color,
        [Description("Exact short-label filter (max 4 chars)")] string? @short,
        [Description("Soft-delete filter (true to include deleted groups)")] bool? deleted,
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Ordering (e.g. 'name' or '-name')")] string? ordering,
        [Description("Search terms (allowed fields: name, short)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var groups = await client.ListContactDetailsGroupsAsync(
                name, color, @short, deleted, idIn, ordering, search, ct);
            return JsonSerializer.Serialize(groups, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single contact-details group by its unique identifier.</summary>
    [McpServerTool(Name = "get_contact_details_group"), Description("Retrieve a contact-details group by its ID")]
    public async Task<string> GetContactDetailsGroup(
        [Description("The ID of the contact-details group")] long id,
        CancellationToken ct)
    {
        try
        {
            var group = await client.GetContactDetailsGroupAsync(id, ct);
            return group != null
                ? JsonSerializer.Serialize(group, new JsonSerializerOptions { WriteIndented = true })
                : $"Contact-details group with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new contact-details group in easyVerein.</summary>
    [McpServerTool(Name = "create_contact_details_group"), Description("Create a new contact-details group (address group)")]
    public async Task<string> CreateContactDetailsGroup(
        [Description("The group name (required, max 200 chars)")] string name,
        [Description("Hex color (required for groups, max 7 chars, e.g. '#ff8800')")] string? color,
        [Description("Short label (required for groups, max 4 chars)")] string? @short,
        [Description("Order sequence (sort position)")] int? orderSequence,
        CancellationToken ct)
    {
        try
        {
            var group = new ContactDetailsGroup { Name = name };
            if (HasValue(color)) group.Color = color;
            if (HasValue(@short)) group.Short = @short;
            if (orderSequence.HasValue) group.OrderSequence = orderSequence;

            var created = await client.CreateContactDetailsGroupAsync(group, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing contact-details group (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_contact_details_group"), Description("Update a contact-details group (only provided fields are changed)")]
    public async Task<string> UpdateContactDetailsGroup(
        [Description("The ID of the contact-details group to update")] long id,
        [Description("New name")] string? name,
        [Description("New hex color")] string? color,
        [Description("New short label")] string? @short,
        [Description("New order sequence")] int? orderSequence,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[ContactDetailsGroupFields.Name] = name!;
            if (HasValue(color)) patch[ContactDetailsGroupFields.Color] = color!;
            if (HasValue(@short)) patch[ContactDetailsGroupFields.Short] = @short!;
            if (orderSequence.HasValue) patch[ContactDetailsGroupFields.OrderSequence] = orderSequence.Value;

            var updated = await client.UpdateContactDetailsGroupAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a contact-details group by its unique identifier.</summary>
    [McpServerTool(Name = "delete_contact_details_group"), Description("Delete a contact-details group. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteContactDetailsGroup(
        [Description("The ID of the contact-details group to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteContactDetailsGroupAsync(id, ct);
            return $"Contact-details group with ID {id} has been deleted.";
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
