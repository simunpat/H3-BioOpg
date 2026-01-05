using BiografWeb.Domain;

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
        return await _repo.CreateAsync(movie, ct);
    }

    public Task<Movie?> UpdateAsync(Guid id, Movie input, CancellationToken ct = default)
        => _repo.UpdateAsync(id, input, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);
}


