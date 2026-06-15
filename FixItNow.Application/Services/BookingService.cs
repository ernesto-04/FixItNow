using FixItNow.Domain.Models.BookingRequest.DTOs;
using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FixItNow.Domain.Models.BookingRequest.Tickets;
using FixItNow.Domain.Models.Bookings;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IBookingService
{
    Task<(bool Success, string Error, NotificationDto? Notification)> CreateBookingAsync(int customerId, CreateBookingRequestDto dto);
    Task<List<BookingRequestDto>> GetCustomerBookingsAsync(int customerId);
    Task<List<BookingRequestDto>> GetTechnicianBookingsAsync(int technicianId);
    Task<(bool Success, string Error, NotificationDto? Notification)> AcceptBookingAsync(int bookingId, int technicianId);
    Task<(bool Success, string Error, NotificationDto? Notification)> DeclineBookingAsync(int bookingId, int technicianId);
    Task<(bool Success, string Error, NotificationDto? Notification)> CancelBookingAsync(int bookingId, int userId, string reason);
}

public class BookingService : IBookingService
{
    private readonly FixItNowDataContext _context;
    private readonly INotificationService _notificationService;

    public BookingService(FixItNowDataContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<(bool Success, string Error, NotificationDto? Notification)> CreateBookingAsync(int customerId, CreateBookingRequestDto dto)
    {
        if (dto.TechnicianId == customerId)
            return (false, "You cannot book yourself.", null);
        var duplicate = await _context.BookingRequests.AnyAsync(b =>
            b.CustomerId == customerId &&
            b.TechnicianId == dto.TechnicianId &&
            b.Status == BookingStatus.Pending);
        if (duplicate)
            return (false, "You already have a pending booking with this technician.", null);
        var technicianProfile = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(t => t.UserId == dto.TechnicianId);
        if (technicianProfile is null || !technicianProfile.IsOnline)
            return (false, "This technician is currently offline and not accepting bookings.", null);
        var booking = new BookingRequest
        {
            CustomerId = customerId,
            TechnicianId = dto.TechnicianId,
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Location = dto.Location,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.BookingRequests.Add(booking);
        await _context.SaveChangesAsync();

        var customer = await _context.Users.FindAsync(customerId);
        var notification = await _notificationService.CreateAsync(
            technicianProfile.UserId,
            $"New booking request from {customer?.FullName ?? "a customer"}: {dto.Title}",
            "booking",
            booking.Id);
        return (true, string.Empty, notification);
    }

    public async Task<List<BookingRequestDto>> GetCustomerBookingsAsync(int customerId)
    {
        var bookings = await _context.BookingRequests
            .Include(b => b.Technician)
            .Include(b => b.Customer)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var bookingIds = bookings.Select(b => b.Id).ToList();
        var tickets = await _context.Tickets
            .Where(t => bookingIds.Contains(t.BookingRequestId!))
            .ToListAsync();

        return bookings.Select(b =>
        {
            var ticket = tickets.FirstOrDefault(t => t.BookingRequestId == b.Id);

            return new BookingRequestDto
            {
                Id = b.Id,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer.FullName,
                TechnicianId = b.TechnicianId,
                TechnicianName = b.Technician.FullName,
                Title = b.Title,
                Description = b.Description,
                Category = b.Category,
                Location = b.Location,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                TicketId = ticket?.Id,
                TicketStatus = ticket?.Status
            };
        }).ToList();
    }
    public async Task<List<BookingRequestDto>> GetTechnicianBookingsAsync(int technicianId)
    {
        return await _context.BookingRequests
            .Include(b => b.Customer)
            .Include(b => b.Technician)
            .Where(b => b.TechnicianId == technicianId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookingRequestDto
            {
                Id = b.Id,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer.FullName,
                TechnicianId = b.TechnicianId,
                TechnicianName = b.Technician.FullName,
                Title = b.Title,
                Description = b.Description,
                Category = b.Category,
                Location = b.Location,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Error, NotificationDto? Notification)> AcceptBookingAsync(int bookingId, int technicianId)
    {
        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId &&
                b.TechnicianId == technicianId &&
                b.Status == BookingStatus.Pending);
        if (booking is null)
            return (false, "Booking not found or already handled.", null);
        booking.Status = BookingStatus.Accepted;
        _context.Tickets.Add(new Ticket
        {
            BookingRequestId = bookingId,
            Title = booking.Title,
            Description = booking.Description,
            Category = booking.Category,
            Location = booking.Location,
            CustomerId = booking.CustomerId,
            AssignedTechnicianId = booking.TechnicianId,
            Status = TicketStatus.Assigned,
            CreatedAt = DateTime.UtcNow,
            Images = []
        });
        await _context.SaveChangesAsync();

        var technician = await _context.Users.FindAsync(technicianId);
        var notification = await _notificationService.CreateAsync(
            booking.CustomerId,
            $"Your booking request was accepted by {technician?.FullName ?? "the technician"}: {booking.Title}",
            "booking",
            booking.Id);
        return (true, string.Empty, notification);
    }

    public async Task<(bool Success, string Error, NotificationDto? Notification)> DeclineBookingAsync(int bookingId, int technicianId)
    {
        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId &&
                b.TechnicianId == technicianId &&
                b.Status == BookingStatus.Pending);
        if (booking is null)
            return (false, "Booking not found or already handled.", null);
        booking.Status = BookingStatus.Declined;
        await _context.SaveChangesAsync();

        var technician = await _context.Users.FindAsync(technicianId);
        var notification = await _notificationService.CreateAsync(
            booking.CustomerId,
            $"Your booking request was declined by {technician?.FullName ?? "the technician"}: {booking.Title}",
            "booking",
            booking.Id);
        return (true, string.Empty, notification);
    }

    public async Task<(bool Success, string Error, NotificationDto? Notification)> CancelBookingAsync(int bookingId, int userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return (false, "A cancellation reason is required.", null);
        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null)
            return (false, "Booking not found.", null);
        if (booking.CustomerId != userId && booking.TechnicianId != userId)
            return (false, "You are not authorized to cancel this booking.", null);
        if (booking.Status is not (BookingStatus.Pending or BookingStatus.Accepted))
            return (false, "This booking can no longer be cancelled.", null);
        var wasAccepted = booking.Status == BookingStatus.Accepted;
        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = reason;
        booking.CancelledByUserId = userId;
        if (wasAccepted)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t =>
                    t.CustomerId == booking.CustomerId &&
                    t.AssignedTechnicianId == booking.TechnicianId &&
                    t.Status != TicketStatus.Completed);
            if (ticket is not null)
                ticket.Status = TicketStatus.Cancelled;
        }
        await _context.SaveChangesAsync();

        var canceller = await _context.Users.FindAsync(userId);
        var recipientId = userId == booking.CustomerId
            ? booking.TechnicianId
            : booking.CustomerId;
        var notification = await _notificationService.CreateAsync(
            recipientId,
            $"Booking cancelled by {canceller?.FullName ?? "the other party"}: {booking.Title}. Reason: {reason}",
            "booking",
            booking.Id);
        return (true, string.Empty, notification);
    }
}