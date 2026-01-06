using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Seed;

public static class TicketTypesSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.TicketTypes.AnyAsync(ct)) return;

        db.TicketTypes.AddRange(new[]
        {
            new TicketType { Id = Guid.Parse("d1a7f5e0-2a9c-4c52-9f6b-1eacb3f7a012"), Name = "Adult", Multiplier = 1m },
            new TicketType { Id = Guid.Parse("f3b2c1d4-5e6f-4a7b-8c9d-0a1b2c3d4e5f"), Name = "Child (under 12)", Multiplier = 0.6m }
        });

        await db.SaveChangesAsync(ct);
    }
}


