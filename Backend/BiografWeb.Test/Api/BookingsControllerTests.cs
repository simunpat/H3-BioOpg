using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.Bookings;
using BiografWeb.Application.Bookings.Models;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class BookingsControllerTests
{
    [Fact]
    /// <summary>
    /// Ensures /stats/summary returns HTTP 200 with an object containing the totalRevenue property.
    /// </summary>
    public async Task Summary_Returns_TotalRevenue_Object()
    {
        var svc = Substitute.For<IBookingsService>();

        svc.GetTotalRevenueAsync(Arg.Any<CancellationToken>()).Returns(123.45m);

        var ctl = new BookingsController(svc);

        var res = await ctl.Summary(CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(res.Result);
        var json = ok.Value!.GetType().GetProperty("totalRevenue")!.GetValue(ok.Value, null);

        Assert.Equal(123.45m, json);
    }

    [Fact]
    /// <summary>
    /// Ensures GET list respects screeningId/userId filters and returns matching bookings.
    /// </summary>
    public async Task List_Returns_Array_With_Filters()
    {
        var svc = Substitute.For<IBookingsService>();
        var screeningId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        svc.ListAsync(screeningId, userId, Arg.Any<CancellationToken>())
           .Returns(new List<Booking> { new Booking { Id = Guid.NewGuid(), ScreeningId = screeningId, UserId = userId } });

        var ctl = new BookingsController(svc);
        var res = await ctl.List(screeningId, userId, CancellationToken.None);

        Assert.Single(res);
        Assert.Equal(screeningId, res[0].ScreeningId);
        Assert.Equal(userId, res[0].UserId);
    }

    [Fact]
    /// <summary>
    /// Ensures GET by id returns HTTP 200 when found and HTTP 404 when not found.
    /// </summary>
    public async Task Get_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IBookingsService>();

        svc.GetAsync(id, Arg.Any<CancellationToken>()).Returns(new Booking { Id = id });

        var ctl = new BookingsController(svc);
        var ok = await ctl.Get(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Booking?)null);

        var nf = await ctl.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures POST creates a booking and returns it (HTTP 200).
    /// </summary>
    public async Task Create_Returns_Booking()
    {
        var svc = Substitute.For<IBookingsService>();
        var created = new Booking { Id = Guid.NewGuid() };

        svc.CreateAsync(Arg.Any<Booking>(), Arg.Any<CancellationToken>()).Returns(created);

        var ctl = new BookingsController(svc);
        var res = await ctl.Create(new Booking(), CancellationToken.None);

        Assert.Equal(created, res);
    }

    [Fact]
    /// <summary>
    /// Ensures PUT returns HTTP 200 when updating an existing booking and HTTP 404 when missing.
    /// </summary>
    public async Task Update_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IBookingsService>();

        svc.UpdateAsync(id, Arg.Any<Booking>(), Arg.Any<CancellationToken>()).Returns(new Booking { Id = id });

        var ctl = new BookingsController(svc);
        var ok = await ctl.Update(id, new Booking(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Booking>(), Arg.Any<CancellationToken>()).Returns((Booking?)null);

        var nf = await ctl.Update(Guid.NewGuid(), new Booking(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures DELETE returns HTTP 204 when the booking exists and HTTP 404 when it does not.
    /// </summary>
    public async Task Delete_Returns_NoContent_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IBookingsService>();
        var ctl = new BookingsController(svc);

        svc.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        var ok = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(ok);

        svc.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var nf = await ctl.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf);
    }

    [Fact]
    /// <summary>
    /// Ensures /stats returns HTTP 200 with a list of per-booking statistics.
    /// </summary>
    public async Task Stats_Returns_List()
    {
        var svc = Substitute.For<IBookingsService>();

        svc.GetStatsAsync(Arg.Any<CancellationToken>())
           .Returns(new List<BookingStatsDto> { new BookingStatsDto { Id = Guid.NewGuid(), ItemsCount = 2 } });

        var ctl = new BookingsController(svc);
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
        var svc = Substitute.For<IBookingsService>();

        svc.GetStatsByIdAsync(id, Arg.Any<CancellationToken>()).Returns(new BookingDetailsStatsDto { Id = id, SeatCount = 3, Total = 10m });

        var ctl = new BookingsController(svc);
        var ok = await ctl.StatsById(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetStatsByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((BookingDetailsStatsDto?)null);

        var nf = await ctl.StatsById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }
}

