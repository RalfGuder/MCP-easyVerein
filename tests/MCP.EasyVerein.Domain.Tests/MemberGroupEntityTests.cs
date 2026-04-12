using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberGroupEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 335646249
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var group = JsonSerializer.Deserialize<MemberGroup>(json, options);

        Assert.NotNull(group);
        Assert.Equal(335646249L, group.Id);
    }
}
