using System;
using System.Linq;
using System.Threading.Tasks;
using BiografWeb.Domain;
using BiografWeb.Infrastructure.Repositories;
using BiografWeb.Test.Infrastructure;
using Xunit;

namespace BiografWeb.Test.InfrastructureTests;

public class TicketTypesRepositoryTests
{
    /// <summary>
    /// Verifies GetInUseCountsAsync counts booking item rows per ticket type (not quantities).
    /// </summary>
    [Fact]
    public async Task GetInUseCountsAsync_Reflects_BookingItems()
    {
        await using var t = new TestDb();

        var tt = new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 1m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var movie = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 1, Cols = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow, Price = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var booking = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var item = new BookingItem { Id = Guid.NewGuid(), BookingId = booking.Id, TicketTypeId = tt.Id, Qty = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        t.Db.AddRange(tt, movie, aud, screening, booking, item);

        await t.Db.SaveChangesAsync();

        var repo = new TicketTypesRepository(t.Db);
        var res = await repo.GetInUseCountsAsync();
        var row = res.Single(r => r.Id == tt.Id);

        Assert.Equal(1, row.InUseCount); // booking items count entries, not quantities
    }

    /// <summary>
    /// Verifies GetRevenueAsync sums qty * multiplier * price per ticket type.
    /// </summary>
    [Fact]
    public async Task GetRevenueAsync_Sums_Qty_Multiplier_Price()
    {
        await using var t = new TestDb();

        var tt = new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 2m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var movie = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var aud = new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 1, Cols = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var screening = new Screening { Id = Guid.NewGuid(), MovieId = movie.Id, AuditoriumId = aud.Id, StartTime = DateTime.UtcNow, Price = 50, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var booking = new Booking { Id = Guid.NewGuid(), ScreeningId = screening.Id, UserId = Guid.NewGuid(), TotalPrice = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var item = new BookingItem { Id = Guid.NewGuid(), BookingId = booking.Id, TicketTypeId = tt.Id, Qty = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        t.Db.AddRange(tt, movie, aud, screening, booking, item);

        await t.Db.SaveChangesAsync();

        var repo = new TicketTypesRepository(t.Db);
        var res = await repo.GetRevenueAsync();
        var row = res.Single(r => r.Id == tt.Id);

        Assert.Equal(4 * 2m * 50m, row.TotalRevenue);
    }
}

