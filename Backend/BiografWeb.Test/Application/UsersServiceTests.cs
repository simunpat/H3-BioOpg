using System;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Application.Users;
using BiografWeb.Domain;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Application;

public class UsersServiceTests
{
    /// <summary>
    /// Ensures CreateAsync throws when Email is missing.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_Email_Missing()
    {
        var repo = Substitute.For<IUsersRepository>();
        var svc = new UsersService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new User { Email = "", PasswordHash = "x" }, CancellationToken.None));
    }

    /// <summary>
    /// Ensures CreateAsync throws when PasswordHash is missing.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_PasswordHash_Missing()
    {
        var repo = Substitute.For<IUsersRepository>();
        var svc = new UsersService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new User { Email = "a@b.c", PasswordHash = "" }, CancellationToken.None));
    }
}

