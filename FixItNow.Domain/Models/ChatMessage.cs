using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public User Sender { get; set; }
        public User Receiver { get; set; }

        public Ticket Ticket { get; set; }

    }
}