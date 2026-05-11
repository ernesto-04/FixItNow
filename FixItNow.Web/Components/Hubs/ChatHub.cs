using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    // Join ticket room
    public async Task JoinTicketRoom(int ticketId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            $"ticket-{ticketId}"
        );
    }

    // Send message
    public async Task SendMessage(SendMessageDto dto)
    {

        var userIdClaim = Context.User?
    .FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new HubException("Unauthorized");
        }

        var senderId = int.Parse(userIdClaim.Value);

        var message = await _chatService.SendMessageAsync(
            senderId,
            dto
        );

        await Clients.Group($"ticket-{dto.TicketId}")
            .SendAsync("ReceiveMessage", message);
    }
}