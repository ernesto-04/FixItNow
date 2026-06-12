using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FixItNow.Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/tickets")]
[ApiController]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        await _ticketService.AcceptTicketAsync(id, GetUserId());
        return Ok();
    }

    [HttpGet("technician-tickets")]
    public async Task<IActionResult> GetTechnicianTickets()
    {
        var tickets = await _ticketService.GetTechnicianTicketsAsync(GetUserId());
        return Ok(tickets);
    }

    [HttpGet("customer-tickets")]
    public async Task<IActionResult> GetCustomerTickets()
    {
        var tickets = await _ticketService.GetCustomerTicketsAsync(GetUserId());
        return Ok(tickets);
    }

    [HttpPut("{ticketId:int}/status")]
    public async Task<IActionResult> UpdateStatus(int ticketId, [FromQuery] TicketStatus status)
    {
        await _ticketService.UpdateStatusAsync(ticketId, GetUserId(), status);
        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomerTicketDetail(int id)
    {
        var ticket = await _ticketService.GetCustomerTicketDetailAsync(id, GetUserId());
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpGet("{id:int}/technician-detail")]
    public async Task<IActionResult> GetTechnicianTicketDetail(int id)
    {
        var ticket = await _ticketService.GetTechnicianTicketDetailAsync(id, GetUserId());
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpGet("{ticketId}/chat")]
    public async Task<IActionResult> GetTicketChat(int ticketId)
    {
        var result = await _ticketService.GetTicketChatAsync(ticketId, GetUserId());
        return Ok(result);
    }

    [HttpPost("{ticketId:int}/cancel")]
    public async Task<IActionResult> Cancel(int ticketId, [FromBody] CancelBookingRequestDto dto)
    {
        await _ticketService.CancelTicketAsync(ticketId, GetUserId(), dto.Reason);
        return Ok();
    }

    private int GetUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException();
}