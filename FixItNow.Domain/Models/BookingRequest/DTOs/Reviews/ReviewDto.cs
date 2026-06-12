namespace FixItNow.Domain.Models.BookingRequest.DTOs.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}