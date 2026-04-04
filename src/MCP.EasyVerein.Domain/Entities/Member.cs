using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>Represents an association member from the easyVerein API.</summary>
public class Member
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(MemberFields.Id)] public long Id { get; set; }

    /// <summary>Gets or sets the date the member joined. Maps to API field '<c>joinDate</c>'.</summary>
    [JsonPropertyName(MemberFields.JoinDate)] public DateTime? JoinDate { get; set; }

    /// <summary>Gets or sets the date the member resigned. Maps to API field '<c>resignationDate</c>'.</summary>
    [JsonPropertyName(MemberFields.ResignationDate)] public DateTime? ResignationDate { get; set; }

    /// <summary>Gets or sets the date the resignation notice was received. Maps to API field '<c>resignationNoticeDate</c>'.</summary>
    [JsonPropertyName(MemberFields.ResignationNoticeDate)] public DateTime? ResignationNoticeDate { get; set; }

    /// <summary>Gets or sets the declaration of application text. Maps to API field '<c>declarationOfApplication</c>'.</summary>
    [JsonPropertyName(MemberFields.DeclarationOfApplication)] public string DeclarationOfApplication { get; set; } = string.Empty;

    /// <summary>Gets or sets the payment start date. Maps to API field '<c>_paymentStartDate</c>'.</summary>
    [JsonPropertyName(MemberFields.PaymentStartDate)] public DateTime? PaymentStartDate { get; set; }

    /// <summary>Gets or sets the payment amount. Maps to API field '<c>paymentAmount</c>'.</summary>
    [JsonPropertyName(MemberFields.PaymentAmount)] public decimal? PaymentAmount { get; set; }

    /// <summary>Gets or sets the payment interval in months. Maps to API field '<c>paymentIntervallMonths</c>'.</summary>
    [JsonPropertyName(MemberFields.PaymentIntervalMonths)] public int? PaymentIntervalMonths { get; set; }

    /// <summary>Gets or sets the ID of the related member. Maps to API field '<c>_relatedMember</c>'.</summary>
    [JsonPropertyName(MemberFields.RelatedMember)] public long? RelatedMember { get; set; }

    /// <summary>Gets or sets whether balance is used for the membership fee. Maps to API field '<c>useBalanceForMembershipFee</c>'.</summary>
    [JsonPropertyName(MemberFields.UseBalanceForMembershipFee)] public bool UseBalanceForMembershipFee { get; set; }

    /// <summary>Gets or sets whether the member receives bulletin board notifications. Maps to API field '<c>bulletinBoardNewPostNotification</c>'.</summary>
    [JsonPropertyName(MemberFields.BulletinBoardNewPostNotification)] public bool BulletinBoardNewPostNotification { get; set; }

    /// <summary>Gets or sets whether this record is a membership application. Maps to API field '<c>_isApplication</c>'.</summary>
    [JsonPropertyName(MemberFields.IsApplication)] public bool IsApplication { get; set; }

    /// <summary>Gets or sets the membership number. Maps to API field '<c>membershipNumber</c>'.</summary>
    [JsonPropertyName(MemberFields.MembershipNumber)] public string? MembershipNumber { get; set; }

    /// <summary>Gets or sets the email address or username. Maps to API field '<c>emailOrUserName</c>'.</summary>
    [JsonPropertyName(MemberFields.EmailOrUserName)] public string EmailOrUserName { get; set; } = string.Empty;

    /// <summary>Gets or sets the associated contact details. Maps to API field '<c>contactDetails</c>'.</summary>
    [JsonPropertyName(MemberFields.ContactDetails)] public ContactDetails? ContactDetails { get; set; }

    /// <summary>Gets or sets the application date. Maps to API field '<c>_applicationDate</c>'.</summary>
    [JsonPropertyName(MemberFields.ApplicationDate)] public DateTime? ApplicationDate { get; set; }

    /// <summary>Gets or sets the date the application was accepted. Maps to API field '<c>_applicationWasAcceptedAt</c>'.</summary>
    [JsonPropertyName(MemberFields.ApplicationWasAcceptedAt)] public DateTime? ApplicationWasAcceptedAt { get; set; }

    /// <summary>Gets or sets whether the member is a chairman. Maps to API field '<c>_isChairman</c>'.</summary>
    [JsonPropertyName(MemberFields.IsChairman)] public bool IsChairman { get; set; }

    /// <summary>Gets or sets the chairman permission group ID. Maps to API field '<c>_chairmanPermissionGroup</c>'.</summary>
    [JsonPropertyName(MemberFields.ChairmanPermissionGroup)] public int? ChairmanPermissionGroup { get; set; }

    /// <summary>Gets or sets whether the member is blocked. Maps to API field '<c>_isBlocked</c>'.</summary>
    [JsonPropertyName(MemberFields.IsBlocked)] public bool IsBlocked { get; set; }

    /// <summary>Gets or sets the reason for blocking the member. Maps to API field '<c>blockReason</c>'.</summary>
    [JsonPropertyName(MemberFields.BlockReason)] public string? BlockReason { get; set; }

    /// <summary>Gets or sets the profile picture URL. Maps to API field '<c>_profilePicture</c>'.</summary>
    [JsonPropertyName(MemberFields.ProfilePicture)] public string? ProfilePicture { get; set; }

    /// <summary>Gets or sets whether related members can edit this member. Maps to API field '<c>_editableByRelatedMembers</c>'.</summary>
    [JsonPropertyName(MemberFields.EditableByRelatedMembers)] public bool EditableByRelatedMembers { get; set; }

    /// <summary>Gets or sets the SEPA mandate file reference. Maps to API field '<c>sepaMandateFile</c>'.</summary>
    [JsonPropertyName(MemberFields.SepaMandateFile)] public string? SepaMandateFile { get; set; }

    /// <summary>Gets or sets whether a password change is required. Maps to API field '<c>requirePasswordChange</c>'.</summary>
    [JsonPropertyName(MemberFields.RequirePasswordChange)] public bool RequirePasswordChange { get; set; }

    /// <summary>Gets or sets whether the member is searchable in the matrix. Maps to API field '<c>_isMatrixSearchable</c>'.</summary>
    [JsonPropertyName(MemberFields.IsMatrixSearchable)] public bool IsMatrixSearchable { get; set; }

    /// <summary>Gets or sets the reason for blocking the member from the matrix. Maps to API field '<c>matrixBlockReason</c>'.</summary>
    [JsonPropertyName(MemberFields.MatrixBlockReason)] public string? MatrixBlockReason { get; set; }

    /// <summary>Gets or sets whether the member is blocked from the matrix. Maps to API field '<c>blockedFromMatrix</c>'.</summary>
    [JsonPropertyName(MemberFields.BlockedFromMatrix)] public bool BlockedFromMatrix { get; set; }

    /// <summary>Gets or sets the matrix communication permission level. Maps to API field '<c>_matrixCommunicationPermission</c>'.</summary>
    [JsonPropertyName(MemberFields.MatrixCommunicationPermission)] public int? MatrixCommunicationPermission { get; set; }

    /// <summary>Gets or sets whether the member uses matrix group settings. Maps to API field '<c>useMatrixGroupSettings</c>'.</summary>
    [JsonPropertyName(MemberFields.UseMatrixGroupSettings)] public bool UseMatrixGroupSettings { get; set; }

    /// <summary>Gets or sets whether warnings and notes are shown to admins in the profile. Maps to API field '<c>showWarningsAndNotesToAdminsInProfile</c>'.</summary>
    [JsonPropertyName(MemberFields.ShowWarningsAndNotesToAdminsInProfile)] public bool ShowWarningsAndNotesToAdminsInProfile { get; set; }

    /// <summary>Gets or sets the signature text. Maps to API field '<c>signatureText</c>'.</summary>
    [JsonPropertyName(MemberFields.SignatureText)] public string? SignatureText { get; set; }

    /// <summary>Gets whether the member is active (not resigned and not blocked).</summary>
    [JsonIgnore] public bool IsActive => ResignationDate == null && !IsBlocked;

    /// <summary>Gets the full name from contact details, or the email/username as fallback.</summary>
    [JsonIgnore] public string FullName => ContactDetails != null
        ? $"{ContactDetails.FirstName} {ContactDetails.FamilyName}" : EmailOrUserName;
}
