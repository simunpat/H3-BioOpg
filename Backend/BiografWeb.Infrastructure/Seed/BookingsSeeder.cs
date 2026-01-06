using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Seed;

public static class BookingsSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Bookings.AnyAsync(ct)) return;

        var adultId = Guid.Parse("d1a7f5e0-2a9c-4c52-9f6b-1eacb3f7a012");
        var childId = Guid.Parse("f3b2c1d4-5e6f-4a7b-8c9d-0a1b2c3d4e5f");

        void AddBooking(Guid id, Guid screeningId, Guid userId, decimal total, (int row, int number)[] seats, (Guid ticketTypeId, int qty)[] items)
        {
            db.Bookings.Add(new Booking
            {
                Id = id,
                ScreeningId = screeningId,
                UserId = userId,
                TotalPrice = total,
                Seats = seats.Select(s => new BookingSeat { BookingId = id, Row = s.row, Number = s.number }).ToList(),
                Items = items.Select(i => new BookingItem { TicketTypeId = i.ticketTypeId, Qty = i.qty }).ToList()
            });
        }

        AddBooking(
            Guid.Parse("0248cdba-07a0-4ed4-a4b2-35df850417cc"),
            Guid.Parse("e8a9c2b7-6d6e-4d2f-b3e3-3dc9c9d5a1a7"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            120m,
            new[] { (3, 8) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1) }
        );

        AddBooking(
            Guid.Parse("a7e5c335-13af-4ca9-87f2-438bf2e72d82"),
            Guid.Parse("e8a9c2b7-6d6e-4d2f-b3e3-3dc9c9d5a1a7"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            360m,
            new[] { (4, 5), (4, 6), (4, 7) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 3) }
        );

        AddBooking(
            Guid.Parse("f6e608fc-e5b7-4669-855c-75cd9ab2439d"),
            Guid.Parse("9f8aaf45-fb1a-4c74-8f2a-6fc0b2e2ff2b"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            330m,
            new[] { (6, 4), (6, 5), (6, 7) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 3) }
        );

        AddBooking(
            Guid.Parse("39952538-5d6d-4305-81f2-e3bc9fb6c2a4"),
            Guid.Parse("4b2c59a0-1f6a-4a1a-9dbe-0d3f4f7a2f9e"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            500m,
            new[] { (9, 5), (9, 6), (9, 7), (9, 8) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 4) }
        );

        AddBooking(
            Guid.Parse("8e39fefb-e96d-4425-a11a-b40ed664d852"),
            Guid.Parse("13d6c0b5-9f24-4af7-af0a-8d0df9f9c5c4"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            345m,
            new[] { (4, 5), (4, 6), (4, 7) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 3) }
        );

        AddBooking(
            Guid.Parse("aebb98be-65d9-44b2-ae45-c29d1f5ebc02"),
            Guid.Parse("9f8aaf45-fb1a-4c74-8f2a-6fc0b2e2ff2b"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            110m,
            new[] { (6, 6) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1) }
        );

        AddBooking(
            Guid.Parse("ce7a41cb-cdae-45b3-a977-e89c7b19540c"),
            Guid.Parse("13d6c0b5-9f24-4af7-af0a-8d0df9f9c5c4"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            115m,
            new[] { (7, 6) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1) }
        );

        AddBooking(
            Guid.Parse("b11ee5ac-ff26-4386-94f6-b821ae7988ac"),
            Guid.Parse("7dd06875-5f21-4f3d-97ab-48c03509bbcc"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            192m,
            new[] { (3, 6), (3, 7) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1), (childId, 1) }
        );

        AddBooking(
            Guid.Parse("22410d5e-a2a7-46dd-b09f-dec66c2f3b76"),
            Guid.Parse("7dd06875-5f21-4f3d-97ab-48c03509bbcc"),
            Guid.Parse("2153115a-32f4-4696-9fef-1b4c2df4c4ef"),
            1920m,
            new[]
            {
                (5, 1), (5, 2), (5, 3), (5, 4), (5, 5), (5, 6),
                (5, 7), (5, 8), (5, 9), (5, 10), (5, 11), (5, 12),
                (6, 12), (6, 11), (6, 10), (6, 9), (6, 8), (6, 7),
                (6, 6), (6, 5)
            },
            new (Guid ticketTypeId, int qty)[] { (adultId, 10), (childId, 10) }
        );

        AddBooking(
            Guid.Parse("77088d3c-7ee1-4206-b70c-d1d8beec1908"),
            Guid.Parse("7dd06875-5f21-4f3d-97ab-48c03509bbcc"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            120m,
            new[] { (8, 7) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1) }
        );

        AddBooking(
            Guid.Parse("d630ec3f-a38b-4af0-8e08-e58e53599d3c"),
            Guid.Parse("7dd06875-5f21-4f3d-97ab-48c03509bbcc"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            120m,
            new[] { (2, 9) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1) }
        );

        AddBooking(
            Guid.Parse("8d37aaee-6a37-4991-8e2e-946cc1163c8a"),
            Guid.Parse("c89b20eb-35d1-4cc6-b25f-b77ead7be687"),
            Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
            330m,
            new[] { (6, 8), (6, 9), (6, 10) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1), (childId, 2) }
        );

        AddBooking(
            Guid.Parse("b5566667-2a95-45ab-8645-6621763f6d1f"),
            Guid.Parse("1205db71-fbb9-4875-a45f-3ecbd61d4a7d"),
            Guid.Parse("1fa41231-cc27-48ca-83a2-b1ed14ba2871"),
            122m,
            new[] { (6, 8) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 1) }
        );

        AddBooking(
            Guid.Parse("2a3ce4b8-8056-46d2-8685-4882c79db8b6"),
            Guid.Parse("1205db71-fbb9-4875-a45f-3ecbd61d4a7d"),
            Guid.Parse("1fa41231-cc27-48ca-83a2-b1ed14ba2871"),
            390m,
            new[] { (5, 6), (5, 7), (5, 8), (5, 9) },
            new (Guid ticketTypeId, int qty)[] { (adultId, 2), (childId, 2) }
        );

        await db.SaveChangesAsync(ct);
    }
}


