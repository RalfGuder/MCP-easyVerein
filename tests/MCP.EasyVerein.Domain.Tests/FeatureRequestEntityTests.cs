using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class FeatureRequestEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 1,
                "label": "C101",
                "title": "einen DATEV-Export zu integrieren",
                "description": "Buchhaltungsdaten im DATEV-Format exportieren",
                "response": "Integriert in V1.9.4",
                "author": {"id": 109, "org": {"id": 23, "short": "admin", "name": "easyVerein Verwaltung"}},
                "proVotesCount": 26,
                "contraVotesCount": 4,
                "hasVoted": false,
                "status": 3,
                "approved": true,
                "date": "2026-09-16T14:47:41.791281+02:00",
                "category": 10
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var request = JsonSerializer.Deserialize<FeatureRequest>(json, options);

        Assert.NotNull(request);
        Assert.Equal(1L, request.Id);
        Assert.Equal("C101", request.Label);
        Assert.Equal("einen DATEV-Export zu integrieren", request.Title);
        Assert.Equal("Buchhaltungsdaten im DATEV-Format exportieren", request.Description);
        Assert.Equal("Integriert in V1.9.4", request.Response);
        Assert.NotNull(request.Author);
        Assert.Equal(109L, request.Author!.Id);
        Assert.Equal(23L, request.Author.Org!.Id);
        Assert.Equal("admin", request.Author.Org.Short);
        Assert.Equal("easyVerein Verwaltung", request.Author.Org.Name);
        Assert.Equal(26, request.ProVotesCount);
        Assert.Equal(4, request.ContraVotesCount);
        Assert.False(request.HasVoted);
        Assert.Equal(3, request.Status);
        Assert.True(request.Approved);
        Assert.NotNull(request.Date);
        Assert.Equal(10, request.Category);
    }

    [Fact]
    public void Deserialize_WithUnknownAuthor_MapsNonNumericIdsToNull()
    {
        // Live API (2026-09-16): 163 of 1965 entries carry an anonymized author with string ids.
        var json = """
            {
                "id": 26,
                "title": "Anonymer Wunsch",
                "author": {"id": "Unbekannt", "org": {"id": "", "short": "", "name": "Unbekannt"}}
            }
            """;

        var request = JsonSerializer.Deserialize<FeatureRequest>(json);

        Assert.NotNull(request);
        Assert.NotNull(request.Author);
        Assert.Null(request.Author!.Id);
        Assert.Null(request.Author.Org!.Id);
        Assert.Equal("Unbekannt", request.Author.Org.Name);
    }

    [Fact]
    public void JsonPropertyNames_WithNullDateAndMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 4,
                "title": "Webseitenintegration",
                "date": null
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var request = JsonSerializer.Deserialize<FeatureRequest>(json, options);

        Assert.NotNull(request);
        Assert.Equal(4L, request.Id);
        Assert.Null(request.Date);
        Assert.Null(request.Author);
        Assert.Null(request.Category);
        Assert.Null(request.Status);
    }
}
