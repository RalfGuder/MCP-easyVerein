using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing the organization's member forums via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class ForumTools(IEasyVereinApiClient client)
{
    /// <summary>Maximum length of a forum name accepted by the API.</summary>
    private const int MaxNameLength = 100;

    /// <summary>Lists forums with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_forums"), Description("List the organization's member forums")]
    public async Task<string> ListForums(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Exact forum name filter")] string? name,
        [Description("Forum name to exclude")] string? nameNot,
        [Description("Exact slug filter")] string? slug,
        [Description("Slug to exclude")] string? slugNot,
        [Description("Forum kind filter (0 = regular forum)")] int? type,
        [Description("Created after this date (YYYY-MM-DD)")] string? createdGt,
        [Description("Created before this date (YYYY-MM-DD)")] string? createdLt,
        [Description("Updated after this date (YYYY-MM-DD)")] string? updatedGt,
        [Description("Updated before this date (YYYY-MM-DD)")] string? updatedLt,
        [Description("Ordering (e.g. 'order' or '-created')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var forums = await client.ListForumsAsync(
                idIn, name, nameNot, slug, slugNot, type,
                createdGt, createdLt, updatedGt, updatedLt, ordering, search, ct);
            return JsonSerializer.Serialize(forums, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single forum by its unique identifier.</summary>
    [McpServerTool(Name = "get_forum"), Description("Retrieve a forum by its ID")]
    public async Task<string> GetForum(
        [Description("The ID of the forum")] long id,
        CancellationToken ct)
    {
        try
        {
            var forum = await client.GetForumAsync(id, ct);
            return forum != null
                ? JsonSerializer.Serialize(forum, new JsonSerializerOptions { WriteIndented = true })
                : $"Forum with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new forum in easyVerein.</summary>
    [McpServerTool(Name = "create_forum"), Description("Create a new member forum (visible to members)")]
    public async Task<string> CreateForum(
        [Description("The forum name (required, max 100 chars)")] string name,
        [Description("Forum description")] string? description,
        [Description("External forum link (URL, max 200 chars)")] string? link,
        [Description("Count how often the forum link is followed")] bool? linkRedirects,
        [Description("Sort position")] int? order,
        [Description("Show the forum in its parent's sub-forum list")] bool? displaySubForumList,
        CancellationToken ct)
    {
        try
        {
            if (!HasValue(name) || string.IsNullOrWhiteSpace(name))
                return "ERROR: The forum name is required.";
            if (name.Length > MaxNameLength)
                return $"ERROR: The forum name must not exceed {MaxNameLength} characters.";

            var forum = new Forum
            {
                Name = name,
                Description = HasValue(description) ? description : null,
                Link = HasValue(link) ? link : null,
                LinkRedirects = linkRedirects,
                Order = order,
                DisplaySubForumList = displaySubForumList
            };
            var created = await client.CreateForumAsync(forum, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates a forum (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_forum"), Description("Update a forum (only provided fields are changed)")]
    public async Task<string> UpdateForum(
        [Description("The ID of the forum to update")] long id,
        [Description("New forum name (max 100 chars)")] string? name,
        [Description("New forum description")] string? description,
        [Description("New external forum link (URL, max 200 chars)")] string? link,
        [Description("Count how often the forum link is followed")] bool? linkRedirects,
        [Description("New sort position")] int? order,
        [Description("Show the forum in its parent's sub-forum list")] bool? displaySubForumList,
        CancellationToken ct)
    {
        try
        {
            if (HasValue(name) && name!.Length > MaxNameLength)
                return $"ERROR: The forum name must not exceed {MaxNameLength} characters.";

            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[ForumFields.Name] = name!;
            if (HasValue(description)) patch[ForumFields.Description] = description!;
            if (HasValue(link)) patch[ForumFields.Link] = link!;
            if (linkRedirects.HasValue) patch[ForumFields.LinkRedirects] = linkRedirects.Value;
            if (order.HasValue) patch[ForumFields.Order] = order.Value;
            if (displaySubForumList.HasValue) patch[ForumFields.DisplaySubForumList] = displaySubForumList.Value;

            var updated = await client.UpdateForumAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a forum by its unique identifier.</summary>
    [McpServerTool(Name = "delete_forum"), Description("Delete a forum. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteForum(
        [Description("The ID of the forum to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteForumAsync(id, ct);
            return $"Forum with ID {id} has been deleted.";
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
