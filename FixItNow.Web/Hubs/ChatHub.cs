using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs;
using FixItNow.Domain.Models.BookingRequest.DTOs.Chat;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FixItNow.Web.Hubs
{
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
        public async Task SendMessage(SendMessageDto dto,
            [FromServices] IValidator<SendMessageDto> validator)
        {
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
                throw new HubException("Invalid message");

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

        public static async Task SendNotificationToUser(
            IHubContext<ChatHub> hubContext,
            int userId,
            NotificationDto notification)
        {
            await hubContext.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }
    }
}