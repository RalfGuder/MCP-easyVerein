using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class LendingEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 37839,
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "parentInventoryObject": "https://easyverein.com/api/v2.0/inventory-object/335646309",
                "borrowAddress": "https://easyverein.com/api/v2.0/contact-details/335646225",
                "name": "Ausleihe von Ralf Guder",
                "created_at": "2026-05-27T06:24:03.066348+02:00",
                "updated_at": "2026-05-27T06:24:03.207172+02:00",
                "_deleteAfterDate": "2026-10-27T10:00:00+02:00",
                "_deletedBy": "Max Mustermann",
                "borrowingDate": "2025-10-27",
                "returnDate": "2025-11-03",
                "quantity": 2,
                "borrowTime": "14:30:00",
                "returnTime": "09:15:00",
                "state": "lent"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var lending = JsonSerializer.Deserialize<Lending>(json, options);

        Assert.NotNull(lending);
        Assert.Equal(37839L, lending.Id);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", lending.Org);
        Assert.Equal("Ausleihe von Ralf Guder", lending.Name);
        Assert.NotNull(lending.CreatedAt);
        Assert.NotNull(lending.UpdatedAt);
        Assert.NotNull(lending.DeleteAfterDate);
        Assert.Equal("Max Mustermann", lending.DeletedBy);
        Assert.Equal(2, lending.Quantity);
        Assert.Equal("14:30:00", lending.BorrowTime);
        Assert.Equal("09:15:00", lending.ReturnTime);
        Assert.Equal("lent", lending.State);
    }

    [Fact]
    public void ForeignKeys_AreReadAsIds_FromUrlReferences()
    {
        var json = """
            {
                "id": 1,
                "parentInventoryObject": "https://easyverein.com/api/v2.0/inventory-object/335646309",
                "borrowAddress": "https://easyverein.com/api/v2.0/contact-details/335646225"
            }
            """;

        var lending = JsonSerializer.Deserialize<Lending>(json);

        Assert.NotNull(lending);
        Assert.Equal(335646309L, lending.ParentInventoryObjectId);
        Assert.Equal(335646225L, lending.BorrowAddressId);
    }

    [Fact]
    public void ForeignKeys_AreReadAsIds_FromPlainNumbers()
    {
        var json = """
            {
                "id": 1,
                "parentInventoryObject": 335646309,
                "borrowAddress": 335646225
            }
            """;

        var lending = JsonSerializer.Deserialize<Lending>(json);

        Assert.NotNull(lending);
        Assert.Equal(335646309L, lending.ParentInventoryObjectId);
        Assert.Equal(335646225L, lending.BorrowAddressId);
    }

    [Fact]
    public void Dates_AcceptDateOnlyFormat()
    {
        var json = """
            {
                "id": 1,
                "borrowingDate": "2025-10-27",
                "returnDate": "2025-11-03"
            }
            """;

        var lending = JsonSerializer.Deserialize<Lending>(json);

        Assert.NotNull(lending);
        Assert.Equal(new DateTime(2025, 10, 27), lending.BorrowingDate);
        Assert.Equal(new DateTime(2025, 11, 3), lending.ReturnDate);
    }

    [Fact]
    public void Serialize_WritesDatesAsDateOnly_NotFullIso()
    {
        var lending = new Lending
        {
            BorrowingDate = new DateTime(2025, 10, 27),
            ReturnDate = new DateTime(2025, 11, 3)
        };

        var json = JsonSerializer.Serialize(lending);

        Assert.Contains("\"borrowingDate\":\"2025-10-27\"", json);
        Assert.Contains("\"returnDate\":\"2025-11-03\"", json);
        Assert.DoesNotContain("T00:00:00", json);
    }

    [Fact]
    public void JsonPropertyNames_WithNullValues_AreCorrect()
    {
        var json = """
            {
                "id": 37839,
                "returnDate": null,
                "borrowTime": null,
                "returnTime": null,
                "_deleteAfterDate": null,
                "state": "lent"
            }
            """;

        var lending = JsonSerializer.Deserialize<Lending>(json);

        Assert.NotNull(lending);
        Assert.Null(lending.ReturnDate);
        Assert.Null(lending.BorrowTime);
        Assert.Null(lending.ReturnTime);
        Assert.Null(lending.DeleteAfterDate);
        Assert.Null(lending.CreatedAt);
    }

    [Fact]
    public void Serialize_ForCreate_OmitsReadOnlyFields()
    {
        var lending = new Lending
        {
            ParentInventoryObjectId = 335646309,
            BorrowAddressId = 335646225,
            Quantity = 1,
            State = "lent"
        };

        var json = JsonSerializer.Serialize(lending);

        Assert.Contains("\"parentInventoryObject\":335646309", json);
        Assert.Contains("\"borrowAddress\":335646225", json);
        Assert.Contains("\"quantity\":1", json);
        Assert.Contains("\"state\":\"lent\"", json);
        Assert.DoesNotContain("org", json);
        Assert.DoesNotContain("\"name\"", json);
        Assert.DoesNotContain("created_at", json);
        Assert.DoesNotContain("updated_at", json);
        Assert.DoesNotContain("_delete", json);
    }

    [Fact]
    public void Serialize_WritesForeignKeysAsIntegers_NotUrls()
    {
        var json = """{"id":1,"parentInventoryObject":"https://easyverein.com/api/v2.0/inventory-object/335646309"}""";
        var lending = JsonSerializer.Deserialize<Lending>(json);

        var serialized = JsonSerializer.Serialize(lending);

        Assert.Contains("\"parentInventoryObject\":335646309", serialized);
        Assert.DoesNotContain("https://", serialized);
    }
}
