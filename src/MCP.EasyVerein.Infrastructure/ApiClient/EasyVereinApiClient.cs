using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// HTTP-Client für die easyVerein REST-API (FR-002 bis FR-007, NFR-001, NFR-002).
/// </summary>
public class EasyVereinApiClient : IEasyVereinApiClient
{
    private readonly EasyVereinConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public EasyVereinApiClient(HttpClient httpClient, EasyVereinConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        // Authentifizierung (FR-002)
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");
    }

    public async Task<ContactDetails> CreateContactDetailsAsync(
        ContactDetails contact, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("contact-details"), contact, ct), ct);
        return await HandleResponse<ContactDetails>(response, ct);
    }

    public async Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("event"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("invoice"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task<Member> CreateMemberAsync(
        string emailOrUserName, ContactDetails contactDetails, CancellationToken ct = default)
    {
        var createdContact = await CreateContactDetailsAsync(contactDetails, ct);
        var payload = new { emailOrUserName, contactDetails = createdContact.Id };
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("member"), payload, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    public async Task DeleteContactDetailsAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"contact-details/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    public async Task DeleteEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"event/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    public async Task DeleteInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"invoice/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    public async Task DeleteMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"member/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    // --- Kontaktdaten (FR-007) ---
    public async Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"contact-details/{id}", ApiQueries.ContactDetails), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<ContactDetails>(response, ct);
    }

    public async Task<Event?> GetEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"event/{id}", ApiQueries.Event), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Event>(response, ct);
    }

    public async Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Event>(
            BuildListUrl("event", ApiQueries.Event), ct);
    }

    public async Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"invoice/{id}", ApiQueries.Invoice), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Invoice>(
            BuildListUrl("invoice", ApiQueries.Invoice), ct);
    }

    public async Task<Member?> GetMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"member/{id}", ApiQueries.Member), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<Member>(response, ct);
    }

    public async Task<IReadOnlyList<ContactDetails>> ListContactDetailsAsync(long? id = null, string? firstName = null,
        string? familyName = null, string? name = null, CancellationToken ct = default)
    {
        ApiQueries.ContactDetailsQuery.Id = id;
        ApiQueries.ContactDetailsQuery.FirstName = firstName;
        ApiQueries.ContactDetailsQuery.FamilyName = familyName;
        ApiQueries.ContactDetailsQuery.Name = name;
        return await HandleListResponseWithPagination<ContactDetails>(
            BuildListUrl("contact-details", ApiQueries.ContactDetails), ct);
    }

    public async Task<IReadOnlyList<Member>> ListMembersAsync(long? id = null, string? membershipNumber = null, string[]? search = null, CancellationToken ct = default)
    {
        ApiQueries.MemberQuery.Id = id;
        ApiQueries.MemberQuery.MembershipNumber = membershipNumber;
        ApiQueries.MemberQuery.Search = search;

        return await HandleListResponseWithPagination<Member>(
            BuildListUrl("member", ApiQueries.Member), ct);
    }
    public async Task<ContactDetails> UpdateContactDetailsAsync(
        long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"contact-details/{id}"), content, ct), ct);
        return await HandleResponse<ContactDetails>(response, ct);
    }

    // --- Veranstaltungen (FR-006) ---
    public async Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"event/{id}"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    // --- Rechnungen (FR-005) ---
    public async Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"invoice/{id}"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    // --- Mitglieder (FR-003, FR-004) ---
    public async Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"member/{id}"), member, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    private string BuildGetUrl(string resource, string query)
    {
        return $"{BuildUrl(resource)}{Uri.EscapeDataString(query)}";
    }

    private string BuildListUrl(string resource, string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return $"{BuildUrl(resource)}?limit=100";
        }
        return $"{BuildUrl(resource)}?{query}&limit=100";
    }

    private string BuildUrl(string resource, string? apiVersionOverride = null)
    {
        return $"{_config.GetVersionedBaseUrl(apiVersionOverride)}/{resource}";
    }
    private async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken ct)
    {
        // NFR-001: Fehlerbehandlung bei ungültigen API-Tokens
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException(
                $"Authentifizierung fehlgeschlagen (HTTP {(int)response.StatusCode}). " +
                "Bitte prüfen Sie Ihren API-Token.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase}): {body}");
        }
    }

    private async Task<IReadOnlyList<T>> HandleListResponseWithPagination<T>(
        string initialUrl, CancellationToken ct)
    {
        var allResults = new List<T>();
        string? url = initialUrl;

        while (url != null)
        {
            var response = await SendWithErrorHandling(
                () => _httpClient.GetAsync(url, ct), ct);

            await EnsureSuccessOrThrowAsync(response, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            var page = JsonSerializer.Deserialize<ApiListResponse<T>>(content, _jsonOptions);

            if (page?.Results != null)
                allResults.AddRange(page.Results);

            url = page?.Next;
        }

        return allResults.AsReadOnly();
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessOrThrowAsync(response, ct);

        var single = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, ct);
        return single ?? throw new InvalidOperationException("Leere API-Antwort.");
    }
    private async Task<HttpResponseMessage> SendWithErrorHandling(
        Func<Task<HttpResponseMessage>> request, CancellationToken ct)
    {
        try
        {
            return await request();
        }
        catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
        {
            // NFR-002: Netzwerkfehler
            throw new InvalidOperationException(
                "Netzwerkfehler: Verbindung zum easyVerein-Server konnte nicht hergestellt werden. " +
                "Bitte prüfen Sie Ihre Internetverbindung.", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            // NFR-002: Timeout
            throw new InvalidOperationException(
                "Zeitüberschreitung: Die easyVerein-API hat nicht rechtzeitig geantwortet.", ex);
        }
    }
}

/// <summary>
/// API-Antwort-Wrapper für Listen-Endpunkte.
/// </summary>
internal class ApiListResponse<T>
{
    public int? Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public List<T> Results { get; set; } = new();
}
