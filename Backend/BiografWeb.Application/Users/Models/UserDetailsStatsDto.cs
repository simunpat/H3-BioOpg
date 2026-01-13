namespace BiografWeb.Application.Users.Models;

public sealed class UserDetailsStatsDto
{
    public Guid Id { get; set; }
    public DateTime? NextScreeningStart { get; set; }
    public decimal TotalSpent { get; set; }
}

