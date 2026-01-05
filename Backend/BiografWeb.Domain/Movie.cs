namespace BiografWeb.Domain;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMin { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
}


