using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class ContactDetailsLogEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 11,
                "creator": "https://easyverein.com/api/v2.0/member/500",
                "creatorName": "Max Mustermann",
                "kind": "Custom",
                "relatedAddress": "https://easyverein.com/api/v2.0/contact-details/345175845",
                "relatedFile": "2026/05/log.pdf",
                "date": "2026-05-31T16:51:21",
                "description": "Adresse aktualisiert",
                "shared": true
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var log = JsonSerializer.Deserialize<ContactDetailsLog>(json, options);

        Assert.NotNull(log);
        Assert.Equal(11L, log.Id);
        Assert.Equal(500L, log.Creator);
        Assert.Equal("Max Mustermann", log.CreatorName);
        Assert.Equal("Custom", log.Kind);
        Assert.Equal(345175845L, log.RelatedAddress);
        Assert.Equal("2026/05/log.pdf", log.RelatedFile);
        Assert.Equal(new DateTime(2026, 5, 31, 16, 51, 21), log.Date);
        Assert.Equal("Adresse aktualisiert", log.Description);
        Assert.True(log.Shared);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "kind": "Membership"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var log = JsonSerializer.Deserialize<ContactDetailsLog>(json, options);

        Assert.NotNull(log);
        Assert.Equal(42L, log.Id);
        Assert.Equal("Membership", log.Kind);
        Assert.Null(log.Creator);
        Assert.Null(log.CreatorName);
        Assert.Null(log.RelatedAddress);
        Assert.Null(log.RelatedFile);
        Assert.Null(log.Date);
        Assert.Null(log.Description);
        Assert.Null(log.Shared);
    }
}
