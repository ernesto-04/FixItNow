using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(CreateTicketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var createdTicket = await _ticketService.CreateTicketAsync(userId, request);

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
                .Select(t => new TechnicianTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Location = t.Location,
                    Status = t.Status,
                    CustomerName = t.Customer.Email
                })
                .ToList();

            return Ok(jobs);
        }
        [HttpGet("customer-tickets")]
        public IActionResult GetCustomerTickets()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tickets = _context.Tickets
                .Include(t => t.AssignedTechnician)
                .Where(t => t.CustomerId == userId)
                .Select(t => new CustomerTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Location = t.Location,
                    Status = t.Status,
                    TechnicianName = t.AssignedTechnician != null
                        ? t.AssignedTechnician.Email
                        : "Not assigned"
                })
                .ToList();

            return Ok(tickets);
        }
        [HttpPut("{ticketId}/status")]
        public IActionResult UpdateStatus(int ticketId, [FromQuery] TicketStatus status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _ticketService.UpdateStatus(ticketId, userId, status);

            return Ok();
        }
    }
}
