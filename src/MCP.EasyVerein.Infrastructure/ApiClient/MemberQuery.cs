using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient
{
    /// <summary>
    /// Builds the query string for the member API endpoint, including field selection and optional filters.
    /// </summary>
    internal class MemberQuery
    {
        /// <summary>
        /// Gets or sets an optional member identifier filter.
        /// </summary>
        internal long? Id { get; set; }

        /// <summary>
        /// Gets or sets an optional membership number filter.
        /// </summary>
        public string? MembershipNumber { get; set; }

        /// <summary>
        /// Gets or sets optional search terms to filter members.
        /// </summary>
        public string[]? Search { get; set; }

        /// <summary>
        /// The base field selection query requesting member and nested contact details fields.
        /// </summary>
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
                MemberFields.RelatedMember +
                "{" +
                    MemberFields.Id  + "," +
                    MemberFields.ContactDetails +
                    "{" +
                        ContactDetailsFields.Id  + "," +
                        ContactDetailsFields.FirstName + "," +
                        ContactDetailsFields.FamilyName +
                    "}" +
                "}," +
                MemberFields.RelatedMembers +
                "{" +
                    MemberFields.Id + "," +
                    MemberFields.ContactDetails +
                    "{" +
                        ContactDetailsFields.Id + "," +
                        ContactDetailsFields.FirstName + "," +
                        ContactDetailsFields.FamilyName +
                    "}" +
                "}," +
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

        /// <summary>
        /// Returns the complete query string with field selection and any active filters.
        /// </summary>
        /// <returns>A URL query string for the member endpoint.</returns>
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
