using FixItNow.Domain.Models.Bookings;
using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.BookingRequest.DTOs.Bookings
{
    public class BookingRequestDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TicketId { get; set; }
        public TicketStatus? TicketStatus { get; set; }
        public string? CancellationReason { get; set; }
        public int? CancelledByUserId { get; set; }
    }
}