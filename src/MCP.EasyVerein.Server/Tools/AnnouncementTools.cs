using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing announcements via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class AnnouncementTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists announcements with optional filters and automatic pagination.
    /// </summary>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching announcements, or an error message.</returns>
    [McpServerTool(Name = "list_announcements"), Description("List all announcements")]
    public async Task<string> ListAnnouncements(
        [Description("Ordering criteria (e.g. 'start' or '-start')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var announcements = await client.ListAnnouncementsAsync(ordering, search, ct);
            return JsonSerializer.Serialize(announcements, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single announcement by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the announcement.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the announcement, or a not-found message.</returns>
    [McpServerTool(Name = "get_announcement"), Description("Retrieve an announcement by its ID")]
    public async Task<string> GetAnnouncement(
        [Description("The ID of the announcement")] long id,
        CancellationToken ct)
    {
        try
        {
            var announcement = await client.GetAnnouncementAsync(id, ct);
            return announcement != null
                ? JsonSerializer.Serialize(announcement, new JsonSerializerOptions { WriteIndented = true })
                : $"Announcement with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new announcement in easyVerein.
    /// </summary>
    /// <param name="text">The HTML text content (required).</param>
    /// <param name="start">Optional start date/time in ISO 8601 format.</param>
    /// <param name="end">Optional end date/time in ISO 8601 format.</param>
    /// <param name="showBanner">Optional flag whether to show as banner.</param>
    /// <param name="isDismissible">Optional flag whether the announcement can be dismissed.</param>
    /// <param name="isPublic">Optional flag whether the announcement is publicly visible.</param>
    /// <param name="showForNormalMembers">Optional flag whether to show for normal members.</param>
    /// <param name="platform">Optional platform identifier.</param>
    /// <param name="bannerLevel">Optional banner level (e.g. success, warning, info, danger).</param>
    /// <param name="accountTypeVisibility">Optional account type visibility.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created announcement, or an error message.</returns>
    [McpServerTool(Name = "create_announcement"), Description("Create a new announcement")]
    public async Task<string> CreateAnnouncement(
        [Description("The HTML text content (required)")] string text,
        [Description("Start date/time (ISO 8601)")] string? start,
        [Description("End date/time (ISO 8601)")] string? end,
        [Description("Show as banner")] bool? showBanner,
        [Description("Can be dismissed by users")] bool? isDismissible,
        [Description("Publicly visible")] bool? isPublic,
        [Description("Show for normal members")] bool? showForNormalMembers,
        [Description("Platform (integer)")] int? platform,
        [Description("Banner level (e.g. success, warning, info, danger)")] string? bannerLevel,
        [Description("Account type visibility (integer)")] int? accountTypeVisibility,
        CancellationToken ct)
    {
        try
        {
            var announcement = new Announcement { Text = text };

            if (DateTime.TryParse(start, out var s)) announcement.Start = s;
            if (DateTime.TryParse(end, out var e)) announcement.End = e;
            if (showBanner.HasValue) announcement.ShowBanner = showBanner.Value;
            if (isDismissible.HasValue) announcement.IsDismissible = isDismissible.Value;
            if (isPublic.HasValue) announcement.IsPublic = isPublic.Value;
            if (showForNormalMembers.HasValue) announcement.ShowForNormalMembers = showForNormalMembers.Value;
            if (platform.HasValue) announcement.Platform = platform.Value;
            if (!string.IsNullOrEmpty(bannerLevel)) announcement.BannerLevel = bannerLevel;
            if (accountTypeVisibility.HasValue) announcement.AccountTypeVisibility = accountTypeVisibility.Value;

            var created = await client.CreateAnnouncementAsync(announcement, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing announcement. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the announcement to update.</param>
    /// <param name="text">Optional new HTML text content.</param>
    /// <param name="start">Optional new start date/time in ISO 8601 format.</param>
    /// <param name="end">Optional new end date/time in ISO 8601 format.</param>
    /// <param name="showBanner">Optional new show-banner flag.</param>
    /// <param name="isDismissible">Optional new dismissible flag.</param>
    /// <param name="isPublic">Optional new public visibility flag.</param>
    /// <param name="showForNormalMembers">Optional new normal-members visibility flag.</param>
    /// <param name="platform">Optional new platform identifier.</param>
    /// <param name="bannerLevel">Optional new banner level.</param>
    /// <param name="accountTypeVisibility">Optional new account type visibility.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated announcement, or an error message.</returns>
    [McpServerTool(Name = "update_announcement"), Description("Update an announcement (only provided fields are changed)")]
    public async Task<string> UpdateAnnouncement(
        [Description("The ID of the announcement to update")] long id,
        [Description("The new HTML text content")] string? text,
        [Description("New start date/time (ISO 8601)")] string? start,
        [Description("New end date/time (ISO 8601)")] string? end,
        [Description("Show as banner")] bool? showBanner,
        [Description("Can be dismissed by users")] bool? isDismissible,
        [Description("Publicly visible")] bool? isPublic,
        [Description("Show for normal members")] bool? showForNormalMembers,
        [Description("Platform (integer)")] int? platform,
        [Description("Banner level (e.g. success, warning, info, danger)")] string? bannerLevel,
        [Description("Account type visibility (integer)")] int? accountTypeVisibility,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();

            if (HasValue(text)) patch[AnnouncementFields.Text] = text!;
            if (DateTime.TryParse(start, out var s)) patch[AnnouncementFields.Start] = s;
            if (DateTime.TryParse(end, out var e)) patch[AnnouncementFields.End] = e;
            if (showBanner.HasValue) patch[AnnouncementFields.ShowBanner] = showBanner.Value;
            if (isDismissible.HasValue) patch[AnnouncementFields.IsDismissible] = isDismissible.Value;
            if (isPublic.HasValue) patch[AnnouncementFields.IsPublic] = isPublic.Value;
            if (showForNormalMembers.HasValue) patch[AnnouncementFields.ShowForNormalMembers] = showForNormalMembers.Value;
            if (platform.HasValue) patch[AnnouncementFields.Platform] = platform.Value;
            if (HasValue(bannerLevel)) patch[AnnouncementFields.BannerLevel] = bannerLevel!;
            if (accountTypeVisibility.HasValue) patch[AnnouncementFields.AccountTypeVisibility] = accountTypeVisibility.Value;

            var updated = await client.UpdateAnnouncementAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes an announcement by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the announcement to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_announcement"), Description("Delete an announcement. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteAnnouncement(
        [Description("The ID of the announcement to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteAnnouncementAsync(id, ct);
            return $"Announcement with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
