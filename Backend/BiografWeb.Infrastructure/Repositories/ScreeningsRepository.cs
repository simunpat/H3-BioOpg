using BiografWeb.Application.Screenings;
using BiografWeb.Application.Screenings.Models;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class ScreeningsRepository(AppDbContext db) : IScreeningsRepository
{
    private readonly AppDbContext _db = db;

    /// <summary>
    /// Returns screenings filtered by optional movie id, ordered by start time, without tracking.
    /// </summary>
    /// <param name="movieId">Optional movie id to filter by.</param>
    /// <returns>List of screenings.</returns>
    public async Task<List<Screening>> ListAsync(Guid? movieId, CancellationToken ct = default)
    {
        var q = _db.Screenings.AsNoTracking().AsQueryable();
        if (movieId.HasValue) q = q.Where(s => s.MovieId == movieId.Value);
        return await q.OrderBy(s => s.StartTime).ToListAsync(ct);
    }

    /// <summary>
    /// Returns a single screening by id without tracking, or null if not found.
    /// </summary>
    /// <param name="id">Screening id.</param>
    /// <returns>Screening or null.</returns>
    public async Task<Screening?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Screenings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <summary>
    /// Persists a new screening and returns the created entity.
    /// </summary>
    /// <param name="s">Screening to create.</param>
    /// <returns>Created screening.</returns>
    public async Task<Screening> CreateAsync(Screening s, CancellationToken ct = default)
    {
        _db.Screenings.Add(s);
        await _db.SaveChangesAsync(ct);
        return s;
    }

    /// <summary>
    /// Updates an existing screening by id and returns the updated entity, or null if not found.
    /// </summary>
    /// <param name="id">Screening id.</param>
    /// <param name="s">Values to apply.</param>
    /// <returns>Updated screening or null.</returns>
    public async Task<Screening?> UpdateAsync(Guid id, Screening s, CancellationToken ct = default)
    {
        var existing = await _db.Screenings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;
        existing.MovieId = s.MovieId;
        existing.AuditoriumId = s.AuditoriumId;
        existing.StartTime = s.StartTime;
        existing.Price = s.Price;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    /// <summary>
    /// Deletes a screening by id.
    /// </summary>
    /// <param name="id">Screening id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Screenings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.Screenings.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Returns per-screening statistics:
    /// - MovieTitle, AuditoriumName
    /// - BookingCount
    /// - AvailableSeats (capacity minus reserved seats)
    /// </summary>
    /// <returns>List of per-screening statistics.</returns>
    public async Task<List<ScreeningStatsDto>> GetStatsAsync(CancellationToken ct = default)
    {
        var screenings = await _db.Screenings.AsNoTracking().ToListAsync(ct);
        var movieTitles = await _db.Movies.AsNoTracking().ToDictionaryAsync(m => m.Id, m => m.Title, ct);
        var auds = await _db.Auditoriums.AsNoTracking().ToDictionaryAsync(a => a.Id, a => new { a.Name, a.Rows, a.Cols }, ct);

        var result = new List<ScreeningStatsDto>(screenings.Count);

        foreach (var s in screenings)
        {
            var bookingsCount = await _db.Bookings.CountAsync(b => b.ScreeningId == s.Id, ct);
            var bookingIds = _db.Bookings.Where(b => b.ScreeningId == s.Id).Select(b => b.Id);
            var bookedSeats = await _db.BookingSeats.CountAsync(bs => bookingIds.Contains(bs.BookingId), ct);
            var capacity = auds.TryGetValue(s.AuditoriumId, out var aud) ? aud.Rows * aud.Cols : 0;
            var available = Math.Max(0, capacity - bookedSeats);

            result.Add(new ScreeningStatsDto
            {
                Id = s.Id,
                MovieTitle = movieTitles.TryGetValue(s.MovieId, out var t) ? t : string.Empty,
                AuditoriumName = auds.TryGetValue(s.AuditoriumId, out var a) ? a.Name : string.Empty,
                BookingCount = bookingsCount,
                AvailableSeats = available,
            });
        }

        return result;
    }

    /// <summary>
    /// Returns detailed statistics for a single screening:
    /// - BookedSeats: total reserved seats
    /// - RevenueEstimate: sum of qty * ticket multiplier * screening price
    /// </summary>
    /// <param name="id">Screening id.</param>
    /// <returns>Detailed screening statistics, or null if not found.</returns>
    public async Task<ScreeningDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default)
    {
        var exists = await _db.Screenings.AsNoTracking().AnyAsync(s => s.Id == id, ct);
        if (!exists) return null;

        var bookingIds = _db.Bookings.Where(b => b.ScreeningId == id).Select(b => b.Id);
        var bookedSeats = await _db.BookingSeats.CountAsync(bs => bookingIds.Contains(bs.BookingId), ct);

        var price = await _db.Screenings.Where(s => s.Id == id).Select(s => (double)s.Price).FirstAsync(ct);

        var revenueDouble = await (
            from i in _db.BookingItems
            join tt in _db.TicketTypes on i.TicketTypeId equals tt.Id
            where bookingIds.Contains(i.BookingId)
            select (double)i.Qty * (double)tt.Multiplier * price
        ).SumAsync(ct);

        return new ScreeningDetailsStatsDto
        {
            Id = id,
            BookedSeats = bookedSeats,
            RevenueEstimate = (decimal)revenueDouble
        };
    }
}


