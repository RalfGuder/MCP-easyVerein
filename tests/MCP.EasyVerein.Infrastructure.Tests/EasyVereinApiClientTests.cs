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

    public MultiPageFakeHttpHandler(IEnumerable<(HttpStatusCode, string)> responses)
    {
        _responses = new Queue<(HttpStatusCode, string)>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
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
