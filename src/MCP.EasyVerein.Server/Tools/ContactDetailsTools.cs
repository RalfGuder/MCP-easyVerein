using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class ContactDetailsTools
{
    private readonly IEasyVereinApiClient _client;

    public ContactDetailsTools(IEasyVereinApiClient client) { _client = client; }

    [McpServerTool, Description("Alle Kontaktdaten auflisten")]
    public async Task<string> ListContactDetails(CancellationToken ct)
    {
        var contacts = await _client.GetContactDetailsAsync(ct);
        return JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Kontaktdaten anhand der ID abrufen")]
    public async Task<string> GetContactDetails(long id, CancellationToken ct)
    {
        var contact = await _client.GetContactDetailsAsync(id, ct);
        return contact != null
            ? JsonSerializer.Serialize(contact, new JsonSerializerOptions { WriteIndented = true })
            : $"Kontaktdaten mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neue Kontaktdaten anlegen")]
    public async Task<string> CreateContactDetails(
        string firstName, string familyName, string? privateEmail, CancellationToken ct)
    {
        var contact = new ContactDetails
        {
            FirstName = firstName,
            FamilyName = familyName,
            PrivateEmail = privateEmail
        };
        var created = await _client.CreateContactDetailsAsync(contact, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Kontaktdaten löschen")]
    public async Task<string> DeleteContactDetails(long id, CancellationToken ct)
    {
        await _client.DeleteContactDetailsAsync(id, ct);
        return $"Kontaktdaten mit ID {id} wurden gelöscht.";
    }
}
