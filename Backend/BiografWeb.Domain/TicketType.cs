namespace BiografWeb.Domain;

public class TicketType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Multiplier { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


