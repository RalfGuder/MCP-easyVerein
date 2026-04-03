using System.Text.Json.Serialization;
namespace MCP.EasyVerein.Domain.Entities;

public class Event
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("prologue")] public string? Prologue { get; set; }
    [JsonPropertyName("note")] public string? Note { get; set; }
    [JsonPropertyName("start")] public DateTime? Start { get; set; }
    [JsonPropertyName("end")] public DateTime? End { get; set; }
    [JsonPropertyName("allDay")] public bool AllDay { get; set; }
    [JsonPropertyName("locationName")] public string? LocationName { get; set; }
    [JsonPropertyName("locationObject")] public long? LocationObject { get; set; }
    [JsonPropertyName("parent")] public long? Parent { get; set; }
    [JsonPropertyName("minParticipators")] public int? MinParticipants { get; set; }
    [JsonPropertyName("maxParticipators")] public int? MaxParticipants { get; set; }
    [JsonPropertyName("startParticipation")] public DateTime? StartParticipation { get; set; }
    [JsonPropertyName("endParticipation")] public DateTime? EndParticipation { get; set; }
    [JsonPropertyName("access")] public int? Access { get; set; }
    [JsonPropertyName("weekdays")] public string? Weekdays { get; set; }
    [JsonPropertyName("sendMailCheck")] public bool SendMailCheck { get; set; }
    [JsonPropertyName("showMemberarea")] public bool ShowMemberArea { get; set; }
    [JsonPropertyName("isPublic")] public bool IsPublic { get; set; }
    [JsonPropertyName("massParticipations")] public bool MassParticipations { get; set; }
    [JsonPropertyName("canceled")] public bool Canceled { get; set; }
    [JsonPropertyName("isReservation")] public bool IsReservation { get; set; }
    [JsonPropertyName("creator")] public long? Creator { get; set; }
    [JsonPropertyName("reservationParentEvent")] public long? ReservationParentEvent { get; set; }
}
