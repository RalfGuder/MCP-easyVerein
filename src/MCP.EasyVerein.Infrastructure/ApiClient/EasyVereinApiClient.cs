using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// HTTP-Client für die easyVerein REST-API (FR-002 bis FR-007, NFR-001, NFR-002).
/// </summary>
public class EasyVereinApiClient : IEasyVereinApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EasyVereinConfiguration _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public EasyVereinApiClient(HttpClient httpClient, EasyVereinConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Authentifizierung (FR-002)
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {config.ApiToken}");
    }

    private string BuildUrl(string resource, string? apiVersionOverride = null)
    {
        return $"{_config.GetVersionedBaseUrl(apiVersionOverride)}/{resource}";
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        // NFR-001: Fehlerbehandlung bei ungültigen API-Tokens
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException(
                $"Authentifizierung fehlgeschlagen (HTTP {(int)response.StatusCode}). " +
                "Bitte prüfen Sie Ihren API-Token.");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiListResponse<T>>(_jsonOptions, ct);
        if (result?.Results != null && result.Results.Any())
            return result.Results.First();

        var single = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, ct);
        return single ?? throw new InvalidOperationException("Leere API-Antwort.");
    }

    private async Task<IReadOnlyList<T>> HandleListResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException(
                $"Authentifizierung fehlgeschlagen (HTTP {(int)response.StatusCode}). " +
                "Bitte prüfen Sie Ihren API-Token.");
        }

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);

        // easyVerein API gibt Listen als { "results": [...] } zurück
        try
        {
            var listResponse = JsonSerializer.Deserialize<ApiListResponse<T>>(content, _jsonOptions);
            if (listResponse?.Results != null)
                return listResponse.Results.AsReadOnly();
        }
        catch (JsonException)
        {
            // Fallback: direktes Array
        }

        var items = JsonSerializer.Deserialize<List<T>>(content, _jsonOptions);
        return items?.AsReadOnly() ?? (IReadOnlyList<T>)Array.Empty<T>();
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

    // --- Mitglieder (FR-003, FR-004) ---

    public async Task<IReadOnlyList<Member>> GetMembersAsync(CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl("member"), ct), ct);
        return await HandleListResponse<Member>(response, ct);
    }

    public async Task<Member?> GetMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl($"member/{id}"), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<Member>(response, ct);
    }

    public async Task<Member> CreateMemberAsync(Member member, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("member"), member, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    public async Task<Member> UpdateMemberAsync(long id, Member member, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"member/{id}"), member, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    public async Task DeleteMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"member/{id}"), ct), ct);
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException("Authentifizierung fehlgeschlagen.");
        response.EnsureSuccessStatusCode();
    }

    // --- Rechnungen (FR-005) ---

    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl("invoice"), ct), ct);
        return await HandleListResponse<Invoice>(response, ct);
    }

    public async Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl($"invoice/{id}"), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("invoice"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task<Invoice> UpdateInvoiceAsync(long id, Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"invoice/{id}"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    public async Task DeleteInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"invoice/{id}"), ct), ct);
        response.EnsureSuccessStatusCode();
    }

    // --- Veranstaltungen (FR-006) ---

    public async Task<IReadOnlyList<Event>> GetEventsAsync(CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl("event"), ct), ct);
        return await HandleListResponse<Event>(response, ct);
    }

    public async Task<Event?> GetEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl($"event/{id}"), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Event>(response, ct);
    }

    public async Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("event"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    public async Task<Event> UpdateEventAsync(long id, Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"event/{id}"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    public async Task DeleteEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"event/{id}"), ct), ct);
        response.EnsureSuccessStatusCode();
    }

    // --- Kontaktdaten (FR-007) ---

    public async Task<IReadOnlyList<Contact>> GetContactsAsync(CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl("contact-details"), ct), ct);
        return await HandleListResponse<Contact>(response, ct);
    }

    public async Task<Contact?> GetContactAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildUrl($"contact-details/{id}"), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Contact>(response, ct);
    }

    public async Task<Contact> CreateContactAsync(Contact contact, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("contact-details"), contact, ct), ct);
        return await HandleResponse<Contact>(response, ct);
    }

    public async Task<Contact> UpdateContactAsync(long id, Contact contact, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"contact-details/{id}"), contact, ct), ct);
        return await HandleResponse<Contact>(response, ct);
    }

    public async Task DeleteContactAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"contact-details/{id}"), ct), ct);
        response.EnsureSuccessStatusCode();
    }
}

/// <summary>
/// API-Antwort-Wrapper für Listen-Endpunkte.
/// </summary>
internal class ApiListResponse<T>
{
    public List<T> Results { get; set; } = new();
    public int? Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
}
