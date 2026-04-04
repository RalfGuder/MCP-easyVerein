using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities
{
    public class BillingAccount
    {
        [JsonPropertyName(BillingAccountFields.Id)] public long Id { get; set; }
    }
}
