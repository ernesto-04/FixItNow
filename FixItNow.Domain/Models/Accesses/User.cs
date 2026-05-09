using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.Accesses
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public TechnicianProfile? TechnicianProfile { get; set; }

        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

        public ICollection<ChatMessage> SentMessages { get; set; }
    = new List<ChatMessage>();

        public ICollection<ChatMessage> ReceivedMessages { get; set; }
            = new List<ChatMessage>();
    }
}
