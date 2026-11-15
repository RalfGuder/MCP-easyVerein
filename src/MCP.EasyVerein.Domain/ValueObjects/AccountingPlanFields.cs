namespace MCP.EasyVerein.Domain.ValueObjects
{
    /// <summary>Constants for easyVerein accounting-plan API field names (US-0035).</summary>
    internal static class AccountingPlanFields
    {
        /// <summary>API field name for the unique accounting plan identifier.</summary>
        internal const string Id = "id";

        /// <summary>API field name for the owning organization URL reference.</summary>
        internal const string Org = "org";

        /// <summary>API field name for the billing accounts enabled in the plan.</summary>
        internal const string BillingAccounts = "billingAccounts";

        /// <summary>Nested field selection for the billing accounts (ID, number and name).</summary>
        internal const string BillingAccountsWithDetails = "billingAccounts{id,number,name}";

        /// <summary>API field name for the creation timestamp.</summary>
        internal const string CreatedAt = "created_at";

        /// <summary>API field name for the last update timestamp.</summary>
        internal const string UpdatedAt = "updated_at";
    }
}
