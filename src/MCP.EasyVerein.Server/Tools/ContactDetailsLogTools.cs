using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing contact-details logs (change/audit logs of contact details) via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class ContactDetailsLogTools(IEasyVereinApiClient client)
{
    /// <summary>Lists contact-details logs with optional filters and automatic pagination.</summary>
    [McpServerTool(Name = "list_contact_details_logs"), Description("List all contact-details logs (change/audit logs of contact details)")]
    public async Task<string> ListContactDetailsLogs(
        [Description("Comma-separated list of IDs filter")] string? idIn,
        [Description("Exact date filter (ISO-8601)")] string? date,
        [Description("Date greater-or-equal filter (ISO-8601)")] string? dateGte,
        [Description("Date less-or-equal filter (ISO-8601)")] string? dateLte,
        [Description("Ordering (e.g. 'date' or '-date')")] string? ordering,
        CancellationToken ct)
    {
        try
        {
            var logs = await client.ListContactDetailsLogsAsync(idIn, date, dateGte, dateLte, ordering, ct);
            return JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Retrieves a single contact-details log by its unique identifier.</summary>
    [McpServerTool(Name = "get_contact_details_log"), Description("Retrieve a contact-details log by its ID")]
    public async Task<string> GetContactDetailsLog(
        [Description("The ID of the contact-details log")] long id,
        CancellationToken ct)
    {
        try
        {
            var log = await client.GetContactDetailsLogAsync(id, ct);
            return log != null
                ? JsonSerializer.Serialize(log, new JsonSerializerOptions { WriteIndented = true })
                : $"Contact-details log with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Creates a new contact-details log in easyVerein.</summary>
    [McpServerTool(Name = "create_contact_details_log"), Description("Create a new contact-details log entry")]
    public async Task<string> CreateContactDetailsLog(
        [Description("Kind of log, lowercase: 'balance', 'custom', 'membership', 'email', 'dsgvo', 'articles' (default 'custom'). Note: the API rejects capitalized values.")] string? kind,
        [Description("Related address ID (foreign key to contact-details)")] long? relatedAddress,
        [Description("Related file path (max 360 chars)")] string? relatedFile,
        [Description("Date and time of the event (ISO-8601)")] string? date,
        [Description("Description text of the log")] string? description,
        [Description("Show the log to the related member (default false)")] bool? shared,
        CancellationToken ct)
    {
        try
        {
            var log = new ContactDetailsLog();
            if (HasValue(kind)) log.Kind = kind;
            if (relatedAddress.HasValue) log.RelatedAddress = relatedAddress;
            if (HasValue(relatedFile)) log.RelatedFile = relatedFile;
            if (HasValue(date)) log.Date = DateTime.Parse(date!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            if (HasValue(description)) log.Description = description;
            if (shared.HasValue) log.Shared = shared;

            var created = await client.CreateContactDetailsLogAsync(log, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Updates an existing contact-details log (PATCH — only provided fields are changed). Note: the API only allows changing the 'shared' attribute on already-created log events; other fields are rejected with HTTP 400.</summary>
    [McpServerTool(Name = "update_contact_details_log"), Description("Update a contact-details log. Note: easyVerein only permits changing the 'shared' attribute on existing logs — other fields are rejected.")]
    public async Task<string> UpdateContactDetailsLog(
        [Description("The ID of the contact-details log to update")] long id,
        [Description("New kind of log")] string? kind,
        [Description("New related address ID (foreign key to contact-details)")] long? relatedAddress,
        [Description("New related file path")] string? relatedFile,
        [Description("New date and time (ISO-8601)")] string? date,
        [Description("New description text")] string? description,
        [Description("New shared flag")] bool? shared,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(kind)) patch[ContactDetailsLogFields.Kind] = kind!;
            if (relatedAddress.HasValue) patch[ContactDetailsLogFields.RelatedAddress] = relatedAddress.Value;
            if (HasValue(relatedFile)) patch[ContactDetailsLogFields.RelatedFile] = relatedFile!;
            if (HasValue(date))
                patch[ContactDetailsLogFields.Date] =
                    DateTime.Parse(date!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToString("O", CultureInfo.InvariantCulture);
            if (HasValue(description)) patch[ContactDetailsLogFields.Description] = description!;
            if (shared.HasValue) patch[ContactDetailsLogFields.Shared] = shared.Value;

            var updated = await client.UpdateContactDetailsLogAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Deletes a contact-details log by its unique identifier.</summary>
    [McpServerTool(Name = "delete_contact_details_log"), Description("Delete a contact-details log. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteContactDetailsLog(
        [Description("The ID of the contact-details log to delete")] long id,
        CancellationToken ct)
    {
        try
        {
            await client.DeleteContactDetailsLogAsync(id, ct);
            return $"Contact-details log with ID {id} has been deleted.";
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
