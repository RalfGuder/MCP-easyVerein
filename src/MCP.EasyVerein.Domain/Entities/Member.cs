using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

public class Member
{
    [JsonPropertyName(MemberFields.Id)] public long Id { get; set; }
    [JsonPropertyName(MemberFields.JoinDate)] public DateTime? JoinDate { get; set; }
    [JsonPropertyName(MemberFields.ResignationDate)] public DateTime? ResignationDate { get; set; }
    [JsonPropertyName(MemberFields.ResignationNoticeDate)] public DateTime? ResignationNoticeDate { get; set; }
    [JsonPropertyName(MemberFields.DeclarationOfApplication)] public string DeclarationOfApplication { get; set; } = string.Empty;
    [JsonPropertyName(MemberFields.PaymentStartDate)] public DateTime? PaymentStartDate { get; set; }
    [JsonPropertyName(MemberFields.PaymentAmount)] public decimal? PaymentAmount { get; set; }
    [JsonPropertyName(MemberFields.PaymentIntervalMonths)] public int? PaymentIntervalMonths { get; set; }
    [JsonPropertyName(MemberFields.RelatedMember)] public long? RelatedMember { get; set; }
    [JsonPropertyName(MemberFields.UseBalanceForMembershipFee)] public bool UseBalanceForMembershipFee { get; set; }
    [JsonPropertyName(MemberFields.BulletinBoardNewPostNotification)] public bool BulletinBoardNewPostNotification { get; set; }
    [JsonPropertyName(MemberFields.IsApplication)] public bool IsApplication { get; set; }
    [JsonPropertyName(MemberFields.MembershipNumber)] public string? MembershipNumber { get; set; }
    [JsonPropertyName(MemberFields.EmailOrUserName)] public string EmailOrUserName { get; set; } = string.Empty;
    [JsonPropertyName(MemberFields.ContactDetails)] public ContactDetails? ContactDetails { get; set; }

    [JsonPropertyName(MemberFields.ApplicationDate)] public DateTime? ApplicationDate { get; set; }
    [JsonPropertyName(MemberFields.ApplicationWasAcceptedAt)] public DateTime? ApplicationWasAcceptedAt { get; set; }
    [JsonPropertyName(MemberFields.IsChairman)] public bool IsChairman { get; set; }
    [JsonPropertyName(MemberFields.ChairmanPermissionGroup)] public int? ChairmanPermissionGroup { get; set; }
    [JsonPropertyName(MemberFields.IsBlocked)] public bool IsBlocked { get; set; }
    [JsonPropertyName(MemberFields.BlockReason)] public string? BlockReason { get; set; }
    [JsonPropertyName(MemberFields.ProfilePicture)] public string? ProfilePicture { get; set; }
    [JsonPropertyName(MemberFields.EditableByRelatedMembers)] public bool EditableByRelatedMembers { get; set; }
    [JsonPropertyName(MemberFields.SepaMandateFile)] public string? SepaMandateFile { get; set; }
    [JsonPropertyName(MemberFields.RequirePasswordChange)] public bool RequirePasswordChange { get; set; }
    [JsonPropertyName(MemberFields.IsMatrixSearchable)] public bool IsMatrixSearchable { get; set; }
    [JsonPropertyName(MemberFields.MatrixBlockReason)] public string? MatrixBlockReason { get; set; }
    [JsonPropertyName(MemberFields.BlockedFromMatrix)] public bool BlockedFromMatrix { get; set; }
    [JsonPropertyName(MemberFields.MatrixCommunicationPermission)] public int? MatrixCommunicationPermission { get; set; }
    [JsonPropertyName(MemberFields.UseMatrixGroupSettings)] public bool UseMatrixGroupSettings { get; set; }
    [JsonPropertyName(MemberFields.ShowWarningsAndNotesToAdminsInProfile)] public bool ShowWarningsAndNotesToAdminsInProfile { get; set; }
    [JsonPropertyName(MemberFields.SignatureText)] public string? SignatureText { get; set; }

    [JsonIgnore] public bool IsActive => ResignationDate == null && !IsBlocked;
    [JsonIgnore] public string FullName => ContactDetails != null
        ? $"{ContactDetails.FirstName} {ContactDetails.FamilyName}" : EmailOrUserName;
}
