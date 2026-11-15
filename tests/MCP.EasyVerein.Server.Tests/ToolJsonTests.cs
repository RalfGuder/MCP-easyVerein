using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.ValueObjects;
using MCP.EasyVerein.Server.Tools;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the central tool output serialization <see cref="ToolJson"/> (US-0064).</summary>
public class ToolJsonTests
{
    /// <summary>Verifies that the shared options are one reused instance.</summary>
    [Fact]
    public void Options_IsSingleSharedInstance()
    {
        Assert.Same(ToolJson.Options, ToolJson.Options);
    }

    /// <summary>Verifies that output is compact and omits null fields.</summary>
    [Fact]
    public void Serialize_IsCompactAndOmitsNull()
    {
        var json = ToolJson.Serialize(new BankAccount { Id = 7, Name = "Handkasse" });

        Assert.DoesNotContain("\n", json);
        Assert.DoesNotContain("null", json);
        Assert.Contains("\"id\":7", json);
    }

    /// <summary>Verifies that references to other resources are written as their ID only.</summary>
    [Fact]
    public void Serialize_WritesReferencesAsIdOnly()
    {
        var booking = new Booking
        {
            Id = 1,
            BankAccount = new BankAccount { Id = 361879047, Name = "Vereinskonto" },
            BillingAccount = new BillingAccount { Id = 58770 },
            RelatedInvoice = new List<Invoice> { new() { Id = 607989039 }, new() { Id = 42 } }
        };

        var json = ToolJson.Serialize(booking);

        Assert.Contains("\"bankAccount\":361879047", json);
        Assert.Contains("\"billingAccount\":58770", json);
        Assert.Contains("\"relatedInvoice\":[607989039,42]", json);
        Assert.DoesNotContain("Vereinskonto", json);
    }

    /// <summary>Verifies that the member's contact details stay embedded because they carry the member's data.</summary>
    [Fact]
    public void Serialize_KeepsMemberContactDetailsEmbedded()
    {
        var member = new Member { Id = 3, ContactDetails = new ContactDetails { Id = 9, FirstName = "Heidrun" } };

        var json = ToolJson.Serialize(member);

        Assert.Contains("\"firstName\":\"Heidrun\"", json);
    }

    /// <summary>Verifies that a page is written as an envelope with paging metadata.</summary>
    [Fact]
    public void SerializePage_WritesEnvelopeWithMetadata()
    {
        var page = new PagedResult<BankAccount>(new[] { new BankAccount { Id = 1 } }, 95, true, 2, 50);

        var json = ToolJson.SerializePage(page);

        Assert.Equal("{\"items\":[{\"id\":1}],\"totalCount\":95,\"hasMore\":true,\"page\":2,\"limit\":50}", json);
    }

    /// <summary>Verifies that an unknown total count is left out of the envelope.</summary>
    [Fact]
    public void SerializePage_UnknownTotal_OmitsTotalCount()
    {
        var page = new PagedResult<BankAccount>(new[] { new BankAccount { Id = 1 } }, null, true, 1, 1);

        var json = ToolJson.SerializePage(page);

        Assert.Equal("{\"items\":[{\"id\":1}],\"hasMore\":true,\"page\":1,\"limit\":1}", json);
    }

    /// <summary>Verifies that valid paging arguments become a page request.</summary>
    [Fact]
    public void ToPageRequest_ValidArguments_ReturnsRequest()
    {
        var request = ToolJson.ToPageRequest(20, 3);

        Assert.Equal(20, request.Limit);
        Assert.Equal(3, request.Page);
    }
}
