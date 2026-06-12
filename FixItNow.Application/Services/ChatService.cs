using FixItNow.Domain.Models.BookingRequest.Chat;
using FixItNow.Domain.Models.BookingRequest.DTOs.Chat;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IChatService
{
    Task<ChatMessage> SendMessageAsync(int senderId, SendMessageDto dto);
    Task<List<ChatMessage>> GetMessagesByTicketIdAsync(int ticketId);
}

public class ChatService : IChatService
{
    private readonly FixItNowDataContext _context;

    public ChatService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task<ChatMessage> SendMessageAsync(int senderId, SendMessageDto dto)
    {
        var receiverExists = await _context.Users.AnyAsync(u => u.Id == dto.ReceiverId);
        if (!receiverExists)
            throw new KeyNotFoundException("Receiver not found.");

        var message = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            TicketId = dto.TicketId,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<ChatMessage>> GetMessagesByTicketIdAsync(int ticketId)
    {
        return await _context.ChatMessages
            .Where(x => x.TicketId == ticketId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new ChatMessage
            {
                Id = x.Id,
                SenderId = x.SenderId,
                ReceiverId = x.ReceiverId,
                TicketId = x.TicketId,
                Message = x.Message,
                CreatedAt = x.CreatedAt,
                IsRead = x.IsRead
            })
            .ToListAsync();
    }
}