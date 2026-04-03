using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberEntityTests
{
    [Fact]
    public void IsActive_True_WhenNoResignationAndNotBlocked()
    {
        var member = new Member
        {
            Id = 1,
            EmailOrUserName = "max@example.com",
            ResignationDate = null,
            IsBlocked = false
        };

        Assert.True(member.IsActive);
    }

    [Fact]
    public void IsActive_False_WhenResignationDateSet()
    {
        var member = new Member
        {
            Id = 1,
            EmailOrUserName = "max@example.com",
            ResignationDate = new DateTime(2025, 1, 1),
            IsBlocked = false
        };

        Assert.False(member.IsActive);
    }

    [Fact]
    public void IsActive_False_WhenBlocked()
    {
        var member = new Member
        {
            Id = 1,
            EmailOrUserName = "max@example.com",
            ResignationDate = null,
            IsBlocked = true
        };

        Assert.False(member.IsActive);
    }

    [Fact]
    public void FullName_UsesContactDetails_WhenPresent()
    {
        var member = new Member
        {
            Id = 1,
            EmailOrUserName = "max@example.com",
            ContactDetails = new ContactDetails
            {
                FirstName = "Max",
                FamilyName = "Mustermann"
            }
        };

        Assert.Equal("Max Mustermann", member.FullName);
    }

    [Fact]
    public void FullName_FallsBackToEmail_WhenNoContactDetails()
    {
        var member = new Member
        {
            Id = 1,
            EmailOrUserName = "max@example.com",
            ContactDetails = null
        };

        Assert.Equal("max@example.com", member.FullName);
    }

    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        const string json = """
            {
                "id": 42,
                "emailOrUserName": "test@example.com",
                "membershipNumber": "M-001",
                "joinDate": "2023-01-15T00:00:00",
                "resignationDate": null,
                "paymentAmount": 49.99,
                "paymentIntervallMonths": 12,
                "_isApplication": false,
                "_isChairman": true,
                "_isBlocked": false,
                "requirePasswordChange": false,
                "_isMatrixSearchable": true,
                "blockedFromMatrix": false,
                "useMatrixGroupSettings": false,
                "showWarningsAndNotesToAdminsInProfile": true,
                "useBalanceForMembershipFee": false,
                "bulletinBoardNewPostNotification": true,
                "_editableByRelatedMembers": false,
                "contactDetails": {
                    "id": 99,
                    "firstName": "Anna",
                    "familyName": "Schmidt"
                }
            }
            """;

        var member = JsonSerializer.Deserialize<Member>(json);

        Assert.NotNull(member);
        Assert.Equal(42, member.Id);
        Assert.Equal("test@example.com", member.EmailOrUserName);
        Assert.Equal("M-001", member.MembershipNumber);
        Assert.Equal(new DateTime(2023, 1, 15), member.JoinDate);
        Assert.Null(member.ResignationDate);
        Assert.Equal(49.99m, member.PaymentAmount);
        Assert.Equal(12, member.PaymentIntervalMonths);
        Assert.False(member.IsApplication);
        Assert.True(member.IsChairman);
        Assert.False(member.IsBlocked);
        Assert.True(member.IsMatrixSearchable);
        Assert.True(member.ShowWarningsAndNotesToAdminsInProfile);
        Assert.NotNull(member.ContactDetails);
        Assert.Equal(99, member.ContactDetails.Id);
        Assert.Equal("Anna", member.ContactDetails.FirstName);
        Assert.Equal("Schmidt", member.ContactDetails.FamilyName);
        Assert.Equal("Anna Schmidt", member.FullName);
        Assert.True(member.IsActive);
    }
}
