namespace BiografWeb.Application.Bookings.Models;

public sealed class BookingStatsDto
{
    public Guid Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime ScreeningStart { get; set; }
    public int ItemsCount { get; set; }
}

