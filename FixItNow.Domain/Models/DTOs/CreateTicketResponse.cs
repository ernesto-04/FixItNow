using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixItNow.Domain.Models.DTOs
{
    public class CreateTicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? AssignedTechnicianId { get; set; }
        public string Status { get; set; }
    }
}
