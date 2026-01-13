using BiografWeb.Domain;
using BiografWeb.Application.TicketTypes.Models;

namespace BiografWeb.Application.TicketTypes;

public class TicketTypesService(ITicketTypesRepository repo) : ITicketTypesService
{
    private readonly ITicketTypesRepository _repo = repo;

    public Task<List<TicketType>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);
    public Task<TicketType?> GetAsync(Guid id, CancellationToken ct = default) => _repo.GetAsync(id, ct);

    public async Task<TicketType> CreateAsync(TicketType tt, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(tt.Name)) throw new ArgumentException("Name required");

        if (tt.Multiplier <= 0) throw new ArgumentException("Multiplier must be positive");

        return await _repo.CreateAsync(tt, ct);
    }

    public Task<TicketType?> UpdateAsync(Guid id, TicketType tt, CancellationToken ct = default) => _repo.UpdateAsync(id, tt, ct);
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);

    public Task<List<TicketTypeInUseDto>> GetInUseCountsAsync(CancellationToken ct = default)
        => _repo.GetInUseCountsAsync(ct);

    public Task<List<TicketTypeRevenueDto>> GetRevenueAsync(CancellationToken ct = default)
        => _repo.GetRevenueAsync(ct);
}


