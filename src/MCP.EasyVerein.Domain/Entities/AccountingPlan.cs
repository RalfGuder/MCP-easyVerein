using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities
{
    public class AccountingPlan : IHasId
    {
        [JsonPropertyName(AccountingPlanFields.Id)] public long Id { get; set; }
    }
}
