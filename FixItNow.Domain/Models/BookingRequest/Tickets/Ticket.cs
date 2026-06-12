using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.Chat;
using FixItNow.Domain.Models.BookingRequest.Reviews;
using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.BookingRequest.Tickets;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Unassigned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    public int? AssignedTechnicianId { get; set; }
    public User? AssignedTechnician { get; set; }
    public List<TicketImage> Images { get; set; } = [];
    public ICollection<ChatMessage> ChatMessages { get; set; } = [];
    public Review? Review { get; set; }
}