using FixItNow.Domain.Models.Tickets;
using Microsoft.AspNetCore.Http;

namespace FixItNow.Domain.Models.DTOs
{
    public class CustomerTicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string TechnicianName { get; set; }
        public TicketStatus Status { get; set; }

        public List<IFormFile> Images { get; set; } = new();
    }
}
