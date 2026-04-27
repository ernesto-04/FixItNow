using Microsoft.AspNetCore.Http;

namespace FixItNow.Domain.Models.DTOs
{
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }

        public List<IFormFile> Images { get; set; } = new();
    }
}
