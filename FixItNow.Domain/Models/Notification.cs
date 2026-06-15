using FixItNow.Domain.Models.Accesses;

namespace FixItNow.Domain.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "booking", "ticket", "chat"
        public int? ReferenceId { get; set; } // BookingId or TicketId
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}