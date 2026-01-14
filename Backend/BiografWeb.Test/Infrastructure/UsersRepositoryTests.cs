using System;
using System.Linq;
using System.Threading.Tasks;
using BiografWeb.Domain;
using BiografWeb.Infrastructure.Repositories;
using BiografWeb.Test.Infrastructure;
using Xunit;

namespace BiografWeb.Test.InfrastructureTests;

public class UsersRepositoryTests
{
    /// <summary>
    /// Verifies FindByEmailAsync returns null for a non-existent email.
    /// </summary>
    [Fact]
    public async Task FindByEmailAsync_Returns_Null_When_NotFound()
    {
        await using var t = new TestDb();
        var repo = new UsersRepository(t.Db);
        var u = await repo.FindByEmailAsync("none@test");

        Assert.Null(u);
    }

    /// <summary>
    /// Verifies that a user with no bookings has BookingsCount=0 and LastBookingAt=null in stats.
    /// </summary>
    [Fact]
    public async Task GetStatsAsync_Returns_Zeroes_For_User_With_No_Bookings()
    {
        await using var t = new TestDb();
        var u = new User { Id = Guid.NewGuid(), Email = "u@test", IsAdmin = false, PasswordHash = "x", PasswordSalt = "y", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        t.Db.Users.Add(u);

        await t.Db.SaveChangesAsync();

        var repo = new UsersRepository(t.Db);
        var stats = await repo.GetStatsAsync();
        var s = stats.Single(x => x.Id == u.Id);

        Assert.Equal(0, s.BookingsCount);
        Assert.Null(s.LastBookingAt);
    }
}

