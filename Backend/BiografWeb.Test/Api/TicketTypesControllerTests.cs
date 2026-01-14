using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.TicketTypes;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class TicketTypesControllerTests
{
    [Fact]
    /// <summary>
    /// Ensures GET list returns HTTP 200 with an array of ticket types.
    /// </summary>
    public async Task List_Returns_Array()
    {
        var svc = Substitute.For<ITicketTypesService>();
        svc.ListAsync(Arg.Any<CancellationToken>()).Returns(new List<TicketType> { new TicketType { Id = Guid.NewGuid(), Name = "Adult", Multiplier = 1m } });

        var ctl = new TicketTypesController(svc);

        var res = await ctl.List(CancellationToken.None);

        Assert.Single(res);
    }

    [Fact]
    /// <summary>
    /// Ensures POST creates a ticket type and returns it (HTTP 200).
    /// </summary>
    public async Task Create_Returns_TicketType()
    {
        var svc = Substitute.For<ITicketTypesService>();
        var created = new TicketType { Id = Guid.NewGuid(), Name = "Child", Multiplier = 0.5m };

        svc.CreateAsync(Arg.Any<TicketType>(), Arg.Any<CancellationToken>()).Returns(created);

        var ctl = new TicketTypesController(svc);
        var res = await ctl.Create(new TicketType { Name = "Child", Multiplier = 0.5m }, CancellationToken.None);

        Assert.Equal(created, res);
    }

    [Fact]
    /// <summary>
    /// Ensures PUT returns HTTP 200 when updating an existing ticket type and HTTP 404 when missing.
    /// </summary>
    public async Task Update_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<ITicketTypesService>();

        svc.UpdateAsync(id, Arg.Any<TicketType>(), Arg.Any<CancellationToken>()).Returns(new TicketType { Id = id, Name = "VIP", Multiplier = 2m });

        var ctl = new TicketTypesController(svc);
        var ok = await ctl.Update(id, new TicketType { Name = "VIP", Multiplier = 2m }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.UpdateAsync(Arg.Any<Guid>(), Arg.Any<TicketType>(), Arg.Any<CancellationToken>()).Returns((TicketType?)null);

        var nf = await ctl.Update(Guid.NewGuid(), new TicketType { Name = "X", Multiplier = 1m }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures DELETE returns HTTP 204 when the ticket type exists and HTTP 404 when it does not.
    /// </summary>
    public async Task Delete_Returns_NoContent_And_NotFound()
    {
        var id = Guid.NewGuid();

        var svc = Substitute.For<ITicketTypesService>();
        var ctl = new TicketTypesController(svc);

        svc.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        var ok = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(ok);

        svc.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var nf = await ctl.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf);
    }

    [Fact]
    /// <summary>
    /// Ensures /stats/inuse returns HTTP 200 with a list of usage counts per ticket type.
    /// </summary>
    public async Task InUseStats_Returns_List()
    {
        var svc = Substitute.For<ITicketTypesService>();

        svc.GetInUseCountsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<BiografWeb.Application.TicketTypes.Models.TicketTypeInUseDto>
            {
                new BiografWeb.Application.TicketTypes.Models.TicketTypeInUseDto { Id = Guid.NewGuid(), Name = "Adult", InUseCount = 1 }
            });

        var ctl = new TicketTypesController(svc);
        var res = await ctl.InUseStats(CancellationToken.None);

        Assert.Single(res);
    }

    [Fact]
    /// <summary>
    /// Ensures /stats/revenue returns HTTP 200 with a list of revenue totals per ticket type.
    /// </summary>
    public async Task RevenueStats_Returns_List()
    {
        var svc = Substitute.For<ITicketTypesService>();

        svc.GetRevenueAsync(Arg.Any<CancellationToken>())
            .Returns(new List<BiografWeb.Application.TicketTypes.Models.TicketTypeRevenueDto>
            {
                new BiografWeb.Application.TicketTypes.Models.TicketTypeRevenueDto { Id = Guid.NewGuid(), Name = "Adult", TotalRevenue = 100m }
            });

        var ctl = new TicketTypesController(svc);
        var res = await ctl.RevenueStats(CancellationToken.None);

        Assert.Single(res);
    }
}

