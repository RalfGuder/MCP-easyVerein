using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class AnnouncementEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "text": "<p>Wichtige Ankündigung</p>",
                "start": "2026-05-01T08:00:00",
                "end": "2026-05-31T23:59:59",
                "showBanner": true,
                "isDismissible": false,
                "isPublic": true,
                "showForNormalMembers": true,
                "platform": 1,
                "bannerLevel": "success",
                "accountTypeVisibility": 0
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var announcement = JsonSerializer.Deserialize<Announcement>(json, options);

        Assert.NotNull(announcement);
        Assert.Equal(42L, announcement.Id);
        Assert.Equal("<p>Wichtige Ankündigung</p>", announcement.Text);
        Assert.Equal(new DateTime(2026, 5, 1, 8, 0, 0), announcement.Start);
        Assert.Equal(new DateTime(2026, 5, 31, 23, 59, 59), announcement.End);
        Assert.True(announcement.ShowBanner);
        Assert.False(announcement.IsDismissible);
        Assert.True(announcement.IsPublic);
        Assert.True(announcement.ShowForNormalMembers);
        Assert.Equal(1, announcement.Platform);
        Assert.Equal("success", announcement.BannerLevel);
        Assert.Equal(0, announcement.AccountTypeVisibility);
    }

    [Fact]
    public void JsonPropertyNames_WithNullOptionalFields_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "text": "Minimal",
                "showBanner": false,
                "isDismissible": true,
                "isPublic": false,
                "showForNormalMembers": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var announcement = JsonSerializer.Deserialize<Announcement>(json, options);

        Assert.NotNull(announcement);
        Assert.Equal(99L, announcement.Id);
        Assert.Equal("Minimal", announcement.Text);
        Assert.Null(announcement.Start);
        Assert.Null(announcement.End);
        Assert.False(announcement.ShowBanner);
        Assert.True(announcement.IsDismissible);
        Assert.Null(announcement.Platform);
        Assert.Null(announcement.BannerLevel);
        Assert.Null(announcement.AccountTypeVisibility);
    }
}
