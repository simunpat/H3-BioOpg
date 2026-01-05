using BiografWeb.Domain;

namespace BiografWeb.Application.TicketTypes;

public interface ITicketTypesService
{
    Task<List<TicketType>> ListAsync(CancellationToken ct = default);
    Task<TicketType?> GetAsync(Guid id, CancellationToken ct = default);
    Task<TicketType> CreateAsync(TicketType tt, CancellationToken ct = default);
    Task<TicketType?> UpdateAsync(Guid id, TicketType tt, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}


