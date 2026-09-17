namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein inventory object API field names used in JSON serialization and query building.</summary>
internal static class InventoryObjectFields
{
    /// <summary>API field name for the unique inventory object identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the owning organization reference (read-only).</summary>
    internal const string Org = "org";

    /// <summary>API field name for the member responsible for lending.</summary>
    internal const string LendingResponsible = "lendingResponsible";

    /// <summary>API field name for the assigned inventory object groups (read-only).</summary>
    internal const string InventoryObjectGroups = "inventoryObjectGroups";

    /// <summary>API field name for the picture URL (read-only in this client).</summary>
    internal const string Picture = "picture";

    /// <summary>API field name for the number of pieces currently lent (read-only).</summary>
    internal const string CurrentlyLend = "currentlyLend";

    /// <summary>API field name for the lending references (read-only).</summary>
    internal const string Lendings = "lendings";

    /// <summary>API field name for the custom field assignments (read-only).</summary>
    internal const string CustomFields = "customFields";

    /// <summary>API field name for the location reference (read-only).</summary>
    internal const string LocationObject = "locationObject";

    /// <summary>API field name for the date of the latest lending (read-only).</summary>
    internal const string LastLendDate = "lastLendDate";

    /// <summary>API field name for the date of the latest return (read-only).</summary>
    internal const string LastReturnDate = "lastReturnDate";

    /// <summary>API field name for the creation timestamp (read-only).</summary>
    internal const string CreatedAt = "created_at";

    /// <summary>API field name for the last-update timestamp (read-only).</summary>
    internal const string UpdatedAt = "updated_at";

    /// <summary>API field name for the date the object is removed from the wastebasket (read-only).</summary>
    internal const string DeleteAfterDate = "_deleteAfterDate";

    /// <summary>API field name for the name of the user who deleted the object (read-only).</summary>
    internal const string DeletedBy = "_deletedBy";

    /// <summary>API field name for the object name (max 500 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the article number (max 500 characters).</summary>
    internal const string Identifier = "identifier";

    /// <summary>API field name for the description.</summary>
    internal const string Description = "description";

    /// <summary>API field name for the number of pieces.</summary>
    internal const string Pieces = "pieces";

    /// <summary>API field name for the purchase price.</summary>
    internal const string Price = "price";

    /// <summary>API field name for the purchase date.</summary>
    internal const string PurchaseDate = "purchaseDate";

    /// <summary>API field name for the free-text location name (max 500 characters).</summary>
    internal const string LocationName = "locationName";

    /// <summary>API field name for the flag that allows lending.</summary>
    internal const string LendingAvailable = "lendingAvailable";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering by lending state.</summary>
    internal const string LendingState = "lending__state";

    /// <summary>API query parameter for filtering by soft-deleted state.</summary>
    internal const string Deleted = "deleted";

    /// <summary>API query parameter for excluding a location.</summary>
    internal const string LocationObjectNot = "locationObject__not";

    /// <summary>API query parameter for excluding inventory object groups.</summary>
    internal const string InventoryObjectGroupsNot = "inventoryObjectGroups__not";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
