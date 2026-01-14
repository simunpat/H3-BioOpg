using System;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.Users;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class UsersControllerTests
{
    [Fact]
    /// <summary>
    /// Ensures /byEmail returns null when the user is not found.
    /// </summary>
    public async Task ByEmail_Returns_Null_When_Missing()
    {
        var svc = Substitute.For<IUsersService>();
        svc.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);
        var ctl = new UsersController(svc);

        var res = await ctl.FindByEmail("nope@test", CancellationToken.None);

        Assert.Null(res);
    }

    [Fact]
    /// <summary>
    /// Ensures /byEmail returns the user when it exists.
    /// </summary>
    public async Task ByEmail_Returns_User_When_Found()
    {
        var svc = Substitute.For<IUsersService>();
        var user = new User { Id = Guid.NewGuid(), Email = "yes@test" };

        svc.FindByEmailAsync("yes@test", Arg.Any<CancellationToken>()).Returns(user);

        var ctl = new UsersController(svc);
        var res = await ctl.FindByEmail("yes@test", CancellationToken.None);

        Assert.Equal(user, res);
    }

    [Fact]
    /// <summary>
    /// Ensures GET list returns HTTP 200 with an array of users.
    /// </summary>
    public async Task List_Returns_Array()
    {
        var svc = Substitute.For<IUsersService>();

        svc.ListAsync(Arg.Any<CancellationToken>())
           .Returns(new System.Collections.Generic.List<User> { new User { Id = Guid.NewGuid(), Email = "e@test" } });

        var ctl = new UsersController(svc);
        var res = await ctl.List(CancellationToken.None);

        Assert.Single(res);
    }

    [Fact]
    /// <summary>
    /// Ensures GET by id returns HTTP 200 when found and HTTP 404 when not found.
    /// </summary>
    public async Task Get_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IUsersService>();

        svc.GetAsync(id, Arg.Any<CancellationToken>()).Returns(new User { Id = id, Email = "e@test" });

        var ctl = new UsersController(svc);
        var ok = await ctl.Get(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var nf = await ctl.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures POST creates a user and returns it (HTTP 200).
    /// </summary>
    public async Task Create_Returns_User()
    {
        var svc = Substitute.For<IUsersService>();
        var created = new User { Id = Guid.NewGuid(), Email = "c@test" };

        svc.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(created);

        var ctl = new UsersController(svc);
        var res = await ctl.Create(new User { Email = "c@test" }, CancellationToken.None);

        Assert.Equal(created, res);
    }

    [Fact]
    /// <summary>
    /// Ensures PUT returns HTTP 200 when updating an existing user and HTTP 404 when missing.
    /// </summary>
    public async Task Update_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IUsersService>();

        svc.UpdateAsync(id, Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(new User { Id = id, Email = "u@test" });

        var ctl = new UsersController(svc);
        var ok = await ctl.Update(id, new User { Email = "u@test" }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.UpdateAsync(Arg.Any<Guid>(), Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var nf = await ctl.Update(Guid.NewGuid(), new User { Email = "x@test" }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures DELETE returns HTTP 204 when the user exists and HTTP 404 when it does not.
    /// </summary>
    public async Task Delete_Returns_NoContent_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IUsersService>();
        var ctl = new UsersController(svc);

        svc.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        var ok = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(ok);

        svc.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var nf = await ctl.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf);
    }

    [Fact]
    /// <summary>
    /// Ensures /stats returns HTTP 200 with a list of per-user statistics.
    /// </summary>
    public async Task Stats_Returns_List()
    {
        var svc = Substitute.For<IUsersService>();

        svc.GetStatsAsync(Arg.Any<CancellationToken>())
            .Returns(new System.Collections.Generic.List<BiografWeb.Application.Users.Models.UserStatsDto>
            {
                new BiografWeb.Application.Users.Models.UserStatsDto { Id = Guid.NewGuid(), Email = "e@test", BookingsCount = 0 }
            });

        var ctl = new UsersController(svc);

        var res = await ctl.Stats(CancellationToken.None);

        Assert.Single(res);
    }

    [Fact]
    /// <summary>
    /// Ensures /{id}/stats returns HTTP 200 when stats exist and HTTP 404 when missing.
    /// </summary>
    public async Task StatsById_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IUsersService>();

        svc.GetStatsByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new BiografWeb.Application.Users.Models.UserDetailsStatsDto { Id = id, TotalSpent = 10m });

        var ctl = new UsersController(svc);
        var ok = await ctl.StatsById(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetStatsByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((BiografWeb.Application.Users.Models.UserDetailsStatsDto?)null);

        var nf = await ctl.StatsById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }
}

