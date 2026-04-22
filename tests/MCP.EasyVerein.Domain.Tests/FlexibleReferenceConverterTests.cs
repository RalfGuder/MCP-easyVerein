using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleReferenceConverterTests
{
    private sealed class ContactWrapper
    {
        [JsonConverter(typeof(FlexibleReferenceConverter<ContactDetails>))]
        public ContactDetails? Value { get; set; }
    }

    private sealed class OrgWrapper
    {
        [JsonConverter(typeof(FlexibleReferenceConverter<Organization>))]
        public Organization? Value { get; set; }
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<ContactWrapper>("{\"Value\":null}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_UrlString_PopulatesOnlyId()
    {
        const string json = "{\"Value\":\"https://easyverein.com/api/v2.0/contact-details/335684097\"}";
        var w = JsonSerializer.Deserialize<ContactWrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(335684097L, w.Value!.Id);
        Assert.Equal(string.Empty, w.Value.FamilyName);
        Assert.Equal(string.Empty, w.Value.FirstName);
    }

    [Fact]
    public void Read_EmbeddedObject_PopulatesFullEntity()
    {
        const string json = "{\"Value\":{\"id\":42,\"familyName\":\"Rose\",\"firstName\":\"Kathleen\"}}";
        var w = JsonSerializer.Deserialize<ContactWrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(42L, w.Value!.Id);
        Assert.Equal("Rose", w.Value.FamilyName);
        Assert.Equal("Kathleen", w.Value.FirstName);
    }

    [Fact]
    public void Read_MalformedUrl_ThrowsJsonException()
    {
        const string json = "{\"Value\":\"https://example.com/no-id-here/\"}";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ContactWrapper>(json));
    }

    [Fact]
    public void Read_NumberToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<ContactWrapper>("{\"Value\":123}"));
    }

    [Fact]
    public void Write_NullValue_WritesJsonNull()
    {
        var json = JsonSerializer.Serialize(new ContactWrapper { Value = null });
        Assert.Equal("{\"Value\":null}", json);
    }

    [Fact]
    public void Write_EntityValue_WritesEmbeddedObject()
    {
        var json = JsonSerializer.Serialize(new ContactWrapper { Value = new ContactDetails { Id = 7 } });
        Assert.Contains("\"id\":7", json);
    }

    [Fact]
    public void Read_OrganizationUrlString_PopulatesOnlyId()
    {
        const string json = "{\"Value\":\"https://easyverein.com/api/v2.0/organization/30189\"}";
        var w = JsonSerializer.Deserialize<OrgWrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(30189L, w.Value!.Id);
    }

    [Fact]
    public void Read_OrganizationEmbeddedObject_PopulatesId()
    {
        const string json = "{\"Value\":{\"id\":30189}}";
        var w = JsonSerializer.Deserialize<OrgWrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(30189L, w.Value!.Id);
    }
}
