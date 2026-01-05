using BiografWeb.Domain;

namespace BiografWeb.Application.Bookings;

public interface IBookingsService
{
    Task<List<Booking>> ListAsync(Guid? screeningId, Guid? userId, CancellationToken ct = default);
    Task<Booking?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Booking> CreateAsync(Booking b, CancellationToken ct = default);
    Task<Booking?> UpdateAsync(Guid id, Booking b, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}


