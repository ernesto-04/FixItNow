using FixItNow.Domain.Models.Accesses;

namespace FixItNow.Domain.Models.Tickets
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Unassigned;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CustomerId { get; set; }
        public User Customer { get; set; }

        public int? AssignedTechnicianId { get; set; }
        public User? AssignedTechnician { get; set; }

        public List<TicketImage> Images { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }
    = new List<ChatMessage>();
    }

    public enum TicketStatus
    {
        Unassigned,
        Assigned,
        InProgress,
        Completed
    }
}
