namespace BiografWeb.Application.TicketTypes.Models;

public sealed class TicketTypeInUseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int InUseCount { get; set; }
}

