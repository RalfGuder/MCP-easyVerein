using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents an event from the easyVerein API.
/// </summary>
public class Event
{
    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Id)] 
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the event name. Maps to API field '<c>name</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Name)] 
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event description. Maps to API field '<c>description</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Description)] 
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the prologue text displayed before the event details. Maps to API field '<c>prologue</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Prologue)] 
    public string? Prologue { get; set; }

    /// <summary>
    /// Gets or sets an internal note for the event. Maps to API field ' <c>note</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Note)] 
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the event start date and time. Maps to API field ' <c>start</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Start)] 
    public DateTime? Start { get; set; }

    /// <summary>
    /// Gets or sets the event end date and time. Maps to API field ' <c>end</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.End)] 
    public DateTime? End { get; set; }

    /// <summary>
    /// Gets or sets whether this is an all-day event. Maps to API field ' <c>allDay</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.AllDay)] 
    public bool AllDay { get; set; }

    /// <summary>
    /// Gets or sets the location name. Maps to API field ' <c>locationName</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.LocationName)] 
    public string? LocationName { get; set; }

    /// <summary>
    /// Gets or sets the location object reference URL. Maps to API field ' <c>locationObject</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.LocationObject)]
    public Location? LocationObject { get; set; }

    /// <summary>
    /// Gets or sets the parent event URL reference for recurring events. Maps to API field ' <c>parent</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Parent)]
    public string? Parent { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of participants. Maps to API field ' <c>minParticipators</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.MinParticipators)] 
    public int? MinParticipants { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of participants. Maps to API field ' <c>maxParticipators</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.MaxParticipators)] 
    public int? MaxParticipants { get; set; }

    /// <summary>
    /// Gets or sets the participation registration start date. Maps to API field ' <c>startParticipation</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.StartParticipation)] 
    public DateTime? StartParticipation { get; set; }

    /// <summary>
    /// Gets or sets the participation registration end date. Maps to API field ' <c>endParticipation</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.EndParticipation)] 
    public DateTime? EndParticipation { get; set; }

    /// <summary>
    /// Gets or sets the access level for the event. Maps to API field ' <c>access</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Access)] 
    public int? Access { get; set; }

    /// <summary>
    /// Gets or sets the weekdays for recurring events. Maps to API field ' <c>weekdays</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Weekdays)] 
    public string? Weekdays { get; set; }

    /// <summary>
    /// Gets or sets whether a mail notification is sent. Maps to API field ' <c>sendMailCheck</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.SendMailCheck)] 
    public bool SendMailCheck { get; set; }

    /// <summary>
    /// Gets or sets whether the event is shown in the member area. Maps to API field ' <c>showMemberarea</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.ShowMemberArea)] 
    public bool ShowMemberArea { get; set; }

    /// <summary>
    /// Gets or sets whether the event is publicly visible. Maps to API field ' <c>isPublic</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.IsPublic)] 
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets or sets whether mass participations are enabled. Maps to API field ' <c>massParticipations</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.MassParticipations)] 
    public bool MassParticipations { get; set; }

    /// <summary>
    /// Gets or sets whether the event is canceled. Maps to API field ' <c>canceled</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Canceled)] 
    public bool Canceled { get; set; }

    /// <summary>
    /// Gets or sets whether this is a reservation. Maps to API field ' <c>isReservation</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.IsReservation)] 
    public bool IsReservation { get; set; }

    /// <summary>
    /// Gets or sets the ID of the event creator. Maps to API field ' <c>creator</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Creator)] 
    public long? Creator { get; set; }

    /// <summary>
    /// Gets or sets the parent event ID for reservations. Maps to API field ' <c>reservationParentEvent</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.ReservationParentEvent)]
    public long? ReservationParentEvent { get; set; }

    /// <summary>
    /// Gets or sets the calendar reference. Maps to API field '<c>calendar</c>'.
    /// </summary>
    [JsonPropertyName(EventFields.Calendar)]
    public Calendar? Calendar { get; set; }
}
