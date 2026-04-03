using System.Security.Claims;
using FixItNow.Domain.Models;
using FixItNow.Infrastructure;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }
        [Authorize(Roles = "Admin, User")]
        [HttpPost("create-ticket")]
        public IActionResult Create(Ticket ticket)
        {
            var createdTicket = _ticketService.CreateTicket(ticket);
            return Ok(createdTicket);
        }
        [Authorize(Roles = "Admin, Technician")]
        [HttpGet("get-tickets")]
        public IActionResult GetAvailableTickets()
        {
            var tickets = _ticketService.GetAvailableTickets();
            return Ok(tickets);
        }
        [Authorize(Roles = "Admin, Technician")]
        [HttpPost("{ticketid}/accept")]
        public IActionResult AcceptTicket(int ticketId)
        {
            var technicianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = _ticketService.AcceptTicket(ticketId, technicianId);
            if (!success) return BadRequest("Ticket already taken");

            return Ok();
        }

        [Authorize(Roles = "Admin, Technician")]
        [HttpPatch("{ticketId}/status")]
        public IActionResult UpdateStatus(int ticketId, [FromQuery] TicketStatus status)
        {
            var updated = _ticketService.UpdateStatus(ticketId, status);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
    }
}
