using BiografWeb.Application.TicketTypes;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class TicketTypesRepository(AppDbContext db) : ITicketTypesRepository
{
    private readonly AppDbContext _db = db;

    public async Task<List<TicketType>> ListAsync(CancellationToken ct = default)
        => await _db.TicketTypes.AsNoTracking().ToListAsync(ct);

    public async Task<TicketType?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.TicketTypes.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<TicketType> CreateAsync(TicketType tt, CancellationToken ct = default)
    {
        _db.TicketTypes.Add(tt);
        await _db.SaveChangesAsync(ct);
        return tt;
    }

    public async Task<TicketType?> UpdateAsync(Guid id, TicketType tt, CancellationToken ct = default)
    {
        var existing = await _db.TicketTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;
        existing.Name = tt.Name;
        existing.Multiplier = tt.Multiplier;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.TicketTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.TicketTypes.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}


