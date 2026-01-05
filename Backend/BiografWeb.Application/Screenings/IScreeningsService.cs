using BiografWeb.Domain;

namespace BiografWeb.Application.Screenings;

public interface IScreeningsService
{
    Task<List<Screening>> ListAsync(Guid? movieId, CancellationToken ct = default);
    Task<Screening?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Screening> CreateAsync(Screening s, CancellationToken ct = default);
    Task<Screening?> UpdateAsync(Guid id, Screening s, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}


