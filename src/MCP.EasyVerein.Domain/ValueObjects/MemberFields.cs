namespace MCP.EasyVerein.Domain.ValueObjects
{
    /// <summary>Constants for easyVerein Member API field names used in JSON serialization.</summary>
    public static class MemberFields
    {
        /// <summary>API field name for the application date.</summary>
        public const string ApplicationDate = "_applicationDate";

        /// <summary>API field name for the date the application was accepted.</summary>
        public const string ApplicationWasAcceptedAt = "_applicationWasAcceptedAt";

        /// <summary>API field name for whether the member is blocked from the matrix.</summary>
        public const string BlockedFromMatrix = "blockedFromMatrix";

        /// <summary>API field name for the reason the member is blocked.</summary>
        public const string BlockReason = "blockReason";

        /// <summary>API field name for bulletin board new post notification preference.</summary>
        public const string BulletinBoardNewPostNotification = "bulletinBoardNewPostNotification";

        /// <summary>API field name for the chairman's permission group.</summary>
        public const string ChairmanPermissionGroup = "_chairmanPermissionGroup";

        /// <summary>API field name for the associated contact details.</summary>
        public const string ContactDetails = "contactDetails";

        /// <summary>API field name for the member's declaration of application.</summary>
        public const string DeclarationOfApplication = "declarationOfApplication";

        /// <summary>API field name for whether related members can edit the record.</summary>
        public const string EditableByRelatedMembers = "_editableByRelatedMembers";

        /// <summary>API field name for the email address or username.</summary>
        public const string EmailOrUserName = "emailOrUserName";

        /// <summary>API field name for the unique member identifier.</summary>
        public const string Id = "id";
        /// <summary>API field name for whether the record is an application.</summary>
        public const string IsApplication = "_isApplication";

        /// <summary>API field name for whether the member is blocked.</summary>
        public const string IsBlocked = "_isBlocked";

        /// <summary>API field name for whether the member is a chairman.</summary>
        public const string IsChairman = "_isChairman";

        /// <summary>API field name for whether the member is searchable in the matrix.</summary>
        public const string IsMatrixSearchable = "_isMatrixSearchable";

        /// <summary>API field name for the member's join date.</summary>
        public const string JoinDate = "joinDate";

        /// <summary>API field name for the reason the member is blocked from the matrix.</summary>
        public const string MatrixBlockReason = "matrixBlockReason";

        /// <summary>API field name for the matrix communication permission setting.</summary>
        public const string MatrixCommunicationPermission = "_matrixCommunicationPermission";

        /// <summary>API field name for the membership number.</summary>
        public const string MembershipNumber = "membershipNumber";

        /// <summary>API field name for the payment amount.</summary>
        public const string PaymentAmount = "paymentAmount";

        /// <summary>API field name for the payment interval in months.</summary>
        public const string PaymentIntervalMonths = "paymentIntervallMonths";

        /// <summary>API field name for the payment start date.</summary>
        public const string PaymentStartDate = "_paymentStartDate";

        /// <summary>API field name for the member's profile picture.</summary>
        public const string ProfilePicture = "_profilePicture";

        /// <summary>API field name for the related member reference.</summary>
        public const string RelatedMember = "_relatedMember";

        public const string RelatedMembers = "relatedMembers";

        /// <summary>API field name for requiring a password change.</summary>
        public const string RequirePasswordChange = "requirePasswordChange";

        /// <summary>API field name for the member's resignation date.</summary>
        public const string ResignationDate = "resignationDate";

        /// <summary>API field name for the date the resignation notice was submitted.</summary>
        public const string ResignationNoticeDate = "resignationNoticeDate";

        /// <summary>API field name for the SEPA mandate file.</summary>
        public const string SepaMandateFile = "sepaMandateFile";

        /// <summary>API field name for showing warnings and notes to admins in profile.</summary>
        public const string ShowWarningsAndNotesToAdminsInProfile = "showWarningsAndNotesToAdminsInProfile";

        /// <summary>API field name for the member's signature text.</summary>
        public const string SignatureText = "signatureText";

        /// <summary>API field name for using balance towards membership fee.</summary>
        public const string UseBalanceForMembershipFee = "useBalanceForMembershipFee";

        /// <summary>API field name for using matrix group settings.</summary>
        public const string UseMatrixGroupSettings = "useMatrixGroupSettings";
    }
}
