using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing the lending of the organization's inventory objects via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class LendingTools(IEasyVereinApiClient client)
{
    /// <summary>The lending states accepted by the API (lowercase codes).</summary>
    private static readonly string[] ValidStates = ["lent", "inquiry", "returned"];

    /// <summary>The time formats accepted for the borrow and return time of day.</summary>
    private static readonly string[] TimeFormats = ["hh\\:mm", "hh\\:mm\\:ss"];

    /// <summary>Lists lendings with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_lendings"), Description("List the organization's lendings of inventory objects")]
    public async Task<string> ListLendings(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Inventory object ID filter")] long? parentInventoryObject,
        [Description("Inventory object ID to exclude")] long? parentInventoryObjectNot,
        [Description("Borrowing member ID filter")] long? borrowMember,
        [Description("Borrowing member ID to exclude")] long? borrowMemberNot,
        [Description("Borrower contact details ID filter")] long? borrowAddress,
        [Description("Borrower contact details ID to exclude")] long? borrowAddressNot,
        [Description("State filter ('lent', 'inquiry' or 'returned')")] string? state,
        [Description("State to exclude ('lent', 'inquiry' or 'returned')")] string? stateNot,
        [Description("Exact borrowing date filter (YYYY-MM-DD)")] string? borrowingDate,
        [Description("Earliest borrowing date filter (YYYY-MM-DD)")] string? borrowingDateGte,
        [Description("Latest borrowing date filter (YYYY-MM-DD)")] string? borrowingDateLte,
        [Description("Exact return date filter (YYYY-MM-DD)")] string? returnDate,
        [Description("Earliest return date filter (YYYY-MM-DD)")] string? returnDateGte,
        [Description("Latest return date filter (YYYY-MM-DD)")] string? returnDateLte,
        [Description("Exact quantity filter")] int? quantity,
        [Description("Minimum quantity filter (exclusive)")] int? quantityGt,
        [Description("Maximum quantity filter (exclusive)")] int? quantityLt,
        [Description("Only lendings whose return date is in the future")] bool? futureReturnDate,
        [Description("Soft-deleted filter (true = only lendings in the wastebasket)")] bool? deleted,
        [Description("Ordering (e.g. 'borrowingDate' or '-quantity')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var lendings = await client.ListLendingsAsync(
                idIn, parentInventoryObject, parentInventoryObjectNot,
                borrowMember, borrowMemberNot, borrowAddress, borrowAddressNot,
                state, stateNot,
                borrowingDate, borrowingDateGte, borrowingDateLte,
                returnDate, returnDateGte, returnDateLte,
                quantity, quantityGt, quantityLt,
                futureReturnDate, deleted, ordering, search, ct);
            return JsonSerializer.Serialize(lendings, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single lending by its unique identifier.</summary>
    [McpServerTool(Name = "get_lending"), Description("Retrieve a lending by its ID")]
    public async Task<string> GetLending(
        [Description("The ID of the lending")] long id,
        CancellationToken ct)
    {
        try
        {
            var lending = await client.GetLendingAsync(id, ct);
            return lending != null
                ? JsonSerializer.Serialize(lending, new JsonSerializerOptions { WriteIndented = true })
                : $"Lending with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new lending in easyVerein.</summary>
    [McpServerTool(Name = "create_lending"), Description("Create a new lending of an inventory object")]
    public async Task<string> CreateLending(
        [Description("ID of the inventory object to lend (required)")] long? parentInventoryObject,
        [Description("Contact details ID of the borrower")] long? borrowAddress,
        [Description("Borrowing date (YYYY-MM-DD)")] string? borrowingDate,
        [Description("Return date (YYYY-MM-DD)")] string? returnDate,
        [Description("Number of pieces lent")] int? quantity,
        [Description("Time of day the object is lent out (HH:mm or HH:mm:ss)")] string? borrowTime,
        [Description("Time of day the object is returned (HH:mm or HH:mm:ss)")] string? returnTime,
        [Description("Lending state: 'lent', 'inquiry' or 'returned'")] string? state,
        CancellationToken ct)
    {
        try
        {
            var error = Validate(borrowingDate, returnDate, quantity, borrowTime, returnTime, state);
            if (error != null) return error;

            // The API requires all four fields and answers with HTTP 400 when one is missing,
            // even though OPTIONS reports parentInventoryObject and borrowAddress as read-only.
            if (!parentInventoryObject.HasValue)
                return "ERROR: The inventory object (parentInventoryObject) is required.";
            if (!borrowAddress.HasValue)
                return "ERROR: The borrower's contact details (borrowAddress) are required.";
            if (!HasValue(borrowingDate))
                return "ERROR: The borrowing date (borrowingDate) is required.";
            if (!quantity.HasValue)
                return "ERROR: The number of pieces (quantity) is required.";

            var lending = new Lending
            {
                ParentInventoryObjectId = parentInventoryObject,
                BorrowAddressId = borrowAddress,
                BorrowingDate = ParseDate(borrowingDate),
                ReturnDate = ParseDate(returnDate),
                Quantity = quantity,
                BorrowTime = HasValue(borrowTime) ? borrowTime : null,
                ReturnTime = HasValue(returnTime) ? returnTime : null,
                State = HasValue(state) ? state : null
            };

            var created = await client.CreateLendingAsync(lending, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates a lending (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_lending"), Description("Update a lending (only provided fields are changed)")]
    public async Task<string> UpdateLending(
        [Description("The ID of the lending to update")] long id,
        [Description("New inventory object ID")] long? parentInventoryObject,
        [Description("New contact details ID of the borrower")] long? borrowAddress,
        [Description("New borrowing date (YYYY-MM-DD)")] string? borrowingDate,
        [Description("New return date (YYYY-MM-DD)")] string? returnDate,
        [Description("New number of pieces lent")] int? quantity,
        [Description("New time of day the object is lent out (HH:mm or HH:mm:ss)")] string? borrowTime,
        [Description("New time of day the object is returned (HH:mm or HH:mm:ss)")] string? returnTime,
        [Description("New lending state: 'lent', 'inquiry' or 'returned'")] string? state,
        CancellationToken ct)
    {
        try
        {
            var error = Validate(borrowingDate, returnDate, quantity, borrowTime, returnTime, state);
            if (error != null) return error;

            var patch = new Dictionary<string, object>();
            if (parentInventoryObject.HasValue)
                patch[LendingFields.ParentInventoryObject] = parentInventoryObject.Value;
            if (borrowAddress.HasValue) patch[LendingFields.BorrowAddress] = borrowAddress.Value;
            if (HasValue(borrowingDate)) patch[LendingFields.BorrowingDate] = borrowingDate!;
            if (HasValue(returnDate)) patch[LendingFields.ReturnDate] = returnDate!;
            if (quantity.HasValue) patch[LendingFields.Quantity] = quantity.Value;
            if (HasValue(borrowTime)) patch[LendingFields.BorrowTime] = borrowTime!;
            if (HasValue(returnTime)) patch[LendingFields.ReturnTime] = returnTime!;
            if (HasValue(state)) patch[LendingFields.State] = state!;

            var updated = await client.UpdateLendingAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a lending by its unique identifier (moves it to the wastebasket).</summary>
    [McpServerTool(Name = "delete_lending"), Description("Delete a lending (moves it to the easyVerein wastebasket). Only authorized users are able to perform this action!")]
    public async Task<string> DeleteLending(
        [Description("The ID of the lending to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteLendingAsync(id, ct);
            return $"Lending with ID {id} has been moved to the wastebasket.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Validates the dates, times, quantity and state shared by create and update.</summary>
    /// <returns>An error message, or <c>null</c> if all supplied values are valid.</returns>
    private static string? Validate(
        string? borrowingDate, string? returnDate, int? quantity,
        string? borrowTime, string? returnTime, string? state)
    {
        if (HasValue(borrowingDate) && ParseDate(borrowingDate) == null)
            return $"ERROR: Invalid borrowing date '{borrowingDate}'. Use YYYY-MM-DD.";
        if (HasValue(returnDate) && ParseDate(returnDate) == null)
            return $"ERROR: Invalid return date '{returnDate}'. Use YYYY-MM-DD.";
        if (HasValue(borrowTime) && !IsValidTime(borrowTime!))
            return $"ERROR: Invalid borrow time '{borrowTime}'. Use HH:mm or HH:mm:ss.";
        if (HasValue(returnTime) && !IsValidTime(returnTime!))
            return $"ERROR: Invalid return time '{returnTime}'. Use HH:mm or HH:mm:ss.";
        if (quantity.HasValue && quantity.Value <= 0)
            return "ERROR: The quantity must be greater than zero.";
        if (HasValue(state) && !ValidStates.Contains(state))
            return $"ERROR: Invalid state '{state}'. Use one of: lent, inquiry, returned (lowercase).";
        return null;
    }

    /// <summary>Parses a date-only value, returning <c>null</c> when it is missing or malformed.</summary>
    private static DateTime? ParseDate(string? value)
    {
        if (!HasValue(value)) return null;
        return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeLocal, out var date) ? date : null;
    }

    /// <summary>Checks whether a value is a valid time of day in one of the accepted formats.</summary>
    private static bool IsValidTime(string value) =>
        TimeSpan.TryParseExact(value, TimeFormats, CultureInfo.InvariantCulture, out _);

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
