using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class InventoryObjectEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 335646309,
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "lendingResponsible": "https://easyverein.com/api/v2.0/member/4424352",
                "inventoryObjectGroups": ["https://easyverein.com/api/v2.0/inventory-object-group/11"],
                "picture": "https://easyverein.com/app/image/defaultInventory.png",
                "currentlyLend": 1,
                "lendings": ["https://easyverein.com/api/v2.0/lending/37839"],
                "customFields": ["https://easyverein.com/api/v2.0/inventory-object/335646309/custom-fields/5"],
                "locationObject": "https://easyverein.com/api/v2.0/location/335646294",
                "lastLendDate": "2025-10-27",
                "lastReturnDate": "2025-11-02",
                "created_at": "2026-05-27T06:23:59.578803+02:00",
                "updated_at": "2026-05-27T06:23:59.701964+02:00",
                "_deleteAfterDate": "2026-06-27T06:23:59+02:00",
                "_deletedBy": "Max Mustermann",
                "name": "Zelt",
                "identifier": "59409381",
                "description": "Ein Zelt für Veranstaltungen",
                "pieces": 2,
                "price": "99.99",
                "purchaseDate": "2025-03-21T00:00:00+01:00",
                "locationName": "Lager",
                "lendingAvailable": true
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var item = JsonSerializer.Deserialize<InventoryObject>(json, options);

        Assert.NotNull(item);
        Assert.Equal(335646309L, item.Id);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", item.Org);
        Assert.Equal(4424352L, item.LendingResponsibleId);
        Assert.Single(item.InventoryObjectGroups!);
        Assert.Equal("https://easyverein.com/app/image/defaultInventory.png", item.Picture);
        Assert.Equal(1, item.CurrentlyLend);
        Assert.Equal("https://easyverein.com/api/v2.0/lending/37839", item.Lendings![0]);
        Assert.Single(item.CustomFields!);
        Assert.Equal(335646294L, item.LocationObjectId);
        Assert.Equal(new DateTime(2025, 10, 27), item.LastLendDate);
        Assert.Equal(new DateTime(2025, 11, 2), item.LastReturnDate);
        Assert.NotNull(item.CreatedAt);
        Assert.NotNull(item.UpdatedAt);
        Assert.NotNull(item.DeleteAfterDate);
        Assert.Equal("Max Mustermann", item.DeletedBy);
        Assert.Equal("Zelt", item.Name);
        Assert.Equal("59409381", item.Identifier);
        Assert.Equal("Ein Zelt für Veranstaltungen", item.Description);
        Assert.Equal(2, item.Pieces);
        Assert.Equal(99.99m, item.Price);
        Assert.Equal(new DateTimeOffset(2025, 3, 21, 0, 0, 0, TimeSpan.FromHours(1)), item.PurchaseDate);
        Assert.Equal("Lager", item.LocationName);
        Assert.True(item.LendingAvailable);
    }

    [Fact]
    public void JsonPropertyNames_WithNullReferences_AreCorrect()
    {
        var json = """
            {
                "id": 1,
                "lendingResponsible": null,
                "locationObject": null,
                "lastLendDate": null,
                "lastReturnDate": null,
                "_deleteAfterDate": null,
                "price": null,
                "purchaseDate": null,
                "name": "Leer"
            }
            """;

        var item = JsonSerializer.Deserialize<InventoryObject>(json);

        Assert.NotNull(item);
        Assert.Null(item.LendingResponsibleId);
        Assert.Null(item.LocationObjectId);
        Assert.Null(item.LastLendDate);
        Assert.Null(item.LastReturnDate);
        Assert.Null(item.DeleteAfterDate);
        Assert.Null(item.Price);
        Assert.Null(item.PurchaseDate);
    }

    [Fact]
    public void Serialize_ForCreate_OmitsReadOnlyAndUnsetFields()
    {
        var item = new InventoryObject { Name = "Beamer", Pieces = 1, Price = 450.5m, LendingResponsibleId = 4424352 };

        var json = JsonSerializer.Serialize(item);

        Assert.Contains("\"name\":\"Beamer\"", json);
        Assert.Contains("\"pieces\":1", json);
        Assert.Contains("\"price\":450.5", json);
        Assert.Contains("\"lendingResponsible\":4424352", json);
        Assert.DoesNotContain("org", json);
        Assert.DoesNotContain("picture", json);
        Assert.DoesNotContain("currentlyLend", json);
        Assert.DoesNotContain("lendings", json);
        Assert.DoesNotContain("customFields", json);
        Assert.DoesNotContain("inventoryObjectGroups", json);
        Assert.DoesNotContain("locationObject", json);
        Assert.DoesNotContain("lastLendDate", json);
        Assert.DoesNotContain("created_at", json);
        Assert.DoesNotContain("_delete", json);
        Assert.DoesNotContain("purchaseDate", json);
        Assert.DoesNotContain("description", json);
    }
}
