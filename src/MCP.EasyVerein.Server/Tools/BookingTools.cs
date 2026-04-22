using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing bookings via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class BookingTools(IEasyVereinApiClient client, EasyVereinConfiguration config)
{
    /// <summary>
    /// Lists bookings with an optional ID filter and automatic pagination.
    /// </summary>
    /// <param name="id">Optional booking ID filter.</param>
    /// <param name="date">The date.</param>
    /// <param name="dateGt">The date gt.</param>
    /// <param name="dateLt">The date lt.</param>
    /// <param name="search">The search.</param>
    /// <param name="ordering">The ordering.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching bookings, or an error message.</returns>
    [McpServerTool(Name = "list_bookings"), Description("List all bookings")]
    public async Task<string> ListBookings(
        [Description("The ID of a booking")] long? id, 
        [Description("Booking date")] string? date,
        [Description("Booking date greater than")] string? dateGt,
        [Description("Booking date less than")] string? dateLt,
        [Description("Search terms")] string[]? search, 
        [Description("Ordering criteria")] string? ordering, CancellationToken ct)
    {
        try
        {
            var bookings = await client.ListBookingsAsync(id, date, dateGt, dateLt, ordering, search, ct);
            return JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single booking by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the booking, or a not-found message.</returns>
    [McpServerTool(Name = "get_booking"), Description("Retrieve a booking by its ID")]
    public async Task<string> GetBooking(
        [Description("The ID of the booking")] long id, CancellationToken ct)
    {
        try
        {
            var booking = await client.GetBookingAsync(id, ct);
            return booking != null
                ? JsonSerializer.Serialize(booking, new JsonSerializerOptions { WriteIndented = true })
                : $"Booking with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new booking in easyVerein.
    /// </summary>
    /// <param name="amount">The booking amount.</param>
    /// <param name="receiver">The receiver of the booking.</param>
    /// <param name="description">An optional description.</param>
    /// <param name="date">An optional booking date (ISO 8601 format).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created booking, or an error message.</returns>
    [McpServerTool(Name = "create_booking"), Description("Create a new booking")]
    public async Task<string> CreateBooking(
        [Description("The booking amount")] decimal amount,
        [Description("The receiver of the booking")] string receiver,
        [Description("An optional description")] string? description,
        [Description("The booking date (ISO 8601)")] string? date,
        CancellationToken ct)
    {
        try
        {
            var booking = new Booking
            {
                Amount = amount,
                Receiver = receiver,
                Description = description,
                Date = date != null ? DateTime.Parse(date) : null
            };
            var created = await client.CreateBookingAsync(booking, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing booking. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="amount">An optional new amount.</param>
    /// <param name="description">An optional new description.</param>
    /// <param name="date">An optional new date (ISO 8601 format).</param>
    /// <param name="receiver">An optional new receiver.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated booking, or an error message.</returns>
    [McpServerTool(Name = "update_booking"), Description("Update a booking (only provided fields are changed)")]
    public async Task<string> UpdateBooking(
        [Description("The ID of the booking")] long id,
        [Description("The new amount")] decimal? amount,
        [Description("The new description")] string? description,
        [Description("The new date (ISO 8601)")] string? date,
        [Description("The new receiver")] string? receiver,
        [Description("The new booking-project ID (the numeric ID of the target booking-project; the tool builds the full resource URL)")] long? bookingProjectId,
        [Description("The new billing-account ID (SKR42 account; the numeric ID of the billing-account entry; the tool builds the full resource URL)")] long? billingAccountId,
        [Description("The new SKR42 sphere (integer). Typical values: 1=ideeller Bereich, 2=Vermögensverwaltung, 3=Zweckbetrieb, 4=wirtschaftlicher Geschäftsbetrieb, 9=unkategorisiert (default)")] long? sphere,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (amount != null) patch[BookingFields.Amount] = amount;
            if (description != null) patch[BookingFields.Description] = description;
            if (date != null) patch[BookingFields.Date] = date;
            if (receiver != null) patch[BookingFields.Receiver] = receiver;
            if (bookingProjectId != null)
                patch[BookingFields.BookingProject] = $"{config.GetVersionedBaseUrl()}/booking-project/{bookingProjectId.Value}";
            if (billingAccountId != null)
                patch[BookingFields.BillingAccount] = $"{config.GetVersionedBaseUrl()}/billing-account/{billingAccountId.Value}";
            if (sphere != null) patch[BookingFields.Sphere] = sphere;

            var updated = await client.UpdateBookingAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes a booking by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_booking"), Description("Delete a booking. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteBooking(
        [Description("The ID of the booking")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteBookingAsync(id, ct);
            return $"Booking with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }
}
