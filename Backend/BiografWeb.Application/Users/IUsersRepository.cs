using BiografWeb.Domain;
using BiografWeb.Application.Users.Models;

namespace BiografWeb.Application.Users;

public interface IUsersRepository
{
    Task<List<User>> ListAsync(CancellationToken ct = default);
    Task<User?> GetAsync(Guid id, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<User> CreateAsync(User u, CancellationToken ct = default);
    Task<User?> UpdateAsync(Guid id, User u, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<List<UserStatsDto>> GetStatsAsync(CancellationToken ct = default);
    Task<UserDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default);
}


