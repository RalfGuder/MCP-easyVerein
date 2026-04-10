using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing members via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class MemberTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists members with optional filters for ID, membership number, and search terms.
    /// </summary>
    /// <param name="id">Optional member ID filter.</param>
    /// <param name="membershipNumber">Optional membership number filter.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching members, or an error message.</returns>
    [McpServerTool(Name = "list_members"), Description("List all members")]
    public async Task<string> ListMembers(
        [Description("The ID of a member")]long? id,
        [Description("The membership number of a member")] string? membershipNumber,
        [Description("Search terms for the member")] string[]? search , CancellationToken ct)
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

    /// <summary>
    /// Retrieves a single member by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the member, or a not-found message.</returns>
    [McpServerTool(Name="get_member"), Description("Retrieve a member using their ID.")]
    public async Task<string> GetMember(
        [Description("The ID of the member")] long id, CancellationToken ct)
    {
        try
        {
            var member = await client.GetMemberAsync(id, ct);
            return member != null
                ? JsonSerializer.Serialize(member, new JsonSerializerOptions { WriteIndented = true })
                : $"Member with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }

    }

    /// <summary>
    /// Creates a new member with associated contact details.
    /// </summary>
    /// <param name="emailOrUserName">The email address or username for the new member.</param>
    /// <param name="firstName">The first name of the new member.</param>
    /// <param name="familyName">The family name of the new member.</param>
    /// <param name="privateEmail">An optional private email address.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created member.</returns>
    [McpServerTool(Name="create_member"), Description("Create new member (creates ContactDetails automatically))")]
    public async Task<string> CreateMember(
        [Description("The email or username of the new member")] string emailOrUserName,
        [Description("The first name of the new member")] string firstName,
        [Description("The family name of the new member")] string familyName,
        [Description("The private email of the new member")] string? privateEmail,
        CancellationToken ct)
    {
        try
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
        catch (Exception e)
        {
            return $"ERROR: {e.GetType().Name}: {e.Message}\nInner: {e.InnerException?.Message}";
        }

    }

    /// <summary>
    /// Updates an existing member's data. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the member to update.</param>
    /// <param name="membershipNumber">An optional new membership number.</param>
    /// <param name="resignationDate">An optional new resignation date.</param>
    /// <param name="resignationNoticeDate">An optional new resignation notice date.</param>
    /// <param name="joinDate">The join date.</param>
    /// <param name="declarationOfApplication">The declaration of application.</param>
    /// <param name="paymentStartDate">The payment start date.</param>
    /// <param name="paymentAmount">The payment amount.</param>
    /// <param name="paymentIntervalMonths">The payment intervall months.</param>
    /// <param name="relatedMember">The related member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated member.</returns>
    [McpServerTool(Name="update_member"), Description("Check if the user is permitted to change values and if so, update")]
    public async Task<string> UpdateMember(
        [Description("The ID of the member")] long id,
        [Description("The membership number of a member")] string? membershipNumber, 
        [Description("The resignation date of the member")] DateTime? resignationDate, 
        [Description("The resignation notice date of the member")] DateTime? resignationNoticeDate,
        [Description("The join date of the member")] DateTime? joinDate, 
        [Description("The declaration of application of the member")] string? declarationOfApplication, 
        [Description("The payment start date of the member")] DateTime? paymentStartDate,
        [Description("The payment amount of the member")] decimal? paymentAmount,
        [Description("The payment interval in months")] int? paymentIntervalMonths,
        [Description("The related member ID")] long? relatedMember,
        [Description("Use balance for membership fee")] bool? useBalanceForMembershipFee, CancellationToken ct)
    {
        try
        {
            var patchData = new Dictionary<string, object>();
            if (membershipNumber != null) patchData[MemberFields.MembershipNumber] = membershipNumber;
            if (resignationDate != null) patchData[MemberFields.ResignationDate] = resignationDate;
            if (resignationNoticeDate != null) patchData[MemberFields.ResignationNoticeDate] = resignationNoticeDate;
            if (joinDate != null) patchData[MemberFields.JoinDate] = joinDate;
            if (declarationOfApplication != null) patchData[MemberFields.DeclarationOfApplication] = declarationOfApplication;
            if (paymentStartDate != null) patchData[MemberFields.PaymentStartDate] = paymentStartDate;
            if (paymentAmount != null) patchData[MemberFields.PaymentAmount] = paymentAmount;
            if (paymentIntervalMonths != null) patchData[MemberFields.PaymentIntervalMonths] = paymentIntervalMonths;
            if (relatedMember != null) patchData[MemberFields.RelatedMember] = relatedMember;
            if (useBalanceForMembershipFee != null) patchData[MemberFields.UseBalanceForMembershipFee] = useBalanceForMembershipFee;

            var updated = await client.UpdateMemberAsync(id, patchData, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });

        }
        catch (Exception e)
        {
            return $"ERROR: {e.GetType().Name}: {e.Message}\nInner: {e.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes a member by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the member to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message.</returns>
    [McpServerTool(Name="delete_member"), Description("Delete a member. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteMember(
        [Description("The ID of the member")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteMemberAsync(id, ct);
            return $"Member with ID {id} has been deleted.";

        }
        catch (Exception e)
        {
            return $"ERROR: {e.GetType().Name}: {e.Message}\nInner: {e.InnerException?.Message}";
        }
    }
}
