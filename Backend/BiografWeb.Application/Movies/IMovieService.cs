using BiografWeb.Domain;

namespace BiografWeb.Application.Movies;

public interface IMovieService
{
    Task<List<Movie>> ListAsync(CancellationToken ct = default);
    Task<Movie?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default);
    Task<Movie?> UpdateAsync(Guid id, Movie input, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}


