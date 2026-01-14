using System;
using System.Threading.Tasks;
using BiografWeb.Domain;

namespace BiografWeb.Test.Infrastructure;

public static class Seeding
{
    public static async Task<(Movie movie, Screening screening, Auditorium auditorium)> AddMovieScreeningAsync(AppDbWrapper dbw, decimal price = 100m, int rows = 5, int cols = 5)
    {
        var movie = new Movie { Id = Guid.NewGuid(), Title = "Test Movie", Genre = "Test", DurationMin = 120, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var auditorium = new Auditorium { Id = Guid.NewGuid(), Name = "A1", Rows = rows, Cols = cols, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = auditorium.Id, StartTime = DateTime.UtcNow.AddDays(1), Price = price, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        dbw.Db.Movies.Add(movie);
        dbw.Db.Auditoriums.Add(auditorium);
        dbw.Db.Screenings.Add(screening);

        await dbw.Db.SaveChangesAsync();

        return (movie, screening, auditorium);
    }

    public static async Task<(User user, TicketType adult, TicketType child)> AddUserAndTicketTypesAsync(AppDbWrapper dbw)
    {
        var user = new User { Id = Guid.NewGuid(), Email = "user@test", IsAdmin = false, PasswordHash = "x", PasswordSalt = "y", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var adult = new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 1.0m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var child = new TicketType { Id = Guid.NewGuid(), Name = "Child", Multiplier = 0.5m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        dbw.Db.Users.Add(user);
        dbw.Db.TicketTypes.Add(adult);
        dbw.Db.TicketTypes.Add(child);

        await dbw.Db.SaveChangesAsync();

        return (user, adult, child);
    }
}

public sealed class AppDbWrapper : IAsyncDisposable
{
    public BiografWeb.Infrastructure.AppDbContext Db { get; }

    public AppDbWrapper(BiografWeb.Infrastructure.AppDbContext db)
    {
        Db = db;
    }

    public ValueTask DisposeAsync() => new();
}

