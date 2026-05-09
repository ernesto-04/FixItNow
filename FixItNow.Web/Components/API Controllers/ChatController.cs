using FixItNow.Application.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/chat")]
[ApiController]
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
        var messages =
            await _chatService
                .GetMessagesByTicketIdAsync(ticketId);

        return Ok(messages);
    }
}