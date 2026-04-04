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
                "bankAccount": {"id": 7},
                "billingAccount": {"id": 3    },
                "description": "Jahresbeitrag 2026",
                "date": "2026-03-15T00:00:00",
                "receiver": "Max Mustermann",
                "blocked": false,
                "paymentDifference": 0.00,
                "counterpartIban": "DE89370400440532013000",
                "counterpartBic": "COBADEFFXXX",
                "twingleDonation": false,
                "bookingProject": "Projekt A",
                "sphere": 1,
                "relatedInvoice": [{"id": 1}, {"id": 2}, {"id": 3}]
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var booking = JsonSerializer.Deserialize<Booking>(json, options);

        Assert.NotNull(booking);
        Assert.Equal(42, booking.Id);
        Assert.Equal(150.50m, booking.Amount);
        Assert.NotNull(booking.BankAccount);
        Assert.Equal(7L, booking.BankAccount.Id);
        Assert.NotNull(booking.BillingAccount);
        Assert.Equal(3L, booking.BillingAccount.Id);
        Assert.Equal("Jahresbeitrag 2026", booking.Description);
        Assert.Equal(new DateTime(2026, 3, 15), booking.Date);
        Assert.Equal("Max Mustermann", booking.Receiver);
        Assert.False(booking.Blocked);
        Assert.Equal(0.00m, booking.PaymentDifference);
        Assert.Equal("DE89370400440532013000", booking.CounterpartIban);
        Assert.Equal("COBADEFFXXX", booking.CounterpartBic);
        Assert.False(booking.TwingleDonation);
        Assert.Equal("Projekt A", booking.BookingProject);
        Assert.Equal(1L, booking.Sphere);
        Assert.NotNull(booking.RelatedInvoice);
        Assert.Equal(3, booking.RelatedInvoice.Count);
        Assert.Equal(1L, booking.RelatedInvoice[0].Id);
        Assert.Equal(2L, booking.RelatedInvoice[1].Id);
        Assert.Equal(3L, booking.RelatedInvoice[2].Id);
    }
}
