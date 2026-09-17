using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents the answer of the easyVerein login endpoint (<c>get-token</c>): either an issued API token
/// or a challenge requesting a two-factor code. The token is a secret and must not be exposed unmasked.
/// </summary>
public class GetTokenResult
{
    /// <summary>Gets or sets the issued API token (secret). Maps to API field '<c>token</c>'.</summary>
    [JsonPropertyName(GetTokenFields.Token)]
    public string? Token { get; set; }

    /// <summary>Gets or sets whether a two-factor code is required to complete the login. Maps to API field '<c>needs2FA</c>'.</summary>
    [JsonPropertyName(GetTokenFields.Needs2FA)]
    public bool? Needs2FA { get; set; }
}
