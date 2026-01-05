using BiografWeb.Domain;

namespace BiografWeb.Application.Bookings;

public class BookingsService(IBookingsRepository repo) : IBookingsService
{
    private readonly IBookingsRepository _repo = repo;

    public Task<List<Booking>> ListAsync(Guid? screeningId, Guid? userId, CancellationToken ct = default)
        => _repo.ListAsync(screeningId, userId, ct);

    public Task<Booking?> GetAsync(Guid id, CancellationToken ct = default)
        => _repo.GetAsync(id, ct);

    public Task<Booking> CreateAsync(Booking b, CancellationToken ct = default)
        => _repo.CreateAsync(b, ct);

    public Task<Booking?> UpdateAsync(Guid id, Booking b, CancellationToken ct = default)
        => _repo.UpdateAsync(id, b, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);
}


