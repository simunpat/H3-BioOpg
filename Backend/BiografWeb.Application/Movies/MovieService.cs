using BiografWeb.Domain;
using BiografWeb.Application.Movies.Models;
using BiografWeb.Domain.Extensions;

namespace BiografWeb.Application.Movies;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repo;

    public MovieService(IMovieRepository repo) => _repo = repo;

    public Task<List<Movie>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);

    public Task<Movie?> GetAsync(Guid id, CancellationToken ct = default) => _repo.GetAsync(id, ct);

    public async Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(movie.Title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(movie.Genre)) throw new ArgumentException("Genre is required");
        if (movie.DurationMin <= 0) throw new ArgumentException("Duration must be positive");

        // Ensure title starts with a capitalized letter
        movie.Title = movie.Title.EnsureCapitalizedFirst();

        return await _repo.CreateAsync(movie, ct);
    }

    public async Task<Movie?> UpdateAsync(Guid id, Movie input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.Title)) throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(input.Genre)) throw new ArgumentException("Genre is required");

        if (input.DurationMin <= 0) throw new ArgumentException("Duration must be positive");

        // Ensure title starts with a capitalized letter
        input.Title = input.Title.EnsureCapitalizedFirst();

        return await _repo.UpdateAsync(id, input, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);

    public Task<List<MovieStatsDto>> GetStatsAsync(CancellationToken ct = default)
        => _repo.GetStatsAsync(ct);

    public Task<MovieDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default)
        => _repo.GetStatsByIdAsync(id, ct);
}


