namespace BiografWeb.Application.Auditoriums.Models;

public sealed class AuditoriumAvgOccupancyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal AverageOccupancyNext7Days { get; set; }
}

