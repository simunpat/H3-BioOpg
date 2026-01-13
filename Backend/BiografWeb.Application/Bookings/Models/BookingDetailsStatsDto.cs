namespace BiografWeb.Application.Bookings.Models;

public sealed class BookingDetailsStatsDto
{
    public Guid Id { get; set; }
    public int SeatCount { get; set; }
    public List<BookingItemDetail> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public sealed class BookingItemDetail
{
    public string TicketTypeName { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Multiplier { get; set; }
    public decimal LineTotal { get; set; }
}

