using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Server.Tools;
using Moq;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Unit tests for the <see cref="GetTokenTools"/> MCP tool wrapper.</summary>
public class GetTokenToolsTests
{
    /// <summary>
    /// Verifies that a successful login returns only a masked token, never the full token.
    /// </summary>
    [Fact]
    public async Task GetToken_OnSuccess_ReturnsMaskedTokenOnly()
    {
        const string token = "abcd1234567890wxyz";
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetTokenAsync("kv_user@example.org", "s3cret", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResult { Token = token });

        var tools = new GetTokenTools(mock.Object);

        var result = await tools.GetToken("kv_user@example.org", "s3cret", null, CancellationToken.None);

        Assert.DoesNotContain(token, result);
        Assert.Contains("abcd…wxyz", result);
        Assert.Contains("18", result);
    }

    /// <summary>
    /// Verifies that a short token is masked completely.
    /// </summary>
    [Fact]
    public async Task GetToken_WithShortToken_MasksCompletely()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResult { Token = "short123" });

        var tools = new GetTokenTools(mock.Object);

        var result = await tools.GetToken("kv_user", "pw", null, CancellationToken.None);

        Assert.DoesNotContain("short", result);
        Assert.DoesNotContain("123", result);
    }

    /// <summary>
    /// Verifies that a two-factor challenge is reported and the provided code is forwarded.
    /// </summary>
    [Fact]
    public async Task GetToken_WithTwoFactorChallenge_ReportsThatCodeIsRequired()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetTokenAsync("kv_user", "pw", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResult { Needs2FA = true });

        var tools = new GetTokenTools(mock.Object);

        var result = await tools.GetToken("kv_user", "pw", "null", CancellationToken.None);

        Assert.Contains("two-factor", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ERROR", result);
    }

    /// <summary>
    /// Verifies that missing credentials are rejected before calling the API.
    /// </summary>
    [Theory]
    [InlineData("", "pw")]
    [InlineData("null", "pw")]
    [InlineData("kv_user", "")]
    [InlineData("kv_user", "   ")]
    public async Task GetToken_WithMissingCredentials_ReturnsErrorWithoutCallingClient(string username, string password)
    {
        var mock = new Mock<IEasyVereinApiClient>(MockBehavior.Strict);
        var tools = new GetTokenTools(mock.Object);

        var result = await tools.GetToken(username, password, null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }

    /// <summary>
    /// Verifies that the password is redacted from error messages, even if an exception echoes it.
    /// </summary>
    [Fact]
    public async Task GetToken_OnException_RedactsPassword()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("HTTP 400: invalid password 'Geheim!42'",
                new InvalidOperationException("inner Geheim!42")));

        var tools = new GetTokenTools(mock.Object);

        var result = await tools.GetToken("kv_user", "Geheim!42", null, CancellationToken.None);

        Assert.StartsWith("ERROR: HttpRequestException", result);
        Assert.DoesNotContain("Geheim!42", result);
    }

    /// <summary>
    /// Verifies that an empty API answer (neither token nor challenge) is reported as an error.
    /// </summary>
    [Fact]
    public async Task GetToken_WithoutTokenAndChallenge_ReturnsError()
    {
        var mock = new Mock<IEasyVereinApiClient>();
        mock.Setup(c => c.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResult());

        var tools = new GetTokenTools(mock.Object);

        var result = await tools.GetToken("kv_user", "pw", null, CancellationToken.None);

        Assert.StartsWith("ERROR:", result);
    }
}
