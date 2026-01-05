namespace BiografWeb.Domain;

public class Booking
{
    public Guid Id { get; set; }
    public Guid ScreeningId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }

    public List<BookingSeat> Seats { get; set; } = new();
    public List<BookingItem> Items { get; set; } = new();
}


