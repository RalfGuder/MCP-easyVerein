using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an announcement from the easyVerein API.
/// </summary>
public class Announcement : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the HTML text content. Maps to API field '<c>text</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Text)]
    public string? Text { get; set; }

    /// <summary>Gets or sets the start date/time. Maps to API field '<c>start</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Start)]
    public DateTime? Start { get; set; }

    /// <summary>Gets or sets the end date/time. Maps to API field '<c>end</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.End)]
    public DateTime? End { get; set; }

    /// <summary>Gets or sets whether to show the banner. Maps to API field '<c>showBanner</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.ShowBanner)]
    public bool ShowBanner { get; set; }

    /// <summary>Gets or sets whether the announcement is dismissible. Maps to API field '<c>isDismissible</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.IsDismissible)]
    public bool IsDismissible { get; set; }

    /// <summary>Gets or sets whether the announcement is public. Maps to API field '<c>isPublic</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.IsPublic)]
    public bool IsPublic { get; set; }

    /// <summary>Gets or sets whether to show for normal members. Maps to API field '<c>showForNormalMembers</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.ShowForNormalMembers)]
    public bool ShowForNormalMembers { get; set; }

    /// <summary>Gets or sets the platform identifier. Maps to API field '<c>platform</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.Platform)]
    public int? Platform { get; set; }

    /// <summary>Gets or sets the banner level (e.g. success, warning). Maps to API field '<c>bannerLevel</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.BannerLevel)]
    public string? BannerLevel { get; set; }

    /// <summary>Gets or sets the account type visibility. Maps to API field '<c>accountTypeVisibility</c>'.</summary>
    [JsonPropertyName(AnnouncementFields.AccountTypeVisibility)]
    public int? AccountTypeVisibility { get; set; }
}
