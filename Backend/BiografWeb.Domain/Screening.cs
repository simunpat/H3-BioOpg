namespace BiografWeb.Domain;

public class Screening
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public Guid AuditoriumId { get; set; }
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
}


