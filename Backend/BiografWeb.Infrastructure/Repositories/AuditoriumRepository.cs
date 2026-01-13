using BiografWeb.Application.Auditoriums;
using BiografWeb.Application.Auditoriums.Models;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class AuditoriumRepository(AppDbContext db) : IAuditoriumRepository
{
    private readonly AppDbContext _db = db;

    /// <summary>
    /// Returns all auditoriums without tracking.
    /// </summary>
    /// <returns>List of auditoriums.</returns>
    public async Task<List<Auditorium>> ListAsync(CancellationToken ct = default)
        => await _db.Auditoriums.AsNoTracking().ToListAsync(ct);

    /// <summary>
    /// Returns a single auditorium by id without tracking, or null if not found.
    /// </summary>
    /// <param name="id">Auditorium id.</param>
    /// <returns>Auditorium or null.</returns>
    public async Task<Auditorium?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Auditoriums.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

    /// <summary>
    /// Persists a new auditorium and returns the created entity.
    /// </summary>
    /// <param name="a">Auditorium to create.</param>
    /// <returns>Created auditorium.</returns>
    public async Task<Auditorium> CreateAsync(Auditorium a, CancellationToken ct = default)
    {
        _db.Auditoriums.Add(a);
        await _db.SaveChangesAsync(ct);
        return a;
    }

    /// <summary>
    /// Updates an existing auditorium by id and returns the updated entity, or null if not found.
    /// </summary>
    /// <param name="id">Auditorium id.</param>
    /// <param name="a">Values to apply.</param>
    /// <returns>Updated auditorium or null.</returns>
    public async Task<Auditorium?> UpdateAsync(Guid id, Auditorium a, CancellationToken ct = default)
    {
        var existing = await _db.Auditoriums.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;

        existing.Name = a.Name;
        existing.Rows = a.Rows;
        existing.Cols = a.Cols;

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    /// <summary>
    /// Deletes an auditorium by id.
    /// </summary>
    /// <param name="id">Auditorium id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Auditoriums.FirstOrDefaultAsync(x => x.Id == id, ct);

        if (existing is null) return false;
        _db.Auditoriums.Remove(existing);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// For each auditorium, returns the earliest upcoming screening start time (or null).
    /// </summary>
    /// <returns>List of next start times per auditorium.</returns>
    public async Task<List<AuditoriumNextStartDto>> GetNextStartAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var auds = await _db.Auditoriums.AsNoTracking().ToListAsync(ct);
        var result = new List<AuditoriumNextStartDto>(auds.Count);

        foreach (var a in auds)
        {
            var nextStart = await _db.Screenings
                .Where(s => s.AuditoriumId == a.Id && s.StartTime > now)
                .OrderBy(s => s.StartTime)
                .Select(s => (DateTime?)s.StartTime)
                .FirstOrDefaultAsync(ct);
            result.Add(new AuditoriumNextStartDto { Id = a.Id, Name = a.Name, NextStartTime = nextStart });
        }

        return result;
    }

    /// <summary>
    /// For each auditorium, returns the average occupancy ratio over screenings in the next 7 days.
    /// Occupancy is computed as booked seats divided by capacity (rows * cols).
    /// </summary>
    /// <returns>List of average occupancy ratios per auditorium.</returns>
    public async Task<List<AuditoriumAvgOccupancyDto>> GetAverageOccupancyNext7DaysAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var next7 = now.AddDays(7);
        var auds = await _db.Auditoriums.AsNoTracking().ToListAsync(ct);
        var result = new List<AuditoriumAvgOccupancyDto>(auds.Count);

        foreach (var a in auds)
        {
            var capacity = a.Rows * a.Cols;
            decimal avgOcc = 0m;

            if (capacity > 0)
            {
                var screeningsNext7 = await _db.Screenings
                    .Where(s => s.AuditoriumId == a.Id && s.StartTime > now && s.StartTime <= next7)
                    .Select(s => s.Id)
                    .ToListAsync(ct);

                if (screeningsNext7.Count > 0)
                {
                    var bookedPerScreening = new List<int>(screeningsNext7.Count);

                    foreach (var sid in screeningsNext7)
                    {
                        var bookingIds = _db.Bookings.Where(b => b.ScreeningId == sid).Select(b => b.Id);
                        var booked = await _db.BookingSeats.CountAsync(bs => bookingIds.Contains(bs.BookingId), ct);
                        bookedPerScreening.Add(booked);
                    }

                    avgOcc = bookedPerScreening.Average(x => (decimal)x / capacity);
                }
            }
            result.Add(new AuditoriumAvgOccupancyDto { Id = a.Id, Name = a.Name, AverageOccupancyNext7Days = avgOcc });
        }

        return result;
    }
}


