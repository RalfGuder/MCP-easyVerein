using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.EasyVerein.Domain.ValueObjects
{
    public static class MemberFields
    {
        public const string Id = "id";
        public const string JoinDate = "joinDate";
        public const string ResignationDate = "resignationDate";
        public const string ResignationNoticeDate = "resignationNoticeDate";
        public const string DeclarationOfApplication = "declarationOfApplication";
        public const string PaymentStartDate = "_paymentStartDate";
        public const string PaymentAmount = "paymentAmount";
        public const string PaymentIntervalMonths = "paymentIntervallMonths";
        public const string RelatedMember = "_relatedMember";
        public const string UseBalanceForMembershipFee = "useBalanceForMembershipFee";
        public const string BulletinBoardNewPostNotification = "bulletinBoardNewPostNotification";
        public const string IsApplication = "_isApplication";
        public const string MembershipNumber = "membershipNumber";
        public const string EmailOrUserName = "emailOrUserName";
        public const string ContactDetails = "contactDetails";
        public const string ApplicationDate = "_applicationDate";
        public const string ApplicationWasAcceptedAt = "_applicationWasAcceptedAt";
        public const string IsChairman = "_isChairman";
        public const string ChairmanPermissionGroup = "_chairmanPermissionGroup";
        public const string IsBlocked = "_isBlocked";
        public const string BlockReason = "blockReason";
        public const string ProfilePicture = "_profilePicture";
        public const string EditableByRelatedMembers = "_editableByRelatedMembers";
        public const string SepaMandateFile = "sepaMandateFile";
        public const string RequirePasswordChange = "requirePasswordChange";
        public const string IsMatrixSearchable = "_isMatrixSearchable";
        public const string MatrixBlockReason = "matrixBlockReason";
        public const string BlockedFromMatrix = "blockedFromMatrix";
        public const string MatrixCommunicationPermission = "_matrixCommunicationPermission";
        public const string UseMatrixGroupSettings = "useMatrixGroupSettings";
        public const string ShowWarningsAndNotesToAdminsInProfile = "showWarningsAndNotesToAdminsInProfile";
        public const string SignatureText = "signatureText";
    }
}
