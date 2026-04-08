using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;
        private readonly FixItNowDataContext _context;

        public TicketController(TicketService ticketService, FixItNowDataContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }
        [HttpPost("create-ticket")]
        public IActionResult Create(CreateTicketResponse request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var createdTicket = _ticketService.CreateTicket(userId, request);

            return Ok(createdTicket);
        }
        [HttpGet("available-tickets")]
        public IActionResult GetAvailableTickets()
        {
            var tickets = _ticketService.GetAvailableTickets();
            return Ok(tickets);
        }
        [HttpPost("{id}/accept")]
        public IActionResult Accept(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _ticketService.AcceptTicket(id, userId);

            return Ok(new { message = "Ticket accepted successfully" });
        }
        [HttpGet("technician-tickets")]
        public IActionResult GetTechnicianTickets()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var jobs = _context.Tickets
                .Where(t => t.AssignedTechnicianId == userId)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Category,
                    t.Location,
                    t.Status,
                    CustomerName = t.Customer.Email // or Name later
                })
                .ToList();

            return Ok(jobs);
        }
        [HttpGet("customer-tickets")]
        public IActionResult GetCustomerTickets()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tickets = _context.Tickets
                .Where(t => t.CustomerId == userId)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Category,
                    t.Location,
                    t.Status,
                    TechnicianName = t.AssignedTechnician != null
                        ? t.AssignedTechnician.Email
                        : "Not assigned"
                })
                .ToList();

            return Ok(tickets);
        }
        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromQuery] string status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _ticketService.UpdateStatus(id, userId, status);

            return Ok(new { message = "Status updated successfully" });
        }
    }
}
