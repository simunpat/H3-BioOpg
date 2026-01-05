using BiografWeb.Domain;

namespace BiografWeb.Application.Auditoriums;

public interface IAuditoriumRepository
{
    Task<List<Auditorium>> ListAsync(CancellationToken ct = default);
    Task<Auditorium?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Auditorium> CreateAsync(Auditorium a, CancellationToken ct = default);
    Task<Auditorium?> UpdateAsync(Guid id, Auditorium a, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}


