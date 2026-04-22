using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleReferenceListConverterTests
{
    private sealed class Wrapper
    {
        [JsonConverter(typeof(FlexibleReferenceListConverter<Invoice>))]
        public List<Invoice>? Value { get; set; }
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":null}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_EmptyArray_ReturnsEmptyList()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":[]}");
        Assert.NotNull(w!.Value);
        Assert.Empty(w.Value!);
    }

    [Fact]
    public void Read_ArrayOfEmbeddedObjects_PopulatesFullEntities()
    {
        const string json = "{\"Value\":[{\"id\":1,\"invNumber\":\"A-001\"},{\"id\":2,\"invNumber\":\"A-002\"}]}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(2, w.Value!.Count);
        Assert.Equal(1L, w.Value[0].Id);
        Assert.Equal("A-001", w.Value[0].InvoiceNumber);
        Assert.Equal(2L, w.Value[1].Id);
        Assert.Equal("A-002", w.Value[1].InvoiceNumber);
    }

    [Fact]
    public void Read_ArrayOfUrlStrings_PopulatesOnlyIds()
    {
        const string json = "{\"Value\":[" +
            "\"https://easyverein.com/api/v2.0/invoice/100\"," +
            "\"https://easyverein.com/api/v2.0/invoice/200\"" +
        "]}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(2, w.Value!.Count);
        Assert.Equal(100L, w.Value[0].Id);
        Assert.Null(w.Value[0].InvoiceNumber);
        Assert.Equal(200L, w.Value[1].Id);
    }

    [Fact]
    public void Read_MixedEmbeddedAndUrlElements_PopulatesBoth()
    {
        const string json = "{\"Value\":[" +
            "{\"id\":1,\"invNumber\":\"A-001\"}," +
            "\"https://easyverein.com/api/v2.0/invoice/2\"" +
        "]}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(2, w.Value!.Count);
        Assert.Equal(1L, w.Value[0].Id);
        Assert.Equal("A-001", w.Value[0].InvoiceNumber);
        Assert.Equal(2L, w.Value[1].Id);
        Assert.Null(w.Value[1].InvoiceNumber);
    }

    [Fact]
    public void Read_MalformedUrlElement_ThrowsJsonException()
    {
        const string json = "{\"Value\":[\"https://example.com/no-id-here/\"]}";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Wrapper>(json));
    }

    [Fact]
    public void Read_NonArrayToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"not-an-array\"}"));
    }

    [Fact]
    public void Read_NullInsideArray_IsSkipped()
    {
        const string json = "{\"Value\":[null,{\"id\":5}]}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Single(w.Value!);
        Assert.Equal(5L, w.Value![0].Id);
    }

    [Fact]
    public void Write_NullValue_WritesJsonNull()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = null });
        Assert.Equal("{\"Value\":null}", json);
    }

    [Fact]
    public void Write_EntityList_WritesArrayOfEmbeddedObjects()
    {
        var json = JsonSerializer.Serialize(new Wrapper
        {
            Value = new List<Invoice> { new() { Id = 1 }, new() { Id = 2 } }
        });
        Assert.Contains("\"id\":1", json);
        Assert.Contains("\"id\":2", json);
        Assert.StartsWith("{\"Value\":[", json);
    }
}
