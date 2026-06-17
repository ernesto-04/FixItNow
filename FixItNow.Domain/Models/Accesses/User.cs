using FixItNow.Domain.Models.BookingRequest.Chat;
using FixItNow.Domain.Models.BookingRequest.Tickets;

namespace FixItNow.Domain.Models.Accesses;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public TechnicianProfile? TechnicianProfile { get; set; }
    public ICollection<Ticket> CreatedTickets { get; set; } = [];
    public ICollection<ChatMessage> SentMessages { get; set; } = [];
    public ICollection<ChatMessage> ReceivedMessages { get; set; } = [];
}