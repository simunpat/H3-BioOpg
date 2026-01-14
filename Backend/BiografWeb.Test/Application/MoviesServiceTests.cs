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

    /// <summary>
    /// Ensures CreateAsync capitalizes the first letter of the movie title.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Normalizes_Title_To_Capitalized()
    {
        var repo = Substitute.For<IMovieRepository>();
        var svc = new MovieService(repo);

        Movie? captured = null;

        repo.CreateAsync(Arg.Any<Movie>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                captured = ci.Arg<Movie>();
                // Simulate DB-generated Id
                captured!.Id = Guid.NewGuid();
                return captured!;
            });

        var input = new Movie { Title = "inception", Genre = "Sci-Fi", DurationMin = 120 };
        var result = await svc.CreateAsync(input, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        Assert.NotNull(captured);
        Assert.Equal("Inception", captured!.Title);
    }

    /// <summary>
    /// Ensures UpdateAsync capitalizes the first letter of the movie title.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_Normalizes_Title_To_Capitalized()
    {
        var repo = Substitute.For<IMovieRepository>();
        var svc = new MovieService(repo);

        Guid id = Guid.NewGuid();

        Movie? captured = null;

        repo.UpdateAsync(id, Arg.Any<Movie>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                captured = ci.ArgAt<Movie>(1);

                return new Movie
                {
                    Id = id,
                    Title = captured!.Title,
                    Genre = captured!.Genre,
                    DurationMin = captured!.DurationMin
                };
            });

        var input = new Movie { Title = "avatar", Genre = "Fantasy", DurationMin = 162 };
        var result = await svc.UpdateAsync(id, input, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Avatar", result!.Title);
        Assert.NotNull(captured);
        Assert.Equal("Avatar", captured!.Title);
    }
}

