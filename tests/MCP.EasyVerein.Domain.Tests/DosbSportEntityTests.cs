using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class DosbSportEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 55,
                "title": "Fußball",
                "sportNumber": "042",
                "federationNumber": "07",
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "created_at": "2026-09-16T17:00:00.000000+02:00",
                "updated_at": "2026-09-16T17:05:00.000000+02:00"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var sport = JsonSerializer.Deserialize<DosbSport>(json, options);

        Assert.NotNull(sport);
        Assert.Equal(55L, sport.Id);
        Assert.Equal("Fußball", sport.Title);
        Assert.Equal("042", sport.SportNumber);
        Assert.Equal("07", sport.FederationNumber);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", sport.Org);
        Assert.NotNull(sport.CreatedAt);
        Assert.NotNull(sport.UpdatedAt);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 7,
                "title": "Tischtennis"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var sport = JsonSerializer.Deserialize<DosbSport>(json, options);

        Assert.NotNull(sport);
        Assert.Equal(7L, sport.Id);
        Assert.Equal("Tischtennis", sport.Title);
        Assert.Null(sport.SportNumber);
        Assert.Null(sport.FederationNumber);
        Assert.Null(sport.Org);
        Assert.Null(sport.CreatedAt);
    }
}
