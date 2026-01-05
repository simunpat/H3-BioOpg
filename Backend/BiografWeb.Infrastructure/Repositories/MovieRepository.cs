using BiografWeb.Application.Movies;
using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _db;

    public MovieRepository(AppDbContext db) => _db = db;

    public async Task<List<Movie>> ListAsync(CancellationToken ct = default)
        => await _db.Movies.AsNoTracking().ToListAsync(ct);

    public async Task<Movie?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync(ct);
        return movie;
    }

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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var m = await _db.Movies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (m is null) return false;
        _db.Movies.Remove(m);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}


