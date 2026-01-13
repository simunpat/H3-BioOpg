namespace BiografWeb.Application.Screenings.Models;

public sealed class ScreeningStatsDto
{
    public Guid Id { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string AuditoriumName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public int AvailableSeats { get; set; }
}

