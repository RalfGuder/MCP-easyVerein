namespace MCP.EasyVerein.Domain.Entities;

public class Event
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public int? MaxParticipants { get; set; }
}
