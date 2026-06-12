using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.Tickets;

namespace FixItNow.Domain.Models.BookingRequest.Chat
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        // Navigation
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;

    }
}