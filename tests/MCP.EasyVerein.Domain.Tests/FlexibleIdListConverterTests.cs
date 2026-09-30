using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

/// <summary>Unit tests for <see cref="FlexibleIdListConverter"/>.</summary>
public class FlexibleIdListConverterTests
{
    /// <summary>Test holder that applies the converter to a single list property.</summary>
    private sealed class Holder
    {
        /// <summary>Gets or sets the converted ID list.</summary>
        [JsonConverter(typeof(FlexibleIdListConverter))]
        public List<long>? Value { get; set; }
    }

    /// <summary>Verifies that URL references are reduced to their trailing IDs.</summary>
    [Fact]
    public void Read_FromUrlStrings_ExtractsTrailingIds()
    {
        var holder = JsonSerializer.Deserialize<Holder>(
            """{ "Value": ["https://easyverein.com/api/v2.0/inventory-object-group/11", "https://easyverein.com/api/v2.0/inventory-object-group/12"] }""");

        Assert.Equal(new List<long> { 11, 12 }, holder!.Value);
    }

    /// <summary>Verifies that JSON numbers and numeric strings can be mixed.</summary>
    [Fact]
    public void Read_FromNumbersAndNumericStrings_ReturnsIds()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": [5, "6"] }""");

        Assert.Equal(new List<long> { 5, 6 }, holder!.Value);
    }

    /// <summary>Verifies that an empty array stays an empty list (distinct from a missing field).</summary>
    [Fact]
    public void Read_FromEmptyArray_ReturnsEmptyList()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": [] }""");

        Assert.NotNull(holder!.Value);
        Assert.Empty(holder.Value);
    }

    /// <summary>Verifies that a JSON null yields a null list.</summary>
    [Fact]
    public void Read_FromNull_ReturnsNull()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": null }""");

        Assert.Null(holder!.Value);
    }

    /// <summary>Verifies that an element without an extractable ID is rejected.</summary>
    [Fact]
    public void Read_FromInvalidElement_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Holder>("""{ "Value": ["abc"] }"""));
    }

    /// <summary>Verifies that the list is written as an array of plain integer IDs.</summary>
    [Fact]
    public void Write_WritesIntegerArray()
    {
        var json = JsonSerializer.Serialize(new Holder { Value = new List<long> { 11, 12 } });

        Assert.Equal("""{"Value":[11,12]}""", json);
    }
}
