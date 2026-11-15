using System.ComponentModel;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing contact details via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class ContactDetailsTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists contact details with optional filters for ID, first name, family name, and search terms.
    /// </summary>
    /// <param name="id">Optional contact details ID filter.</param>
    /// <param name="firstName">Optional first name filter.</param>
    /// <param name="familyName">Optional family name filter.</param>
    /// <param name="search">Optional search terms (OR-combined).</param>
    /// <param name="limit">Entries per page (1-100).</param>
    /// <param name="page">Page number, starting at 1.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON page of matching contact details, or an error message.</returns>
    [McpServerTool, Description("List all contact details. Paged: limit (1-100, default 50) and page (from 1); the response holds items, totalCount and hasMore - request the next page while hasMore is true.")]
    public async Task<string> ListContactDetails(
        [Description("Contact details ID filter")] long? id,
        [Description("Exact first name filter")] string? firstName,
        [Description("Exact family name filter")] string? familyName,
        [Description("Search terms; each term is matched against name, company and other searchable fields, several terms are combined with OR")] string[]? search,
        [Description("Entries per page (1-100, default 50)")] int limit = PageRequest.DefaultLimit,
        [Description("Page number, starting at 1 (default 1)")] int page = PageRequest.FirstPage,
        CancellationToken ct = default)
    {
        try
        {
            var contacts = await client.ListContactDetailsAsync(id, firstName, familyName, search, ToolJson.ToPageRequest(limit, page), ct);
            return ToolJson.SerializePage(contacts);
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
            ? ToolJson.Serialize(contact)
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
        return ToolJson.Serialize(created);
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
    /// <param name="contactDetails">The contact details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated contact details, or an error message.</returns>
    [McpServerTool, Description("Update contact details")]
    public async Task<string> UpdateContactDetails(long id, ContactDetails contactDetails, CancellationToken ct)
    {
        try
        {

            var updated = await client.UpdateContactDetailsAsync(id, contactDetails, ct);
            return ToolJson.Serialize(updated);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
