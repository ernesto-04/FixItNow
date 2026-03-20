using FixItNow.Domain.Models.Accesses;

namespace FixItNow.Domain.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedTechnicianId { get; set; }
        public Technician? AssignedTechnician { get; set; }
    }

    public enum TicketStatus
    {
        Pending,
        Assigned,
        InProgress,
        Completed,
        Unassigned
    }
}
