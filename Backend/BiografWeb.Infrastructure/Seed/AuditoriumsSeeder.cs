using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Seed;

public static class AuditoriumsSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Auditoriums.AnyAsync(ct)) return;

        db.Auditoriums.AddRange(new[]
        {
            new Auditorium { Id = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"), Name = "Alpha", Rows = 10, Cols = 12 },
            new Auditorium { Id = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"), Name = "Beta", Rows = 12, Cols = 14 }
        });

        await db.SaveChangesAsync(ct);
    }
}


