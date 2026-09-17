using System.Net;
using System.Text.Json;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Infrastructure.ApiClient;

namespace MCP.EasyVerein.Infrastructure.Tests;

public class EasyVereinApiClientTests
{
    private static EasyVereinApiClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://easyverein.com/api")
        };
        var config = new EasyVereinConfiguration
        {
            ApiKey = "test-token",
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };
        return new EasyVereinApiClient(httpClient, config);
    }

    // ------------------------------------------------------------------ //
    // Constructor
    // ------------------------------------------------------------------ //

    [Fact]
    public void Constructor_SetsAuthorizationHeader()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, "{}");
        var httpClient = new HttpClient(handler);
        var config = new EasyVereinConfiguration { ApiKey = "my-secret-token" };

        _ = new EasyVereinApiClient(httpClient, config);

        Assert.Contains(httpClient.DefaultRequestHeaders.GetValues("Authorization"),
            v => v == "Bearer my-secret-token");
    }

    // ------------------------------------------------------------------ //
    // Members
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetMembers_ReturnsMembers()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, emailOrUserName = "max@test.de" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListMembersAsync();

        Assert.Single(result);
        Assert.Equal("max@test.de", result[0].EmailOrUserName);
    }

    [Fact]
    public async Task GetMembers_FollowsPagination_ReturnsAllPages()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, emailOrUserName = "user1@test.de" } },
            next = "https://easyverein.com/api/v1.7/member?query=...&limit=100&page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, emailOrUserName = "user2@test.de" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(
            new[] { (HttpStatusCode.OK, page1), (HttpStatusCode.OK, page2) });
        var client = CreateClient(handler);

        var result = await client.ListMembersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("user1@test.de", result[0].EmailOrUserName);
        Assert.Equal("user2@test.de", result[1].EmailOrUserName);
    }

    [Fact]
    public async Task GetMembers_SendsQueryParameter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListMembersAsync();

        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("query=", handler.LastRequestUri!.Query);
        Assert.Contains("limit=100", handler.LastRequestUri!.Query);
    }

    [Fact]
    public async Task GetMembers_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListMembersAsync());
    }

    [Fact]
    public async Task GetMembers_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Invalid query field: _invalidField\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.ListMembersAsync());

        Assert.Contains("400", ex.Message);
        Assert.Contains("Invalid query field", ex.Message);
    }

    [Fact]
    public async Task GetMembers_WithInternalServerError_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Internal server error occurred\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.InternalServerError, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.ListMembersAsync());

        Assert.Contains("500", ex.Message);
        Assert.Contains("Internal server error", ex.Message);
    }

    [Fact]
    public async Task GetMember_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Bad request for member\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetMemberAsync(1));

        Assert.Contains("400", ex.Message);
        Assert.Contains("Bad request for member", ex.Message);
    }

    [Fact]
    public async Task DeleteMember_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Cannot delete member\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.DeleteMemberAsync(1));

        Assert.Contains("400", ex.Message);
        Assert.Contains("Cannot delete member", ex.Message);
    }

    [Fact]
    public async Task GetMember_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetMemberAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMember_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, emailOrUserName = "x@y.de" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListMembersAsync(id: 12345, membershipNumber: "M-42", search: new[] { "Mueller" });
        await client.GetMemberAsync(999);

        Assert.NotNull(handler.LastRequestUri);
        var path = handler.LastRequestUri!.AbsolutePath;
        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/member/999", path);
        Assert.DoesNotContain("id=", query);
        Assert.DoesNotContain("membershipNumber=", query);
        Assert.DoesNotContain("search=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task DeleteMember_WithForbidden_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Forbidden, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.DeleteMemberAsync(1));
    }

    // ------------------------------------------------------------------ //
    // Invoices
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetInvoices_ReturnsInvoices()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, invNumber = "R-001", totalPrice = 50.00m }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.GetInvoicesAsync();

        Assert.Single(result);
        Assert.Equal("R-001", result[0].InvoiceNumber);
        Assert.Equal(50.00m, result[0].TotalPrice);
    }

    [Fact]
    public async Task UpdateInvoice_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new { id = 5, description = "neu", isDraft = false });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["description"] = "neu",
            ["isDraft"] = false
        };
        var updated = await client.UpdateInvoiceAsync(5, patch);

        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.Equal("neu", updated.Description);
        Assert.False(updated.IsDraft);
        Assert.EndsWith("/invoice/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateInvoice_TogglesIsDraftFromTrueToFalse()
    {
        var updatedJson = JsonSerializer.Serialize(new { id = 7, isDraft = false });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object> { ["isDraft"] = false };
        var updated = await client.UpdateInvoiceAsync(7, patch);

        Assert.False(updated.IsDraft);
    }

    [Fact]
    public async Task UpdateInvoice_SendsRelatedBookingsList()
    {
        var json = JsonSerializer.Serialize(new { id = 9 });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["relatedBookings"] = new[] { "https://easyverein.com/api/v2.0/booking/123" }
        };
        await client.UpdateInvoiceAsync(9, patch);

        Assert.NotNull(handler.LastRequestBody);
        Assert.Contains("relatedBookings", handler.LastRequestBody);
        Assert.Contains("booking/123", handler.LastRequestBody);
    }

    [Fact]
    public async Task UpdateInvoice_WithBadRequest_ThrowsWithResponseBody()
    {
        var errorBody = "{\"detail\":\"Invalid patch\"}";
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, errorBody);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object> { ["description"] = "x" };
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.UpdateInvoiceAsync(1, patch));

        Assert.Contains("400", ex.Message);
    }

    [Fact]
    public async Task GetInvoice_UsesInvoiceQueryFieldQuery_NotApiQueriesConst()
    {
        var json = JsonSerializer.Serialize(new { id = 999, invNumber = "INV-001" });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.GetInvoiceAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/invoice/999", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("query=", query);
        Assert.DoesNotContain("id=", query);
    }

    // ------------------------------------------------------------------ //
    // Events
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListEvents_ReturnsEvents()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, name = "Jahresversammlung" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListEventsAsync();

        Assert.Single(result);
        Assert.Equal("Jahresversammlung", result[0].Name);
    }

    [Fact]
    public async Task ListEvents_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListEventsAsync());
    }

    [Fact]
    public async Task ListEvents_SendsQueryParameter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListEventsAsync();

        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("query=", handler.LastRequestUri!.Query);
        Assert.Contains("limit=100", handler.LastRequestUri!.Query);
    }

    [Fact]
    public async Task GetEvent_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListEventsAsync(
            name: "Sommerfest",
            startGte: "2026-01-01",
            startLte: "2026-12-31",
            endGte: "2026-01-01",
            endLte: "2026-12-31",
            calendar: "5",
            canceled: "false",
            isPublic: "true",
            idIn: "1,2",
            ordering: "name",
            search: new[] { "fest" });
        await client.GetEventAsync(999);

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/event/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("start__gte=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Theory]
    [InlineData("parent")]
    [InlineData("weekdays")]
    [InlineData("massParticipations")]
    public async Task ListEvents_DoesNotRequestFieldRemovedInV2_InQuerySelector(string field)
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListEventsAsync();

        Assert.NotNull(handler.LastRequestUri);
        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        // easyVerein v2.0 answers HTTP 400 "'<field>' field is not found" for these fields.
        Assert.DoesNotMatch($"[{{,]{field}[,}}]", query);
    }

    [Theory]
    [InlineData("parent")]
    [InlineData("weekdays")]
    [InlineData("massParticipations")]
    public async Task GetEvent_DoesNotRequestFieldRemovedInV2_InQuerySelector(string field)
    {
        var json = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.GetEventAsync(999);

        Assert.NotNull(handler.LastRequestUri);
        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.DoesNotMatch($"[{{,]{field}[,}}]", query);
    }

    [Theory]
    [InlineData("parent")]
    [InlineData("weekdays")]
    [InlineData("massParticipations")]
    public async Task CreateEvent_DoesNotSendFieldRemovedInV2_InRequestBody(string field)
    {
        var json = JsonSerializer.Serialize(new { id = 1, name = "Sommerfest" });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, json);
        var client = CreateClient(handler);

        await client.CreateEventAsync(new Event { Name = "Sommerfest" });

        Assert.NotNull(handler.LastRequestBody);
        Assert.DoesNotContain($"\"{field}\"", handler.LastRequestBody);
    }

    // ------------------------------------------------------------------ //
    // ContactDetails
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetContactDetails_ReturnsContactDetails()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, firstName = "Anna", familyName = "Schmidt" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListContactDetailsAsync();

        Assert.Single(result);
        Assert.Equal("Anna", result[0].FirstName);
        Assert.Equal("Schmidt", result[0].FamilyName);
    }

    [Fact]
    public async Task GetContactDetails_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, firstName = "Anna" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListContactDetailsAsync(id: 12345, firstName: "Bob", familyName: "Smith", name: "BSmith");
        await client.GetContactDetailsAsync(999);

        Assert.NotNull(handler.LastRequestUri);
        var path = handler.LastRequestUri!.AbsolutePath;
        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/contact-details/999", path);
        Assert.DoesNotContain("firstName=", query);
        Assert.DoesNotContain("familyName=", query);
        Assert.DoesNotContain("name=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Bookings
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetBookings_ReturnsBookings()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, amount = 150.50, receiver = "Max Mustermann" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListBookingsAsync();

        Assert.Single(result);
        Assert.Equal("Max Mustermann", result[0].Receiver);
        Assert.Equal(150.50m, result[0].Amount);
    }

    [Fact]
    public async Task GetBooking_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetBookingAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListBookings_WithIdIn_SendsIdInFilter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListBookingsAsync(idIn: "12345,67890");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("id__in=12345%2C67890", query);
        Assert.DoesNotContain("&id=", query);
    }

    [Fact]
    public async Task GetBooking_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var getJson = JsonSerializer.Serialize(new { id = 999, amount = 1.23, receiver = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListBookingsAsync(
            idIn: "12345,67890",
            date: "2026-05-18",
            dateGt: "2026-01-01",
            dateLt: "2026-12-31",
            ordering: "-date",
            search: new[] { "Muster" });

        await client.GetBookingAsync(999);

        Assert.NotNull(handler.LastRequestUri);
        var path = handler.LastRequestUri!.AbsolutePath;
        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/booking/999", path);
        Assert.DoesNotContain("id__in=", query);
        Assert.DoesNotContain("date=", query);
        Assert.DoesNotContain("date__gt=", query);
        Assert.DoesNotContain("date__lt=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.DoesNotContain("search=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task ListBookings_ConcurrentCallsWithDifferentFilters_DoNotLeakBetweenEachOther()
    {
        var emptyPage = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });

        var handlerA = new CapturingFakeHttpHandler(HttpStatusCode.OK, emptyPage);
        var handlerB = new CapturingFakeHttpHandler(HttpStatusCode.OK, emptyPage);
        var clientA = CreateClient(handlerA);
        var clientB = CreateClient(handlerB);

        var taskA = clientA.ListBookingsAsync(idIn: "111,222");
        var taskB = clientB.ListBookingsAsync(idIn: "999");
        await Task.WhenAll(taskA, taskB);

        Assert.NotNull(handlerA.LastRequestUri);
        Assert.NotNull(handlerB.LastRequestUri);
        Assert.Contains("id__in=111%2C222", handlerA.LastRequestUri!.Query);
        Assert.DoesNotContain("id__in=999", handlerA.LastRequestUri!.Query);
        Assert.Contains("id__in=999", handlerB.LastRequestUri!.Query);
        Assert.DoesNotContain("id__in=111", handlerB.LastRequestUri!.Query);
    }

    [Fact]
    public async Task GetBookings_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListBookingsAsync());
    }

    // ------------------------------------------------------------------ //
    // Calendars
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListCalendars_ReturnsCalendars()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, name = "Kulturverein", color = "#f9e4c6", @short = "KVMi" }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListCalendarsAsync();

        Assert.Single(result);
        Assert.Equal("Kulturverein", result[0].Name);
        Assert.Equal("#f9e4c6", result[0].Color);
    }

    [Fact]
    public async Task GetCalendar_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetCalendarAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListCalendars_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListCalendarsAsync());
    }

    [Fact]
    public async Task ListCalendars_SendsQueryParameter()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCalendarsAsync();

        Assert.NotNull(handler.LastRequestUri);
        Assert.Contains("query=", handler.LastRequestUri!.Query);
        Assert.Contains("limit=100", handler.LastRequestUri!.Query);
    }

    [Fact]
    public async Task GetCalendar_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "Cal" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListCalendarsAsync(
            name: "Vereinskalender",
            color: "#f00",
            short_: "VK",
            idIn: "1,2",
            ordering: "name",
            search: new[] { "kal" });
        await client.GetCalendarAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/calendar/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("color=", query);
        Assert.DoesNotContain("short=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Announcements
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListAnnouncements_ReturnsAnnouncements()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 1, text = "<p>Willkommen</p>", showBanner = true, isDismissible = false, isPublic = true, showForNormalMembers = true }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListAnnouncementsAsync();

        Assert.Single(result);
        Assert.Equal("<p>Willkommen</p>", result[0].Text);
        Assert.True(result[0].ShowBanner);
    }

    [Fact]
    public async Task GetAnnouncement_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetAnnouncementAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListAnnouncements_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListAnnouncementsAsync());
    }

    [Fact]
    public async Task GetAnnouncement_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, title = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListAnnouncementsAsync(ordering: "title", search: new[] { "wartung" });
        await client.GetAnnouncementAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/announcement/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("ordering=", query);
        Assert.DoesNotContain("search=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Bank Accounts
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListBankAccounts_ReturnsBankAccounts()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    name = "Vereinskonto",
                    IBAN = "DE89370400440532013000",
                    BIC = "COBADEFFXXX",
                    accountHolder = "TSV Musterhausen e.V.",
                    bankName = "Sparkasse"
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListBankAccountsAsync();

        Assert.Single(result);
        Assert.Equal("Vereinskonto", result[0].Name);
        Assert.Equal("DE89370400440532013000", result[0].Iban);
        Assert.Equal("COBADEFFXXX", result[0].Bic);
    }

    [Fact]
    public async Task GetBankAccount_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetBankAccountAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListBankAccounts_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListBankAccountsAsync());
    }

    [Fact]
    public async Task GetBankAccount_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListBankAccountsAsync(
            name: "Sparkasse",
            iban: "DE00",
            bic: "BIC",
            accountHolder: "Verein",
            bankName: "Sparkasse",
            idIn: "1,2",
            ordering: "name",
            search: new[] { "spk" });
        await client.GetBankAccountAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/bank-account/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("iban=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Billing Accounts
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListBillingAccounts_ReturnsBillingAccounts()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    name = "Buchungskonto 1200",
                    number = 1200,
                    defaultSphere = 9,
                    excludeInEur = false,
                    skr = "42",
                    accountingPlan = 7,
                    deleted = false,
                    linkedBookings = 3
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListBillingAccountsAsync();

        Assert.Single(result);
        Assert.Equal("Buchungskonto 1200", result[0].Name);
        Assert.Equal(1200, result[0].Number);
        Assert.Equal(9, result[0].DefaultSphere);
        Assert.Equal(3, result[0].LinkedBookings);
    }

    [Fact]
    public async Task ListBillingAccounts_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListBillingAccountsAsync(
            name: "Buchungskonto",
            skr: "42",
            numberGte: "1000",
            numberLte: "1999",
            deleted: "false",
            showOwnBillingAccounts: "true",
            ordering: "number");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("name=Buchungskonto", query);
        Assert.Contains("skr=42", query);
        Assert.Contains("number__gte=1000", query);
        Assert.Contains("number__lte=1999", query);
        Assert.Contains("deleted=false", query);
        Assert.Contains("showOwnBillingAccounts=true", query);
        Assert.Contains("ordering=number", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task GetBillingAccount_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetBillingAccountAsync(999);

        Assert.Null(result);
    }

    /// <summary>
    /// Regression for US-0059: the <c>accountingPlan</c> field is not a
    /// billing-account response field in any easyVerein API version.
    /// Requesting it via the <c>query=</c> selector caused HTTP 400
    /// ("'accountingPlan' field is not found") on v2.0.
    /// </summary>
    [Fact]
    public async Task ListBillingAccounts_DoesNotRequestAccountingPlanField_InQuerySelector()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListBillingAccountsAsync();

        Assert.NotNull(handler.LastRequestUri);
        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);

        // The query= selector must not ask for a non-existent response field.
        // The literal "{accountingPlan}" or "accountingPlan," or "accountingPlan}" must not appear.
        Assert.DoesNotContain("{accountingPlan,", query);
        Assert.DoesNotContain(",accountingPlan,", query);
        Assert.DoesNotContain(",accountingPlan}", query);
    }

    /// <summary>
    /// Regression: the <c>deleted</c> field is not a billing-account response
    /// field in easyVerein API v2.0. Requesting it via the <c>query=</c>
    /// selector causes HTTP 400 ("'deleted' field is not found").
    /// </summary>
    [Fact]
    public async Task ListBillingAccounts_DoesNotRequestDeletedField_InQuerySelector()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListBillingAccountsAsync();

        Assert.NotNull(handler.LastRequestUri);
        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);

        // The query= selector must not ask for a non-existent response field.
        Assert.DoesNotContain("{deleted,", query);
        Assert.DoesNotContain(",deleted,", query);
        Assert.DoesNotContain(",deleted}", query);
    }

    [Fact]
    public async Task CreateBillingAccount_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new
        {
            id = 123,
            name = "Neues Konto",
            number = 1400,
            defaultSphere = 9,
            excludeInEur = false
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateBillingAccountAsync(new BillingAccount
        {
            Name = "Neues Konto",
            Number = 1400,
            DefaultSphere = 9,
            ExcludeInEur = false
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Neues Konto", created.Name);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/billing-account", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateBillingAccount_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new
        {
            id = 5,
            name = "Renamed",
            number = 1500
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["name"] = "Renamed",
            ["number"] = 1500
        };
        var updated = await client.UpdateBillingAccountAsync(5, patch);

        Assert.Equal("Renamed", updated.Name);
        Assert.Equal(1500, updated.Number);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/billing-account/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteBillingAccount_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteBillingAccountAsync(42);

        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/billing-account/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task GetBillingAccount_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListBillingAccountsAsync(
            name: "Spendenkonto",
            idIn: "1,2",
            skr: "42",
            deleted: "false",
            ordering: "number");
        await client.GetBillingAccountAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/billing-account/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.DoesNotContain("skr=", query);
        Assert.DoesNotContain("deleted=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task ListBillingAccounts_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListBillingAccountsAsync());
    }

    // ------------------------------------------------------------------ //
    // Booking Projects
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListBookingProjects_ReturnsBookingProjects()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    name = "Sommerfest 2026",
                    color = "#ff8800",
                    @short = "SF26",
                    budget = 1500.75,
                    completed = false,
                    projectCostCentre = "KST-123"
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListBookingProjectsAsync();

        Assert.Single(result);
        Assert.Equal("Sommerfest 2026", result[0].Name);
        Assert.Equal("SF26", result[0].Short);
        Assert.Equal(1500.75m, result[0].Budget);
    }

    [Fact]
    public async Task ListBookingProjects_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListBookingProjectsAsync(
            name: "Sommerfest",
            @short: "SF",
            completed: "false",
            budgetGt: "100",
            budgetLt: "2000",
            ordering: "name");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("name=Sommerfest", query);
        Assert.Contains("short=SF", query);
        Assert.Contains("completed=false", query);
        Assert.Contains("budget__gt=100", query);
        Assert.Contains("budget__lt=2000", query);
        Assert.Contains("ordering=name", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task GetBookingProject_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetBookingProjectAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateBookingProject_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new
        {
            id = 123,
            name = "Neues Projekt",
            budget = 500.0,
            completed = false
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateBookingProjectAsync(new BookingProject
        {
            Name = "Neues Projekt",
            Budget = 500m,
            Completed = false
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Neues Projekt", created.Name);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/booking-project", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateBookingProject_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new
        {
            id = 5,
            name = "Renamed",
            completed = true
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["name"] = "Renamed",
            ["completed"] = true
        };
        var updated = await client.UpdateBookingProjectAsync(5, patch);

        Assert.Equal("Renamed", updated.Name);
        Assert.True(updated.Completed);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/booking-project/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteBookingProject_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteBookingProjectAsync(42);

        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/booking-project/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListBookingProjects_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListBookingProjectsAsync());
    }

    [Fact]
    public async Task GetBookingProject_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListBookingProjectsAsync(
            name: "Dorffest",
            @short: "DF",
            completed: "false",
            idIn: "1,2",
            ordering: "name");
        await client.GetBookingProjectAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/booking-project/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("short=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Chairman Levels
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListChairmanLevels_ReturnsChairmanLevels()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    name = "Vorstand",
                    color = "#336699",
                    @short = "VS",
                    module_members = "W",
                    module_bookings = "N"
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListChairmanLevelsAsync();

        Assert.Single(result);
        Assert.Equal("Vorstand", result[0].Name);
        Assert.Equal("VS", result[0].Short);
        Assert.Equal("W", result[0].ModuleMembers);
        Assert.Equal("N", result[0].ModuleBookings);
    }

    [Fact]
    public async Task ListChairmanLevels_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListChairmanLevelsAsync(
            name: "Vorstand",
            @short: "VS",
            idIn: "1,2,3",
            ordering: "name");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("name=Vorstand", query);
        Assert.Contains("short=VS", query);
        Assert.Contains("id__in=1%2C2%2C3", query);
        Assert.Contains("ordering=name", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task GetChairmanLevel_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetChairmanLevelAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateChairmanLevel_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new
        {
            id = 123,
            name = "Kassenwart",
            module_account = "W"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateChairmanLevelAsync(new ChairmanLevel
        {
            Name = "Kassenwart",
            ModuleAccount = "W"
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Kassenwart", created.Name);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/chairman-level", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateChairmanLevel_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new
        {
            id = 5,
            name = "Renamed",
            module_members = "R"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["name"] = "Renamed",
            ["module_members"] = "R"
        };
        var updated = await client.UpdateChairmanLevelAsync(5, patch);

        Assert.Equal("Renamed", updated.Name);
        Assert.Equal("R", updated.ModuleMembers);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/chairman-level/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteChairmanLevel_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteChairmanLevelAsync(42);

        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/chairman-level/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListChairmanLevels_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListChairmanLevelsAsync());
    }

    [Fact]
    public async Task GetChairmanLevel_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "Vorstand" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListChairmanLevelsAsync(
            name: "Vorstand",
            @short: "VS",
            idIn: "1,2",
            ordering: "name");
        await client.GetChairmanLevelAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/chairman-level/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("short=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Contact-Details Groups
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListContactDetailsGroups_ReturnsContactDetailsGroups()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    name = "Newsletter",
                    color = "#aabbcc",
                    @short = "NL",
                    orderSequence = 2
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListContactDetailsGroupsAsync();

        Assert.Single(result);
        Assert.Equal("Newsletter", result[0].Name);
        Assert.Equal("NL", result[0].Short);
        Assert.Equal(2, result[0].OrderSequence);
    }

    [Fact]
    public async Task ListContactDetailsGroups_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListContactDetailsGroupsAsync(
            name: "Newsletter",
            color: "#aabbcc",
            @short: "NL",
            deleted: false,
            idIn: "1,2,3",
            ordering: "name");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("name=Newsletter", query);
        Assert.Contains("color=%23aabbcc", query);
        Assert.Contains("short=NL", query);
        Assert.Contains("deleted=false", query);
        Assert.Contains("id__in=1%2C2%2C3", query);
        Assert.Contains("ordering=name", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task GetContactDetailsGroup_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetContactDetailsGroupAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateContactDetailsGroup_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new
        {
            id = 123,
            name = "Sponsoren",
            color = "#112233",
            @short = "SPO"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateContactDetailsGroupAsync(new ContactDetailsGroup
        {
            Name = "Sponsoren",
            Color = "#112233",
            Short = "SPO"
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Sponsoren", created.Name);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/contact-details-group", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateContactDetailsGroup_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new
        {
            id = 5,
            name = "Renamed",
            color = "#000000"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["name"] = "Renamed",
            ["color"] = "#000000"
        };
        var updated = await client.UpdateContactDetailsGroupAsync(5, patch);

        Assert.Equal("Renamed", updated.Name);
        Assert.Equal("#000000", updated.Color);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/contact-details-group/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteContactDetailsGroup_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteContactDetailsGroupAsync(42);

        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/contact-details-group/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListContactDetailsGroups_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListContactDetailsGroupsAsync());
    }

    [Fact]
    public async Task GetContactDetailsGroup_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "Newsletter" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListContactDetailsGroupsAsync(
            name: "Newsletter",
            @short: "NL",
            idIn: "1,2",
            ordering: "name");
        await client.GetContactDetailsGroupAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/contact-details-group/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("short=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateContactDetailsGroup_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"name\":\"Y\"}");
        var client = CreateClient(handler);

        await client.CreateContactDetailsGroupAsync(new ContactDetailsGroup { Name = "Y" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    // ------------------------------------------------------------------ //
    // Contact-Details Logs
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListContactDetailsLogs_ReturnsContactDetailsLogs()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    kind = "Custom",
                    description = "Adresse aktualisiert",
                    date = "2026-05-31T10:00:00",
                    shared = true
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListContactDetailsLogsAsync();

        Assert.Single(result);
        Assert.Equal("Custom", result[0].Kind);
        Assert.Equal("Adresse aktualisiert", result[0].Description);
        Assert.True(result[0].Shared);
        Assert.Equal(new DateTime(2026, 5, 31, 10, 0, 0), result[0].Date);
    }

    [Fact]
    public async Task ListContactDetailsLogs_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListContactDetailsLogsAsync(
            idIn: "1,2,3",
            date: "2026-05-31",
            dateGte: "2026-05-01",
            dateLte: "2026-05-31",
            ordering: "date");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("id__in=1%2C2%2C3", query);
        Assert.Contains("date=2026-05-31", query);
        Assert.Contains("date__gte=2026-05-01", query);
        Assert.Contains("date__lte=2026-05-31", query);
        Assert.Contains("ordering=date", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task GetContactDetailsLog_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetContactDetailsLogAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateContactDetailsLog_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new
        {
            id = 123,
            kind = "Custom",
            description = "Neuer Log",
            relatedAddress = "https://easyverein.com/api/v2.0/contact-details/555"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateContactDetailsLogAsync(new ContactDetailsLog
        {
            Kind = "Custom",
            Description = "Neuer Log",
            RelatedAddress = 555
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Custom", created.Kind);
        Assert.Equal(555L, created.RelatedAddress);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/contact-details-log", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateContactDetailsLog_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new
        {
            id = 5,
            kind = "Custom",
            description = "Geaendert"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["description"] = "Geaendert"
        };
        var updated = await client.UpdateContactDetailsLogAsync(5, patch);

        Assert.Equal("Geaendert", updated.Description);
        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/contact-details-log/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteContactDetailsLog_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteContactDetailsLogAsync(42);

        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/contact-details-log/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListContactDetailsLogs_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListContactDetailsLogsAsync());
    }

    [Fact]
    public async Task GetContactDetailsLog_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, kind = "Custom" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListContactDetailsLogsAsync(
            idIn: "1,2",
            dateGte: "2026-05-01",
            ordering: "date");
        await client.GetContactDetailsLogAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/contact-details-log/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("id__in=", query);
        Assert.DoesNotContain("date__gte=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateContactDetailsLog_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"kind\":\"Custom\"}");
        var client = CreateClient(handler);

        await client.CreateContactDetailsLogAsync(new ContactDetailsLog { Kind = "Custom" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    // ------------------------------------------------------------------ //
    // Custom Fields
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListCustomFields_ReturnsCustomFields()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new
                {
                    id = 1,
                    name = "Lieblingsfarbe",
                    settings_type = "T",
                    kind = "E",
                    member_show = true,
                    collection = "https://easyverein.com/api/v2.0/custom-field-collection/777"
                }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListCustomFieldsAsync();

        Assert.Single(result);
        Assert.Equal("Lieblingsfarbe", result[0].Name);
        Assert.Equal("T", result[0].SettingsType);
        Assert.Equal("E", result[0].Kind);
        Assert.True(result[0].MemberShow);
        Assert.Equal(777L, result[0].Collection);
    }

    [Fact]
    public async Task ListCustomFields_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomFieldsAsync(
            idIn: "1,2,3",
            name: "Farbe",
            settingsType: "T",
            memberShow: true,
            collection: "777",
            ordering: "name");

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("id__in=1%2C2%2C3", query);
        Assert.Contains("name=Farbe", query);
        Assert.Contains("settings_type=T", query);
        Assert.Contains("member_show=true", query);
        Assert.Contains("collection=777", query);
        Assert.Contains("ordering=name", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task GetCustomField_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetCustomFieldAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCustomField_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new
        {
            id = 123,
            name = "Lieblingsfarbe",
            settings_type = "T",
            kind = "E"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateCustomFieldAsync(new CustomField
        {
            Name = "Lieblingsfarbe",
            SettingsType = "T",
            Kind = "E"
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Lieblingsfarbe", created.Name);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/custom-field", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateCustomField_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new
        {
            id = 5,
            name = "Umbenannt"
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object>
        {
            ["name"] = "Umbenannt"
        };
        var updated = await client.UpdateCustomFieldAsync(5, patch);

        Assert.Equal("Umbenannt", updated.Name);
        Assert.NotNull(handler.LastRequestUri);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/custom-field/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteCustomField_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteCustomFieldAsync(42);

        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/custom-field/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListCustomFields_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListCustomFieldsAsync());
    }

    [Fact]
    public async Task GetCustomField_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "Farbe" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListCustomFieldsAsync(
            name: "Farbe",
            settingsType: "T",
            idIn: "1,2",
            ordering: "name");
        await client.GetCustomFieldAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/custom-field/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("settings_type=", query);
        Assert.DoesNotContain("id__in=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateCustomField_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"name\":\"Y\"}");
        var client = CreateClient(handler);

        await client.CreateCustomFieldAsync(new CustomField { Name = "Y" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    // ------------------------------------------------------------------ //
    // Custom Field Collections
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListCustomFieldCollections_ReturnsCollections()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = new[]
            {
                new { id = 777, name = "Vereinsdaten", orderSequence = 2, position = 5 }
            },
            next = (string?)null
        });
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListCustomFieldCollectionsAsync();

        Assert.Single(result);
        Assert.Equal(777L, result[0].Id);
        Assert.Equal("Vereinsdaten", result[0].Name);
        Assert.Equal(2, result[0].OrderSequence);
        Assert.Equal(5, result[0].Position);
    }

    [Fact]
    public async Task ListCustomFieldCollections_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomFieldCollectionsAsync(
            idIn: "1,2,3",
            position: 4,
            ordering: "-position",
            search: new[] { "Verein" });

        Assert.NotNull(handler.LastRequestUri);
        var query = handler.LastRequestUri!.Query;
        Assert.Contains("custom-field-collection", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2%2C3", query);
        Assert.Contains("position=4", query);
        Assert.Contains("ordering=-position", query);
        Assert.Contains("search=Verein", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListCustomFieldCollections_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, name = "A" } },
            next = "https://easyverein.com/api/v2.0/custom-field-collection?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, name = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListCustomFieldCollectionsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task GetCustomFieldCollection_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetCustomFieldCollectionAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCustomFieldCollection_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new { id = 123, name = "Vereinsdaten", position = 1 });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateCustomFieldCollectionAsync(new CustomFieldCollection
        {
            Name = "Vereinsdaten",
            Position = 1
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal("Vereinsdaten", created.Name);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/custom-field-collection", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("\"name\":\"Vereinsdaten\"", handler.LastRequestBody);
        Assert.DoesNotContain("orderSequence", handler.LastRequestBody);
    }

    [Fact]
    public async Task UpdateCustomFieldCollection_SendsPatchDictionary()
    {
        var updatedJson = JsonSerializer.Serialize(new { id = 5, name = "Umbenannt" });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, updatedJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object> { ["name"] = "Umbenannt" };
        var updated = await client.UpdateCustomFieldCollectionAsync(5, patch);

        Assert.Equal("Umbenannt", updated.Name);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/custom-field-collection/5", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"name\":\"Umbenannt\"}", handler.LastRequestBody);
    }

    [Fact]
    public async Task DeleteCustomFieldCollection_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteCustomFieldCollectionAsync(42);

        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.EndsWith("/custom-field-collection/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListCustomFieldCollections_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListCustomFieldCollectionsAsync());
    }

    [Fact]
    public async Task GetCustomFieldCollection_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListCustomFieldCollectionsAsync(idIn: "1,2", position: 3, ordering: "name");
        await client.GetCustomFieldCollectionAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/custom-field-collection/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("id__in=", query);
        Assert.DoesNotContain("position=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CustomFieldCollection_QuerySelector_RequestsOnlyDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomFieldCollectionsAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains("query={id,name,orderSequence,position}", query);
    }

    [Fact]
    public async Task CreateCustomFieldCollection_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"name\":\"Y\"}");
        var client = CreateClient(handler);

        await client.CreateCustomFieldCollectionAsync(new CustomFieldCollection { Name = "Y" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    // ------------------------------------------------------------------ //
    // Custom Filters
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListCustomFilters_ReturnsFiltersWithRules()
    {
        var json = """
            {
                "results": [
                    {
                        "id": 73383,
                        "name": "2024",
                        "model": "bookingFilter",
                        "rules": {"condition": "AND", "rules": [{"field": "date", "operator": "greater_or_equal", "value": "2024-01-01"}], "valid": true},
                        "created_at": "2026-05-27T06:23:55.206540+02:00",
                        "updated_at": "2026-05-27T06:23:55.308413+02:00"
                    }
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListCustomFiltersAsync();

        Assert.Single(result);
        Assert.Equal(73383L, result[0].Id);
        Assert.Equal("bookingFilter", result[0].Model);
        Assert.Equal("AND", result[0].Rules!.Value.GetProperty("condition").GetString());
        Assert.NotNull(result[0].CreatedAt);
    }

    [Fact]
    public async Task ListCustomFilters_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomFiltersAsync(
            idIn: "1,2",
            name: "2024",
            model: "bookingFilter",
            modelIn: "bookingFilter,userFilter",
            ordering: "-name",
            search: new[] { "Saldo" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/custom-filter", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2", query);
        Assert.Contains("name=2024", query);
        Assert.Contains("model=bookingFilter", query);
        Assert.Contains("model__in=bookingFilter%2CuserFilter", query);
        Assert.Contains("ordering=-name", query);
        Assert.Contains("search=Saldo", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListCustomFilters_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, name = "A" } },
            next = "https://easyverein.com/api/v2.0/custom-filter?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, name = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListCustomFiltersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task CustomFilter_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomFiltersAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains("query={id,name,model,rules,created_at,updated_at}", query);
    }

    [Fact]
    public async Task GetCustomFilter_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetCustomFilterAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCustomFilter_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListCustomFiltersAsync(name: "X", model: "userFilter", ordering: "name");
        await client.GetCustomFilterAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/custom-filter/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("model=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateCustomFilter_PostsRulesAsJsonObject_WithoutTimestamps()
    {
        var createdJson = """{"id":123,"name":"Test","model":"bookingFilter","rules":{"condition":"AND","rules":[]}}""";
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);
        using var rules = JsonDocument.Parse("""{"condition":"AND","rules":[]}""");

        var created = await client.CreateCustomFilterAsync(new CustomFilter
        {
            Name = "Test",
            Model = "bookingFilter",
            Rules = rules.RootElement.Clone()
        });

        Assert.Equal(123L, created.Id);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/custom-filter", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("\"rules\":{\"condition\":\"AND\",\"rules\":[]}", handler.LastRequestBody);
        Assert.Contains("\"model\":\"bookingFilter\"", handler.LastRequestBody);
        Assert.DoesNotContain("created_at", handler.LastRequestBody);
        Assert.DoesNotContain("updated_at", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateCustomFilter_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"name\":\"Y\"}");
        var client = CreateClient(handler);

        await client.CreateCustomFilterAsync(new CustomFilter { Name = "Y", Model = "userFilter" });

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task UpdateCustomFilter_SendsPatchDictionary()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"id":5,"name":"Umbenannt"}""");
        var client = CreateClient(handler);

        var updated = await client.UpdateCustomFilterAsync(5, new Dictionary<string, object> { ["name"] = "Umbenannt" });

        Assert.Equal("Umbenannt", updated.Name);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/custom-filter/5", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"name\":\"Umbenannt\"}", handler.LastRequestBody);
    }

    [Fact]
    public async Task DeleteCustomFilter_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteCustomFilterAsync(42);

        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.EndsWith("/custom-filter/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListCustomFilters_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListCustomFiltersAsync());
    }

    // ------------------------------------------------------------------ //
    // Custom Tax Rates
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListCustomTaxRates_ReturnsRates()
    {
        var json = """
            {
                "results": [
                    {"id": 1272, "taxName": "", "customTaxRate": "19.00", "countryCode": "DE", "org": null},
                    {"id": 1962, "taxName": "Eigener Satz", "customTaxRate": "2.25", "countryCode": "", "org": "https://easyverein.com/api/v2.0/organization/30189"}
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListCustomTaxRatesAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(19.00m, result[0].CustomTaxRateValue);
        Assert.Equal("DE", result[0].CountryCode);
        Assert.Null(result[0].Org);
        Assert.Equal("Eigener Satz", result[1].TaxName);
        Assert.NotNull(result[1].Org);
    }

    [Fact]
    public async Task ListCustomTaxRates_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomTaxRatesAsync(
            idIn: "1,2",
            taxName: "Ermäßigt",
            taxNameNe: "X",
            customTaxRate: "19",
            customTaxRateNe: "0",
            orgIsnull: false,
            deleted: true,
            showAllowedToUse: true,
            ordering: "-customTaxRate",
            search: new[] { "Satz" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/custom-tax-rate", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2", query);
        Assert.Contains("taxName=Erm%C3%A4%C3%9Figt", query);
        Assert.Contains("taxName__ne=X", query);
        Assert.Contains("customTaxRate=19", query);
        Assert.Contains("customTaxRate__ne=0", query);
        Assert.Contains("org__isnull=false", query);
        Assert.Contains("deleted=true", query);
        Assert.Contains("showAllowedToUse=true", query);
        Assert.Contains("ordering=-customTaxRate", query);
        Assert.Contains("search=Satz", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListCustomTaxRates_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, customTaxRate = "7.00" } },
            next = "https://easyverein.com/api/v2.0/custom-tax-rate?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, customTaxRate = "19.00" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListCustomTaxRatesAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(19.00m, result[1].CustomTaxRateValue);
    }

    [Fact]
    public async Task CustomTaxRate_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListCustomTaxRatesAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains("query={id,taxName,customTaxRate,countryCode,org,created_at,updated_at}", query);
    }

    [Fact]
    public async Task GetCustomTaxRate_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetCustomTaxRateAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCustomTaxRate_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, customTaxRate = "7.00" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListCustomTaxRatesAsync(customTaxRate: "7", showAllowedToUse: true, ordering: "taxName");
        await client.GetCustomTaxRateAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/custom-tax-rate/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("customTaxRate=", query);
        Assert.DoesNotContain("showAllowedToUse=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateCustomTaxRate_PostsRateAsNumber_WithoutReadOnlyFields()
    {
        var createdJson = """{"id":123,"taxName":"Test","customTaxRate":"1.50","countryCode":"","org":"https://easyverein.com/api/v2.0/organization/1"}""";
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateCustomTaxRateAsync(new CustomTaxRate { TaxName = "Test", CustomTaxRateValue = 1.5m });

        Assert.Equal(123L, created.Id);
        Assert.Equal(1.50m, created.CustomTaxRateValue);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/custom-tax-rate", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("\"customTaxRate\":1.5", handler.LastRequestBody);
        Assert.Contains("\"taxName\":\"Test\"", handler.LastRequestBody);
        Assert.DoesNotContain("org", handler.LastRequestBody);
        Assert.DoesNotContain("countryCode", handler.LastRequestBody);
        Assert.DoesNotContain("created_at", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateCustomTaxRate_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1}");
        var client = CreateClient(handler);

        await client.CreateCustomTaxRateAsync(new CustomTaxRate { TaxName = "Y", CustomTaxRateValue = 1m });

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task UpdateCustomTaxRate_SendsPatchDictionary()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"id":5,"customTaxRate":"2.25"}""");
        var client = CreateClient(handler);

        var updated = await client.UpdateCustomTaxRateAsync(5, new Dictionary<string, object> { ["customTaxRate"] = 2.25m });

        Assert.Equal(2.25m, updated.CustomTaxRateValue);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/custom-tax-rate/5", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"customTaxRate\":2.25}", handler.LastRequestBody);
    }

    [Fact]
    public async Task DeleteCustomTaxRate_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteCustomTaxRateAsync(42);

        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.EndsWith("/custom-tax-rate/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListCustomTaxRates_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListCustomTaxRatesAsync());
    }

    // ------------------------------------------------------------------ //
    // DOSB Sports (read-only)
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListDosbSports_ReturnsSports()
    {
        var json = """
            {
                "results": [
                    {"id": 55, "title": "Fußball", "sportNumber": "042", "federationNumber": "07"}
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListDosbSportsAsync();

        Assert.Single(result);
        Assert.Equal(55L, result[0].Id);
        Assert.Equal("Fußball", result[0].Title);
        Assert.Equal("042", result[0].SportNumber);
        Assert.Equal("07", result[0].FederationNumber);
    }

    [Fact]
    public async Task ListDosbSports_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListDosbSportsAsync(
            idIn: "1,2",
            title: "Fußball",
            sportNumber: "042",
            federationNumber: "07",
            ordering: "-title",
            search: new[] { "Ball" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/dosb-sport", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2", query);
        Assert.Contains("title=Fu%C3%9Fball", query);
        Assert.Contains("sportNumber=042", query);
        Assert.Contains("federationNumber=07", query);
        Assert.Contains("ordering=-title", query);
        Assert.Contains("search=Ball", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListDosbSports_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, title = "A" } },
            next = "https://easyverein.com/api/v2.0/dosb-sport?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, title = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListDosbSportsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Title);
    }

    [Fact]
    public async Task DosbSport_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListDosbSportsAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains("query={id,title,sportNumber,federationNumber,org,created_at,updated_at}", query);
    }

    [Fact]
    public async Task GetDosbSport_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetDosbSportAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetDosbSport_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, title = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListDosbSportsAsync(title: "X", sportNumber: "1", ordering: "title");
        await client.GetDosbSportAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/dosb-sport/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("title=", query);
        Assert.DoesNotContain("sportNumber=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task ListDosbSports_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListDosbSportsAsync());
    }

    // ------------------------------------------------------------------ //
    // Feature Requests (easyVerein product idea board)
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListFeatureRequests_ReturnsRequestsWithAuthor()
    {
        var json = """
            {
                "results": [
                    {
                        "id": 7,
                        "label": "C107/T47",
                        "title": "eine eigene App",
                        "author": {"id": 109, "org": {"id": 23, "short": "admin", "name": "easyVerein Verwaltung"}},
                        "proVotesCount": 301,
                        "contraVotesCount": 28,
                        "status": 3,
                        "category": 1
                    }
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListFeatureRequestsAsync();

        Assert.Single(result);
        Assert.Equal("C107/T47", result[0].Label);
        Assert.Equal(301, result[0].ProVotesCount);
        Assert.Equal("admin", result[0].Author!.Org!.Short);
    }

    [Fact]
    public async Task ListFeatureRequests_WithUnknownAuthor_DoesNotFail()
    {
        var json = """
            {
                "results": [
                    {"id": 1, "title": "A", "author": {"id": 109, "org": {"id": 23, "short": "admin", "name": "easyVerein Verwaltung"}}},
                    {"id": 26, "title": "B", "author": {"id": "Unbekannt", "org": {"id": "", "short": "", "name": "Unbekannt"}}}
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListFeatureRequestsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(109L, result[0].Author!.Id);
        Assert.Null(result[1].Author!.Id);
    }

    [Fact]
    public async Task ListFeatureRequests_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListFeatureRequestsAsync(
            idIn: "1,4",
            status: "3",
            category: "10",
            authorIsme: true,
            ordering: "-proVotesCount",
            search: new[] { "DATEV" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/feature-request", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C4", query);
        Assert.Contains("status=3", query);
        Assert.Contains("category=10", query);
        Assert.Contains("author__isme=true", query);
        Assert.Contains("ordering=-proVotesCount", query);
        Assert.Contains("search=DATEV", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListFeatureRequests_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, title = "A" } },
            next = "https://easyverein.com/api/v2.0/feature-request?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, title = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListFeatureRequestsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Title);
    }

    [Fact]
    public async Task FeatureRequest_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListFeatureRequestsAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains(
            "query={id,label,title,description,response,author,proVotesCount,contraVotesCount,hasVoted,status,approved,date,category}",
            query);
    }

    [Fact]
    public async Task GetFeatureRequest_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetFeatureRequestAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetFeatureRequest_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, title = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListFeatureRequestsAsync(status: "3", category: "10", ordering: "title");
        await client.GetFeatureRequestAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/feature-request/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("status=", query);
        Assert.DoesNotContain("category=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateFeatureRequest_PostsOnlyWritableFields()
    {
        var createdJson = """{"id":6800,"label":"C6900","title":"T","description":"D","status":0,"category":6}""";
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateFeatureRequestAsync(new FeatureRequest
        {
            Title = "T",
            Description = "D",
            Category = 6
        });

        Assert.Equal(6800L, created.Id);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/feature-request", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"title\":\"T\",\"description\":\"D\",\"category\":6}", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateFeatureRequest_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1}");
        var client = CreateClient(handler);

        await client.CreateFeatureRequestAsync(new FeatureRequest { Title = "T", Description = "D" });

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Theory]
    [InlineData(true, "/feature-request/7/voteFor")]
    [InlineData(false, "/feature-request/7/voteAgainst")]
    public async Task VoteFeatureRequest_SendsGetToVotePath(bool inFavor, string expectedPath)
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, "{\"ok\":true}");
        var client = CreateClient(handler);

        var result = await client.VoteFeatureRequestAsync(7, inFavor);

        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.EndsWith(expectedPath, handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"ok\":true}", result);
    }

    [Fact]
    public async Task VoteFeatureRequest_WithServerError_ThrowsHttpRequestException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest, "{\"detail\":\"bereits abgestimmt\"}");
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.VoteFeatureRequestAsync(7, true));
        Assert.Contains("bereits abgestimmt", ex.Message);
    }

    [Fact]
    public async Task ListFeatureRequests_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListFeatureRequestsAsync());
    }

    // ------------------------------------------------------------------ //
    // Forums
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListForums_ReturnsForums()
    {
        var json = """
            {
                "results": [
                    {"id": 31875, "name": "Kulturverein Milower Land e.V.", "slug": "kulturverein-milower-land-ev", "type": 0, "last_post": null, "display_sub_forum_list": true},
                    {"id": 31876, "name": "Vorstand", "slug": "vorstand", "type": 0, "direct_topics_count": 3}
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListForumsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Kulturverein Milower Land e.V.", result[0].Name);
        Assert.True(result[0].DisplaySubForumList);
        Assert.Equal("vorstand", result[1].Slug);
        Assert.Equal(3, result[1].DirectTopicsCount);
    }

    [Fact]
    public async Task ListForums_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListForumsAsync(
            idIn: "1,2",
            name: "Vorstand & Beirat",
            nameNot: "Alt",
            slug: "vorstand",
            slugNot: "alt",
            type: 0,
            createdGt: "2025-01-01",
            createdLt: "2026-01-01",
            updatedGt: "2025-02-01",
            updatedLt: "2026-02-01",
            ordering: "-order",
            search: new[] { "Verein" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/forum", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2", query);
        Assert.Contains("name=Vorstand%20%26%20Beirat", query);
        Assert.Contains("name__not=Alt", query);
        Assert.Contains("slug=vorstand", query);
        Assert.Contains("slug__not=alt", query);
        Assert.Contains("type=0", query);
        Assert.Contains("created__gt=2025-01-01", query);
        Assert.Contains("created__lt=2026-01-01", query);
        Assert.Contains("updated__gt=2025-02-01", query);
        Assert.Contains("updated__lt=2026-02-01", query);
        Assert.Contains("ordering=-order", query);
        Assert.Contains("search=Verein", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListForums_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, name = "A" } },
            next = "https://easyverein.com/api/v2.0/forum?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, name = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListForumsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task Forum_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListForumsAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains(
            "query={id,org,last_post,created,updated,name,slug,description,image,link,link_redirects,type," +
            "direct_posts_count,direct_topics_count,link_redirects_count,order,last_post_on,display_sub_forum_list}",
            query);
    }

    [Fact]
    public async Task GetForum_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetForumAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetForum_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListForumsAsync(name: "X", type: 0, ordering: "name");
        var forum = await client.GetForumAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.Equal(999L, forum!.Id);
        Assert.EndsWith("/forum/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("type=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateForum_PostsWritableFields_WithoutReadOnlyFields()
    {
        var createdJson = """{"id":123,"name":"Vorstand","slug":"vorstand","org":"https://easyverein.com/api/v2.0/organization/1","type":0}""";
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateForumAsync(new Forum { Name = "Vorstand", Order = 2, DisplaySubForumList = false });

        Assert.Equal(123L, created.Id);
        Assert.Equal("vorstand", created.Slug);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/forum", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("\"name\":\"Vorstand\"", handler.LastRequestBody);
        Assert.Contains("\"order\":2", handler.LastRequestBody);
        Assert.Contains("\"display_sub_forum_list\":false", handler.LastRequestBody);
        Assert.DoesNotContain("org", handler.LastRequestBody);
        Assert.DoesNotContain("slug", handler.LastRequestBody);
        Assert.DoesNotContain("\"type\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateForum_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1}");
        var client = CreateClient(handler);

        await client.CreateForumAsync(new Forum { Name = "Y" });

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task UpdateForum_SendsPatchDictionary()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"id":5,"name":"Neu"}""");
        var client = CreateClient(handler);

        var updated = await client.UpdateForumAsync(5, new Dictionary<string, object> { ["name"] = "Neu" });

        Assert.Equal("Neu", updated.Name);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/forum/5", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"name\":\"Neu\"}", handler.LastRequestBody);
    }

    [Fact]
    public async Task DeleteForum_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteForumAsync(42);

        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.EndsWith("/forum/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListForums_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListForumsAsync());
    }

    // ------------------------------------------------------------------ //
    // Get Token (login with user credentials)
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetToken_PostsCredentials_AndReturnsToken()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"token":"abc123def456"}""");
        var client = CreateClient(handler);

        var result = await client.GetTokenAsync("kv_user@example.org", "s3cret");

        Assert.Equal("abc123def456", result.Token);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/get-token", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("""{"username":"kv_user@example.org","password":"s3cret"}""", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetToken_WithTwoFactorCode_SendsTwoFactorField()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"token":"t"}""");
        var client = CreateClient(handler);

        await client.GetTokenAsync("kv_user", "pw", "123456");

        Assert.Contains("\"2FA\":\"123456\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetToken_WithTwoFactorChallenge_ReturnsNeeds2FA()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, """{"needs2FA":true}""");
        var client = CreateClient(handler);

        var result = await client.GetTokenAsync("kv_user", "pw");

        Assert.True(result.Needs2FA);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task GetToken_WithInvalidCredentials_ThrowsHttpRequestException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest,
            """{"non_field_errors":["Die angegebenen Zugangsdaten stimmen nicht."]}""");
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetTokenAsync("kv_user", "pw"));

        Assert.Contains("Zugangsdaten", ex.Message);
    }

    [Fact]
    public async Task GetToken_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"token":"t"}""");
        var client = CreateClient(handler);

        await client.GetTokenAsync("kv_user", "pw");

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
    }

    // ------------------------------------------------------------------ //
    // HTTP Transport — POST regression coverage for issue:
    // easyVerein's reverse proxy rejects chunked POST bodies with HTTP 411
    // (Length Required). All Create*Async methods must send the body as
    // fixed-length StringContent (i.e. expose Content-Length).
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task CreateBookingProject_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"name\":\"X\"}");
        var client = CreateClient(handler);

        await client.CreateBookingProjectAsync(new BookingProject { Name = "X" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task CreateInvoiceItem_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"title\":\"X\"}");
        var client = CreateClient(handler);

        await client.CreateInvoiceItemAsync(new InvoiceItem { Title = "X" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task CreateChairmanLevel_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"name\":\"Y\"}");
        var client = CreateClient(handler);

        await client.CreateChairmanLevelAsync(new ChairmanLevel { Name = "Y" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding);
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task CreateAnnouncement_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1,\"title\":\"Z\"}");
        var client = CreateClient(handler);

        await client.CreateAnnouncementAsync(new Announcement { Text = "Z" });

        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.False(handler.LastRequestUsedChunkedEncoding);
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    // ------------------------------------------------------------------ //
    // POST body serialisation — null properties must be omitted so that
    // easyVerein keeps the server-side default instead of being overwritten
    // with "null". (US-0058)
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task CreateInvoice_OmitsNullProperties_FromJsonBody()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1}");
        var client = CreateClient(handler);

        await client.CreateInvoiceAsync(new Invoice
        {
            InvoiceNumber = "R-001",
            TotalPrice = 99.99m,
            Description = "Unit test"
        });

        Assert.NotNull(handler.LastRequestBody);
        var body = handler.LastRequestBody!;

        Assert.Contains("\"invNumber\":\"R-001\"", body);
        Assert.Contains("\"totalPrice\":99.99", body);
        Assert.Contains("\"description\":\"Unit test\"", body);

        // Nullable properties that were not set must not appear in the JSON body.
        Assert.DoesNotContain("\"kind\"", body);
        Assert.DoesNotContain("\"refNumber\"", body);
        Assert.DoesNotContain("\"paymentInformation\"", body);
        Assert.DoesNotContain("\"actualCallStateName\"", body);
        Assert.DoesNotContain("\"callStateDelayDays\"", body);
        Assert.DoesNotContain("\"accnumber\"", body);
        Assert.DoesNotContain("\"guid\"", body);
        Assert.DoesNotContain("\"mode\"", body);
        Assert.DoesNotContain("\"offerStatus\"", body);
        Assert.DoesNotContain("\"relatedAddress\"", body);
        Assert.DoesNotContain("\"bankAccount\"", body);
    }

    // ------------------------------------------------------------------ //
    // Invoice Items
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListInvoiceItems_SendsExpectedQuery()
    {
        var json = JsonSerializer.Serialize(new
        {
            results = Array.Empty<object>(),
            next = (string?)null
        });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListInvoiceItemsAsync(relatedInvoice: "469271649");

        Assert.NotNull(handler.LastRequestUri);
        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains("relatedInvoice=469271649", query);
        Assert.Contains("query={id,", query);
        Assert.Contains(",costCentre,", query);
    }

    [Fact]
    public async Task GetInvoiceItem_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);
        var result = await client.GetInvoiceItemAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateInvoiceItem_PostsEntityAndReturnsCreated()
    {
        var createdJson = JsonSerializer.Serialize(new { id = 99, title = "New", sphere = 9 });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateInvoiceItemAsync(new InvoiceItem { Title = "New", Sphere = 9 });

        Assert.Equal(99L, created.Id);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/invoice-item", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateInvoiceItem_SendsPatchDictionary()
    {
        var responseJson = JsonSerializer.Serialize(new { id = 5, sphere = 2, costCentre = "2901" });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, responseJson);
        var client = CreateClient(handler);

        var patch = new Dictionary<string, object> { ["sphere"] = 2, ["costCentre"] = "2901" };
        var updated = await client.UpdateInvoiceItemAsync(5, patch);

        Assert.Equal(2, updated.Sphere);
        Assert.Equal("2901", updated.CostCentre);
        Assert.EndsWith("/invoice-item/5", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteInvoiceItem_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);
        await client.DeleteInvoiceItemAsync(42);
        Assert.NotNull(handler.LastRequestUri);
        Assert.EndsWith("/invoice-item/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListInvoiceItems_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListInvoiceItemsAsync());
    }

    [Fact]
    public async Task GetInvoiceItem_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, title = "Pos" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListInvoiceItemsAsync(
            idIn: "1,2",
            relatedInvoice: "42",
            ordering: "id",
            search: new[] { "Pos" });
        await client.GetInvoiceItemAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/invoice-item/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("id__in=", query);
        Assert.DoesNotContain("relatedInvoice=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.DoesNotContain("search=", query);
        Assert.Contains("query=", query);
    }

    // ------------------------------------------------------------------ //
    // Inventory Objects
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListInventoryObjects_ReturnsInventoryObjects()
    {
        var json = """
            {
                "results": [
                    {"id": 335646309, "name": "Zelt", "pieces": 2, "price": "99.99", "lendingResponsible": "https://easyverein.com/api/v2.0/member/4424352"},
                    {"id": 2, "name": "Beamer", "lendingAvailable": false, "locationObject": null}
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListInventoryObjectsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Zelt", result[0].Name);
        Assert.Equal(99.99m, result[0].Price);
        Assert.Equal(4424352L, result[0].LendingResponsibleId);
        Assert.False(result[1].LendingAvailable);
    }

    [Fact]
    public async Task ListInventoryObjects_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListInventoryObjectsAsync(
            idIn: "1,2",
            name: "Zelt & Plane",
            identifier: "59409381",
            lendingAvailable: true,
            deleted: false,
            locationObject: 335646294,
            locationObjectNot: 7,
            inventoryObjectGroups: "11,12",
            inventoryObjectGroupsNot: "13",
            lendingState: "lent",
            ordering: "-name",
            search: new[] { "Zelt" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/inventory-object", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2", query);
        Assert.Contains("name=Zelt%20%26%20Plane", query);
        Assert.Contains("identifier=59409381", query);
        Assert.Contains("lendingAvailable=true", query);
        Assert.Contains("deleted=false", query);
        Assert.Contains("locationObject=335646294", query);
        Assert.Contains("locationObject__not=7", query);
        Assert.Contains("inventoryObjectGroups=11%2C12", query);
        Assert.Contains("inventoryObjectGroups__not=13", query);
        Assert.Contains("lending__state=lent", query);
        Assert.Contains("ordering=-name", query);
        Assert.Contains("search=Zelt", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListInventoryObjects_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, name = "A" } },
            next = "https://easyverein.com/api/v2.0/inventory-object?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, name = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListInventoryObjectsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task InventoryObject_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListInventoryObjectsAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains(
            "query={id,org,lendingResponsible,inventoryObjectGroups,picture,currentlyLend,lendings,customFields," +
            "locationObject,lastLendDate,lastReturnDate,created_at,updated_at,_deleteAfterDate,_deletedBy," +
            "name,identifier,description,pieces,price,purchaseDate,locationName,lendingAvailable}",
            query);
    }

    [Fact]
    public async Task GetInventoryObject_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetInventoryObjectAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetInventoryObject_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListInventoryObjectsAsync(name: "X", lendingAvailable: true, ordering: "name");
        var item = await client.GetInventoryObjectAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.Equal(999L, item!.Id);
        Assert.EndsWith("/inventory-object/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("lendingAvailable=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateInventoryObject_PostsWritableFields_WithoutReadOnlyFields()
    {
        var createdJson = """{"id":123,"name":"Beamer","org":"https://easyverein.com/api/v2.0/organization/1","picture":"https://easyverein.com/app/image/defaultInventory.png","currentlyLend":0}""";
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateInventoryObjectAsync(
            new InventoryObject { Name = "Beamer", Pieces = 1, LendingAvailable = true });

        Assert.Equal(123L, created.Id);
        Assert.Equal(0, created.CurrentlyLend);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/inventory-object", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("\"name\":\"Beamer\"", handler.LastRequestBody);
        Assert.Contains("\"pieces\":1", handler.LastRequestBody);
        Assert.Contains("\"lendingAvailable\":true", handler.LastRequestBody);
        Assert.DoesNotContain("org", handler.LastRequestBody);
        Assert.DoesNotContain("picture", handler.LastRequestBody);
        Assert.DoesNotContain("currentlyLend", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateInventoryObject_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1}");
        var client = CreateClient(handler);

        await client.CreateInventoryObjectAsync(new InventoryObject { Name = "Y" });

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task UpdateInventoryObject_SendsPatchDictionary()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"id":5,"name":"Neu","pieces":3}""");
        var client = CreateClient(handler);

        var updated = await client.UpdateInventoryObjectAsync(5,
            new Dictionary<string, object> { ["name"] = "Neu", ["pieces"] = 3 });

        Assert.Equal("Neu", updated.Name);
        Assert.Equal(3, updated.Pieces);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/inventory-object/5", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"name\":\"Neu\",\"pieces\":3}", handler.LastRequestBody);
    }

    [Fact]
    public async Task DeleteInventoryObject_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteInventoryObjectAsync(42);

        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.EndsWith("/inventory-object/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListInventoryObjects_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListInventoryObjectsAsync());
    }

    // ------------------------------------------------------------------ //
    // Inventory Object Groups
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task ListInventoryObjectGroups_ReturnsGroups()
    {
        var json = """
            {
                "results": [
                    {"id": 1, "name": "Zelte", "color": "#ff8800", "short": "ZLT", "linkedItems": []},
                    {"id": 2, "name": "Technik", "color": "#0000ff", "short": "TEC"}
                ],
                "next": null
            }
            """;
        var handler = new FakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.ListInventoryObjectGroupsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Zelte", result[0].Name);
        Assert.Equal("#ff8800", result[0].Color);
        Assert.Equal("TEC", result[1].Short);
    }

    [Fact]
    public async Task ListInventoryObjectGroups_SendsFilterParameters()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListInventoryObjectGroupsAsync(
            idIn: "1,2",
            name: "Zelte & Planen",
            color: "#ff8800",
            @short: "ZLT",
            deleted: false,
            ordering: "-name",
            search: new[] { "Zelt" });

        var query = handler.LastRequestUri!.Query;
        Assert.EndsWith("/inventory-object-group", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("id__in=1%2C2", query);
        Assert.Contains("name=Zelte%20%26%20Planen", query);
        Assert.Contains("color=%23ff8800", query);
        Assert.Contains("short=ZLT", query);
        Assert.Contains("deleted=false", query);
        Assert.Contains("ordering=-name", query);
        Assert.Contains("search=Zelt", query);
        Assert.Contains("limit=100", query);
    }

    [Fact]
    public async Task ListInventoryObjectGroups_FollowsPagination()
    {
        var page1 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 1, name = "A" } },
            next = "https://easyverein.com/api/v2.0/inventory-object-group?page=2"
        });
        var page2 = JsonSerializer.Serialize(new
        {
            results = new[] { new { id = 2, name = "B" } },
            next = (string?)null
        });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, page1),
            (HttpStatusCode.OK, page2)
        });
        var client = CreateClient(handler);

        var result = await client.ListInventoryObjectGroupsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task InventoryObjectGroup_QuerySelector_RequestsDocumentedFields()
    {
        var json = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        await client.ListInventoryObjectGroupsAsync();

        var query = Uri.UnescapeDataString(handler.LastRequestUri!.Query);
        Assert.Contains(
            "query={id,org,_deleteAfterDate,_deletedBy,created_at,updated_at,name,color,short,linkedItems}",
            query);
    }

    [Fact]
    public async Task GetInventoryObjectGroup_WithNotFound_ReturnsNull()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "{}");
        var client = CreateClient(handler);

        var result = await client.GetInventoryObjectGroupAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetInventoryObjectGroup_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
    {
        var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
        var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
        var handler = new MultiPageFakeHttpHandler(new[]
        {
            (HttpStatusCode.OK, listJson),
            (HttpStatusCode.OK, getJson)
        });
        var client = CreateClient(handler);

        await client.ListInventoryObjectGroupsAsync(name: "X", @short: "Y", ordering: "name");
        var group = await client.GetInventoryObjectGroupAsync(999);

        var query = handler.LastRequestUri!.Query;
        Assert.Equal(999L, group!.Id);
        Assert.EndsWith("/inventory-object-group/999", handler.LastRequestUri!.AbsolutePath);
        Assert.DoesNotContain("name=", query);
        Assert.DoesNotContain("short=", query);
        Assert.DoesNotContain("ordering=", query);
        Assert.Contains("query=", query);
    }

    [Fact]
    public async Task CreateInventoryObjectGroup_PostsWritableFields_WithoutReadOnlyFields()
    {
        var createdJson = """{"id":123,"name":"Zelte","color":"#ff8800","short":"ZLT","org":"https://easyverein.com/api/v2.0/organization/1","linkedItems":[]}""";
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, createdJson);
        var client = CreateClient(handler);

        var created = await client.CreateInventoryObjectGroupAsync(
            new InventoryObjectGroup { Name = "Zelte", Color = "#ff8800", Short = "ZLT" });

        Assert.Equal(123L, created.Id);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.EndsWith("/inventory-object-group", handler.LastRequestUri!.AbsolutePath);
        Assert.Contains("\"name\":\"Zelte\"", handler.LastRequestBody);
        Assert.Contains("\"short\":\"ZLT\"", handler.LastRequestBody);
        Assert.Contains("\"color\":", handler.LastRequestBody);
        Assert.DoesNotContain("org", handler.LastRequestBody);
        Assert.DoesNotContain("linkedItems", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateInventoryObjectGroup_SendsFixedLengthBody_NotChunked()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.Created, "{\"id\":1}");
        var client = CreateClient(handler);

        await client.CreateInventoryObjectGroupAsync(new InventoryObjectGroup { Name = "Y", Color = "#000000", Short = "Y" });

        Assert.False(handler.LastRequestUsedChunkedEncoding,
            "POST must not use Transfer-Encoding: chunked — easyVerein rejects chunked bodies with HTTP 411.");
        Assert.NotNull(handler.LastRequestContentLength);
        Assert.True(handler.LastRequestContentLength > 0);
    }

    [Fact]
    public async Task UpdateInventoryObjectGroup_SendsPatchDictionary()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, """{"id":5,"name":"Neu","short":"NEU"}""");
        var client = CreateClient(handler);

        var updated = await client.UpdateInventoryObjectGroupAsync(5,
            new Dictionary<string, object> { ["name"] = "Neu", ["short"] = "NEU" });

        Assert.Equal("Neu", updated.Name);
        Assert.Equal("NEU", updated.Short);
        Assert.Equal(HttpMethod.Patch, handler.LastRequestMethod);
        Assert.EndsWith("/inventory-object-group/5", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("{\"name\":\"Neu\",\"short\":\"NEU\"}", handler.LastRequestBody);
    }

    [Fact]
    public async Task DeleteInventoryObjectGroup_SendsDeleteToExpectedPath()
    {
        var handler = new CapturingFakeHttpHandler(HttpStatusCode.NoContent, string.Empty);
        var client = CreateClient(handler);

        await client.DeleteInventoryObjectGroupAsync(42);

        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.EndsWith("/inventory-object-group/42", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ListInventoryObjectGroups_WithUnauthorized_ThrowsUnauthorizedAccessException()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.Unauthorized, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.ListInventoryObjectGroupsAsync());
    }
}

// ------------------------------------------------------------------ //
// Test helpers
// ------------------------------------------------------------------ //

/// <summary>
/// Einfacher Fake-HttpHandler für Unit-Tests (einzelne Antwort).
/// </summary>
public class FakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    public FakeHttpHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}

/// <summary>
/// Fake-HttpHandler der nacheinander mehrere Antworten zurückgibt (für Paginierungs-Tests).
/// </summary>
public class MultiPageFakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<(HttpStatusCode StatusCode, string Content)> _responses;

    /// <summary>Gets the URI of the most recently sent request.</summary>
    public Uri? LastRequestUri { get; private set; }

    public MultiPageFakeHttpHandler(IEnumerable<(HttpStatusCode, string)> responses)
    {
        _responses = new Queue<(HttpStatusCode, string)>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        if (!_responses.TryDequeue(out var entry))
            throw new InvalidOperationException("Keine weiteren Antworten in der Queue.");

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = entry.StatusCode,
            Content = new StringContent(entry.Content, System.Text.Encoding.UTF8, "application/json")
        });
    }
}

/// <summary>
/// Fake-HttpHandler der die zuletzt gesendete Request-URI speichert.
/// </summary>
public class CapturingFakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    /// <summary>Gets the URI of the last captured request.</summary>
    public Uri? LastRequestUri { get; private set; }

    /// <summary>Gets the HTTP method of the last captured request.</summary>
    public HttpMethod? LastRequestMethod { get; private set; }

    /// <summary>Gets the <c>Content-Length</c> of the last captured request body, or <c>null</c> if absent.</summary>
    public long? LastRequestContentLength { get; private set; }

    /// <summary>Gets whether the last captured request sent <c>Transfer-Encoding: chunked</c> instead of a fixed length.</summary>
    public bool LastRequestUsedChunkedEncoding { get; private set; }

    /// <summary>Gets the raw request body of the last captured request as a UTF-8 string, or <c>null</c> if absent.</summary>
    public string? LastRequestBody { get; private set; }

    public CapturingFakeHttpHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        LastRequestMethod = request.Method;
        LastRequestUsedChunkedEncoding =
            request.Headers.TransferEncodingChunked == true ||
            (request.Headers.TransferEncoding?.Any(v => v.Value == "chunked") ?? false);

        if (request.Content != null)
        {
            // Force body materialization so Content-Length is populated on fixed-length content.
            await request.Content.LoadIntoBufferAsync();
            LastRequestContentLength = request.Content.Headers.ContentLength;
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        return new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_content, System.Text.Encoding.UTF8, "application/json")
        };
    }
}
