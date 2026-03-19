using FixItNow.Domain.Models;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly FixItNowDataContext _context;

        public TicketController(FixItNowDataContext context)
        {
            _context = context;
        }

        [HttpPost("create-ticket")]
        public IActionResult Create(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

        [HttpGet("get-tickets")]
        public IActionResult GetAll()
        {
            return Ok(_context.Tickets.ToList());
        }
    }
}
