using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class ContactDetailsTools(IEasyVereinApiClient client)
{
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

    [McpServerTool, Description("Retrieve contact details by ID")]
    public async Task<string> GetContactDetails(long id, CancellationToken ct)
    {
        var contact = await client.GetContactDetailsAsync(id, ct);
        return contact != null
            ? JsonSerializer.Serialize(contact, new JsonSerializerOptions { WriteIndented = true })
            : $"Contact details with ID {id} not found.";
    }

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

    [McpServerTool, Description("Delete contact details")]
    public async Task<string> DeleteContactDetails(long id, CancellationToken ct)
    {
        await client.DeleteContactDetailsAsync(id, ct);
        return $"Contact details with ID {id} have been deleted.";
    }
    
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
