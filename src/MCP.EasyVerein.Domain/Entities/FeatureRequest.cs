using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an entry of the public easyVerein product idea board (feature request).
/// Only <see cref="Title"/>, <see cref="Description"/> and <see cref="Category"/> are writable;
/// all other fields are maintained by the API.
/// </summary>
public class FeatureRequest : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'. Omitted when writing a new request.</summary>
    [JsonPropertyName(FeatureRequestFields.Id)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long Id { get; set; }

    /// <summary>Gets or sets the public label (e.g. 'C101'; read-only). Maps to API field '<c>label</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Label)]
    public string? Label { get; set; }

    /// <summary>Gets or sets the title (required on create). Maps to API field '<c>title</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Title)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the description (required on create). Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Description)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the vendor's response (read-only). Maps to API field '<c>response</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Response)]
    public string? Response { get; set; }

    /// <summary>Gets or sets the author including the author's organization (read-only). Maps to API field '<c>author</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Author)]
    public FeatureRequestAuthor? Author { get; set; }

    /// <summary>Gets or sets the number of votes in favor (read-only). Maps to API field '<c>proVotesCount</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.ProVotesCount)]
    public int? ProVotesCount { get; set; }

    /// <summary>Gets or sets the number of votes against (read-only). Maps to API field '<c>contraVotesCount</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.ContraVotesCount)]
    public int? ContraVotesCount { get; set; }

    /// <summary>Gets or sets whether the current user has voted (read-only). Maps to API field '<c>hasVoted</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.HasVoted)]
    public bool? HasVoted { get; set; }

    /// <summary>Gets or sets the status code (read-only). Maps to API field '<c>status</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Status)]
    public int? Status { get; set; }

    /// <summary>Gets or sets the approval flag (read-only). Maps to API field '<c>approved</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Approved)]
    public bool? Approved { get; set; }

    /// <summary>Gets or sets the creation date (read-only). Maps to API field '<c>date</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Date)]
    public DateTimeOffset? Date { get; set; }

    /// <summary>Gets or sets the numeric category code. Maps to API field '<c>category</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Category)]
    public int? Category { get; set; }
}
