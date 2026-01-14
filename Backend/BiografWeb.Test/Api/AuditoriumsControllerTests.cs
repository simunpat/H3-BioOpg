using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.Auditoriums;
using BiografWeb.Application.Auditoriums.Models;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class AuditoriumsControllerTests
{
    /// <summary>
    /// Ensures GET list returns HTTP 200 with an array of auditoriums.
    /// </summary>
    [Fact]
    public async Task List_Returns_Array()
    {
        var svc = Substitute.For<IAuditoriumService>();

        svc.ListAsync(Arg.Any<CancellationToken>())
           .Returns(new List<Auditorium> { new Auditorium { Id = Guid.NewGuid(), Name = "A", Rows = 10, Cols = 10 } });

        var ctl = new AuditoriumsController(svc);
        var res = await ctl.List(CancellationToken.None);

        Assert.Single(res);
    }

    /// <summary>
    /// Ensures GET by id returns HTTP 200 when found and HTTP 404 when not found.
    /// </summary>
    [Fact]
    public async Task Get_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IAuditoriumService>();

        svc.GetAsync(id, Arg.Any<CancellationToken>()).Returns(new Auditorium { Id = id, Name = "A", Rows = 10, Cols = 10 });

        var ctl = new AuditoriumsController(svc);
        var ok = await ctl.Get(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Auditorium?)null);

        var nf = await ctl.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    /// <summary>
    /// Ensures POST creates an auditorium and returns HTTP 201 Created with a proper Location route.
    /// </summary>
    [Fact]
    public async Task Create_Returns_CreatedAtAction()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IAuditoriumService>();

        svc.CreateAsync(Arg.Any<Auditorium>(), Arg.Any<CancellationToken>())
           .Returns(new Auditorium { Id = id, Name = "A", Rows = 10, Cols = 10 });

        var ctl = new AuditoriumsController(svc);
        var res = await ctl.Create(new Auditorium { Name = "A", Rows = 10, Cols = 10 }, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(res.Result);

        Assert.Equal(nameof(AuditoriumsController.Get), created.ActionName);
        Assert.Equal(id, ((Auditorium)created.Value!).Id);
    }

    /// <summary>
    /// Ensures PUT returns HTTP 200 when updating an existing auditorium and HTTP 404 when missing.
    /// </summary>
    [Fact]
    public async Task Update_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IAuditoriumService>();

        svc.UpdateAsync(id, Arg.Any<Auditorium>(), Arg.Any<CancellationToken>())
           .Returns(new Auditorium { Id = id, Name = "U", Rows = 12, Cols = 12 });

        var ctl = new AuditoriumsController(svc);
        var ok = await ctl.Update(id, new Auditorium { Name = "U", Rows = 12, Cols = 12 }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Auditorium>(), Arg.Any<CancellationToken>()).Returns((Auditorium?)null);

        var nf = await ctl.Update(Guid.NewGuid(), new Auditorium { Name = "X", Rows = 1, Cols = 1 }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    /// <summary>
    /// Ensures DELETE returns HTTP 204 when the auditorium exists and HTTP 404 when it does not.
    /// </summary>
    [Fact]
    public async Task Delete_Returns_NoContent_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IAuditoriumService>();
        var ctl = new AuditoriumsController(svc);

        svc.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        var ok = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(ok);

        svc.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var nf = await ctl.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf);
    }

    /// <summary>
    /// Ensures /stats/next-start returns HTTP 200 with the earliest upcoming screening per auditorium.
    /// </summary>
    [Fact]
    public async Task NextStart_Returns_List()
    {
        var svc = Substitute.For<IAuditoriumService>();

        svc.GetNextStartAsync(Arg.Any<CancellationToken>())
           .Returns(new List<AuditoriumNextStartDto> { new AuditoriumNextStartDto { Id = Guid.NewGuid(), Name = "A", NextStartTime = DateTime.UtcNow.AddHours(1) } });

        var ctl = new AuditoriumsController(svc);
        var res = await ctl.NextStart(CancellationToken.None);

        Assert.Single(res);
    }

    /// <summary>
    /// Ensures /stats/avg-occupancy returns HTTP 200 with average occupancy values per auditorium.
    /// </summary>
    [Fact]
    public async Task AverageOccupancy_Returns_List()
    {
        var svc = Substitute.For<IAuditoriumService>();

        svc.GetAverageOccupancyNext7DaysAsync(Arg.Any<CancellationToken>())
           .Returns(new List<AuditoriumAvgOccupancyDto> { new AuditoriumAvgOccupancyDto { Id = Guid.NewGuid(), Name = "A", AverageOccupancyNext7Days = 0.5m } });

        var ctl = new AuditoriumsController(svc);
        var res = await ctl.AverageOccupancy(CancellationToken.None);

        Assert.Single(res);
    }
}

