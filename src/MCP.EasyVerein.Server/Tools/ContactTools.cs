using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class ContactTools
{
    private readonly IEasyVereinApiClient _client;

    public ContactTools(IEasyVereinApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Alle Kontakte auflisten")]
    public async Task<string> ListContacts(CancellationToken ct)
    {
        var contacts = await _client.GetContactsAsync(ct);
        return JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Einen Kontakt anhand der ID abrufen")]
    public async Task<string> GetContact(long id, CancellationToken ct)
    {
        var contact = await _client.GetContactAsync(id, ct);
        return contact != null
            ? JsonSerializer.Serialize(contact, new JsonSerializerOptions { WriteIndented = true })
            : $"Kontakt mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neuen Kontakt anlegen")]
    public async Task<string> CreateContact(string firstName, string lastName, string? email, CancellationToken ct)
    {
        var contact = new Contact { FirstName = firstName, LastName = lastName, Email = email };
        var created = await _client.CreateContactAsync(contact, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Kontakt löschen")]
    public async Task<string> DeleteContact(long id, CancellationToken ct)
    {
        await _client.DeleteContactAsync(id, ct);
        return $"Kontakt mit ID {id} wurde gelöscht.";
    }
}
