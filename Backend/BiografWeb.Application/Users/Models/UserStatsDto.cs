namespace BiografWeb.Application.Users.Models;

public sealed class UserStatsDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public int BookingsCount { get; set; }
    public DateTime? LastBookingAt { get; set; }
}

