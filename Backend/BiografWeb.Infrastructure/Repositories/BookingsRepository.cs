using BiografWeb.Application.Bookings;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class BookingsRepository(AppDbContext db) : IBookingsRepository
{
    private readonly AppDbContext _db = db;

    public async Task<List<Booking>> ListAsync(Guid? screeningId, Guid? userId, CancellationToken ct = default)
    {
        var q = _db.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Items)
            .AsNoTracking()
            .AsQueryable();

        if (screeningId.HasValue) q = q.Where(b => b.ScreeningId == screeningId.Value);
        if (userId.HasValue) q = q.Where(b => b.UserId == userId.Value);

        return await q.ToListAsync(ct);
    }

    public async Task<Booking?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<Booking> CreateAsync(Booking b, CancellationToken ct = default)
    {
        _db.Bookings.Add(b);
        await _db.SaveChangesAsync(ct);
        return b;
    }

    public async Task<Booking?> UpdateAsync(Guid id, Booking input, CancellationToken ct = default)
    {
        var b = await _db.Bookings
            .Include(x => x.Seats)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b is null) return null;

        b.ScreeningId = input.ScreeningId;
        b.UserId = input.UserId;
        b.TotalPrice = input.TotalPrice;

        // Replace seats/items for simplicity
        _db.BookingSeats.RemoveRange(b.Seats);
        _db.BookingItems.RemoveRange(b.Items);
        b.Seats = input.Seats;
        b.Items = input.Items;

        await _db.SaveChangesAsync(ct);
        return b;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b is null) return false;
        _db.Bookings.Remove(b);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}


