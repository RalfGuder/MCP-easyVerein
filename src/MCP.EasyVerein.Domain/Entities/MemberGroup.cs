using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a member group reference from the easyVerein API.
/// </summary>
public class MemberGroup : IHasId
{
    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
    /// </summary>
    [JsonPropertyName("id")] public long Id { get; set; }
}
