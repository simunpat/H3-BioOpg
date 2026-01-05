using BiografWeb.Application.Screenings;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class ScreeningsRepository(AppDbContext db) : IScreeningsRepository
{
    private readonly AppDbContext _db = db;

    public async Task<List<Screening>> ListAsync(Guid? movieId, CancellationToken ct = default)
    {
        var q = _db.Screenings.AsNoTracking().AsQueryable();
        if (movieId.HasValue) q = q.Where(s => s.MovieId == movieId.Value);
        return await q.OrderBy(s => s.StartTime).ToListAsync(ct);
    }

    public async Task<Screening?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Screenings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Screening> CreateAsync(Screening s, CancellationToken ct = default)
    {
        _db.Screenings.Add(s);
        await _db.SaveChangesAsync(ct);
        return s;
    }

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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Screenings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.Screenings.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}


