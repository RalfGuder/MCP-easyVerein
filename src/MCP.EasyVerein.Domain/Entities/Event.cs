using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

public class Event
{
    [JsonPropertyName(EventFields.Id)] public long Id { get; set; }
    [JsonPropertyName(EventFields.Name)] public string Name { get; set; } = string.Empty;
    [JsonPropertyName(EventFields.Description)] public string? Description { get; set; }
    [JsonPropertyName(EventFields.Prologue)] public string? Prologue { get; set; }
    [JsonPropertyName(EventFields.Note)] public string? Note { get; set; }
    [JsonPropertyName(EventFields.Start)] public DateTime? Start { get; set; }
    [JsonPropertyName(EventFields.End)] public DateTime? End { get; set; }
    [JsonPropertyName(EventFields.AllDay)] public bool AllDay { get; set; }
    [JsonPropertyName(EventFields.LocationName)] public string? LocationName { get; set; }
    [JsonPropertyName(EventFields.LocationObject)] public long? LocationObject { get; set; }
    [JsonPropertyName(EventFields.Parent)] public long? Parent { get; set; }
    [JsonPropertyName(EventFields.MinParticipators)] public int? MinParticipants { get; set; }
    [JsonPropertyName(EventFields.MaxParticipators)] public int? MaxParticipants { get; set; }
    [JsonPropertyName(EventFields.StartParticipation)] public DateTime? StartParticipation { get; set; }
    [JsonPropertyName(EventFields.EndParticipation)] public DateTime? EndParticipation { get; set; }
    [JsonPropertyName(EventFields.Access)] public int? Access { get; set; }
    [JsonPropertyName(EventFields.Weekdays)] public string? Weekdays { get; set; }
    [JsonPropertyName(EventFields.SendMailCheck)] public bool SendMailCheck { get; set; }
    [JsonPropertyName(EventFields.ShowMemberArea)] public bool ShowMemberArea { get; set; }
    [JsonPropertyName(EventFields.IsPublic)] public bool IsPublic { get; set; }
    [JsonPropertyName(EventFields.MassParticipations)] public bool MassParticipations { get; set; }
    [JsonPropertyName(EventFields.Canceled)] public bool Canceled { get; set; }
    [JsonPropertyName(EventFields.IsReservation)] public bool IsReservation { get; set; }
    [JsonPropertyName(EventFields.Creator)] public long? Creator { get; set; }
    [JsonPropertyName(EventFields.ReservationParentEvent)] public long? ReservationParentEvent { get; set; }
}
