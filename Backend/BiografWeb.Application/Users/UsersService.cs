using BiografWeb.Domain;

namespace BiografWeb.Application.Users;

public class UsersService(IUsersRepository repo) : IUsersService
{
    private readonly IUsersRepository _repo = repo;

    public Task<List<User>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);
    public Task<User?> GetAsync(Guid id, CancellationToken ct = default) => _repo.GetAsync(id, ct);
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) => _repo.FindByEmailAsync(email, ct);

    public async Task<User> CreateAsync(User u, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(u.Email)) throw new ArgumentException("Email required");
        if (string.IsNullOrWhiteSpace(u.PasswordHash)) throw new ArgumentException("PasswordHash required");
        return await _repo.CreateAsync(u, ct);
    }

    public Task<User?> UpdateAsync(Guid id, User u, CancellationToken ct = default) => _repo.UpdateAsync(id, u, ct);
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);
}


