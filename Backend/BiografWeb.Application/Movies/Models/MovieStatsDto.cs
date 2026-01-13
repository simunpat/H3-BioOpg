namespace BiografWeb.Application.Movies.Models;

public sealed class MovieStatsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ScreeningsCount { get; set; }
    public DateTime? NextStartTime { get; set; }
    public decimal AveragePrice { get; set; }
}

