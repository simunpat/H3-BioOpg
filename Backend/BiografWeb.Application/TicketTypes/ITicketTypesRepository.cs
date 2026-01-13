using BiografWeb.Domain;
using BiografWeb.Application.TicketTypes.Models;

namespace BiografWeb.Application.TicketTypes;

public interface ITicketTypesRepository
{
    Task<List<TicketType>> ListAsync(CancellationToken ct = default);
    Task<TicketType?> GetAsync(Guid id, CancellationToken ct = default);
    Task<TicketType> CreateAsync(TicketType tt, CancellationToken ct = default);
    Task<TicketType?> UpdateAsync(Guid id, TicketType tt, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<List<TicketTypeInUseDto>> GetInUseCountsAsync(CancellationToken ct = default);
    Task<List<TicketTypeRevenueDto>> GetRevenueAsync(CancellationToken ct = default);
}


