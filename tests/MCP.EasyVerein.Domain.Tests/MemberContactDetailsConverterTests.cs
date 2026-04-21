using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberContactDetailsConverterTests
{
    private sealed class Wrapper
    {
        [JsonConverter(typeof(MemberContactDetailsConverter))]
        public ContactDetails? Value { get; set; }
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":null}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_UrlString_PopulatesOnlyId()
    {
        const string json = "{\"Value\":\"https://easyverein.com/api/v2.0/contact-details/335684097\"}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(335684097L, w.Value!.Id);
        Assert.Equal(string.Empty, w.Value.FamilyName);
        Assert.Equal(string.Empty, w.Value.FirstName);
    }

    [Fact]
    public void Read_EmbeddedObject_PopulatesFullEntity()
    {
        const string json = "{\"Value\":{\"id\":42,\"familyName\":\"Rose\",\"firstName\":\"Kathleen\"}}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(42L, w.Value!.Id);
        Assert.Equal("Rose", w.Value.FamilyName);
        Assert.Equal("Kathleen", w.Value.FirstName);
    }

    [Fact]
    public void Read_MalformedUrl_ThrowsJsonException()
    {
        const string json = "{\"Value\":\"https://example.com/no-id-here/\"}";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Wrapper>(json));
    }

    [Fact]
    public void Read_NumberToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Wrapper>("{\"Value\":123}"));
    }

    [Fact]
    public void Write_NullValue_WritesJsonNull()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = null });
        Assert.Equal("{\"Value\":null}", json);
    }

    [Fact]
    public void Write_EntityValue_WritesEmbeddedObject()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = new ContactDetails { Id = 7 } });
        Assert.Contains("\"id\":7", json);
    }
}
