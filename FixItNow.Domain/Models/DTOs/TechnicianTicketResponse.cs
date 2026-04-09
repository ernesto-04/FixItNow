using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixItNow.Domain.Models.Tickets;

namespace FixItNow.Domain.Models.DTOs
{
    public class TechnicianTicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string CustomerName { get; set; }
        public TicketStatus Status { get; set; }
    }
}
