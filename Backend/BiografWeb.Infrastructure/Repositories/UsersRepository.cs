using BiografWeb.Application.Users;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class UsersRepository(AppDbContext db) : IUsersRepository
{
    private readonly AppDbContext _db = db;

    public async Task<List<User>> ListAsync(CancellationToken ct = default)
        => await _db.Users.AsNoTracking().ToListAsync(ct);

    public async Task<User?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User> CreateAsync(User u, CancellationToken ct = default)
    {
        _db.Users.Add(u);
        await _db.SaveChangesAsync(ct);
        return u;
    }

    public async Task<User?> UpdateAsync(Guid id, User u, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return null;
        existing.Email = u.Email;
        existing.Role = u.Role;
        existing.PasswordHash = u.PasswordHash;
        existing.PasswordSalt = u.PasswordSalt;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        _db.Users.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}


