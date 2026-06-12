namespace FixItNow.Domain.Models.BookingRequest.Tickets;

public class TicketImage
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
}