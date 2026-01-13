namespace BiografWeb.Application.TicketTypes.Models;

public sealed class TicketTypeRevenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
}

