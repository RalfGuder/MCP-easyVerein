using System.Reflection;
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using MCP.EasyVerein.Server.Tools;
using ModelContextProtocol.Server;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Tests for the paging parameters and compact output of the <c>list_*</c> tools (US-0064).</summary>
public class ListToolPagingTests
{
    /// <summary>Creates a v2.0 configuration for tools that need one.</summary>
    /// <returns>The configuration.</returns>
    private static EasyVereinConfiguration CreateConfig() => new()
    {
        ApiKey = "test",
        ApiUrl = "https://easyverein.com/api",
        ApiVersion = "v2.0"
    };

    /// <summary>Gets all list tool methods of the server assembly.</summary>
    /// <returns>The list tool methods.</returns>
    private static List<MethodInfo> ListToolMethods() =>
        typeof(ToolJson).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<McpServerToolTypeAttribute>() is not null)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() is not null
                        && m.Name.StartsWith("List", StringComparison.Ordinal))
            .ToList();

    /// <summary>Verifies that every list tool has optional <c>limit</c> and <c>page</c> parameters with defaults 50 and 1.</summary>
    [Fact]
    public void AllListTools_HaveOptionalLimitAndPage()
    {
        var methods = ListToolMethods();

        Assert.Equal(25, methods.Count);
        Assert.All(methods, method =>
        {
            var parameters = method.GetParameters();
            var limit = parameters.Single(p => p.Name == "limit");
            var page = parameters.Single(p => p.Name == "page");
            Assert.Equal(PageRequest.DefaultLimit, limit.DefaultValue);
            Assert.Equal(PageRequest.FirstPage, page.DefaultValue);
            Assert.True(parameters[^1].HasDefaultValue, $"{method.Name}: ct must be optional");
        });
    }

    /// <summary>Verifies that a list tool forwards the requested page and writes the paging envelope.</summary>
    [Fact]
    public async Task ListBankAccounts_ForwardsPageAndWritesEnvelope()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        PageRequest? received = null;
        mock.Setup(c => c.ListBankAccountsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string[]?>(), It.IsAny<PageRequest?>(), It.IsAny<CancellationToken>()))
            .Callback<string?, string?, string?, string?, string?, string?, string?, string[]?, PageRequest?, CancellationToken>(
                (_, _, _, _, _, _, _, _, p, _) => received = p)
            .ReturnsAsync(new PagedResult<BankAccount>(new[] { new BankAccount { Id = 1 } }, 45, true, 3, 20));
        var tools = new BankAccountTools(mock.Object);

        var result = await tools.ListBankAccounts(null, null, null, null, null, null, null, null, limit: 20, page: 3);

        Assert.Equal(new PageRequest(20, 3), received);
        Assert.Equal("{\"items\":[{\"id\":1}],\"totalCount\":45,\"hasMore\":true,\"page\":3,\"limit\":20}", result);
    }

    /// <summary>Verifies that an invalid limit is rejected with a readable message and no API call.</summary>
    [Theory]
    [InlineData(0, 1, "limit muss zwischen 1 und 100")]
    [InlineData(101, 1, "limit muss zwischen 1 und 100")]
    [InlineData(50, 0, "page muss mindestens 1")]
    public async Task ListBankAccounts_InvalidPaging_ReturnsReadableError(int limit, int page, string expected)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new BankAccountTools(mock.Object);

        var result = await tools.ListBankAccounts(null, null, null, null, null, null, null, null, limit: limit, page: page);

        Assert.StartsWith("ERROR: ArgumentOutOfRangeException", result);
        Assert.Contains(expected, result);
    }

    /// <summary>Verifies that <c>list_bookings</c> writes referenced accounts and invoices as IDs.</summary>
    [Fact]
    public async Task ListBookings_WritesReferencesAsIds()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        var booking = new Booking
        {
            Id = 5,
            BankAccount = new BankAccount { Id = 361879047 },
            BillingAccount = new BillingAccount { Id = 58770 },
            RelatedInvoice = new List<Invoice> { new() { Id = 607989039 } }
        };
        mock.Setup(c => c.ListBookingsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string[]?>(), It.IsAny<PageRequest?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Booking>(new[] { booking }, 1, false, 1, 50));
        var tools = new BookingTools(mock.Object, CreateConfig());

        var result = await tools.ListBookings(null, null, null, null, null, null);

        Assert.Contains("\"bankAccount\":361879047", result);
        Assert.Contains("\"billingAccount\":58770", result);
        Assert.Contains("\"relatedInvoice\":[607989039]", result);
        Assert.DoesNotContain("null", result);
    }
}
