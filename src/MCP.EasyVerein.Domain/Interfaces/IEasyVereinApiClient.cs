using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Interfaces;

/// <summary>Defines the contract for communicating with the easyVerein REST API.</summary>
public interface IEasyVereinApiClient
{
    /// <summary>Lists all announcements, optionally filtered.</summary>
    Task<IReadOnlyList<Announcement>> ListAnnouncementsAsync(
        string? ordering = null, string[]? search = null,
        CancellationToken ct = default);

    /// <summary>Gets a single announcement by ID.</summary>
    Task<Announcement?> GetAnnouncementAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new announcement.</summary>
    Task<Announcement> CreateAnnouncementAsync(Announcement announcement, CancellationToken ct = default);

    /// <summary>Updates an announcement with PATCH semantics.</summary>
    Task<Announcement> UpdateAnnouncementAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Deletes an announcement.</summary>
    Task DeleteAnnouncementAsync(long id, CancellationToken ct = default);

    /// <summary>Lists all bank accounts, optionally filtered.</summary>
    /// <param name="name">Optional name filter (exact match).</param>
    /// <param name="iban">Optional IBAN filter (exact match).</param>
    /// <param name="bic">Optional BIC filter (exact match).</param>
    /// <param name="accountHolder">Optional account-holder filter.</param>
    /// <param name="bankName">Optional bank-name filter.</param>
    /// <param name="idIn">Optional comma-separated list of IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IReadOnlyList<BankAccount>> ListBankAccountsAsync(
        string? name = null, string? iban = null, string? bic = null,
        string? accountHolder = null, string? bankName = null,
        string? idIn = null, string? ordering = null, string[]? search = null,
        CancellationToken ct = default);

    /// <summary>Gets a single bank account by ID.</summary>
    /// <param name="id">The bank account ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The bank account, or <c>null</c> if not found.</returns>
    Task<BankAccount?> GetBankAccountAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new bank account.</summary>
    /// <param name="bankAccount">The bank account to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created bank account.</returns>
    Task<BankAccount> CreateBankAccountAsync(BankAccount bankAccount, CancellationToken ct = default);

    /// <summary>Partially updates a bank account (PATCH semantics).</summary>
    /// <param name="id">The bank account ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated bank account.</returns>
    Task<BankAccount> UpdateBankAccountAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Deletes a bank account by ID.</summary>
    /// <param name="id">The bank account ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteBankAccountAsync(long id, CancellationToken ct = default);

    /// <summary>Creates a new booking.</summary>
    /// <param name="booking">The booking to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created booking.</returns>
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct = default);

    /// <summary>Creates a new calendar.</summary>
    /// <param name="calendar">The calendar to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created calendar.</returns>
    Task<Calendar> CreateCalendarAsync(Calendar calendar, CancellationToken ct = default);

    /// <summary>Creates a new contact details record.</summary>
    /// <param name="contact">The contact details to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created contact details.</returns>
    Task<ContactDetails> CreateContactDetailsAsync(ContactDetails contact, CancellationToken ct = default);

    /// <summary>Creates a new event.</summary>
    /// <param name="ev">The event to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created event.</returns>
    Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default);

    /// <summary>Creates a new invoice.</summary>
    /// <param name="invoice">The invoice to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created invoice.</returns>
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default);

    /// <summary>Creates a new member with the specified email/username and contact details.</summary>
    /// <param name="emailOrUserName">The email address or username for the new member.</param>
    /// <param name="contactDetails">The contact details for the new member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created member.</returns>
    Task<Member> CreateMemberAsync(string emailOrUserName,
        ContactDetails contactDetails, CancellationToken ct = default);

    /// <summary>Deletes a booking by ID.</summary>
    /// <param name="id">The booking ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteBookingAsync(long id, CancellationToken ct = default);

    /// <summary>Deletes a calendar by ID.</summary>
    /// <param name="id">The calendar ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteCalendarAsync(long id, CancellationToken ct = default);

    /// <summary>Deletes a contact details record by ID.</summary>
    /// <param name="id">The contact details ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteContactDetailsAsync(long id, CancellationToken ct = default);

    /// <summary>Deletes an event by ID.</summary>
    /// <param name="id">The event ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteEventAsync(long id, CancellationToken ct = default);

    /// <summary>Deletes an invoice by ID.</summary>
    /// <param name="id">The invoice ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteInvoiceAsync(long id, CancellationToken ct = default);

    /// <summary>Deletes a member by ID.</summary>
    /// <param name="id">The member ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteMemberAsync(long id, CancellationToken ct = default);

    /// <summary>Gets a single booking by ID.</summary>
    /// <param name="id">The booking ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The booking, or <c>null</c> if not found.</returns>
    Task<Booking?> GetBookingAsync(long id, CancellationToken ct = default);

    /// <summary>Gets a single calendar by ID.</summary>
    /// <param name="id">The calendar ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The calendar, or <c>null</c> if not found.</returns>
    Task<Calendar?> GetCalendarAsync(long id, CancellationToken ct = default);

    /// <summary>Gets a single contact details record by ID.</summary>
    /// <param name="id">The contact details ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The contact details, or <c>null</c> if not found.</returns>
    Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default);

    /// <summary>Gets a single event by ID.</summary>
    /// <param name="id">The event ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The event, or <c>null</c> if not found.</returns>
    Task<Event?> GetEventAsync(long id, CancellationToken ct = default);

    /// <summary>Lists events, optionally filtered by name, date range, calendar, and other criteria.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="startGte">Optional start date greater than or equal filter (ISO 8601).</param>
    /// <param name="startLte">Optional start date less than or equal filter (ISO 8601).</param>
    /// <param name="endGte">Optional end date greater than or equal filter (ISO 8601).</param>
    /// <param name="endLte">Optional end date less than or equal filter (ISO 8601).</param>
    /// <param name="calendar">Optional calendar ID filter.</param>
    /// <param name="canceled">Optional canceled filter.</param>
    /// <param name="isPublic">Optional public visibility filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching events.</returns>
    Task<IReadOnlyList<Event>> ListEventsAsync(string? name = null, string? startGte = null,
        string? startLte = null, string? endGte = null, string? endLte = null,
        string? calendar = null, string? canceled = null, string? isPublic = null,
        string? idIn = null, string? ordering = null, string[]? search = null,
        CancellationToken ct = default);

    /// <summary>Gets a single invoice by ID.</summary>
    /// <param name="id">The invoice ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The invoice, or <c>null</c> if not found.</returns>
    Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default);

    /// <summary>Gets all invoices.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all invoices.</returns>
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default);

    /// <summary>Gets a single member by ID.</summary>
    /// <param name="id">The member ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The member, or <c>null</c> if not found.</returns>
    Task<Member?> GetMemberAsync(long id, CancellationToken ct = default);

    // Bookings
    /// <summary>Lists bookings, optionally filtered by ID, date range, ordering, or search terms.</summary>
    /// <param name="id">Optional booking ID to filter by.</param>
    /// <param name="date">Optional exact date filter.</param>
    /// <param name="dateGt">Optional filter for dates greater than the specified value.</param>
    /// <param name="dateLt">Optional filter for dates less than the specified value.</param>
    /// <param name="ordering">Optional ordering criterion for the results.</param>
    /// <param name="search">Optional search terms to filter bookings.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching bookings.</returns>
    Task<IReadOnlyList<Booking>> ListBookingsAsync(long? id = null, string? date = default, string? dateGt = default, string? dateLt = default, string? ordering = default, string[]? search = default, CancellationToken ct = default);

    /// <summary>Lists calendars, optionally filtered by name, color, short, ordering, or search terms.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="color">Optional color filter.</param>
    /// <param name="short_">Optional short name filter.</param>
    /// <param name="nameNot">Optional name negation filter.</param>
    /// <param name="colorNot">Optional color negation filter.</param>
    /// <param name="shortNot">Optional short name negation filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="allowedGroups">Optional allowed groups filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching calendars.</returns>
    Task<IReadOnlyList<Calendar>> ListCalendarsAsync(string? name = null, string? color = null,
        string? short_ = null, string? nameNot = null, string? colorNot = null,
        string? shortNot = null, string? idIn = null, string? allowedGroups = null,
        string? ordering = null, string[]? search = null, CancellationToken ct = default);

    /// <summary>Lists contact details, optionally filtered by ID, first name, family name, or full name.</summary>
    /// <param name="id">Optional contact details ID to filter by.</param>
    /// <param name="firstName">Optional first name to filter by.</param>
    /// <param name="familyName">Optional family name to filter by.</param>
    /// <param name="name">Optional full name to search for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching contact details.</returns>
    Task<IReadOnlyList<ContactDetails>> ListContactDetailsAsync(long? id = null, string? firstName = null,
        string? familyName = null, string? name = null, CancellationToken ct = default);

    /// <summary>Lists members, optionally filtered by ID, membership number, or search terms.</summary>
    /// <param name="id">Optional member ID to filter by.</param>
    /// <param name="membershipNumber">Optional membership number to filter by.</param>
    /// <param name="search">Optional search terms to filter members.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching members.</returns>
    Task<IReadOnlyList<Member>> ListMembersAsync(long? id, string? membershipNumber, string[]? search, CancellationToken ct = default);
    /// <summary>Partially updates a booking.</summary>
    /// <param name="id">The booking ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated booking.</returns>
    Task<Booking> UpdateBookingAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Partially updates a calendar.</summary>
    /// <param name="id">The calendar ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated calendar.</returns>
    Task<Calendar> UpdateCalendarAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>Partially updates a contact details record.</summary>
    /// <param name="id">The contact details ID to update.</param>
    /// <param name="patchData">An object containing the fields to patch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated contact details.</returns>
    Task<ContactDetails> UpdateContactDetailsAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing event.
    /// </summary>
    /// <param name="id">The event ID to update.</param>
    /// <param name="patchData">The patch data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The raw JSON response from the API.</returns>
    Task<string> UpdateEventAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing invoice.
    /// </summary>
    /// <param name="id">The invoice ID to update.</param>
    /// <param name="patchData">The patch data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated invoice.</returns>
    Task<Invoice> UpdateInvoiceAsync(long id, object patchData, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing member.
    /// </summary>
    /// <param name="id">The member ID to update.</param>
    /// <param name="patchData">The patch data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated member.</returns>
    Task<Member> UpdateMemberAsync(long id, object patchData, CancellationToken ct = default);
}
