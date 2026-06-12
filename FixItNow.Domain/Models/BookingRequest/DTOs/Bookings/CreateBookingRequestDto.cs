namespace FixItNow.Domain.Models.BookingRequest.DTOs.Bookings
{
    public class CreateBookingRequestDto
    {
        public int TechnicianId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}