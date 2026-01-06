using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Seed;

public static class UsersSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(ct)) return;

        db.Users.AddRange(new[]
        {
            new User
            {
                Id = Guid.Parse("990610e1-9d38-41f6-a8a1-ef7df515f1dc"),
                Email = "test@test.test",
                Role = "Admin",
                PasswordHash = "27aca0ecd35b29530e2842dd5505737a3ac30ee41e49fdee9ed4bdb5d3c76a7e",
                PasswordSalt = "e357dc6e-24ec-427d-ad12-65e4ae4604ca"
            },

            new User
            {
                Id = Guid.Parse("2153115a-32f4-4696-9fef-1b4c2df4c4ef"),
                Email = "sn@lunnar.fo",
                Role = "Admin",
                PasswordHash = "dbbd4fd39918e8ab6e88cb8d24d175787cc05cb7df706952743bd4d62461f4fb",
                PasswordSalt = "d2d09e62-e044-4f88-aae0-29932e3a9443"
            },

            new User
            {
                Id = Guid.Parse("6324fba5-5159-4f0b-ad91-6a5ebeea184a"),
                Email = "ab@ab.ab",
                Role = "Customer",
                PasswordHash = "28f8d7da2be56e8cda362041d522fdb4557b568aca9d1c1b6a3f6f06021a9656",
                PasswordSalt = "2ce8ce70-01d3-4543-bcf1-e2d09e5c55f0"
            },

            new User
            {
                Id = Guid.Parse("1fa41231-cc27-48ca-83a2-b1ed14ba2871"),
                Email = "oliver@tec.dk",
                Role = "Customer",
                PasswordHash = "6f6ed4749b0692cb0c40526a2aa9afe711a94936593ccf57857989bbeee9cb4d",
                PasswordSalt = "d2839be3-41a4-4846-b164-4dc51102bc20"
            }
        });

        await db.SaveChangesAsync(ct);
    }
}


