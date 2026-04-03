using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class MemberTools
{
    private readonly IEasyVereinApiClient _client;

    public MemberTools(IEasyVereinApiClient client) { _client = client; }

    [McpServerTool, Description("Alle Mitglieder auflisten")]
    public async Task<string> ListMembers(CancellationToken ct)
    {
        var members = await _client.GetMembersAsync(ct);
        return JsonSerializer.Serialize(members, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Ein Mitglied anhand der ID abrufen")]
    public async Task<string> GetMember(long id, CancellationToken ct)
    {
        var member = await _client.GetMemberAsync(id, ct);
        return member != null
            ? JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true })
            : $"Mitglied mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neues Mitglied anlegen (erstellt ContactDetails automatisch)")]
    public async Task<string> CreateMember(
        string emailOrUserName, string firstName, string familyName,
        string? privateEmail, CancellationToken ct)
    {
        var contactDetails = new ContactDetails
        {
            FirstName = firstName,
            FamilyName = familyName,
            PrivateEmail = privateEmail
        };
        var created = await _client.CreateMemberAsync(emailOrUserName, contactDetails, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Mitglied aktualisieren")]
    public async Task<string> UpdateMember(
        long id, string? emailOrUserName, string? membershipNumber, CancellationToken ct)
    {
        var member = new Member { Id = id };
        if (emailOrUserName != null) member.EmailOrUserName = emailOrUserName;
        if (membershipNumber != null) member.MembershipNumber = membershipNumber;
        var updated = await _client.UpdateMemberAsync(id, member, ct);
        return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Mitglied löschen")]
    public async Task<string> DeleteMember(long id, CancellationToken ct)
    {
        await _client.DeleteMemberAsync(id, ct);
        return $"Mitglied mit ID {id} wurde gelöscht.";
    }
}
