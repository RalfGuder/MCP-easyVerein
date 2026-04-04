using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing contact details via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class ContactDetailsTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists contact details with optional filters for ID, first name, family name, and full name.
    /// </summary>
    /// <param name="id">Optional contact details ID filter.</param>
    /// <param name="firstName">Optional first name filter.</param>
    /// <param name="familyName">Optional family name filter.</param>
    /// <param name="fullName">Optional full name filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching contact details, or an error message.</returns>
    [McpServerTool, Description("List all contact details")]
    public async Task<string> ListContactDetails(long? id, string? firstName, string? familyName, string? fullName, CancellationToken ct)
    {
        try
        {
            var contacts = await client.ListContactDetailsAsync(id, firstName, familyName, fullName, ct);
            return JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single contact details entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contact details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the contact details, or a not-found message.</returns>
    [McpServerTool, Description("Retrieve contact details by ID")]
    public async Task<string> GetContactDetails(long id, CancellationToken ct)
    {
        var contact = await client.GetContactDetailsAsync(id, ct);
        return contact != null
            ? JsonSerializer.Serialize(contact, new JsonSerializerOptions { WriteIndented = true })
            : $"Contact details with ID {id} not found.";
    }

    /// <summary>
    /// Creates new contact details in easyVerein.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="familyName">The family name.</param>
    /// <param name="privateEmail">An optional private email address.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created contact details.</returns>
    [McpServerTool, Description("Create new contact details")]
    public async Task<string> CreateContactDetails(
        string firstName, string familyName, string? privateEmail, CancellationToken ct)
    {
        var contact = new ContactDetails
        {
            FirstName = firstName,
            FamilyName = familyName,
            PrivateEmail = privateEmail
        };
        var created = await client.CreateContactDetailsAsync(contact, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Deletes contact details by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contact details to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message.</returns>
    [McpServerTool, Description("Delete contact details")]
    public async Task<string> DeleteContactDetails(long id, CancellationToken ct)
    {
        await client.DeleteContactDetailsAsync(id, ct);
        return $"Contact details with ID {id} have been deleted.";
    }

    /// <summary>
    /// Updates existing contact details. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the contact details to update.</param>
    /// <param name="firstName">An optional new first name.</param>
    /// <param name="familyName">An optional new family name.</param>
    /// <param name="privateEmail">An optional new private email address.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated contact details, or an error message.</returns>
    [McpServerTool, Description("Update contact details")]
    public async Task<string> UpdateContactDetails(long id, string? firstName, string? familyName, string? privateEmail, CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (firstName != null) patch["firstName"] = firstName;
            if (familyName != null) patch["familyName"] = familyName;
            if (privateEmail != null) patch["privateEmail"] = privateEmail;

            var updated = await client.UpdateContactDetailsAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
