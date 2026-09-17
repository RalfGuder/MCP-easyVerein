namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Get-Token (login) API field names.</summary>
internal static class GetTokenFields
{
    /// <summary>API field name for the login name, built as <c>$orgShort_$emailOrUsername</c> (required).</summary>
    internal const string Username = "username";

    /// <summary>API field name for the user's password (required).</summary>
    internal const string Password = "password";

    /// <summary>API field name for the two-factor authentication code (only for users with 2FA enabled).</summary>
    internal const string TwoFactor = "2FA";

    /// <summary>API field name for the issued API token (response only).</summary>
    internal const string Token = "token";

    /// <summary>API field name for the flag signalling that a two-factor code is required (response only).</summary>
    internal const string Needs2FA = "needs2FA";
}
