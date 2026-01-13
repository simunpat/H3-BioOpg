using BiografWeb.Domain;
using BiografWeb.Application.Screenings.Models;

namespace BiografWeb.Application.Screenings;

public class ScreeningsService(IScreeningsRepository repo) : IScreeningsService
{
    private readonly IScreeningsRepository _repo = repo;

    public Task<List<Screening>> ListAsync(Guid? movieId, CancellationToken ct = default)
        => _repo.ListAsync(movieId, ct);

    public Task<Screening?> GetAsync(Guid id, CancellationToken ct = default)
        => _repo.GetAsync(id, ct);

    public Task<Screening> CreateAsync(Screening s, CancellationToken ct = default)
        => _repo.CreateAsync(s, ct);

    public Task<Screening?> UpdateAsync(Guid id, Screening s, CancellationToken ct = default)
        => _repo.UpdateAsync(id, s, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    public Task<List<ScreeningStatsDto>> GetStatsAsync(CancellationToken ct = default)
        => _repo.GetStatsAsync(ct);

    public Task<ScreeningDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default)
        => _repo.GetStatsByIdAsync(id, ct);
}


