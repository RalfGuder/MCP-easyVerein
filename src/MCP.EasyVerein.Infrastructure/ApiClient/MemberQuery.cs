using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient
{
    internal class MemberQuery
    {
        internal long? Id { get; set; }

        public string? MembershipNumber { get; set; }
        public string[]? Search { get; set; }

        private const string FieldQuery =
            "query=" + 
            "{" + 
                MemberFields.Id + "," + 
                MemberFields.JoinDate + "," + 
                MemberFields.ResignationDate + "," + 
                MemberFields.ResignationNoticeDate + "," + 
                MemberFields.DeclarationOfApplication + "," + 
                MemberFields.PaymentStartDate + "," + 
                MemberFields.PaymentAmount + "," + 
                MemberFields.PaymentIntervalMonths + "," + 
                MemberFields.RelatedMember + "," + 
                MemberFields.UseBalanceForMembershipFee + "," + 
                MemberFields.BulletinBoardNewPostNotification + "," + 
                MemberFields.MembershipNumber + "," + 
                MemberFields.EmailOrUserName + "," +
                MemberFields.ContactDetails  + 
                "{" + 
                    ContactDetailsFields.Id  + "," + 
                    ContactDetailsFields.FirstName + "," + 
                    ContactDetailsFields.FamilyName + "," + 
                    ContactDetailsFields.DateOfBirth + "," + 
                    ContactDetailsFields.PrivateEmail + "," + 
                    ContactDetailsFields.PrimaryEmail + 
                "}" + 
            "}";

        public override string ToString()
        {
            var parts = new List<string> { FieldQuery };

            if (Id != null)
                parts.Add($"id={Id}");

            if (!string.IsNullOrEmpty(MembershipNumber))
                parts.Add($"membershipNumber={Uri.EscapeDataString(MembershipNumber)}");
            
            if (Search!= null && Search.Length != 0)
                parts.Add($"search={Uri.EscapeDataString(string.Join(",", Search))}");
            

            return string.Join("&", parts);
        }
    }
}
