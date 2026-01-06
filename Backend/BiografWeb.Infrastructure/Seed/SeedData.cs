namespace BiografWeb.Infrastructure.Seed;

public static class SeedData
{
    public static async Task ApplyAsync(AppDbContext db, CancellationToken ct = default)
    {
        await MoviesSeeder.SeedAsync(db, ct);
        await AuditoriumsSeeder.SeedAsync(db, ct);
        await TicketTypesSeeder.SeedAsync(db, ct);
        await UsersSeeder.SeedAsync(db, ct);
        await ScreeningsSeeder.SeedAsync(db, ct);
        await BookingsSeeder.SeedAsync(db, ct);
    }
}

