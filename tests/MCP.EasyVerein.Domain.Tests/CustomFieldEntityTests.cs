using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class CustomFieldEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 11,
                "name": "Lieblingsfarbe",
                "color": "#aabbcc",
                "short": "LF",
                "settings_type": "T",
                "kind": "E",
                "description": "Feld für Mitglieder",
                "additional": "x",
                "member_show": true,
                "member_edit": false,
                "member_dsgvo": false,
                "position": 3,
                "collection": "https://easyverein.com/api/v2.0/custom-field-collection/777",
                "needsAdminApproval": true
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var field = JsonSerializer.Deserialize<CustomField>(json, options);

        Assert.NotNull(field);
        Assert.Equal(11L, field.Id);
        Assert.Equal("Lieblingsfarbe", field.Name);
        Assert.Equal("#aabbcc", field.Color);
        Assert.Equal("LF", field.Short);
        Assert.Equal("T", field.SettingsType);
        Assert.Equal("E", field.Kind);
        Assert.Equal("Feld für Mitglieder", field.Description);
        Assert.Equal("x", field.Additional);
        Assert.True(field.MemberShow);
        Assert.False(field.MemberEdit);
        Assert.False(field.MemberDsgvo);
        Assert.Equal(3, field.Position);
        Assert.Equal(777L, field.Collection);
        Assert.True(field.NeedsAdminApproval);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "name": "Minimal"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var field = JsonSerializer.Deserialize<CustomField>(json, options);

        Assert.NotNull(field);
        Assert.Equal(42L, field.Id);
        Assert.Equal("Minimal", field.Name);
        Assert.Null(field.Color);
        Assert.Null(field.Short);
        Assert.Null(field.SettingsType);
        Assert.Null(field.Kind);
        Assert.Null(field.Collection);
        Assert.Null(field.MemberShow);
        Assert.Null(field.NeedsAdminApproval);
    }
}
