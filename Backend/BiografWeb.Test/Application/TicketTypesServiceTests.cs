using System;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Application.TicketTypes;
using BiografWeb.Domain;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Application;

public class TicketTypesServiceTests
{
    /// <summary>
    /// Ensures CreateAsync throws when Name is empty.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_Name_Empty()
    {
        var repo = Substitute.For<ITicketTypesRepository>();
        var svc = new TicketTypesService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new TicketType { Name = "", Multiplier = 1m }, CancellationToken.None));
    }

    /// <summary>
    /// Ensures CreateAsync throws when Multiplier is non-positive.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_Multiplier_NonPositive()
    {
        var repo = Substitute.For<ITicketTypesRepository>();
        var svc = new TicketTypesService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new TicketType { Name = "Adult", Multiplier = 0m }, CancellationToken.None));
    }
}

