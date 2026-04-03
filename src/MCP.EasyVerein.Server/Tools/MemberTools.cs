using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class MemberTools(IEasyVereinApiClient client)
{
    [McpServerTool(Name = "list_members"), Description("List all members")]
    public async Task<string> ListMembers(
        [Description("The ID of a member")]long? id, 
        [Description("The membership number of a member")] string? membershipNumber,
        string[]? search , CancellationToken ct)
    {
        try
        {
            var members = await client.ListMembersAsync(id, membershipNumber, search, ct);
            return JsonSerializer.Serialize(members, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    [McpServerTool(Name="get_member"), Description("Retrieve a member using their ID.")]
    public async Task<string> GetMember([Description("The ID of the member")] long id, CancellationToken ct)
    {
        var member = await client.GetMemberAsync(id, ct);
        return member != null
            ? JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true })
            : $"Member with ID {id} not found.";
    }

    [McpServerTool(Name="create_member"), Description("Create new member (creates ContactDetails automatically))")]
    public async Task<string> CreateMember(
        [Description("The email or username of the new member")] string emailOrUserName,
        [Description("The first name of the new member")] string firstName,
        [Description("The family name of the new member")] string familyName,
        [Description("The private email of the new member")] string? privateEmail,
        CancellationToken ct)
    {
        var contactDetails = new ContactDetails
        {
            FirstName = firstName,
            FamilyName = familyName,
            PrivateEmail = privateEmail
        };
        var created = await client.CreateMemberAsync(emailOrUserName, contactDetails, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool(Name="update_member"), Description("Check if the user is permitted to change values and if so, update")]
    public async Task<string> UpdateMember(
        [Description("The ID of the member")] long id,
        [Description("The email or user name of the member")] string? emailOrUserName,
        [Description("The membership number of a member")] string? membershipNumber, CancellationToken ct)
    {
        var member = new Member { Id = id };
        if (emailOrUserName != null) member.EmailOrUserName = emailOrUserName;
        if (membershipNumber != null) member.MembershipNumber = membershipNumber;
        var updated = await client.UpdateMemberAsync(id, member, ct);
        return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool(Name="delete_member"), Description("Delete a member. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteMember([Description("The ID of the member")] long id, CancellationToken ct)
    {
        await client.DeleteMemberAsync(id, ct);
        return $"Member with ID {id} has been deleted.";
    }
}
