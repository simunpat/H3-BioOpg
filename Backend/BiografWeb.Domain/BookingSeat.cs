namespace BiografWeb.Domain;

public class BookingSeat
{
    public Guid BookingId { get; set; }
    public int Row { get; set; }
    public int Number { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


