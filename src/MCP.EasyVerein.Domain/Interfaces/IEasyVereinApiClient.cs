using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Interfaces;

public interface IEasyVereinApiClient
{
    // Member
    Task<IReadOnlyList<Member>> ListMembersAsync(long? id, string? membershipNumber, string[]? search, CancellationToken ct = default);
    Task<Member?> GetMemberAsync(long id, CancellationToken ct = default);
    Task<Member> CreateMemberAsync(string emailOrUserName,
        ContactDetails contactDetails, CancellationToken ct = default);
    Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default);
    Task DeleteMemberAsync(long id, CancellationToken ct = default);

    // ContactDetails
    Task<IReadOnlyList<ContactDetails>> ListContactDetailsAsync(long? id = null, string? firstName = null,
        string? familyName = null, string? name = null, CancellationToken ct = default);
    Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default);
    Task<ContactDetails> CreateContactDetailsAsync(ContactDetails contact, CancellationToken ct = default);
    Task<ContactDetails> UpdateContactDetailsAsync(long id, object patchData, CancellationToken ct = default);
    Task DeleteContactDetailsAsync(long id, CancellationToken ct = default);

    // Invoices
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default);
    Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default);
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default);
    Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default);
    Task DeleteInvoiceAsync(long id, CancellationToken ct = default);

    // Events
    Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default);
    Task<Event?> GetEventAsync(long id, CancellationToken ct = default);
    Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default);
    Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default);
    Task DeleteEventAsync(long id, CancellationToken ct = default);
}
