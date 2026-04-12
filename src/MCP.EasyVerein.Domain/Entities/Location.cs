using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities
{
    public class Location
    {
        [JsonPropertyName(LocationFields.Id)] public long Id { get; set; }
        [JsonPropertyName(LocationFields.Name)] public string? Name { get; set; }
    }
}
