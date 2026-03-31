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
        private readonly FixItNowDataContext _context;
        private readonly TicketService _ticketService;

        public TicketController(FixItNowDataContext context, TicketService ticketService)
        {
            _context = context;
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
        public IActionResult GetAll()
        {
            var tickets = _ticketService.GetAllTickets();
            return Ok(tickets);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, TicketStatus status)
        {
            return Ok(_ticketService.UpdateStatus(id, status));
        }
    }
}
