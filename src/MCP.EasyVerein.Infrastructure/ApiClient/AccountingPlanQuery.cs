using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the accounting-plan endpoint (US-0035). The endpoint has no
/// filters; billing accounts are selected nested with ID, number and name.
/// </summary>
internal static class AccountingPlanQuery
{
    /// <summary>Field selection for list and single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            AccountingPlanFields.Id + "," +
            AccountingPlanFields.Org + "," +
            AccountingPlanFields.BillingAccountsWithDetails + "," +
            AccountingPlanFields.CreatedAt + "," +
            AccountingPlanFields.UpdatedAt +
        "}";
}
