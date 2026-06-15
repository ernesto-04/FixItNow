using FixItNow.Domain.Models;
using FixItNow.Domain.Models.BookingRequest.DTOs;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

public interface INotificationService
{
    Task<NotificationDto> CreateAsync(int userId, string message, string type, int? referenceId = null);
    Task<List<NotificationDto>> GetUnreadAsync(int userId);
    Task MarkAllReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
}

public class NotificationService : INotificationService
{
    private readonly FixItNowDataContext _context;

    public NotificationService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task<NotificationDto> CreateAsync(int userId, string message, string type, int? referenceId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Type = type,
            ReferenceId = referenceId
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return MapToDto(notification);
    }

    public async Task<List<NotificationDto>> GetUnreadAsync(int userId) =>
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => MapToDto(n))
            .ToListAsync();

    public async Task<int> GetUnreadCountAsync(int userId) =>
        await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task MarkAllReadAsync(int userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, n => true));
    }

    private static NotificationDto MapToDto(Notification n) => new()
    {
        Id = n.Id,
        UserId = n.UserId,
        Message = n.Message,
        Type = n.Type,
        ReferenceId = n.ReferenceId,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}