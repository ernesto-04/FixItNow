using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.DTOs
{
    public class TicketChatResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public int? AssignedTechnicianId { get; set; }

        public string? TechnicianName { get; set; }

        public string? CustomerName { get; set; }

        public TicketStatus Status { get; set; }

        public int ReceiverId { get; set; }
    }
}
