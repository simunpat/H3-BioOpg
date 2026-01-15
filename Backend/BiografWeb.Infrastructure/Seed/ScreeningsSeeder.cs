using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BiografWeb.Infrastructure.Seed;

public static class ScreeningsSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        var todayUtc = DateTime.UtcNow.Date;
        DateTime At(int days, int hour, int minute) => todayUtc.AddDays(days).AddHours(hour).AddMinutes(minute);

        // Insert the original sample screenings only on an empty table (idempotent)
        if (!await db.Screenings.AnyAsync(ct))
        {
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
                    StartTime = At(6, 21, 22), // capped to within 6 days
                    Price = 122m
                },

                new Screening
                {
                    Id = Guid.Parse("7dd06875-5f21-4f3d-97ab-48c03509bbcc"),
                    MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                    AuditoriumId = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"),
                    StartTime = At(6, 9, 22), // capped to within 6 days
                    Price = 120m
                },

                new Screening
                {
                    Id = Guid.Parse("c89b20eb-35d1-4cc6-b25f-b77ead7be687"),
                    MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                    AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                    StartTime = At(5, 23, 0), // capped to within 6 days
                    Price = 150m
                },

                new Screening
                {
                    Id = Guid.Parse("f264a155-9f28-4188-9873-581743b19fd8"),
                    MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                    AuditoriumId = Guid.Parse("7f9b221e-3d18-4e1a-8a5a-4a0f1a9dd829"),
                    StartTime = At(6, 20, 0), // capped to within 6 days
                    Price = 120m
                },

                new Screening
                {
                    Id = Guid.Parse("41f6ce53-704b-494b-b586-d5f70ce9c78f"),
                    MovieId = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"),
                    AuditoriumId = Guid.Parse("e1a0be96-7013-4e70-b2d7-1b745dafc5c4"),
                    StartTime = At(6, 14, 0), // capped to within 6 days
                    Price = 150m
                }
            });
        }

        // Pick a random subset of movies and add 4–8 new screenings for each, within [today, today+6 days].
        var movies = await db.Movies.AsNoTracking().ToListAsync(ct);
        var auds = await db.Auditoriums.AsNoTracking().ToListAsync(ct);

        if (movies.Any() && auds.Any())
        {
            // All existing screenings (for duplicate/time-window checks)
            var existingAll = await db.Screenings.AsNoTracking().ToListAsync(ct);
            int audIdx = 0;

            foreach (var m in movies)
            {
                // Deterministic selection: pick about two-thirds of movies
                var bytes = m.Id.ToByteArray();
                var pick = (bytes[0] % 3) != 0; // ~66.7% selected

                if (!pick) continue; // skip this movie (we don't select all)

                // Count existing screenings for this movie within the next 6 days
                var inWindow = existingAll
                    .Where(s => s.MovieId == m.Id)
                    .Where(s =>
                    {
                        var delta = (s.StartTime.Date - todayUtc).TotalDays;

                        return delta >= 0 && delta <= 6;
                    })
                    .Select(s => s.StartTime)
                    .ToList();

                var startsSet = new HashSet<DateTime>(inWindow);

                // Create a deterministic RNG for this movie
                var seed = BitConverter.ToInt32(bytes, 0);
                var rng = new Random(seed);

                // Candidate showtime templates (typical show hours)
                var timeOptions = new (int hour, int minute)[] { (16, 30), (18, 0), (19, 30), (21, 0) };

                // Decide how many to add for this movie: 4..8 inclusive
                var desiredAddCount = rng.Next(4, 9);
                int added = 0;
                int attempts = 0;

                while (added < desiredAddCount && attempts < 40)
                {
                    attempts++;

                    var dayOffset = rng.Next(0, 7); // 0..6
                    var t = timeOptions[rng.Next(0, timeOptions.Length)];
                    var start = todayUtc.AddDays(dayOffset).AddHours(t.hour).AddMinutes(t.minute);

                    // Ensure within window and not duplicate for this movie
                    var delta = (start.Date - todayUtc).TotalDays;

                    if (delta < 0 || delta > 6) continue;

                    if (startsSet.Contains(start)) continue;

                    var aud = auds[audIdx % auds.Count];
                    audIdx++;

                    db.Screenings.Add(new Screening
                    {
                        Id = Guid.NewGuid(),
                        MovieId = m.Id,
                        AuditoriumId = aud.Id,
                        StartTime = start,
                        Price = 100m + Convert.ToDecimal((m.DurationMin % 4) * 5)
                    });

                    // Track the newly added start time locally to avoid duplicates
                    startsSet.Add(start);
                    added++;
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}