using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Interfaces;

/// <summary>Defines the contract for communicating with the easyVerein REST API.</summary>
public interface IEasyVereinApiClient
{
    /// <summary>Lists members, optionally filtered by ID, membership number, or search terms.</summary>
    /// <param name="id">Optional member ID to filter by.</param>
    /// <param name="membershipNumber">Optional membership number to filter by.</param>
    /// <param name="search">Optional search terms to filter members.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching members.</returns>
    Task<IReadOnlyList<Member>> ListMembersAsync(long? id, string? membershipNumber, string[]? search, CancellationToken ct = default);

    /// <summary>Gets a single member by ID.</summary>
    /// <param name="id">The member ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The member, or <c>null</c> if not found.</returns>
    Task<Member?> GetMemberAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new member with the specified email/username and contact details.</summary>
    /// <param name="emailOrUserName">The email address or username for the new member.</param>
    /// <param name="contactDetails">The contact details for the new member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created member.</returns>
    Task<Member> CreateMemberAsync(string emailOrUserName,
        ContactDetails contactDetails, CancellationToken ct = default);

    /// <summary>Updates an existing member.</summary>
    /// <param name="id">The member ID to update.</param>
    /// <param name="member">The member data to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated member.</returns>
    Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default);

    /// <summary>Deletes a member by ID.</summary>
    /// <param name="id">The member ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteMemberAsync(long id, CancellationToken ct = default);

    /// <summary>Lists contact details, optionally filtered by ID, first name, family name, or full name.</summary>
    /// <param name="id">Optional contact details ID to filter by.</param>
    /// <param name="firstName">Optional first name to filter by.</param>
    /// <param name="familyName">Optional family name to filter by.</param>
    /// <param name="name">Optional full name to search for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching contact details.</returns>
    Task<IReadOnlyList<ContactDetails>> ListContactDetailsAsync(long? id = null, string? firstName = null,
        string? familyName = null, string? name = null, CancellationToken ct = default);

    /// <summary>Gets a single contact details record by ID.</summary>
    /// <param name="id">The contact details ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The contact details, or <c>null</c> if not found.</returns>
    Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new contact details record.</summary>
    /// <param name="contact">The contact details to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created contact details.</returns>
    Task<ContactDetails> CreateContactDetailsAsync(ContactDetails contact, CancellationToken ct = default);

    /// <summary>Partially updates a contact details record.</summary>
    /// <param name="id">The contact details ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated contact details.</returns>
    Task<ContactDetails> UpdateContactDetailsAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Deletes a contact details record by ID.</summary>
    /// <param name="id">The contact details ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteContactDetailsAsync(long id, CancellationToken ct = default);

    /// <summary>Gets all invoices.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all invoices.</returns>
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default);

    /// <summary>Gets a single invoice by ID.</summary>
    /// <param name="id">The invoice ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The invoice, or <c>null</c> if not found.</returns>
    Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new invoice.</summary>
    /// <param name="invoice">The invoice to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created invoice.</returns>
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default);

    /// <summary>Updates an existing invoice.</summary>
    /// <param name="id">The invoice ID to update.</param>
    /// <param name="invoice">The invoice data to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated invoice.</returns>
    Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default);

    /// <summary>Deletes an invoice by ID.</summary>
    /// <param name="id">The invoice ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteInvoiceAsync(long id, CancellationToken ct = default);

    /// <summary>Gets all events.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all events.</returns>
    Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default);

    /// <summary>Gets a single event by ID.</summary>
    /// <param name="id">The event ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The event, or <c>null</c> if not found.</returns>
    Task<Event?> GetEventAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new event.</summary>
    /// <param name="ev">The event to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created event.</returns>
    Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default);

    /// <summary>Updates an existing event.</summary>
    /// <param name="id">The event ID to update.</param>
    /// <param name="ev">The event data to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated event.</returns>
    Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default);

    /// <summary>Deletes an event by ID.</summary>
    /// <param name="id">The event ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteEventAsync(long id, CancellationToken ct = default);

    // Bookings
    /// <summary>Lists bookings, optionally filtered by ID.</summary>
    /// <param name="id">Optional booking ID to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching bookings.</returns>
    Task<IReadOnlyList<Booking>> ListBookingsAsync(long? id = null, CancellationToken ct = default);

    /// <summary>Gets a single booking by ID.</summary>
    /// <param name="id">The booking ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The booking, or <c>null</c> if not found.</returns>
    Task<Booking?> GetBookingAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new booking.</summary>
    /// <param name="booking">The booking to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created booking.</returns>
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct = default);

    /// <summary>Partially updates a booking.</summary>
    /// <param name="id">The booking ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated booking.</returns>
    Task<Booking> UpdateBookingAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Deletes a booking by ID.</summary>
    /// <param name="id">The booking ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteBookingAsync(long id, CancellationToken ct = default);
}
