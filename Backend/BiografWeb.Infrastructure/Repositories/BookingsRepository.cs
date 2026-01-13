using BiografWeb.Application.Bookings;
using BiografWeb.Application.Bookings.Models;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class BookingsRepository(AppDbContext db) : IBookingsRepository
{
    private readonly AppDbContext _db = db;

    /// <summary>
    /// Returns bookings (optionally filtered by screeningId and/or userId),
    /// including seats and items, without tracking.
    /// </summary>
    /// <param name="screeningId">Optional screening filter.</param>
    /// <param name="userId">Optional user filter.</param>
    /// <returns>List of bookings.</returns>
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

    /// <summary>
    /// Returns a single booking by id, including seats and items, without tracking.
    /// </summary>
    /// <param name="id">Booking id.</param>
    /// <returns>Booking or null.</returns>
    public async Task<Booking?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    /// <summary>
    /// Persists a new booking and returns the created entity.
    /// </summary>
    /// <param name="b">Booking to create.</param>
    /// <returns>Created booking.</returns>
    public async Task<Booking> CreateAsync(Booking b, CancellationToken ct = default)
    {
        _db.Bookings.Add(b);
        await _db.SaveChangesAsync(ct);
        return b;
    }

    /// <summary>
    /// Updates an existing booking including replacing its seats and items.
    /// Returns the updated entity, or null if not found.
    /// </summary>
    /// <param name="id">Booking id.</param>
    /// <param name="input">New values to apply.</param>
    /// <returns>Updated booking or null.</returns>
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

    /// <summary>
    /// Deletes a booking by id.
    /// </summary>
    /// <param name="id">Booking id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b is null) return false;

        _db.Bookings.Remove(b);

        await _db.SaveChangesAsync(ct);

        return true;
    }

    /// <summary>
    /// Returns per-booking statistics:
    /// - UserEmail
    /// - ScreeningStart
    /// - ItemsCount (sum of quantities across booking items)
    /// </summary>
    /// <returns>List of per-booking statistics.</returns>
    public async Task<List<BookingStatsDto>> GetStatsAsync(CancellationToken ct = default)
    {
        var query =
            from b in _db.Bookings.AsNoTracking()
            join u in _db.Users.AsNoTracking() on b.UserId equals u.Id
            join s in _db.Screenings.AsNoTracking() on b.ScreeningId equals s.Id
            select new BookingStatsDto
            {
                Id = b.Id,
                UserEmail = u.Email,
                ScreeningStart = s.StartTime,
                ItemsCount = _db.BookingItems.Where(i => i.BookingId == b.Id).Select(i => (int?)i.Qty).Sum() ?? 0
            };

        return await query.ToListAsync(ct);
    }

    /// <summary>
    /// Returns detailed statistics for a single booking:
    /// - SeatCount
    /// - Itemized lines with multipliers and line totals
    /// - Total payable amount
    /// </summary>
    /// <param name="id">Booking id.</param>
    /// <returns>Detailed booking statistics, or null if not found.</returns>
    public async Task<BookingDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default)
    {
        var b = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (b is null) return null;

        var seatCount = await _db.BookingSeats.CountAsync(s => s.BookingId == id, ct);
        var price = await _db.Screenings.Where(s => s.Id == b.ScreeningId).Select(s => s.Price).FirstAsync(ct);

        var items = await (
            from i in _db.BookingItems.AsNoTracking()
            join tt in _db.TicketTypes.AsNoTracking() on i.TicketTypeId equals tt.Id
            where i.BookingId == id
            select new BookingItemDetail
            {
                TicketTypeName = tt.Name,
                Qty = i.Qty,
                Multiplier = tt.Multiplier,
                LineTotal = (decimal)i.Qty * tt.Multiplier * price
            }).ToListAsync(ct);

        var total = items.Sum(x => x.LineTotal);

        return new BookingDetailsStatsDto
        {
            Id = id,
            SeatCount = seatCount,
            Items = items,
            Total = total
        };
    }

    /// <summary>
    /// Computes total revenue across all bookings as sum of qty * ticket multiplier * screening price.
    /// </summary>
    /// <returns>Total revenue.</returns>
    public async Task<decimal> GetTotalRevenueAsync(CancellationToken ct = default)
    {
        var query =
            from b in _db.Bookings
            join i in _db.BookingItems on b.Id equals i.BookingId
            join s in _db.Screenings on b.ScreeningId equals s.Id
            join tt in _db.TicketTypes on i.TicketTypeId equals tt.Id
            select (decimal)i.Qty * tt.Multiplier * s.Price;

        var sum = await query.SumAsync(ct);

        return sum;
    }
}


