using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.Tickets;

namespace FixItNow.Domain.Models.BookingRequest.Reviews
{
    public class Review
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public int CustomerId { get; set; }
        public User Customer { get; set; } = null!;
        public int TechnicianId { get; set; }
        public User Technician { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}