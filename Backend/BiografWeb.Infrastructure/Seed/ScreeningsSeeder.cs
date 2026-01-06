using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Seed;

public static class ScreeningsSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Screenings.AnyAsync(ct)) return;

        var todayUtc = DateTime.UtcNow.Date;
        DateTime At(int days, int hour, int minute) => todayUtc.AddDays(days).AddHours(hour).AddMinutes(minute);

        db.Screenings.AddRange(new[]
        {
            new Screening
            {
                Id = Guid.Parse("9f8aaf45-fb1a-4c74-8f2a-6fc0b2e2ff2b"),
                MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                AuditoriumId = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"),
                StartTime = At(2, 18, 0),
                Price = 110m
            },

            new Screening
            {
                Id = Guid.Parse("e8a9c2b7-6d6e-4d2f-b3e3-3dc9c9d5a1a7"),
                MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                StartTime = At(3, 20, 30),
                Price = 120m
            },

            new Screening
            {
                Id = Guid.Parse("13d6c0b5-9f24-4af7-af0a-8d0df9f9c5c4"),
                MovieId = Guid.Parse("6c0f3d5e-0f98-4f12-97a7-13f88b95d6ec"),
                AuditoriumId = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"),
                StartTime = At(3, 17, 15),
                Price = 115m
            },

            new Screening
            {
                Id = Guid.Parse("4b2c59a0-1f6a-4a1a-9dbe-0d3f4f7a2f9e"),
                MovieId = Guid.Parse("a8199a4a-343a-4d5c-9a57-5e1f3b1e89b2"),
                AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                StartTime = At(4, 19, 45),
                Price = 125m
            },

            new Screening
            {
                Id = Guid.Parse("1205db71-fbb9-4875-a45f-3ecbd61d4a7d"),
                MovieId = Guid.Parse("6c0f3d5e-0f98-4f12-97a7-13f88b95d6ec"),
                AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                StartTime = At(9, 21, 22),
                Price = 122m
            },

            new Screening
            {
                Id = Guid.Parse("7dd06875-5f21-4f3d-97ab-48c03509bbcc"),
                MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                AuditoriumId = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"),
                StartTime = At(8, 9, 22),
                Price = 120m
            },

            new Screening
            {
                Id = Guid.Parse("c89b20eb-35d1-4cc6-b25f-b77ead7be687"),
                MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                StartTime = At(7, 23, 0),
                Price = 150m
            },

            new Screening
            {
                Id = Guid.Parse("f264a155-9f28-4188-9873-581743b19fd8"),
                MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                AuditoriumId = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"),
                StartTime = At(8, 20, 0),
                Price = 120m
            },

            new Screening
            {
                Id = Guid.Parse("41f6ce53-704b-494b-b586-d5f70ce9c78f"),
                MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                StartTime = At(8, 14, 0),
                Price = 150m
            }
        });

        await db.SaveChangesAsync(ct);
    }
}