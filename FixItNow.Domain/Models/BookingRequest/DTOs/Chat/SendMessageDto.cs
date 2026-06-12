namespace FixItNow.Domain.Models.BookingRequest.DTOs.Chat
{
    public class SendMessageDto
    {
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
