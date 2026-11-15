using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="BookingTools"/> MCP tool wrapper.</summary>
public class BookingToolsTests
{
    /// <summary>Versioned base URL used to build resource references in the tests.</summary>
    private const string BaseUrl = "https://easyverein.com/api/v2.0";

    /// <summary>Creates a configuration with API URL <c>https://easyverein.com/api</c> and version <c>v2.0</c>.</summary>
    private static EasyVereinConfiguration CreateConfig() => new()
    {
        ApiKey = "test",
        ApiUrl = "https://easyverein.com/api",
        ApiVersion = "v2.0"
    };

    /// <summary>
    /// Verifies that <c>create_booking</c> assigns bank account, billing account, sphere and
    /// project by patching the freshly created booking with resource URLs.
    /// </summary>
    [Fact]
    public async Task CreateBooking_WithAssignments_PatchesCreatedBooking()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Booking { Id = 5 });
        Dictionary<string, object>? patch = null;
        mock.Setup(c => c.UpdateBookingAsync(5, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, p, _) => patch = (Dictionary<string, object>)p)
            .ReturnsAsync(new Booking { Id = 5 });
        var tools = new BookingTools(mock.Object, CreateConfig());

        var result = await tools.CreateBooking(
            amount: -200m, receiver: "Vereinskonto", description: "Bargeldeinzahlung auf Vereinskonto",
            date: "2026-05-21", bankAccountId: 335646228, billingAccountId: 57780, sphere: 9,
            bookingProjectId: 362887461, ct: CancellationToken.None);

        Assert.DoesNotContain("ERROR", result);
        Assert.NotNull(patch);
        Assert.Equal($"{BaseUrl}/bank-account/335646228", patch!["bankAccount"]);
        Assert.Equal($"{BaseUrl}/billing-account/57780", patch["billingAccount"]);
        Assert.Equal(9L, patch["sphere"]);
        Assert.Equal($"{BaseUrl}/booking-project/362887461", patch["bookingProject"]);
    }

    /// <summary>Verifies that <c>create_booking</c> without assignments sends no follow-up PATCH.</summary>
    [Fact]
    public async Task CreateBooking_WithoutAssignments_DoesNotPatch()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Booking { Id = 5 });
        var tools = new BookingTools(mock.Object, CreateConfig());

        await tools.CreateBooking(
            amount: 10m, receiver: "X", description: null, date: null, bankAccountId: null,
            billingAccountId: null, sphere: null, bookingProjectId: null, ct: CancellationToken.None);

        mock.Verify(c => c.UpdateBookingAsync(It.IsAny<long>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies that a failing follow-up PATCH reports the ID of the already created booking,
    /// so the caller can complete or remove it.
    /// </summary>
    [Fact]
    public async Task CreateBooking_PatchFails_ReportsCreatedBookingId()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Booking { Id = 777 });
        mock.Setup(c => c.UpdateBookingAsync(777, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("boom"));
        var tools = new BookingTools(mock.Object, CreateConfig());

        var result = await tools.CreateBooking(
            amount: 10m, receiver: "X", description: null, date: null, bankAccountId: 1,
            billingAccountId: null, sphere: null, bookingProjectId: null, ct: CancellationToken.None);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("777", result);
    }

    /// <summary>
    /// Verifies that the assignment parameters added later are optional in the generated MCP
    /// tool schema, so clients with an older schema can still call the tools.
    /// </summary>
    [Theory]
    [InlineData(nameof(BookingTools.CreateBooking), "bankAccountId")]
    [InlineData(nameof(BookingTools.CreateBooking), "billingAccountId")]
    [InlineData(nameof(BookingTools.CreateBooking), "sphere")]
    [InlineData(nameof(BookingTools.CreateBooking), "bookingProjectId")]
    [InlineData(nameof(BookingTools.UpdateBooking), "bankAccountId")]
    public void ToolSchema_AssignmentParameter_IsNotRequired(string methodName, string parameter)
    {
        var tools = new BookingTools(new Mock<IEasyVereinApiClient>().Object, CreateConfig());
        var method = typeof(BookingTools).GetMethod(methodName)!;

        var tool = ModelContextProtocol.Server.McpServerTool.Create(method, tools);
        var schema = tool.ProtocolTool.InputSchema;

        Assert.True(schema.GetProperty("properties").TryGetProperty(parameter, out _));
        var required = schema.TryGetProperty("required", out var r)
            ? r.EnumerateArray().Select(e => e.GetString()).ToList()
            : new List<string?>();
        Assert.DoesNotContain(parameter, required);
    }

    /// <summary>Verifies that <c>update_booking</c> sets the bank account as resource URL.</summary>
    [Fact]
    public async Task UpdateBooking_WithBankAccountId_SetsBankAccountUrl()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        Dictionary<string, object>? patch = null;
        mock.Setup(c => c.UpdateBookingAsync(9, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, p, _) => patch = (Dictionary<string, object>)p)
            .ReturnsAsync(new Booking { Id = 9 });
        var tools = new BookingTools(mock.Object, CreateConfig());

        await tools.UpdateBooking(
            id: 9, amount: null, description: null, date: null, receiver: null, bookingProjectId: null,
            billingAccountId: null, sphere: null, bankAccountId: 335646228, ct: CancellationToken.None);

        Assert.NotNull(patch);
        Assert.Equal($"{BaseUrl}/bank-account/335646228", patch!["bankAccount"]);
        Assert.Single(patch);
    }
}
