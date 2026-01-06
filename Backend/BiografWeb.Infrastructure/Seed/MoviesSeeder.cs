using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Seed;

public static class MoviesSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Movies.AnyAsync(ct)) return;

        db.Movies.AddRange(new[]
        {
            new Movie { Id = Guid.Parse("0f2f4b0d-0b1f-4c94-9b7a-2b2a3a3ce3c1"), Title = "The Quantum Heist", DurationMin = 125, Genre = "Sci-Fi" },
            new Movie { Id = Guid.Parse("53b0197d-b9db-4a86-9bdf-1b9a8e6f7a63"), Title = "Midnight Bistro", DurationMin = 108, Genre = "Drama" },
            new Movie { Id = Guid.Parse("6c0f3d5e-0f98-4f12-97a7-13f88b95d6ec"), Title = "Neon Knights", DurationMin = 116, Genre = "Action" },
            new Movie { Id = Guid.Parse("b44c7c85-5a57-4d8e-b2c9-2b43a1c3ed53"), Title = "Whispers in Winter", DurationMin = 102, Genre = "Mystery" },
            new Movie { Id = Guid.Parse("a8199a4a-343a-4d5c-9a57-5e1f3b1e89b2"), Title = "Orbit 9", DurationMin = 131, Genre = "Sci-Fi" },
            new Movie { Id = Guid.Parse("c2a25da5-2f1c-4eaf-9e23-5a79cd35b1d9"), Title = "Greenfields", DurationMin = 99, Genre = "Romance" },
            new Movie { Id = Guid.Parse("5d62b91f-1231-4e3d-abcd-6ea4424b5d52"), Title = "Test", DurationMin = 121, Genre = "Horror" },
            new Movie { Id = Guid.Parse("e50a7c2e-b1a8-4b2a-9fda-5018dbb0d5a1"), Title = "Crimson Harbor", DurationMin = 112, Genre = "Thriller" },
            new Movie { Id = Guid.Parse("9b4f3a62-0e71-4c63-a9a4-8f9479d8e7b2"), Title = "Starlight Echoes", DurationMin = 127, Genre = "Sci-Fi" },
            new Movie { Id = Guid.Parse("a3d7c9e0-2f51-4b30-8bfa-2b55f1d8c4e3"), Title = "Forgotten Valley", DurationMin = 104, Genre = "Adventure" },
            new Movie { Id = Guid.Parse("c7f0b6d2-4d1a-4d32-9e70-3a78e2a1b9d5"), Title = "Paper Lanterns", DurationMin = 98, Genre = "Drama" },
            new Movie { Id = Guid.Parse("d2a19f73-7b2e-4e3f-a6e1-d4b01f6c2a19"), Title = "Rogue Frequency", DurationMin = 115, Genre = "Action" },
            new Movie { Id = Guid.Parse("f8b6e4a1-2c3d-4f8a-9a2b-1c2d3e4f5a6b"), Title = "Midnight Cartographer", DurationMin = 109, Genre = "Mystery" },
            new Movie { Id = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"), Title = "Sunset Over Atlas", DurationMin = 122, Genre = "Adventure" },
            new Movie { Id = Guid.Parse("2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e"), Title = "Velvet Mirage", DurationMin = 101, Genre = "Romance" },
            new Movie { Id = Guid.Parse("3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f"), Title = "Iron Cliffs", DurationMin = 134, Genre = "Action" },
            new Movie { Id = Guid.Parse("4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f9a"), Title = "The Glass Chronicle", DurationMin = 118, Genre = "Drama" },
            new Movie { Id = Guid.Parse("5e6f7a8b-9c0d-1e2f-3a4b-5c6d7e8f9a0b"), Title = "Night Market Tales", DurationMin = 96, Genre = "Comedy" },
            new Movie { Id = Guid.Parse("6f7a8b9c-0d1e-2f3a-4b5c-6d7e8f9a0b1c"), Title = "Orbit of Ashes", DurationMin = 129, Genre = "Sci-Fi" }
        });

        await db.SaveChangesAsync(ct);
    }
}


