using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a forum of the organization's member forum in the easyVerein API.
/// </summary>
public class Forum : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(ForumFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the owning organization URL reference (read-only). Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(ForumFields.Org)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the ID of the latest post (read-only). Maps to API field '<c>last_post</c>'.</summary>
    [JsonPropertyName(ForumFields.LastPost)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? LastPostId { get; set; }

    /// <summary>Gets or sets the creation timestamp (read-only). Maps to API field '<c>created</c>'.</summary>
    [JsonPropertyName(ForumFields.Created)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Created { get; set; }

    /// <summary>Gets or sets the last-update timestamp (read-only). Maps to API field '<c>updated</c>'.</summary>
    [JsonPropertyName(ForumFields.Updated)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Updated { get; set; }

    /// <summary>Gets or sets the forum name (required, max 100 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(ForumFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the URL slug derived from the name (read-only). Maps to API field '<c>slug</c>'.</summary>
    [JsonPropertyName(ForumFields.Slug)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Slug { get; set; }

    /// <summary>Gets or sets the forum description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(ForumFields.Description)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the forum image URL (read-only in this client; upload needs multipart). Maps to API field '<c>image</c>'.</summary>
    [JsonPropertyName(ForumFields.Image)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Image { get; set; }

    /// <summary>Gets or sets an external forum link (max 200 chars). Maps to API field '<c>link</c>'.</summary>
    [JsonPropertyName(ForumFields.Link)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Link { get; set; }

    /// <summary>Gets or sets whether followed forum links are counted. Maps to API field '<c>link_redirects</c>'.</summary>
    [JsonPropertyName(ForumFields.LinkRedirects)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LinkRedirects { get; set; }

    /// <summary>Gets or sets the forum kind (read-only; 0 = regular forum). Maps to API field '<c>type</c>'.</summary>
    [JsonPropertyName(ForumFields.Type)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Type { get; set; }

    /// <summary>Gets or sets the number of posts directly in this forum (read-only). Maps to API field '<c>direct_posts_count</c>'.</summary>
    [JsonPropertyName(ForumFields.DirectPostsCount)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DirectPostsCount { get; set; }

    /// <summary>Gets or sets the number of topics directly in this forum (read-only). Maps to API field '<c>direct_topics_count</c>'.</summary>
    [JsonPropertyName(ForumFields.DirectTopicsCount)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DirectTopicsCount { get; set; }

    /// <summary>Gets or sets the number of followed forum links (read-only). Maps to API field '<c>link_redirects_count</c>'.</summary>
    [JsonPropertyName(ForumFields.LinkRedirectsCount)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LinkRedirectsCount { get; set; }

    /// <summary>Gets or sets the sort position. Maps to API field '<c>order</c>'.</summary>
    [JsonPropertyName(ForumFields.Order)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Order { get; set; }

    /// <summary>Gets or sets the timestamp of the latest post (read-only). Maps to API field '<c>last_post_on</c>'.</summary>
    [JsonPropertyName(ForumFields.LastPostOn)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? LastPostOn { get; set; }

    /// <summary>Gets or sets whether the forum is shown in its parent's sub-forum list. Maps to API field '<c>display_sub_forum_list</c>'.</summary>
    [JsonPropertyName(ForumFields.DisplaySubForumList)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? DisplaySubForumList { get; set; }
}
