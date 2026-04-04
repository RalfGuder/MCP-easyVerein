using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities
{
    /// <summary>Represents an organization from the easyVerein API.</summary>
    public class Organization
    {
        /// <summary>
        /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonPropertyName(OrganizationFields.Id)] 
        public long Id { get; set; }
    }
}
