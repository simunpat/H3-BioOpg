using BiografWeb.Domain;
using BiografWeb.Application.Auditoriums.Models;

namespace BiografWeb.Application.Auditoriums;

public class AuditoriumService(IAuditoriumRepository repo) : IAuditoriumService
{
    private readonly IAuditoriumRepository _repo = repo;

    public Task<List<Auditorium>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);

    public Task<Auditorium?> GetAsync(Guid id, CancellationToken ct = default) => _repo.GetAsync(id, ct);

    public async Task<Auditorium> CreateAsync(Auditorium a, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(a.Name)) throw new ArgumentException("Name required");
        if (a.Rows <= 0 || a.Cols <= 0) throw new ArgumentException("Rows/Cols must be positive");
        return await _repo.CreateAsync(a, ct);
    }

    public Task<Auditorium?> UpdateAsync(Guid id, Auditorium a, CancellationToken ct = default)
        => _repo.UpdateAsync(id, a, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);

    public Task<List<AuditoriumNextStartDto>> GetNextStartAsync(CancellationToken ct = default)
        => _repo.GetNextStartAsync(ct);

    public Task<List<AuditoriumAvgOccupancyDto>> GetAverageOccupancyNext7DaysAsync(CancellationToken ct = default)
        => _repo.GetAverageOccupancyNext7DaysAsync(ct);
}


