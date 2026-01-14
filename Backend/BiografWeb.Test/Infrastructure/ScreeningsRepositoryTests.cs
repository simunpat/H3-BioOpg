using System;
using System.Linq;
using System.Threading.Tasks;
using BiografWeb.Domain;
using BiografWeb.Infrastructure.Repositories;
using BiografWeb.Test.Infrastructure;
using Xunit;

namespace BiografWeb.Test.InfrastructureTests;

public class ScreeningsRepositoryTests
{
    /// <summary>
    /// Verifies that ListAsync correctly filters screenings by the provided movieId.
    /// </summary>
    [Fact]
    public async Task ListAsync_Filters_By_MovieId()
    {
        await using var t = new TestDb();
        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 5, Cols = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        t.Db.Auditoriums.Add(aud);

        var m1 = new Movie { Id = Guid.NewGuid(), Title = "M1", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var m2 = new Movie { Id = Guid.NewGuid(), Title = "M2", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        t.Db.Movies.AddRange(m1, m2);
        t.Db.Screenings.Add(new Screening { Id = Guid.NewGuid(), MovieId = m1.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow.AddHours(1), Price = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        t.Db.Screenings.Add(new Screening { Id = Guid.NewGuid(), MovieId = m2.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow.AddHours(2), Price = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

        await t.Db.SaveChangesAsync();

        var repo = new ScreeningsRepository(t.Db);
        var all = await repo.ListAsync(null);
        var onlyM1 = await repo.ListAsync(m1.Id);

        Assert.Equal(2, all.Count);
        Assert.Single(onlyM1);
        Assert.Equal(m1.Id, onlyM1.Single().MovieId);
    }

    /// <summary>
    /// Verifies that per-screening stats compute booking count and available seats from reserved seats and capacity.
    /// </summary>
    [Fact]
    public async Task GetStatsAsync_Computes_AvailableSeats_And_BookingCount()
    {
        await using var t = new TestDb();
        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 3, Cols = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var movie = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow.AddHours(1), Price = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        t.Db.Auditoriums.Add(aud);
        t.Db.Movies.Add(movie);
        t.Db.Screenings.Add(screening);

        // 2 bookings: 3 seats reserved total
        var b1 = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var b2 = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        t.Db.Bookings.AddRange(b1, b2);
        t.Db.BookingSeats.AddRange(
            new BookingSeat { BookingId = b1.Id, Row = 1, Number = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new BookingSeat { BookingId = b1.Id, Row = 1, Number = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new BookingSeat { BookingId = b2.Id, Row = 2, Number = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        await t.Db.SaveChangesAsync();

        var repo = new ScreeningsRepository(t.Db);
        var stats = await repo.GetStatsAsync();
        var s = stats.Single(x => x.Id == screening.Id);

        Assert.Equal(2, s.BookingCount);
        Assert.Equal(aud.Rows * aud.Cols - 3, s.AvailableSeats);
    }

    /// <summary>
    /// Verifies that per-screening detailed stats compute revenue as sum of qty * ticket multiplier * screening price.
    /// </summary>
    [Fact]
    public async Task GetStatsByIdAsync_Computes_Revenue_Estimate()
    {
        await using var t = new TestDb();

        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 2, Cols = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var movie = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow.AddHours(1), Price = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var tt = new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 1.5m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var booking = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        t.Db.AddRange(aud, movie, screening, tt, booking);
        t.Db.BookingItems.Add(new BookingItem { Id = Guid.NewGuid(), BookingId = booking.Id, TicketTypeId = tt.Id, Qty = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await t.Db.SaveChangesAsync();

        var repo = new ScreeningsRepository(t.Db);
        var detail = await repo.GetStatsByIdAsync(screening.Id);

        Assert.NotNull(detail);
        Assert.Equal(100m * 1.5m * 2, detail!.RevenueEstimate);
    }
}

