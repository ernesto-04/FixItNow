using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.DTOs
{
    public class CustomerTicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string TechnicianName { get; set; }
        public TicketStatus Status { get; set; }
        public int? AssignedTechnicianId { get; set; }
        public int CustomerId { get; set; }

        public List<string> ImageUrls { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
