namespace BiografWeb.Application.Movies.Models;

public sealed class MovieDetailsStatsDto
{
    public Guid Id { get; set; }
    public int TotalScreenings { get; set; }
    public DateTime? NextStartTime { get; set; }
    public bool HasFutureScreenings { get; set; }
}

