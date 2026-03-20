using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public CreateTicketResponse CreateTicket(Ticket ticket)
        {
            var technician = AssignTechnician(ticket.Category);
            if(technician != null)
            {
                ticket.AssignedTechnicianId = technician.Id;
                ticket.Status = TicketStatus.Assigned;
            }
            else
            {
                ticket.Status = TicketStatus.Unassigned;
            }
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return new CreateTicketResponse
            {
                Id = ticket.Id,
                AssignedTechnicianId = technician?.Id,
                TechnicianName = technician?.Name
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
                    TechnicianName = t.AssignedTechnician != null ? t.AssignedTechnician.Name : "Not assigned"
                })
                .ToList();
        }
    }
}
