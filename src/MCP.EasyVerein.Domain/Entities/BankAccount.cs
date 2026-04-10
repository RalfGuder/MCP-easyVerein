using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities
{
    /// <summary>Represents a bank account from the easyVerein API.</summary>
    public class BankAccount
    {
        /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
        [JsonPropertyName(BankAccountFields.Id)] public long Id { get; set; }
    }
}
