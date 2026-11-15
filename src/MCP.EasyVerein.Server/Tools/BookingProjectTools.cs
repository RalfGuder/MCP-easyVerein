using System.ComponentModel;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing booking projects (Buchungsprojekte) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class BookingProjectTools(IEasyVereinApiClient client)
{
    /// <summary>Lists booking projects with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_booking_projects"), Description("List all booking projects. Paged: limit (1-100, default 50) and page (from 1); the response holds items, totalCount and hasMore - request the next page while hasMore is true.")]
    public async Task<string> ListBookingProjects(
        [Description("Exact name filter")] string? name,
        [Description("Exact short-label filter (max 4 chars)")] string? @short,
        [Description("Completed flag filter ('true' or 'false')")] string? completed,
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Budget greater-than filter")] string? budgetGt,
        [Description("Budget less-than filter")] string? budgetLt,
        [Description("Ordering (e.g. 'name' or '-budget')")] string? ordering,
        [Description("Search terms")] string[]? search,
        [Description("Entries per page (1-100, default 50)")] int limit = PageRequest.DefaultLimit,
        [Description("Page number, starting at 1 (default 1)")] int page = PageRequest.FirstPage,
        CancellationToken ct = default)
    {
        try
        {
            var projects = await client.ListBookingProjectsAsync(
                name, @short, completed, idIn, budgetGt, budgetLt, ordering, search, ToolJson.ToPageRequest(limit, page), ct);
            return ToolJson.SerializePage(projects);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single booking project by its unique identifier.</summary>
    [McpServerTool(Name = "get_booking_project"), Description("Retrieve a booking project by its ID")]
    public async Task<string> GetBookingProject(
        [Description("The ID of the booking project")] long id,
        CancellationToken ct)
    {
        try
        {
            var project = await client.GetBookingProjectAsync(id, ct);
            return project != null
                ? ToolJson.Serialize(project)
                : $"Booking project with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new booking project in easyVerein. Injects API-required defaults for optional fields.</summary>
    [McpServerTool(Name = "create_booking_project"), Description("Create a new booking project")]
    public async Task<string> CreateBookingProject(
        [Description("The booking project name (required, max 200 chars)")] string name,
        [Description("Short label (required by easyVerein, max 4 chars, must be unique)")] string? @short,
        [Description("Hex color (max 7 chars, e.g. '#ff8800'). Optional.")] string? color,
        [Description("Budget amount (decimal). Default: 0")] decimal? budget,
        [Description("Completed flag. Default: false")] bool? completed,
        [Description("Project cost centre. Default: empty string")] string? projectCostCentre,
        CancellationToken ct)
    {
        try
        {
            if (!HasValue(@short))
                return "ERROR: 'short' is required — easyVerein rejects creation without a short label (max 4 chars, must be unique per project).";

            var project = new BookingProject
            {
                Name = name,
                Short = @short,
                Budget = budget ?? 0m,
                Completed = completed ?? false,
                ProjectCostCentre = HasValue(projectCostCentre) ? projectCostCentre : string.Empty
            };
            if (HasValue(color)) project.Color = color;

            var created = await client.CreateBookingProjectAsync(project, ct);
            return ToolJson.Serialize(created);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing booking project (PATCH — only provided fields are changed).</summary>
    [McpServerTool(Name = "update_booking_project"), Description("Update a booking project (only provided fields are changed)")]
    public async Task<string> UpdateBookingProject(
        [Description("The ID of the booking project to update")] long id,
        [Description("New name")] string? name,
        [Description("New hex color")] string? color,
        [Description("New short label")] string? @short,
        [Description("New budget")] decimal? budget,
        [Description("New completed flag")] bool? completed,
        [Description("New project cost centre")] string? projectCostCentre,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[BookingProjectFields.Name] = name!;
            if (HasValue(color)) patch[BookingProjectFields.Color] = color!;
            if (HasValue(@short)) patch[BookingProjectFields.Short] = @short!;
            if (budget.HasValue) patch[BookingProjectFields.Budget] = budget.Value;
            if (completed.HasValue) patch[BookingProjectFields.Completed] = completed.Value;
            if (HasValue(projectCostCentre)) patch[BookingProjectFields.ProjectCostCentre] = projectCostCentre!;

            var updated = await client.UpdateBookingProjectAsync(id, patch, ct);
            return ToolJson.Serialize(updated);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a booking project by its unique identifier.</summary>
    [McpServerTool(Name = "delete_booking_project"), Description("Delete a booking project. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteBookingProject(
        [Description("The ID of the booking project to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteBookingProjectAsync(id, ct);
            return $"Booking project with ID {id} has been deleted.";
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
