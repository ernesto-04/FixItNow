namespace FixItNow.Domain.Models.DTOs
{
    public class CreateTicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
