using BiografWeb.Application.Auditoriums;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class AuditoriumRepository(AppDbContext db) : IAuditoriumRepository
{
    private readonly AppDbContext _db = db;

    public async Task<List<Auditorium>> ListAsync(CancellationToken ct = default)
        => await _db.Auditoriums.AsNoTracking().ToListAsync(ct);

    public async Task<Auditorium?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Auditoriums.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<Auditorium> CreateAsync(Auditorium a, CancellationToken ct = default)
    {
        _db.Auditoriums.Add(a);
        await _db.SaveChangesAsync(ct);
        return a;
    }

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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Auditoriums.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.Auditoriums.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}


