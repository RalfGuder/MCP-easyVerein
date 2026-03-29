using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberEntityTests
{
    [Fact]
    public void Member_CanBeCreated_WithRequiredProperties()
    {
        var member = new Member
        {
            Id = 1,
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@example.com"
        };

        Assert.Equal(1, member.Id);
        Assert.Equal("Max", member.FirstName);
        Assert.Equal("Mustermann", member.LastName);
        Assert.Equal("max@example.com", member.Email);
    }

    [Fact]
    public void Member_FullName_ReturnsCombinedName()
    {
        var member = new Member
        {
            FirstName = "Max",
            LastName = "Mustermann"
        };

        Assert.Equal("Max Mustermann", member.FullName);
    }
}
