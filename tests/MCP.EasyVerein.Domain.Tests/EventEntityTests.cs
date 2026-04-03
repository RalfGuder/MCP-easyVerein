using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class EventEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "name": "Jahreshauptversammlung",
                "description": "Beschreibung des Events",
                "prologue": "Vorwort",
                "note": "Interne Notiz",
                "start": "2026-05-01T10:00:00",
                "end": "2026-05-01T12:00:00",
                "allDay": false,
                "locationName": "Vereinsheim",
                "locationObject": 7,
                "parent": 3,
                "minParticipators": 5,
                "maxParticipators": 50,
                "startParticipation": "2026-04-01T00:00:00",
                "endParticipation": "2026-04-28T23:59:59",
                "access": 1,
                "weekdays": "MO,DI",
                "sendMailCheck": true,
                "showMemberarea": true,
                "isPublic": false,
                "massParticipations": false,
                "canceled": false,
                "isReservation": true,
                "creator": 42,
                "reservationParentEvent": 11
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var ev = JsonSerializer.Deserialize<Event>(json, options);

        Assert.NotNull(ev);
        Assert.Equal(99, ev.Id);
        Assert.Equal("Jahreshauptversammlung", ev.Name);
        Assert.Equal("Beschreibung des Events", ev.Description);
        Assert.Equal("Vorwort", ev.Prologue);
        Assert.Equal("Interne Notiz", ev.Note);
        Assert.Equal(new DateTime(2026, 5, 1, 10, 0, 0), ev.Start);
        Assert.Equal(new DateTime(2026, 5, 1, 12, 0, 0), ev.End);
        Assert.False(ev.AllDay);
        Assert.Equal("Vereinsheim", ev.LocationName);
        Assert.Equal(7L, ev.LocationObject);
        Assert.Equal(3L, ev.Parent);
        Assert.Equal(5, ev.MinParticipants);
        Assert.Equal(50, ev.MaxParticipants);
        Assert.Equal(new DateTime(2026, 4, 1, 0, 0, 0), ev.StartParticipation);
        Assert.Equal(new DateTime(2026, 4, 28, 23, 59, 59), ev.EndParticipation);
        Assert.Equal(1, ev.Access);
        Assert.Equal("MO,DI", ev.Weekdays);
        Assert.True(ev.SendMailCheck);
        Assert.True(ev.ShowMemberArea);
        Assert.False(ev.IsPublic);
        Assert.False(ev.MassParticipations);
        Assert.False(ev.Canceled);
        Assert.True(ev.IsReservation);
        Assert.Equal(42L, ev.Creator);
        Assert.Equal(11L, ev.ReservationParentEvent);
    }
}
