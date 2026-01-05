namespace BiografWeb.Domain;

public class BookingItem
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Qty { get; set; }
}


