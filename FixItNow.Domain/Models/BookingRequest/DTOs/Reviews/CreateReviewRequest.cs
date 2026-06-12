namespace FixItNow.Domain.Models.BookingRequest.DTOs.Reviews
{
    public class CreateReviewRequest
    {
        public int TicketId { get; set; }
        public int TechnicianId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}