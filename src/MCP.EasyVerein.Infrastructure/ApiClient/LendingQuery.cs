using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the lending API endpoint with field selection and filters.
/// </summary>
internal class LendingQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional inventory object filter.</summary>
    internal long? ParentInventoryObject { get; set; }

    /// <summary>Gets or sets an optional inventory object to exclude.</summary>
    internal long? ParentInventoryObjectNot { get; set; }

    /// <summary>Gets or sets an optional borrowing member filter.</summary>
    internal long? BorrowMember { get; set; }

    /// <summary>Gets or sets an optional borrowing member to exclude.</summary>
    internal long? BorrowMemberNot { get; set; }

    /// <summary>Gets or sets an optional borrower address filter.</summary>
    internal long? BorrowAddress { get; set; }

    /// <summary>Gets or sets an optional borrower address to exclude.</summary>
    internal long? BorrowAddressNot { get; set; }

    /// <summary>Gets or sets an optional lending state filter.</summary>
    internal string? State { get; set; }

    /// <summary>Gets or sets an optional lending state to exclude.</summary>
    internal string? StateNot { get; set; }

    /// <summary>Gets or sets an optional exact borrowing date filter.</summary>
    internal string? BorrowingDate { get; set; }

    /// <summary>Gets or sets an optional minimum borrowing date filter.</summary>
    internal string? BorrowingDateGte { get; set; }

    /// <summary>Gets or sets an optional maximum borrowing date filter.</summary>
    internal string? BorrowingDateLte { get; set; }

    /// <summary>Gets or sets an optional exact return date filter.</summary>
    internal string? ReturnDate { get; set; }

    /// <summary>Gets or sets an optional minimum return date filter.</summary>
    internal string? ReturnDateGte { get; set; }

    /// <summary>Gets or sets an optional maximum return date filter.</summary>
    internal string? ReturnDateLte { get; set; }

    /// <summary>Gets or sets an optional exact quantity filter.</summary>
    internal int? Quantity { get; set; }

    /// <summary>Gets or sets an optional minimum quantity filter (exclusive).</summary>
    internal int? QuantityGt { get; set; }

    /// <summary>Gets or sets an optional maximum quantity filter (exclusive).</summary>
    internal int? QuantityLt { get; set; }

    /// <summary>Gets or sets an optional filter that limits the result to lendings returned in the future.</summary>
    internal bool? FutureReturnDate { get; set; }

    /// <summary>Gets or sets an optional soft-deleted filter.</summary>
    internal bool? Deleted { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            LendingFields.Id + "," +
            LendingFields.Org + "," +
            LendingFields.ParentInventoryObject + "," +
            LendingFields.BorrowAddress + "," +
            LendingFields.Name + "," +
            LendingFields.CreatedAt + "," +
            LendingFields.UpdatedAt + "," +
            LendingFields.DeleteAfterDate + "," +
            LendingFields.DeletedBy + "," +
            LendingFields.BorrowingDate + "," +
            LendingFields.ReturnDate + "," +
            LendingFields.Quantity + "," +
            LendingFields.BorrowTime + "," +
            LendingFields.ReturnTime + "," +
            LendingFields.State +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        AddText(parts, LendingFields.IdIn, IdIn);
        AddNumber(parts, LendingFields.ParentInventoryObject, ParentInventoryObject);
        AddNumber(parts, LendingFields.ParentInventoryObjectNot, ParentInventoryObjectNot);
        AddNumber(parts, LendingFields.BorrowMember, BorrowMember);
        AddNumber(parts, LendingFields.BorrowMemberNot, BorrowMemberNot);
        AddNumber(parts, LendingFields.BorrowAddress, BorrowAddress);
        AddNumber(parts, LendingFields.BorrowAddressNot, BorrowAddressNot);
        AddText(parts, LendingFields.State, State);
        AddText(parts, LendingFields.StateNot, StateNot);
        AddText(parts, LendingFields.BorrowingDate, BorrowingDate);
        AddText(parts, LendingFields.BorrowingDateGte, BorrowingDateGte);
        AddText(parts, LendingFields.BorrowingDateLte, BorrowingDateLte);
        AddText(parts, LendingFields.ReturnDate, ReturnDate);
        AddText(parts, LendingFields.ReturnDateGte, ReturnDateGte);
        AddText(parts, LendingFields.ReturnDateLte, ReturnDateLte);
        AddNumber(parts, LendingFields.Quantity, Quantity);
        AddNumber(parts, LendingFields.QuantityGt, QuantityGt);
        AddNumber(parts, LendingFields.QuantityLt, QuantityLt);
        if (FutureReturnDate.HasValue)
            parts.Add($"{LendingFields.FutureReturnDate}={(FutureReturnDate.Value ? "true" : "false")}");
        if (Deleted.HasValue)
            parts.Add($"{LendingFields.Deleted}={(Deleted.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{LendingFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{LendingFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }

    /// <summary>Appends an escaped text filter to the query parts when a value is set.</summary>
    private static void AddText(List<string> parts, string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            parts.Add($"{key}={Uri.EscapeDataString(value)}");
    }

    /// <summary>Appends a numeric filter to the query parts when a value is set.</summary>
    private static void AddNumber(List<string> parts, string key, long? value)
    {
        if (value.HasValue)
            parts.Add($"{key}={value.Value}");
    }
}
