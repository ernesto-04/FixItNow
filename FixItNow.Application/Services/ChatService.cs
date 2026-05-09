using FixItNow.Domain.Models;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services
{
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
            var receiverExists = await _context.Users
           .AnyAsync(x => x.Id == dto.ReceiverId);

            if (!receiverExists)
            {
                throw new Exception("Receiver not found");
            }
            var message = new ChatMessage
            {
                SenderId = senderId,

                Id = Guid.NewGuid(),

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
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(x => x.TicketId == ticketId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
