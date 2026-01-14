using System;
using System.Linq;
using System.Threading.Tasks;
using BiografWeb.Infrastructure.Repositories;
using BiografWeb.Test.Infrastructure;
using Xunit;

namespace BiografWeb.Test.InfrastructureTests;

public class MovieRepositoryTests
{
    /// <summary>
    /// Seeds a movie with two screenings and verifies per-movie stats:
    /// - total screenings count, average price, and next future start time.
    /// </summary>
    [Fact]
    public async Task GetStatsAsync_Computes_Count_Average_NextStart()
    {
        await using var t = new TestDb();

        // Arrange: one movie with 2 screenings (now+1h price=100, now+2h price=200)
        var movie = new BiografWeb.Domain.Movie
        {
            Id = Guid.NewGuid(),
            Title = "M",
            Genre = "G",
            DurationMin = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        t.Db.Movies.Add(movie);

        var aud = new BiografWeb.Domain.Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 10, Cols = 10, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        t.Db.Auditoriums.Add(aud);
        t.Db.Screenings.Add(new BiografWeb.Domain.Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow.AddHours(1), Price = 100m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        t.Db.Screenings.Add(new BiografWeb.Domain.Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow.AddHours(2), Price = 200m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

        await t.Db.SaveChangesAsync();

        var repo = new MovieRepository(t.Db);

        // Act
        var stats = await repo.GetStatsAsync();
        var s = stats.Single(x => x.Id == movie.Id);

        // Assert
        Assert.Equal(2, s.ScreeningsCount);
        Assert.True(s.AveragePrice >= 100m && s.AveragePrice <= 200m);
        Assert.True(s.NextStartTime.HasValue);
        Assert.True(s.NextStartTime!.Value > DateTime.UtcNow);
    }

    /// <summary>
    /// Verifies GetStatsByIdAsync returns null for an unknown movie id.
    /// </summary>
    [Fact]
    public async Task GetStatsByIdAsync_Returns_Null_When_Missing()
    {
        await using var t = new TestDb();
        var repo = new MovieRepository(t.Db);
        var r = await repo.GetStatsByIdAsync(Guid.NewGuid());

        Assert.Null(r);
    }
}

