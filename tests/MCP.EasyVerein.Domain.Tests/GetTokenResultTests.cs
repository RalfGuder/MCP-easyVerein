using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class GetTokenResultTests
{
    [Fact]
    public void JsonPropertyNames_WithToken_AreCorrect()
    {
        var json = """{"token": "abc123def456"}""";

        var result = JsonSerializer.Deserialize<GetTokenResult>(json);

        Assert.NotNull(result);
        Assert.Equal("abc123def456", result.Token);
        Assert.Null(result.Needs2FA);
    }

    [Fact]
    public void JsonPropertyNames_WithTwoFactorChallenge_AreCorrect()
    {
        var json = """{"needs2FA": true}""";

        var result = JsonSerializer.Deserialize<GetTokenResult>(json);

        Assert.NotNull(result);
        Assert.True(result.Needs2FA);
        Assert.Null(result.Token);
    }
}
