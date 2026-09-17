using System.ComponentModel;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tool for logging in to easyVerein with user credentials. The issued token is a secret and is
/// only returned masked; the password is redacted from every message.
/// </summary>
[McpServerToolType]
public sealed class GetTokenTools(IEasyVereinApiClient client)
{
    /// <summary>Number of leading and trailing token characters that stay visible in the masked form.</summary>
    private const int VisibleChars = 4;

    /// <summary>Minimum token length for which leading and trailing characters are shown.</summary>
    private const int MinLengthForPartialMask = 3 * VisibleChars + 1;

    /// <summary>Placeholder that replaces the password in error messages.</summary>
    private const string Redacted = "***";

    /// <summary>Logs in with user credentials and reports whether a token was issued (masked).</summary>
    [McpServerTool(Name = "get_token"), Description(
        "SENSITIVE: Log in to easyVerein with user credentials and check whether an API token is issued. " +
        "The password passes through this conversation — only call this tool when the user explicitly provides " +
        "credentials for this purpose. The token is returned masked only.")]
    public async Task<string> GetToken(
        [Description("Login name, built as '<orgShort>_<email or username>' (e.g. 'abc_some@example.de')")] string username,
        [Description("The user's password")] string password,
        [Description("Two-factor code (only if the account has 2FA enabled)")] string? twoFactorCode,
        CancellationToken ct)
    {
        if (!HasValue(username))
            return "ERROR: The username is required.";
        if (!HasValue(password) || string.IsNullOrWhiteSpace(password))
            return "ERROR: The password is required.";

        try
        {
            var code = HasValue(twoFactorCode) ? twoFactorCode : null;
            var result = await client.GetTokenAsync(username, password, code, ct);

            if (!string.IsNullOrEmpty(result.Token))
                return $"Login successful. Token (masked): {Mask(result.Token)} (length {result.Token.Length}). " +
                       "The full token is not returned for security reasons.";

            if (result.Needs2FA == true)
                return "A two-factor code is required. Call get_token again with twoFactorCode.";

            return "ERROR: The API returned neither a token nor a two-factor challenge.";
        }
        catch (Exception ex)
        {
            var message = $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
            return message.Replace(password, Redacted, StringComparison.Ordinal);
        }
    }

    /// <summary>Masks a token, keeping only a few leading and trailing characters of long tokens.</summary>
    private static string Mask(string token) =>
        token.Length >= MinLengthForPartialMask
            ? $"{token[..VisibleChars]}…{token[^VisibleChars..]}"
            : new string('*', token.Length);

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
