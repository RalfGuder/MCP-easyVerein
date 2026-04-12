using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// HTTP client for the easyVerein REST API (FR-002 to FR-007, NFR-001, NFR-002).
/// </summary>
public class EasyVereinApiClient : IEasyVereinApiClient
{
    /// <summary>The easyVerein configuration containing API key, base URL, and version.</summary>
    private readonly EasyVereinConfiguration _config;

    /// <summary>The HTTP client used for all API requests.</summary>
    private readonly HttpClient _httpClient;

    /// <summary>The JSON serializer options used for request and response serialization.</summary>
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="EasyVereinApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for API requests.</param>
    /// <param name="config">The easyVerein configuration containing API key and base URL.</param>
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

    /// <summary>Creates a new booking via the API.</summary>
    /// <param name="booking">The booking to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Booking"/> as returned by the API.</returns>
    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("booking"), booking, ct), ct);
        return await HandleResponse<Booking>(response, ct);
    }

    /// <summary>Creates a new calendar via the API.</summary>
    /// <param name="calendar">The calendar to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Calendar"/> as returned by the API.</returns>
    public async Task<Calendar> CreateCalendarAsync(Calendar calendar, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("calendar"), calendar, ct), ct);
        return await HandleResponse<Calendar>(response, ct);
    }

    /// <summary>
    /// Creates a new contact details record via the API.
    /// </summary>
    /// <param name="contact">The contact details to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="ContactDetails"/> as returned by the API.</returns>
    public async Task<ContactDetails> CreateContactDetailsAsync(
        ContactDetails contact, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("contact-details"), contact, ct), ct);
        return await HandleResponse<ContactDetails>(response, ct);
    }

    /// <summary>
    /// Creates a new event via the API.
    /// </summary>
    /// <param name="ev">The event to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Event"/> as returned by the API.</returns>
    public async Task<Event> CreateEventAsync(Event ev, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("event"), ev, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    /// <summary>
    /// Creates a new invoice via the API.
    /// </summary>
    /// <param name="invoice">The invoice to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Invoice"/> as returned by the API.</returns>
    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("invoice"), invoice, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    /// <summary>
    /// Creates a new member by first creating contact details and then the member record.
    /// </summary>
    /// <param name="emailOrUserName">The email address or username for the new member.</param>
    /// <param name="contactDetails">The contact details to associate with the member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Member"/> as returned by the API.</returns>
    public async Task<Member> CreateMemberAsync(
        string emailOrUserName, ContactDetails contactDetails, CancellationToken ct = default)
    {
        var createdContact = await CreateContactDetailsAsync(contactDetails, ct);
        var payload = new { emailOrUserName, contactDetails = createdContact.Id };
        var response = await SendWithErrorHandling(
            () => _httpClient.PostAsJsonAsync(BuildUrl("member"), payload, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }

    /// <summary>Deletes a booking by its identifier.</summary>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteBookingAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"booking/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    /// <summary>Deletes a calendar by its identifier.</summary>
    /// <param name="id">The unique identifier of the calendar to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteCalendarAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"calendar/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    /// <summary>
    /// Deletes a contact details record by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contact details to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteContactDetailsAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"contact-details/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    /// <summary>
    /// Deletes an event by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"event/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    /// <summary>
    /// Deletes an invoice by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"invoice/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    /// <summary>
    /// Deletes a member by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the member to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task DeleteMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.DeleteAsync(BuildUrl($"member/{id}"), ct), ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    /// <summary>Retrieves a single booking by its identifier.</summary>
    /// <param name="id">The unique identifier of the booking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Booking"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Booking?> GetBookingAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"booking/{id}", ApiQueries.Booking), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Booking>(response, ct);
    }

    /// <summary>Retrieves a single calendar by its identifier.</summary>
    /// <param name="id">The unique identifier of the calendar.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Calendar"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Calendar?> GetCalendarAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"calendar/{id}", ApiQueries.Calendar), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Calendar>(response, ct);
    }

    /// <summary>
    /// Retrieves a single contact details record by its identifier (FR-007).
    /// </summary>
    /// <param name="id">The unique identifier of the contact details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="ContactDetails"/> if found; otherwise <c>null</c>.</returns>
    public async Task<ContactDetails?> GetContactDetailsAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"contact-details/{id}", ApiQueries.ContactDetails), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<ContactDetails>(response, ct);
    }

    /// <summary>
    /// Retrieves a single event by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Event"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Event?> GetEventAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"event/{id}", ApiQueries.Event), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Event>(response, ct);
    }

    /// <summary>Lists events with optional filters and automatic pagination.</summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="startGte">Optional start date greater than or equal filter.</param>
    /// <param name="startLte">Optional start date less than or equal filter.</param>
    /// <param name="endGte">Optional end date greater than or equal filter.</param>
    /// <param name="endLte">Optional end date less than or equal filter.</param>
    /// <param name="calendar">Optional calendar ID filter.</param>
    /// <param name="canceled">Optional canceled filter.</param>
    /// <param name="isPublic">Optional public visibility filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="Event"/> records.</returns>
    public async Task<IReadOnlyList<Event>> ListEventsAsync(string? name = null, string? startGte = null,
        string? startLte = null, string? endGte = null, string? endLte = null,
        string? calendar = null, string? canceled = null, string? isPublic = null,
        string? idIn = null, string? ordering = null, string[]? search = null,
        CancellationToken ct = default)
    {
        ApiQueries.EventQuery.Name = name;
        ApiQueries.EventQuery.StartGte = startGte;
        ApiQueries.EventQuery.StartLte = startLte;
        ApiQueries.EventQuery.EndGte = endGte;
        ApiQueries.EventQuery.EndLte = endLte;
        ApiQueries.EventQuery.Calendar = calendar;
        ApiQueries.EventQuery.Canceled = canceled;
        ApiQueries.EventQuery.IsPublic = isPublic;
        ApiQueries.EventQuery.IdIn = idIn;
        ApiQueries.EventQuery.Ordering = ordering;
        ApiQueries.EventQuery.Search = search;

        return await HandleListResponseWithPagination<Event>(
            BuildListUrl("event", ApiQueries.Event), ct);
    }

    /// <summary>
    /// Retrieves a single invoice by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the invoice.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Invoice"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"invoice/{id}", ApiQueries.Invoice), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        return await HandleResponse<Invoice>(response, ct);
    }

    /// <summary>
    /// Retrieves all invoices with automatic pagination.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all <see cref="Invoice"/> records.</returns>
    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default)
    {
        return await HandleListResponseWithPagination<Invoice>(
            BuildListUrl("invoice", ApiQueries.Invoice), ct);
    }

    /// <summary>
    /// Retrieves a single member by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The <see cref="Member"/> if found; otherwise <c>null</c>.</returns>
    public async Task<Member?> GetMemberAsync(long id, CancellationToken ct = default)
    {
        var response = await SendWithErrorHandling(
            () => _httpClient.GetAsync(BuildGetUrl($"member/{id}", ApiQueries.Member), ct), ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        return await HandleResponse<Member>(response, ct);
    }

    /// <summary>Lists bookings with optional filters and automatic pagination.</summary>
    /// <param name="id">Optional filter by booking identifier.</param>
    /// <param name="date">Optional exact date filter.</param>
    /// <param name="dateGt">Optional filter for dates greater than the specified value.</param>
    /// <param name="dateLt">Optional filter for dates less than the specified value.</param>
    /// <param name="ordering">Optional ordering criterion for the results.</param>
    /// <param name="search">Optional search terms to filter bookings.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="Booking"/> records.</returns>
    public async Task<IReadOnlyList<Booking>> ListBookingsAsync(long? id = null, string? date = default, string? dateGt = default, string? dateLt = default, string? ordering = default, string[]? search = default,
        CancellationToken ct = default)
    {
        ApiQueries.BookingQuery.Id = id;
        ApiQueries.BookingQuery.Search = search;
        ApiQueries.BookingQuery.Date = date;
        ApiQueries.BookingQuery.DateGt = dateGt;
        ApiQueries.BookingQuery.DateLt = dateLt;
        ApiQueries.BookingQuery.Ordering = ordering;

        return await HandleListResponseWithPagination<Booking>(
            BuildListUrl("booking", ApiQueries.Booking), ct);
    }

    /// <summary>Lists calendars with optional filters and automatic pagination.</summary>
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
    /// <returns>A read-only list of matching <see cref="Calendar"/> records.</returns>
    public async Task<IReadOnlyList<Calendar>> ListCalendarsAsync(string? name = null, string? color = null,
        string? short_ = null, string? nameNot = null, string? colorNot = null,
        string? shortNot = null, string? idIn = null, string? allowedGroups = null,
        string? ordering = null, string[]? search = null, CancellationToken ct = default)
    {
        ApiQueries.CalendarQuery.Name = name;
        ApiQueries.CalendarQuery.Color = color;
        ApiQueries.CalendarQuery.Short = short_;
        ApiQueries.CalendarQuery.NameNot = nameNot;
        ApiQueries.CalendarQuery.ColorNot = colorNot;
        ApiQueries.CalendarQuery.ShortNot = shortNot;
        ApiQueries.CalendarQuery.IdIn = idIn;
        ApiQueries.CalendarQuery.AllowedGroups = allowedGroups;
        ApiQueries.CalendarQuery.Ordering = ordering;
        ApiQueries.CalendarQuery.Search = search;

        return await HandleListResponseWithPagination<Calendar>(
            BuildListUrl("calendar", ApiQueries.Calendar), ct);
    }

    /// <summary>
    /// Lists contact details with optional filters and automatic pagination.
    /// </summary>
    /// <param name="id">Optional filter by contact details identifier.</param>
    /// <param name="firstName">Optional filter by first name.</param>
    /// <param name="familyName">Optional filter by family name.</param>
    /// <param name="name">Optional filter by name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="ContactDetails"/> records.</returns>
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

    /// <summary>
    /// Lists members with optional filters and automatic pagination.
    /// </summary>
    /// <param name="id">Optional filter by member identifier.</param>
    /// <param name="membershipNumber">Optional filter by membership number.</param>
    /// <param name="search">Optional search terms to filter members.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of matching <see cref="Member"/> records.</returns>
    public async Task<IReadOnlyList<Member>> ListMembersAsync(long? id = null, string? membershipNumber = null, string[]? search = null, CancellationToken ct = default)
    {
        ApiQueries.MemberQuery.Id = id;
        ApiQueries.MemberQuery.MembershipNumber = membershipNumber;
        ApiQueries.MemberQuery.Search = search;

        return await HandleListResponseWithPagination<Member>(
            BuildListUrl("member", ApiQueries.Member), ct);
    }

    // --- Bookings (FR-045) ---
    /// <summary>Partially updates a booking with a patch dictionary.</summary>
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="patchData">An object containing only the fields to update.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Booking"/> as returned by the API.</returns>
    public async Task<Booking> UpdateBookingAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"booking/{id}"), content, ct), ct);
        return await HandleResponse<Booking>(response, ct);
    }

    /// <summary>Partially updates a calendar with a patch dictionary.</summary>
    /// <param name="id">The unique identifier of the calendar to update.</param>
    /// <param name="patchData">An object containing only the fields to update.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Calendar"/> as returned by the API.</returns>
    public async Task<Calendar> UpdateCalendarAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"calendar/{id}"), content, ct), ct);
        return await HandleResponse<Calendar>(response, ct);
    }

    /// <summary>
    /// Updates an existing contact details record with a partial patch.
    /// </summary>
    /// <param name="id">The unique identifier of the contact details to update.</param>
    /// <param name="patchData">An object containing only the fields to update.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="ContactDetails"/> as returned by the API.</returns>
    public async Task<ContactDetails> UpdateContactDetailsAsync(
        long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"contact-details/{id}"), content, ct), ct);
        return await HandleResponse<ContactDetails>(response, ct);
    }

    /// <summary>
    /// Updates an existing event (FR-006).
    /// </summary>
    /// <param name="id">The unique identifier of the event to update.</param>
    /// <param name="patchData">The patch data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Event" /> as returned by the API.</returns>
    public async Task<Event> UpdateEventAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"event/{id}"), content, ct), ct);
        return await HandleResponse<Event>(response, ct);
    }

    /// <summary>
    /// Updates an existing invoice (FR-005).
    /// </summary>
    /// <param name="id">The unique identifier of the invoice to update.</param>
    /// <param name="patchData">The patch data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Invoice" /> as returned by the API.</returns>
    public async Task<Invoice> UpdateInvoiceAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsJsonAsync(BuildUrl($"invoice/{id}"), content, ct), ct);
        return await HandleResponse<Invoice>(response, ct);
    }

    /// <summary>
    /// Updates an existing member (FR-003, FR-004).
    /// </summary>
    /// <param name="id">The unique identifier of the member to update.</param>
    /// <param name="patchData">The patch data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated <see cref="Member" /> as returned by the API.</returns>
    public async Task<Member> UpdateMemberAsync(long id, object patchData, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(patchData, patchData.GetType(), _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await SendWithErrorHandling(
            () => _httpClient.PatchAsync(BuildUrl($"member/{id}"), content, ct), ct);
        return await HandleResponse<Member>(response, ct);
    }
    /// <summary>
    /// Builds a URL for a single-resource GET request by appending the query string.
    /// </summary>
    /// <param name="resource">The API resource path (e.g. "contact-details/123").</param>
    /// <param name="query">The query string to append.</param>
    /// <returns>The fully constructed GET URL.</returns>
    private string BuildGetUrl(string resource, string? query)
    {
        return string.IsNullOrEmpty(query) ? $"{BuildUrl(resource)}" : $"{BuildUrl(resource)}{Uri.EscapeDataString(query)}";
    }

    /// <summary>
    /// Builds a URL for a list endpoint with pagination limit and optional query parameters.
    /// </summary>
    /// <param name="resource">The API resource path (e.g. "member").</param>
    /// <param name="query">Optional query string for field selection and filters.</param>
    /// <returns>The fully constructed list URL with a limit of 100.</returns>
    private string BuildListUrl(string resource, string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return $"{BuildUrl(resource)}?limit=100";
        }
        return $"{BuildUrl(resource)}?{query}&limit=100";
    }

    /// <summary>
    /// Builds the base URL for an API resource including the versioned API path.
    /// </summary>
    /// <param name="resource">The API resource path.</param>
    /// <param name="apiVersionOverride">Optional API version override.</param>
    /// <returns>The fully constructed resource URL.</returns>
    private string BuildUrl(string resource, string? apiVersionOverride = null)
    {
        return $"{_config.GetVersionedBaseUrl(apiVersionOverride)}/{resource}";
    }

    /// <summary>
    /// Ensures the HTTP response indicates success; throws on authentication failure or other errors.
    /// </summary>
    /// <param name="response">The HTTP response to check.</param>
    /// <param name="ct">Cancellation token.</param>
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

    /// <summary>
    /// Fetches all pages of a paginated list endpoint and aggregates the results.
    /// </summary>
    /// <typeparam name="T">The entity type of the list items.</typeparam>
    /// <param name="initialUrl">The URL of the first page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list containing all results across all pages.</returns>
    private async Task<IReadOnlyList<T>> HandleListResponseWithPagination<T>(
        string initialUrl, CancellationToken ct)
    {
        var allResults = new List<T>();
        var url = initialUrl;

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

    /// <summary>
    /// Deserializes a successful HTTP response into the specified type.
    /// </summary>
    /// <typeparam name="T">The target deserialization type.</typeparam>
    /// <param name="response">The HTTP response to deserialize.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    private async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessOrThrowAsync(response, ct);

        var single = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, ct);
        return single ?? throw new InvalidOperationException("Leere API-Antwort.");
    }

    /// <summary>
    /// Executes an HTTP request with error handling for network failures and timeouts (NFR-002).
    /// </summary>
    /// <param name="request">A factory that produces the HTTP request task.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The HTTP response message.</returns>
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
/// API response wrapper for paginated list endpoints.
/// </summary>
/// <typeparam name="T">The entity type contained in the results.</typeparam>
internal class ApiListResponse<T>
{
    /// <summary>
    /// Gets or sets the total number of results available.
    /// </summary>
    public int? Count { get; set; }

    /// <summary>
    /// Gets or sets the URL for the next page of results, or <c>null</c> if this is the last page.
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// Gets or sets the URL for the previous page of results, or <c>null</c> if this is the first page.
    /// </summary>
    public string? Previous { get; set; }

    /// <summary>
    /// Gets or sets the list of results on the current page.
    /// </summary>
    public List<T> Results { get; set; } = new();
}
