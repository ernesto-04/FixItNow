using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.BookingRequest.DTOs.Tickets;

public class TechnicianTicketResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int? AssignedTechnicianId { get; set; }
    public TicketStatus Status { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}