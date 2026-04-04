using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class BookingEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "amount": 150.50,
                "bankAccount": 7,
                "billingAccount": 3,
                "description": "Jahresbeitrag 2026",
                "date": "2026-03-15T00:00:00",
                "receiver": "Max Mustermann",
                "billingId": "BIL-001",
                "blocked": false,
                "paymentDifference": 0.00,
                "counterpartIban": "DE89370400440532013000",
                "counterpartBic": "COBADEFFXXX",
                "twingleDonation": false,
                "bookingProject": "Projekt A",
                "sphere": "ideell",
                "relatedInvoice": [1, 2, 3]
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var booking = JsonSerializer.Deserialize<Booking>(json, options);

        Assert.NotNull(booking);
        Assert.Equal(42, booking.Id);
        Assert.Equal(150.50m, booking.Amount);
        Assert.Equal(7L, booking.BankAccount);
        Assert.Equal(3L, booking.BillingAccount);
        Assert.Equal("Jahresbeitrag 2026", booking.Description);
        Assert.Equal(new DateTime(2026, 3, 15), booking.Date);
        Assert.Equal("Max Mustermann", booking.Receiver);
        Assert.Equal("BIL-001", booking.BillingId);
        Assert.False(booking.Blocked);
        Assert.Equal(0.00m, booking.PaymentDifference);
        Assert.Equal("DE89370400440532013000", booking.CounterpartIban);
        Assert.Equal("COBADEFFXXX", booking.CounterpartBic);
        Assert.False(booking.TwingleDonation);
        Assert.Equal("Projekt A", booking.BookingProject);
        Assert.Equal("ideell", booking.Sphere);
        Assert.Equal(new List<long> { 1, 2, 3 }, booking.RelatedInvoice);
    }
}
