using BiografWeb.Application.Movies;
using BiografWeb.Application.Movies.Models;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _db;

    public MovieRepository(AppDbContext db) => _db = db;

    /// <summary>
    /// Returns all movies without change tracking.
    /// </summary>
    /// <returns>List of movies.</returns>
    public async Task<List<Movie>> ListAsync(CancellationToken ct = default)
        => await _db.Movies.AsNoTracking().ToListAsync(ct);

    /// <summary>
    /// Returns a single movie by its identifier, or null if not found.
    /// </summary>
    /// <param name="id">Movie identifier.</param>
    /// <returns>The movie or null.</returns>
    public async Task<Movie?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

    /// <summary>
    /// Persists a new movie and returns the created entity.
    /// </summary>
    /// <param name="movie">Movie to create.</param>
    /// <returns>Created movie.</returns>
    public async Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync(ct);
        return movie;
    }

    /// <summary>
    /// Updates an existing movie by id and returns the updated entity, or null if not found.
    /// </summary>
    /// <param name="id">Movie identifier.</param>
    /// <param name="input">New values to apply.</param>
    /// <returns>Updated movie or null.</returns>
    public async Task<Movie?> UpdateAsync(Guid id, Movie input, CancellationToken ct = default)
    {
        var m = await _db.Movies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (m is null) return null;
        m.Title = input.Title;
        m.DurationMin = input.DurationMin;
        m.Genre = input.Genre;
        m.PosterUrl = input.PosterUrl;
        await _db.SaveChangesAsync(ct);
        return m;
    }

    /// <summary>
    /// Deletes a movie by id.
    /// </summary>
    /// <param name="id">Movie identifier.</param>
    /// <returns>True if a row was removed; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var m = await _db.Movies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (m is null) return false;
        _db.Movies.Remove(m);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Returns per-movie statistics for all movies:
    /// - ScreeningsCount: total number of screenings linked to the movie
    /// - NextStartTime: the earliest future screening start (null if none)
    /// - AveragePrice: average ticket price across the movie's screenings
    /// </summary>
    public async Task<List<MovieStatsDto>> GetStatsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var query =
            from m in _db.Movies.AsNoTracking()
            select new MovieStatsDto
            {
                Id = m.Id,
                Title = m.Title,
                ScreeningsCount = _db.Screenings.Count(s => s.MovieId == m.Id),
                NextStartTime = _db.Screenings
                    .Where(s => s.MovieId == m.Id && s.StartTime > now)
                    .OrderBy(s => s.StartTime)
                    .Select(s => (DateTime?)s.StartTime)
                    .FirstOrDefault(),
                AveragePrice = _db.Screenings
                    .Where(s => s.MovieId == m.Id)
                    .Select(s => (decimal?)s.Price)
                    .Average() ?? 0m
            };

        return await query.ToListAsync(ct);
    }

    /// <summary>
    /// Returns detailed statistics for a single movie:
    /// - TotalScreenings: total number of screenings for the movie
    /// - NextStartTime: the earliest future screening start (null if none)
    /// - HasFutureScreenings: whether any future screenings exist
    /// </summary>
    public async Task<MovieDetailsStatsDto?> GetStatsByIdAsync(Guid id, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var exists = await _db.Movies.AsNoTracking().AnyAsync(m => m.Id == id, ct);
        if (!exists) return null;

        var total = await _db.Screenings.CountAsync(s => s.MovieId == id, ct);
        var next = await _db.Screenings
            .Where(s => s.MovieId == id && s.StartTime > now)
            .OrderBy(s => s.StartTime)
            .Select(s => (DateTime?)s.StartTime)
            .FirstOrDefaultAsync(ct);
        var hasFuture = await _db.Screenings.AnyAsync(s => s.MovieId == id && s.StartTime > now, ct);

        return new MovieDetailsStatsDto
        {
            Id = id,
            TotalScreenings = total,
            NextStartTime = next,
            HasFutureScreenings = hasFuture
        };
    }
}


