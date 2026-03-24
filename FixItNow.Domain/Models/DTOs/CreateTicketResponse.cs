namespace FixItNow.Domain.Models.DTOs
{
    public class CreateTicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string TechnicianName { get; set; }
        public int? AssignedTechnicianId { get; set; }
        public TicketStatus Status { get; set; }
    }
}
