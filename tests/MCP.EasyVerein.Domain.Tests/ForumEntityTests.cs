using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class ForumEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 31875,
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "last_post": "https://easyverein.com/api/v2.0/forum-post/777",
                "created": "2025-11-01T17:23:34.957885+01:00",
                "updated": "2025-11-01T17:24:04.371549+01:00",
                "name": "Kulturverein Milower Land e.V.",
                "slug": "kulturverein-milower-land-ev",
                "description": "Forum für alle Mitglieder des Kulturvereins",
                "image": "https://easyverein.com/app/file?path=forum.png",
                "link": "https://example.org",
                "link_redirects": true,
                "type": 0,
                "direct_posts_count": 4,
                "direct_topics_count": 2,
                "link_redirects_count": 3,
                "order": 5,
                "last_post_on": "2025-11-02T10:00:00+01:00",
                "display_sub_forum_list": true
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var forum = JsonSerializer.Deserialize<Forum>(json, options);

        Assert.NotNull(forum);
        Assert.Equal(31875L, forum.Id);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", forum.Org);
        Assert.Equal(777L, forum.LastPostId);
        Assert.NotNull(forum.Created);
        Assert.NotNull(forum.Updated);
        Assert.Equal("Kulturverein Milower Land e.V.", forum.Name);
        Assert.Equal("kulturverein-milower-land-ev", forum.Slug);
        Assert.Equal("Forum für alle Mitglieder des Kulturvereins", forum.Description);
        Assert.Equal("https://easyverein.com/app/file?path=forum.png", forum.Image);
        Assert.Equal("https://example.org", forum.Link);
        Assert.True(forum.LinkRedirects);
        Assert.Equal(0, forum.Type);
        Assert.Equal(4, forum.DirectPostsCount);
        Assert.Equal(2, forum.DirectTopicsCount);
        Assert.Equal(3, forum.LinkRedirectsCount);
        Assert.Equal(5, forum.Order);
        Assert.NotNull(forum.LastPostOn);
        Assert.True(forum.DisplaySubForumList);
    }

    [Fact]
    public void JsonPropertyNames_WithNullReferences_AreCorrect()
    {
        var json = """
            {
                "id": 1,
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "last_post": null,
                "name": "Leer",
                "image": null,
                "link": null,
                "last_post_on": null
            }
            """;

        var forum = JsonSerializer.Deserialize<Forum>(json);

        Assert.NotNull(forum);
        Assert.Null(forum.LastPostId);
        Assert.Null(forum.Image);
        Assert.Null(forum.Link);
        Assert.Null(forum.LastPostOn);
    }

    [Fact]
    public void Serialize_ForCreate_OmitsReadOnlyFields()
    {
        var forum = new Forum { Name = "Vorstand", Description = "Interne Abstimmung" };

        var json = JsonSerializer.Serialize(forum);

        Assert.Contains("\"name\":\"Vorstand\"", json);
        Assert.Contains("\"description\":\"Interne Abstimmung\"", json);
        Assert.DoesNotContain("org", json);
        Assert.DoesNotContain("slug", json);
        Assert.DoesNotContain("last_post", json);
        Assert.DoesNotContain("created", json);
        Assert.DoesNotContain("updated", json);
        Assert.DoesNotContain("\"type\"", json);
        Assert.DoesNotContain("count", json);
        Assert.DoesNotContain("image", json);
    }
}
