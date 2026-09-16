using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for the public easyVerein product idea board (feature requests).
/// These are not organization data: submitted requests and votes are visible to the vendor and other organizations.
/// The API does not allow updating or deleting feature requests.
/// </summary>
[McpServerToolType]
public sealed class FeatureRequestTools(IEasyVereinApiClient client)
{
    /// <summary>Lists feature requests with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_feature_requests"), Description("List entries of the public easyVerein product idea board (feature requests); not organization data")]
    public async Task<string> ListFeatureRequests(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Status code filter (numeric, e.g. '3')")] string? status,
        [Description("Category code filter (numeric, e.g. '10')")] string? category,
        [Description("true = only requests submitted by the current user")] bool? authorIsme,
        [Description("Ordering (e.g. '-proVotesCount' or '-date')")] string? ordering,
        [Description("Search terms (allowed fields: title, description)")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var requests = await client.ListFeatureRequestsAsync(idIn, status, category, authorIsme, ordering, search, ct);
            return JsonSerializer.Serialize(requests, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single feature request by its unique identifier.</summary>
    [McpServerTool(Name = "get_feature_request"), Description("Retrieve an entry of the public easyVerein product idea board by its ID")]
    public async Task<string> GetFeatureRequest(
        [Description("The ID of the feature request")] long id,
        CancellationToken ct)
    {
        try
        {
            var request = await client.GetFeatureRequestAsync(id, ct);
            return request != null
                ? JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true })
                : $"Feature request with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Submits a new feature request to the public easyVerein idea board.</summary>
    [McpServerTool(Name = "create_feature_request"), Description(
        "PUBLIC ACTION: Submit a new feature request to the easyVerein product idea board. " +
        "It is visible to the vendor and other organizations and cannot be changed or deleted afterwards. " +
        "Only call this after explicit confirmation by the user.")]
    public async Task<string> CreateFeatureRequest(
        [Description("The title (required), phrased as a wish, e.g. 'dass es einen DATEV-Export gibt'")] string title,
        [Description("The description (required)")] string description,
        [Description("Optional numeric category code")] int? category,
        CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                return "ERROR: Title and description are required.";

            var request = new FeatureRequest { Title = title, Description = description, Category = category };
            var created = await client.CreateFeatureRequestAsync(request, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Casts a vote for or against a feature request on the public easyVerein idea board.</summary>
    [McpServerTool(Name = "vote_feature_request"), Description(
        "PUBLIC ACTION: Vote for or against a feature request on the easyVerein product idea board. " +
        "Only call this after explicit confirmation by the user.")]
    public async Task<string> VoteFeatureRequest(
        [Description("The ID of the feature request")] long id,
        [Description("true = vote in favor, false = vote against")] bool inFavor,
        CancellationToken ct)
    {
        try
        {
            var response = await client.VoteFeatureRequestAsync(id, inFavor, ct);
            var direction = inFavor ? "in favor of" : "against";
            return $"Voted {direction} feature request {id}. API response: {response}";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
