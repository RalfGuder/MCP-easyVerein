using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for reading DOSB sport disciplines via the easyVerein API.
/// The resource is read-only: the API rejects create, update and delete requests.
/// </summary>
[McpServerToolType]
public sealed class DosbSportTools(IEasyVereinApiClient client)
{
    /// <summary>Lists DOSB sports with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_dosb_sports"), Description("List the DOSB sport disciplines of the organization (read-only)")]
    public async Task<string> ListDosbSports(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Exact title filter")] string? title,
        [Description("DOSB sport number filter")] string? sportNumber,
        [Description("Federation number filter")] string? federationNumber,
        [Description("Ordering (e.g. 'title' or '-sportNumber')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var sports = await client.ListDosbSportsAsync(idIn, title, sportNumber, federationNumber, ordering, search, ct);
            return JsonSerializer.Serialize(sports, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single DOSB sport by its unique identifier.</summary>
    [McpServerTool(Name = "get_dosb_sport"), Description("Retrieve a DOSB sport discipline by its ID (read-only)")]
    public async Task<string> GetDosbSport(
        [Description("The ID of the DOSB sport")] long id,
        CancellationToken ct)
    {
        try
        {
            var sport = await client.GetDosbSportAsync(id, ct);
            return sport != null
                ? JsonSerializer.Serialize(sport, new JsonSerializerOptions { WriteIndented = true })
                : $"DOSB sport with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
