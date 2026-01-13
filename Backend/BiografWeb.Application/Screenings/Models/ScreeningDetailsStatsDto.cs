namespace BiografWeb.Application.Screenings.Models;

public sealed class ScreeningDetailsStatsDto
{
    public Guid Id { get; set; }
    public int BookedSeats { get; set; }
    public decimal RevenueEstimate { get; set; }
}

