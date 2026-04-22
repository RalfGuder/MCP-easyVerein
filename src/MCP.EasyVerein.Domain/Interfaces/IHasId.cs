namespace MCP.EasyVerein.Domain.Interfaces;

/// <summary>
/// Marker interface for entities that carry a numeric identifier and can therefore
/// be referenced from other entities either as a full embedded object or as a URL
/// reference (e.g. <c>"https://easyverein.com/api/v2.0/invoice/42"</c>).
/// </summary>
public interface IHasId
{
    /// <summary>Gets or sets the unique identifier.</summary>
    long Id { get; set; }
}
