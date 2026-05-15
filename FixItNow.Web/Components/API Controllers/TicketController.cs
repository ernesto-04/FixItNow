using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly FixItNowDataContext _context;

        public TicketController(ITicketService ticketService, FixItNowDataContext context)
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
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var tickets = _ticketService.GetAvailableTickets(currentUserId);

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var jobs = _ticketService.GetTechnicianTickets(userId);

            return Ok(jobs);
        }
        [HttpGet("customer-tickets")]
        public IActionResult GetCustomerTickets()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tickets = _ticketService.GetCustomerTickets(userId);

            return Ok(tickets);
        }
        [HttpPut("{ticketId:int}/status")]
        public IActionResult UpdateStatus(int ticketId, [FromQuery] TicketStatus status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _ticketService.UpdateStatus(ticketId, userId, status);

            return Ok();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerTicketDetail(int id)
        {
            var userId = GetUserId();

            var ticket = await _ticketService.GetCustomerTicketDetailAsync(id, userId);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }
        [HttpGet("{id:int}/technician")]
        public async Task<IActionResult> GetTicketForTechnician(int id)
        {
            var userId = GetUserId();

            var ticket = await _ticketService.GetTicketForTechnicianAsync(id, userId);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }
        [HttpGet("{id:int}/technician-detail")]
        public async Task<IActionResult> GetTechnicianTicketDetail(int id)
        {
            var userId = GetUserId();

            var ticket = await _ticketService.GetTechnicianTicketDetailAsync(id, userId);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }
        [HttpGet("{ticketId}/chat")]
        public async Task<IActionResult> GetTicketChat(
    int ticketId)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result =
                await _ticketService.GetTicketChatAsync(
                    ticketId,
                    userId
                );

            return Ok(result);
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }
}
