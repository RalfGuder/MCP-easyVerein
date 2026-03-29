using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Interfaces;

/// <summary>
/// Interface für den easyVerein API-Client (FR-002 bis FR-007).
/// </summary>
public interface IEasyVereinApiClient
{
    // Mitglieder (FR-003, FR-004)
    Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken ct = default);
    Task<Member?> GetMemberAsync(long id, CancellationToken ct = default);
    Task<Member> CreateMemberAsync(Member member, CancellationToken ct = default);
    Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default);
    Task DeleteMemberAsync(long id, CancellationToken ct = default);

    // Rechnungen (FR-005)
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default);
    Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default);
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default);
    Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default);
    Task DeleteInvoiceAsync(long id, CancellationToken ct = default);

    // Veranstaltungen (FR-006)
    Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default);
    Task<Event?> GetEventAsync(long id, CancellationToken ct = default);
    Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default);
    Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default);
    Task DeleteEventAsync(long id, CancellationToken ct = default);

    // Kontaktdaten (FR-007)
    Task<IReadOnlyList<Contact>> GetContactsAsync(CancellationToken ct = default);
    Task<Contact?> GetContactAsync(long id, CancellationToken ct = default);
    Task<Contact> CreateContactAsync(Contact contact, CancellationToken ct = default);
    Task<Contact> UpdateContactAsync(long id, Contact contact, CancellationToken ct = default);
    Task DeleteContactAsync(long id, CancellationToken ct = default);
}
