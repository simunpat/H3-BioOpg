using System;
using System.Linq;
using System.Threading.Tasks;
using BiografWeb.Domain;
using BiografWeb.Infrastructure.Repositories;
using BiografWeb.Test.Infrastructure;
using Xunit;

namespace BiografWeb.Test.InfrastructureTests;

public class BookingsRepositoryTests
{
    /// <summary>
    /// Seeds bookings/items and verifies GetTotalRevenueAsync sums qty * multiplier * price across all items.
    /// </summary>
    [Fact]
    public async Task GetTotalRevenueAsync_Sums_All_Items()
    {
        await using var t = new TestDb();

        var movie = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 2, Cols = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow, Price = 80, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var ttAdult = new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 1.0m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var ttChild = new TicketType { Id = Guid.NewGuid(), Name = "Child", Multiplier = 0.5m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var b = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var i1 = new BookingItem { Id = Guid.NewGuid(), BookingId = b.Id, TicketTypeId = ttAdult.Id, Qty = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var i2 = new BookingItem { Id = Guid.NewGuid(), BookingId = b.Id, TicketTypeId = ttChild.Id, Qty = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        t.Db.AddRange(movie, aud, screening, ttAdult, ttChild, b, i1, i2);

        await t.Db.SaveChangesAsync();

        var repo = new BookingsRepository(t.Db);
        var sum = await repo.GetTotalRevenueAsync();

        Assert.Equal(2 * 1.0m * 80m + 1 * 0.5m * 80m, sum);
    }

    /// <summary>
    /// Verifies UpdateAsync replaces seats and items for an existing booking.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_Replaces_Seats_And_Items()
    {
        await using var t = new TestDb();

        var movie = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 2, Cols = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow, Price = 80, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var tt = new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 1.0m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var b = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var seat = new BookingSeat { BookingId = b.Id, Row = 1, Number = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var item = new BookingItem { Id = Guid.NewGuid(), BookingId = b.Id, TicketTypeId = tt.Id, Qty = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        t.Db.AddRange(movie, aud, screening, tt, b, seat, item);

        await t.Db.SaveChangesAsync();

        var repo = new BookingsRepository(t.Db);

        var updated = await repo.UpdateAsync(b.Id, new Booking
        {
            ScreeningId = screening.Id,
            UserId = b.UserId,
            TotalPrice = 0,
            Seats = { new BookingSeat { BookingId = b.Id, Row = 2, Number = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow } },
            Items = { new BookingItem { Id = Guid.NewGuid(), BookingId = b.Id, TicketTypeId = tt.Id, Qty = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow } }
        });

        Assert.NotNull(updated);

        var seats = t.Db.BookingSeats.Where(s => s.BookingId == b.Id).ToList();
        var items = t.Db.BookingItems.Where(i => i.BookingId == b.Id).ToList();

        Assert.Single(seats);
        Assert.Equal(2, seats[0].Row);
        Assert.Single(items);
        Assert.Equal(3, items[0].Qty);
    }
}

