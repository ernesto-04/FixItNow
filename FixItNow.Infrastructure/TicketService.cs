using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Infrastructure
{
    public class TicketService
    {
        private readonly FixItNowDataContext _context;

        public TicketService(FixItNowDataContext context)
        {
            _context = context;
        }

        public CreateTicketResponse CreateTicket(Ticket ticket)
        {
            ticket.Status = TicketStatus.Unassigned;
            ticket.AssignedTechnicianId = null;
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return new CreateTicketResponse
            {
                Id = ticket.Id,
            };
        }

        public Ticket UpdateStatus(int ticketId, [FromQuery] TicketStatus newStatus)
        {
            var ticket = _context.Tickets.Find(ticketId);
            if (ticket == null) return null;

            ticket.Status = newStatus;
            _context.SaveChanges();

            return ticket;
        }

        public List<CreateTicketResponse> GetAllTickets()
        {
            return _context.Tickets
                .Include(t => t.AssignedTechnician)
                .Select(t => new CreateTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Category = t.Category,
                    Location = t.Location,
                })
                .ToList();
        }

        public List<CreateTicketResponse> GetAvailableTickets()
        {
            return _context.Tickets
                .Where(t => t.Status == TicketStatus.Unassigned)
                .Select(t => new CreateTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Category = t.Category,
                    Location = t.Location,
                })
                .ToList();
            
        }
        
        public bool AcceptTicket(int ticketId, int technicianId)
        {
            var ticket = _context.Tickets.Find(ticketId);

            if (ticket == null) return false;

            if(ticket.Status != TicketStatus.Unassigned) return false;

            ticket.AssignedTechnicianId = technicianId;
            ticket.Status = TicketStatus.Assigned;

            _context.SaveChanges();
            return true;
        }
    }
}
