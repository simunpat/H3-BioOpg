using BiografWeb.Application.Users;
using BiografWeb.Application.Users.Models;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class UsersRepository(AppDbContext db) : IUsersRepository
{
    private readonly AppDbContext _db = db;

    /// <summary>
    /// Returns all users without tracking.
    /// </summary>
    /// <returns>List of users.</returns>
    public async Task<List<User>> ListAsync(CancellationToken ct = default)
        => await _db.Users.AsNoTracking().ToListAsync(ct);

    /// <summary>
    /// Returns a single user by id without tracking, or null if not found.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns>User or null.</returns>
    public async Task<User?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    /// <summary>
    /// Returns a single user by email without tracking, or null if not found.
    /// </summary>
    /// <param name="email">User email.</param>
    /// <returns>User or null.</returns>
    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    /// <summary>
    /// Persists a new user and returns the created entity.
    /// </summary>
    /// <param name="u">User to create.</param>
    /// <returns>Created user.</returns>
    public async Task<User> CreateAsync(User u, CancellationToken ct = default)
    {
        _db.Users.Add(u);
        await _db.SaveChangesAsync(ct);
        return u;
    }

    /// <summary>
    /// Updates an existing user by id and returns the updated entity, or null if not found.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <param name="u">Values to apply.</param>
    /// <returns>Updated user or null.</returns>
    public async Task<User?> UpdateAsync(Guid id, User u, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;
        existing.Email = u.Email;
        existing.IsAdmin = u.IsAdmin;
        existing.PasswordHash = u.PasswordHash;
        existing.PasswordSalt = u.PasswordSalt;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    /// <summary>
    /// Deletes a user by id.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.Users.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Returns per-user statistics:
    /// - BookingsCount
    /// - LastBookingAt (latest booking timestamp or null)
    /// </summary>
    /// <returns>List of per-user statistics.</returns>
    public async Task<List<UserStatsDto>> GetStatsAsync(CancellationToken ct = default)
    {
        var users = await _db.Users.AsNoTracking().ToListAsync(ct);
        var result = new List<UserStatsDto>(users.Count);

        foreach (var u in users)
        {
            var bookingsCount = await _db.Bookings.CountAsync(b => b.UserId == u.Id, ct);
            var lastBooking = await _db.Bookings
                .Where(b => b.UserId == u.Id)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => (DateTime?)b.CreatedAt)
                .FirstOrDefaultAsync(ct);

            result.Add(new UserStatsDto
            {
                Id = u.Id,
                Email = u.Email,
                BookingsCount = bookingsCount,
                LastBookingAt = lastBooking
            });
        }

        return result;
    }

    /// <summary>
    /// Returns detailed statistics for a single user:
    /// - NextScreeningStart: earliest upcoming screening booked by the user (null if none)
    /// - TotalSpent: sum of qty * ticket multiplier * screening price across all bookings
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns>Detailed user statistics, or null if not found.</returns>
    public async Task<UserDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default)
    {
        var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u is null) return null;

        var now = DateTime.UtcNow;

        var nextStart = await (
            from b in _db.Bookings
            join s in _db.Screenings on b.ScreeningId equals s.Id
            where b.UserId == id && s.StartTime > now
            orderby s.StartTime
            select (DateTime?)s.StartTime
        ).FirstOrDefaultAsync(ct);

        var total = await (
            from b in _db.Bookings
            join s in _db.Screenings on b.ScreeningId equals s.Id
            join i in _db.BookingItems on b.Id equals i.BookingId
            join tt in _db.TicketTypes on i.TicketTypeId equals tt.Id
            where b.UserId == id
            select (decimal)i.Qty * tt.Multiplier * s.Price
        ).SumAsync(ct);

        return new UserDetailsStatsDto
        {
            Id = id,
            NextScreeningStart = nextStart,
            TotalSpent = total
        };
    }
}


