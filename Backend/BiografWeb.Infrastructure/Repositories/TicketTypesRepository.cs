using BiografWeb.Application.TicketTypes;
using BiografWeb.Application.TicketTypes.Models;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class TicketTypesRepository(AppDbContext db) : ITicketTypesRepository
{
    private readonly AppDbContext _db = db;

    /// <summary>
    /// Returns all ticket types without tracking.
    /// </summary>
    /// <returns>List of ticket types.</returns>
    public async Task<List<TicketType>> ListAsync(CancellationToken ct = default)
        => await _db.TicketTypes.AsNoTracking().ToListAsync(ct);

    /// <summary>
    /// Returns a single ticket type by id without tracking, or null if not found.
    /// </summary>
    /// <param name="id">Ticket type id.</param>
    /// <returns>Ticket type or null.</returns>
    public async Task<TicketType?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.TicketTypes.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    /// <summary>
    /// Persists a new ticket type and returns the created entity.
    /// </summary>
    /// <param name="tt">Ticket type to create.</param>
    /// <returns>Created ticket type.</returns>
    public async Task<TicketType> CreateAsync(TicketType tt, CancellationToken ct = default)
    {
        _db.TicketTypes.Add(tt);
        await _db.SaveChangesAsync(ct);
        return tt;
    }

    /// <summary>
    /// Updates an existing ticket type by id and returns the updated entity, or null if not found.
    /// </summary>
    /// <param name="id">Ticket type id.</param>
    /// <param name="tt">Values to apply.</param>
    /// <returns>Updated ticket type or null.</returns>
    public async Task<TicketType?> UpdateAsync(Guid id, TicketType tt, CancellationToken ct = default)
    {
        var existing = await _db.TicketTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;

        existing.Name = tt.Name;
        existing.Multiplier = tt.Multiplier;

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    /// <summary>
    /// Deletes a ticket type by id.
    /// </summary>
    /// <param name="id">Ticket type id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.TicketTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;

        _db.TicketTypes.Remove(existing);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Returns per-ticket-type usage counts (number of booking items using each type).
    /// </summary>
    /// <returns>List with in-use counts per ticket type.</returns>
    public async Task<List<TicketTypeInUseDto>> GetInUseCountsAsync(CancellationToken ct = default)
    {
        var tts = await _db.TicketTypes.AsNoTracking().ToListAsync(ct);
        var result = new List<TicketTypeInUseDto>(tts.Count);

        foreach (var t in tts)
        {
            var inUse = await _db.BookingItems.CountAsync(i => i.TicketTypeId == t.Id, ct);
            result.Add(new TicketTypeInUseDto { Id = t.Id, Name = t.Name, InUseCount = inUse });
        }

        return result;
    }

    /// <summary>
    /// Returns per-ticket-type revenue totals:
    /// sum of qty * ticket multiplier * screening price for all items of each type.
    /// </summary>
    /// <returns>List with revenue totals per ticket type.</returns>
    public async Task<List<TicketTypeRevenueDto>> GetRevenueAsync(CancellationToken ct = default)
    {
        var tts = await _db.TicketTypes.AsNoTracking().ToListAsync(ct);
        var result = new List<TicketTypeRevenueDto>(tts.Count);

        foreach (var t in tts)
        {
            var revenueDouble = await (
                from i in _db.BookingItems
                join b in _db.Bookings on i.BookingId equals b.Id
                join s in _db.Screenings on b.ScreeningId equals s.Id
                where i.TicketTypeId == t.Id
                select (double)i.Qty * (double)t.Multiplier * (double)s.Price
            ).SumAsync(ct);
            result.Add(new TicketTypeRevenueDto { Id = t.Id, Name = t.Name, TotalRevenue = (decimal)revenueDouble });
        }

        return result;
    }
}


