using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Infrastructure.Models.Commons;

namespace FixItNow.Infrastructure
{
    public class TicketService
    {
        private readonly FixItNowDataContext _context;

        public TicketService(FixItNowDataContext context)
        {
            _context = context;
        }

        public Technician AssignTechnician(string category)
        {
            // 1. Find Technician with matching skill and available status
            var candidates = _context.Technicians
                .Where(t => t.SkillTypes == category && t.Status == "Available")
                .ToList();
            if (!candidates.Any()) return null;

            // 2. Find the Technician with the least number of assigned tickets
            var technician = candidates
                .Select(t => new
                {
                    Technician = t,
                    TicketCount = _context.Tickets.Count(ticket => ticket.AssignedTechnicianId == t.Id)
                })
                .OrderBy(t => t.TicketCount)
                .First()
                .Technician;

            return technician;
        }

        public Ticket CreateTicket(Ticket ticket)
        {
            var technician = AssignTechnician(ticket.Category);
            if(technician != null)
            {
                ticket.AssignedTechnicianId = technician.Id;
                ticket.Status = "Assigned";
            }
            else
            {
                ticket.Status = "Unassigned";
            }
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return ticket;
        }

        public List<Ticket> GetAllTickets()
        {
            return _context.Tickets.ToList();
        }
    }
}
