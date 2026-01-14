using System;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Application.Movies;
using BiografWeb.Domain;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Application;

public class MoviesServiceTests
{
    /// <summary>
    /// Ensures CreateAsync throws when Title is empty.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_Title_Empty()
    {
        var repo = Substitute.For<IMovieRepository>();
        var svc = new MovieService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new Movie { Title = "", Genre = "Drama", DurationMin = 120 }, CancellationToken.None));
    }

    /// <summary>
    /// Ensures CreateAsync throws when Genre is empty.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_Genre_Empty()
    {
        var repo = Substitute.For<IMovieRepository>();
        var svc = new MovieService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new Movie { Title = "T", Genre = "", DurationMin = 120 }, CancellationToken.None));
    }

    /// <summary>
    /// Ensures CreateAsync throws when DurationMin is non-positive.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_When_Duration_NonPositive()
    {
        var repo = Substitute.For<IMovieRepository>();
        var svc = new MovieService(repo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new Movie { Title = "T", Genre = "G", DurationMin = 0 }, CancellationToken.None));
    }
}

