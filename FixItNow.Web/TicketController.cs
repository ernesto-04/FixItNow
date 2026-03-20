using FixItNow.Domain.Models;
using FixItNow.Infrastructure;
using FixItNow.Infrastructure.Models.Commons;
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

        [HttpPost("create-ticket")]
        public IActionResult Create(Ticket ticket)
        {
            var createdTicket = _ticketService.CreateTicket(ticket);
            return Ok(createdTicket);
        }

        [HttpGet("get-tickets")]
        public IActionResult GetAll()
        {
            return Ok(_context.Tickets.ToList());
        }
    }
}
