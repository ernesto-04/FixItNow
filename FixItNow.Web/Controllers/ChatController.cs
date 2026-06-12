using FixItNow.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/chat")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("ticket/{ticketId}")]
    public async Task<IActionResult> GetMessages(int ticketId)
    {
        return Ok(await _chatService.GetMessagesByTicketIdAsync(ticketId));
    }
}