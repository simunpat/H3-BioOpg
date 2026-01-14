using System;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.Movies;
using BiografWeb.Application.Movies.Models;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class MoviesControllerTests
{
    [Fact]
    /// <summary>
    /// Ensures GET by id returns HTTP 404 when the movie cannot be found.
    /// </summary>
    public async Task Get_Returns_NotFound_When_Missing()
    {
        var svc = Substitute.For<IMovieService>();
        svc.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Movie?)null);
        var ctl = new MoviesController(svc);

        var res = await ctl.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures POST creates a movie and returns HTTP 201 Created with a proper Location route.
    /// </summary>
    public async Task Create_Returns_CreatedAtAction()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.CreateAsync(Arg.Any<Movie>(), Arg.Any<CancellationToken>())
            .Returns(new Movie { Id = id, Title = "T", Genre = "G", DurationMin = 100 });

        var ctl = new MoviesController(svc);

        var res = await ctl.Create(new Movie { Title = "T", Genre = "G", DurationMin = 100 }, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(res.Result);
        Assert.Equal(nameof(MoviesController.Get), created.ActionName);
        Assert.Equal(id, ((Movie)created.Value!).Id);
    }

    [Fact]
    /// <summary>
    /// Ensures GET list returns HTTP 200 with an array payload.
    /// </summary>
    public async Task List_Returns_Array()
    {
        var svc = Substitute.For<IMovieService>();

        svc.ListAsync(Arg.Any<CancellationToken>())
            .Returns(new System.Collections.Generic.List<Movie>
            {
                new Movie { Id = Guid.NewGuid(), Title = "A", Genre = "G", DurationMin = 90 }
            });

        var ctl = new MoviesController(svc);
        var res = await ctl.List(CancellationToken.None);

        Assert.Single(res.Value!);
    }

    [Fact]
    /// <summary>
    /// Ensures GET by id returns HTTP 200 with the movie when it exists.
    /// </summary>
    public async Task Get_Returns_Ok_When_Found()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.GetAsync(id, Arg.Any<CancellationToken>()).Returns(new Movie { Id = id, Title = "X", Genre = "G", DurationMin = 100 });

        var ctl = new MoviesController(svc);
        var res = await ctl.Get(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures PUT updates an existing movie and returns HTTP 200 with the updated entity.
    /// </summary>
    public async Task Update_Returns_Ok_When_Found()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.UpdateAsync(id, Arg.Any<Movie>(), Arg.Any<CancellationToken>())
           .Returns(new Movie { Id = id, Title = "U", Genre = "G", DurationMin = 120 });

        var ctl = new MoviesController(svc);
        var res = await ctl.Update(id, new Movie { Title = "U", Genre = "G", DurationMin = 120 }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures PUT returns HTTP 404 when updating a non-existing movie.
    /// </summary>
    public async Task Update_Returns_NotFound_When_Missing()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.UpdateAsync(id, Arg.Any<Movie>(), Arg.Any<CancellationToken>()).Returns((Movie?)null);

        var ctl = new MoviesController(svc);
        var res = await ctl.Update(id, new Movie { Title = "U", Genre = "G", DurationMin = 120 }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    /// <summary>
    /// Ensures DELETE returns HTTP 204 when the movie is found and removed.
    /// </summary>
    public async Task Delete_Returns_NoContent_When_Found()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        var ctl = new MoviesController(svc);
        var res = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    /// <summary>
    /// Ensures DELETE returns HTTP 404 when the movie does not exist.
    /// </summary>
    public async Task Delete_Returns_NotFound_When_Missing()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var ctl = new MoviesController(svc);
        var res = await ctl.Delete(id, CancellationToken.None);

        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    /// <summary>
    /// Ensures /stats returns HTTP 200 with a list of per-movie statistics.
    /// </summary>
    public async Task Stats_Returns_List()
    {
        var svc = Substitute.For<IMovieService>();

        svc.GetStatsAsync(Arg.Any<CancellationToken>())
            .Returns(new System.Collections.Generic.List<MovieStatsDto>
            {
                new MovieStatsDto { Id = Guid.NewGuid(), Title = "X", ScreeningsCount = 1, AveragePrice = 100m }
            });

        var ctl = new MoviesController(svc);
        var res = await ctl.Stats(CancellationToken.None);

        Assert.Single(res.Value!);
    }

    [Fact]
    /// <summary>
    /// Ensures /{id}/stats returns HTTP 200 when stats exist, and HTTP 404 when missing.
    /// </summary>
    public async Task StatsById_Returns_Ok_And_NotFound()
    {
        var id = Guid.NewGuid();
        var svc = Substitute.For<IMovieService>();

        svc.GetStatsByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new MovieDetailsStatsDto { Id = id, TotalScreenings = 2 });

        var ctl = new MoviesController(svc);
        var ok = await ctl.StatsById(id, CancellationToken.None);

        Assert.IsType<OkObjectResult>(ok.Result);

        svc.GetStatsByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((MovieDetailsStatsDto?)null);

        var nf = await ctl.StatsById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(nf.Result);
    }
}

