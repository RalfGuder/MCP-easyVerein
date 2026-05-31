using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the custom-field API endpoint with field selection and filters.
/// </summary>
internal class CustomFieldQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional color filter (exact match).</summary>
    internal string? Color { get; set; }

    /// <summary>Gets or sets an optional kind-code filter.</summary>
    internal string? Kind { get; set; }

    /// <summary>Gets or sets an optional field-type filter.</summary>
    internal string? SettingsType { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of field types filter.</summary>
    internal string? SettingsTypeIn { get; set; }

    /// <summary>Gets or sets an optional editable-in-member-area filter.</summary>
    internal bool? MemberEdit { get; set; }

    /// <summary>Gets or sets an optional show-in-member-area filter.</summary>
    internal bool? MemberShow { get; set; }

    /// <summary>Gets or sets an optional soft-delete filter.</summary>
    internal bool? Deleted { get; set; }

    /// <summary>Gets or sets an optional collection (tab/group) ID filter.</summary>
    internal string? Collection { get; set; }

    /// <summary>Gets or sets an optional no-collection filter.</summary>
    internal bool? CollectionIsnull { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms (allowed fields: name, color, short).</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            CustomFieldFields.Id + "," +
            CustomFieldFields.Name + "," +
            CustomFieldFields.Color + "," +
            CustomFieldFields.Short + "," +
            CustomFieldFields.SettingsType + "," +
            CustomFieldFields.Kind + "," +
            CustomFieldFields.Description + "," +
            CustomFieldFields.Additional + "," +
            CustomFieldFields.MemberShow + "," +
            CustomFieldFields.MemberEdit + "," +
            CustomFieldFields.MemberDsgvo + "," +
            CustomFieldFields.Position + "," +
            CustomFieldFields.Collection + "," +
            CustomFieldFields.NeedsAdminApproval +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{CustomFieldFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{CustomFieldFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Color))
            parts.Add($"{CustomFieldFields.Color}={Uri.EscapeDataString(Color)}");
        if (!string.IsNullOrEmpty(Kind))
            parts.Add($"{CustomFieldFields.Kind}={Uri.EscapeDataString(Kind)}");
        if (!string.IsNullOrEmpty(SettingsType))
            parts.Add($"{CustomFieldFields.SettingsType}={Uri.EscapeDataString(SettingsType)}");
        if (!string.IsNullOrEmpty(SettingsTypeIn))
            parts.Add($"{CustomFieldFields.SettingsTypeIn}={Uri.EscapeDataString(SettingsTypeIn)}");
        if (MemberEdit.HasValue)
            parts.Add($"{CustomFieldFields.MemberEdit}={(MemberEdit.Value ? "true" : "false")}");
        if (MemberShow.HasValue)
            parts.Add($"{CustomFieldFields.MemberShow}={(MemberShow.Value ? "true" : "false")}");
        if (Deleted.HasValue)
            parts.Add($"{CustomFieldFields.Deleted}={(Deleted.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(Collection))
            parts.Add($"{CustomFieldFields.Collection}={Uri.EscapeDataString(Collection)}");
        if (CollectionIsnull.HasValue)
            parts.Add($"{CustomFieldFields.CollectionIsnull}={(CollectionIsnull.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{CustomFieldFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{CustomFieldFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
