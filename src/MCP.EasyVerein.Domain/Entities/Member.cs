using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

public class Member
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("emailOrUserName")] public string EmailOrUserName { get; set; } = string.Empty;
    [JsonPropertyName("contactDetails")] public ContactDetails? ContactDetails { get; set; }
    [JsonPropertyName("membershipNumber")] public string? MembershipNumber { get; set; }
    [JsonPropertyName("joinDate")] public DateTime? JoinDate { get; set; }
    [JsonPropertyName("resignationDate")] public DateTime? ResignationDate { get; set; }
    [JsonPropertyName("resignationNoticeDate")] public DateTime? ResignationNoticeDate { get; set; }
    [JsonPropertyName("paymentAmount")] public decimal? PaymentAmount { get; set; }
    [JsonPropertyName("paymentIntervallMonths")] public int? PaymentIntervalMonths { get; set; }
    [JsonPropertyName("_paymentStartDate")] public DateTime? PaymentStartDate { get; set; }
    [JsonPropertyName("_isApplication")] public bool IsApplication { get; set; }
    [JsonPropertyName("_applicationDate")] public DateTime? ApplicationDate { get; set; }
    [JsonPropertyName("_applicationWasAcceptedAt")] public DateTime? ApplicationWasAcceptedAt { get; set; }
    [JsonPropertyName("_isChairman")] public bool IsChairman { get; set; }
    [JsonPropertyName("_chairmanPermissionGroup")] public int? ChairmanPermissionGroup { get; set; }
    [JsonPropertyName("_isBlocked")] public bool IsBlocked { get; set; }
    [JsonPropertyName("blockReason")] public string? BlockReason { get; set; }
    [JsonPropertyName("_profilePicture")] public string? ProfilePicture { get; set; }
    [JsonPropertyName("_relatedMember")] public long? RelatedMember { get; set; }
    [JsonPropertyName("_editableByRelatedMembers")] public bool EditableByRelatedMembers { get; set; }
    [JsonPropertyName("useBalanceForMembershipFee")] public bool UseBalanceForMembershipFee { get; set; }
    [JsonPropertyName("bulletinBoardNewPostNotification")] public bool BulletinBoardNewPostNotification { get; set; }
    [JsonPropertyName("declarationOfApplication")] public string? DeclarationOfApplication { get; set; }
    [JsonPropertyName("sepaMandateFile")] public string? SepaMandateFile { get; set; }
    [JsonPropertyName("requirePasswordChange")] public bool RequirePasswordChange { get; set; }
    [JsonPropertyName("_isMatrixSearchable")] public bool IsMatrixSearchable { get; set; }
    [JsonPropertyName("matrixBlockReason")] public string? MatrixBlockReason { get; set; }
    [JsonPropertyName("blockedFromMatrix")] public bool BlockedFromMatrix { get; set; }
    [JsonPropertyName("_matrixCommunicationPermission")] public int? MatrixCommunicationPermission { get; set; }
    [JsonPropertyName("useMatrixGroupSettings")] public bool UseMatrixGroupSettings { get; set; }
    [JsonPropertyName("showWarningsAndNotesToAdminsInProfile")] public bool ShowWarningsAndNotesToAdminsInProfile { get; set; }
    [JsonPropertyName("signatureText")] public string? SignatureText { get; set; }

    [JsonIgnore] public bool IsActive => ResignationDate == null && !IsBlocked;
    [JsonIgnore] public string FullName => ContactDetails != null
        ? $"{ContactDetails.FirstName} {ContactDetails.FamilyName}" : EmailOrUserName;
}
