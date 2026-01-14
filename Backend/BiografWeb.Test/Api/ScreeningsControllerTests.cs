using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.Screenings;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class ScreeningsControllerTests
{
    [Fact]
    /// <summary>
    /// Ensures list endpoint respects the movieId filter and returns matching screenings.
    /// </summary>
    public async Task List_Filters_By_MovieId()
    {
        var svc = Substitute.For<IScreeningsService>();
        var movieId = Guid.NewGuid();

        svc.ListAsync(movieId, Arg.Any<CancellationToken>())
            .Returns(new List<Screening> { new Screening { Id = Guid.NewGuid(), MovieId = movieId } });

        var ctl = new ScreeningsController(svc);

        var res = await ctl.List(movieId, CancellationToken.None);

        Assert.Single(res);
        Assert.Equal(movieId, res[0].MovieId);
    }

    [Fact]
    /// <summary>
    /// Ensures GET by id returns HTTP 200 when found and HTTP 404 when not found.
    /// </summary>
    public async Task Get_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IScreeningsService>();

        svc.GetAsync(id, Arg.Any<CancellationToken>()).Returns(new Screening { Id = id });

        var ctl = new ScreeningsController(svc);
        var ok = await ctl.Get(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Screening?)null);

        var nf = await ctl.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures POST creates a screening and returns it (HTTP 200).
    /// </summary>
    public async Task Create_Returns_Screening()
    {
        var svc = Substitute.For<IScreeningsService>();
        var created = new Screening { Id = Guid.NewGuid() };

        svc.CreateAsync(Arg.Any<Screening>(), Arg.Any<CancellationToken>()).Returns(created);

        var ctl = new ScreeningsController(svc);
        var res = await ctl.Create(new Screening(), CancellationToken.None);

        Assert.Equal(created, res);
    }

    [Fact]
    /// <summary>
    /// Ensures PUT returns HTTP 200 when updating an existing screening and HTTP 404 when missing.
    /// </summary>
    public async Task Update_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IScreeningsService>();

        svc.UpdateAsync(id, Arg.Any<Screening>(), Arg.Any<CancellationToken>()).Returns(new Screening { Id = id });

        var ctl = new ScreeningsController(svc);
        var ok = await ctl.Update(id, new Screening(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Screening>(), Arg.Any<CancellationToken>()).Returns((Screening?)null);

        var nf = await ctl.Update(Guid.NewGuid(), new Screening(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures DELETE returns HTTP 204 when the screening exists and HTTP 404 when it does not.
    /// </summary>
    public async Task Delete_Returns_NoContent_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IScreeningsService>();
        var ctl = new ScreeningsController(svc);

        svc.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        var ok = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(ok);

        svc.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var nf = await ctl.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf);
    }

    [Fact]
    /// <summary>
    /// Ensures /stats returns HTTP 200 with a list of per-screening statistics.
    /// </summary>
    public async Task Stats_Returns_List()
    {
        var svc = Substitute.For<IScreeningsService>();

        svc.GetStatsAsync(Arg.Any<CancellationToken>())
            .Returns(new System.Collections.Generic.List<BiografWeb.Application.Screenings.Models.ScreeningStatsDto>
            {
                new BiografWeb.Application.Screenings.Models.ScreeningStatsDto { Id = Guid.NewGuid(), BookingCount = 1, AvailableSeats = 5 }
            });

        var ctl = new ScreeningsController(svc);
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
        var svc = Substitute.For<IScreeningsService>();

        svc.GetStatsByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new BiografWeb.Application.Screenings.Models.ScreeningDetailsStatsDto { Id = id, BookedSeats = 1 });

        var ctl = new ScreeningsController(svc);
        var ok = await ctl.StatsById(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetStatsByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((BiografWeb.Application.Screenings.Models.ScreeningDetailsStatsDto?)null);

        var nf = await ctl.StatsById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }
}

