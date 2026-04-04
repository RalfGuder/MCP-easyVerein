using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities
{
    public class BankAccount 
    {
        [JsonPropertyName(BankAccountFields.Id)] public long Id { get; set; }
    }
}
