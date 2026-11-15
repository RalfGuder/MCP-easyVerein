using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Tests for the receipt date set by <c>create_receipt</c> (US-0065).</summary>
public class CreateReceiptDateTests
{
    /// <summary>The ID of the booking used in the tests.</summary>
    private const long BookingId = 247581501;

    /// <summary>The ID of the draft invoice returned by the mock.</summary>
    private const long InvoiceId = 900;

    /// <summary>Creates a v2.0 configuration.</summary>
    /// <returns>The configuration.</returns>
    private static EasyVereinConfiguration CreateConfig() => new()
    {
        ApiKey = "test",
        ApiUrl = "https://easyverein.com/api",
        ApiVersion = "v2.0"
    };

    /// <summary>Creates a mock client that returns the booking and records the finalizing patch.</summary>
    /// <param name="booking">The booking returned by <c>GetBookingAsync</c>.</param>
    /// <param name="patch">Receives the patch sent to <c>UpdateInvoiceAsync</c>.</param>
    /// <returns>The configured mock.</returns>
    private static Mock<IEasyVereinApiClient> CreateMock(Booking booking, Action<Dictionary<string, object>> patch)
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetBookingAsync(BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        mock.Setup(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = InvoiceId, IsDraft = true });
        mock.Setup(c => c.CreateInvoiceItemAsync(It.IsAny<InvoiceItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvoiceItem { Id = 901 });
        mock.Setup(c => c.UpdateInvoiceAsync(InvoiceId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<long, object, CancellationToken>((_, p, _) => patch((Dictionary<string, object>)p))
            .ReturnsAsync(new Invoice { Id = InvoiceId, IsDraft = false });
        return mock;
    }

    /// <summary>Creates a booking with the given date.</summary>
    /// <param name="date">The booking date.</param>
    /// <returns>The booking.</returns>
    private static Booking BookingOn(DateTime? date) => new()
    {
        Id = BookingId,
        Amount = 60m,
        Date = date,
        Receiver = "Rudolf Oehme Anneliese Oehme",
        Description = "Beitrag Oehme"
    };

    /// <summary>Runs <c>create_receipt</c> for the test booking.</summary>
    /// <param name="mock">The mock client.</param>
    /// <param name="date">The optional date argument.</param>
    /// <returns>The tool result.</returns>
    private static Task<string> Run(Mock<IEasyVereinApiClient> mock, string? date) =>
        new InvoiceTools(mock.Object, CreateConfig()).CreateReceipt(
            BookingId, billingAccountId: 58228, sphere: 1, costCentre: "1901",
            title: null, description: null, receiver: null, date: date, ct: CancellationToken.None);

    /// <summary>Verifies that without a date argument the booking date becomes the receipt date.</summary>
    [Fact]
    public async Task CreateReceipt_WithoutDate_UsesBookingDate()
    {
        Dictionary<string, object>? patch = null;
        var mock = CreateMock(BookingOn(new DateTime(2026, 10, 2)), p => patch = p);

        await Run(mock, null);

        Assert.Equal("2026-10-02", patch!["date"]);
    }

    /// <summary>
    /// Verifies that a booking date read as an instant (midnight in Berlin = 22:00 UTC the day before)
    /// still yields the Berlin calendar day.
    /// </summary>
    [Fact]
    public async Task CreateReceipt_BookingDateAsUtcInstant_UsesBerlinCalendarDay()
    {
        Dictionary<string, object>? patch = null;
        var instant = DateTimeOffset.Parse("2026-10-02T00:00:00+02:00").UtcDateTime;
        var mock = CreateMock(BookingOn(instant), p => patch = p);

        await Run(mock, null);

        Assert.Equal("2026-10-02", patch!["date"]);
    }

    /// <summary>Verifies that the date argument overrides the booking date.</summary>
    [Fact]
    public async Task CreateReceipt_WithDate_OverridesBookingDate()
    {
        Dictionary<string, object>? patch = null;
        var mock = CreateMock(BookingOn(new DateTime(2026, 10, 2)), p => patch = p);

        await Run(mock, "2026-07-09");

        Assert.Equal("2026-07-09", patch!["date"]);
    }

    /// <summary>Verifies that an invalid date is rejected before anything is created.</summary>
    [Theory]
    [InlineData("09.07.2026")]
    [InlineData("2026-13-01")]
    [InlineData("morgen")]
    public async Task CreateReceipt_InvalidDate_ReturnsErrorWithoutCreating(string date)
    {
        var mock = CreateMock(BookingOn(new DateTime(2026, 10, 2)), _ => { });

        var result = await Run(mock, date);

        Assert.StartsWith("ERROR", result);
        Assert.Contains("yyyy-MM-dd", result);
        mock.Verify(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies that a booking without date and no date argument is rejected before anything is created.</summary>
    [Fact]
    public async Task CreateReceipt_NoDateAnywhere_ReturnsErrorWithoutCreating()
    {
        var mock = CreateMock(BookingOn(null), _ => { });

        var result = await Run(mock, null);

        Assert.StartsWith("ERROR", result);
        mock.Verify(c => c.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
