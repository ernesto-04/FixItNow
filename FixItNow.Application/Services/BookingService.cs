using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FixItNow.Domain.Models.BookingRequest.Tickets;
using FixItNow.Domain.Models.Bookings;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IBookingService
{
    Task<(bool Success, string Error)> CreateBookingAsync(int customerId, CreateBookingRequestDto dto);
    Task<List<BookingRequestDto>> GetCustomerBookingsAsync(int customerId);
    Task<List<BookingRequestDto>> GetTechnicianBookingsAsync(int technicianId);
    Task<(bool Success, string Error)> AcceptBookingAsync(int bookingId, int technicianId);
    Task<(bool Success, string Error)> DeclineBookingAsync(int bookingId, int technicianId);
    Task<(bool Success, string Error)> CancelBookingAsync(int bookingId, int userId, string reason);
}

public class BookingService : IBookingService
{
    private readonly FixItNowDataContext _context;

    public BookingService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Error)> CreateBookingAsync(int customerId, CreateBookingRequestDto dto)
    {
        if (dto.TechnicianId == customerId)
            return (false, "You cannot book yourself.");

        var duplicate = await _context.BookingRequests.AnyAsync(b =>
            b.CustomerId == customerId &&
            b.TechnicianId == dto.TechnicianId &&
            b.Status == BookingStatus.Pending);

        if (duplicate)
            return (false, "You already have a pending booking with this technician.");

        var technicianProfile = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(t => t.UserId == dto.TechnicianId);

        if (technicianProfile is null || !technicianProfile.IsOnline)
            return (false, "This technician is currently offline and not accepting bookings.");

        _context.BookingRequests.Add(new BookingRequest
        {
            CustomerId = customerId,
            TechnicianId = dto.TechnicianId,
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Location = dto.Location,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<List<BookingRequestDto>> GetCustomerBookingsAsync(int customerId)
    {
        var bookings = await _context.BookingRequests
            .Include(b => b.Technician)
            .Include(b => b.Customer)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        // Single query for all related tickets — avoids N+1
        var technicianIds = bookings.Select(b => b.TechnicianId).Distinct().ToList();
        var tickets = await _context.Tickets
            .Where(t => t.CustomerId == customerId && technicianIds.Contains(t.AssignedTechnicianId!.Value))
            .ToListAsync();

        return bookings.Select(b =>
        {
            var ticket = tickets.FirstOrDefault(t =>
                t.CustomerId == b.CustomerId &&
                t.AssignedTechnicianId == b.TechnicianId);

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

    public async Task<(bool Success, string Error)> AcceptBookingAsync(int bookingId, int technicianId)
    {
        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId &&
                b.TechnicianId == technicianId &&
                b.Status == BookingStatus.Pending);

        if (booking is null)
            return (false, "Booking not found or already handled.");

        booking.Status = BookingStatus.Accepted;

        _context.Tickets.Add(new Ticket
        {
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
        return (true, string.Empty);
    }

    public async Task<(bool Success, string Error)> DeclineBookingAsync(int bookingId, int technicianId)
    {
        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b =>
                b.Id == bookingId &&
                b.TechnicianId == technicianId &&
                b.Status == BookingStatus.Pending);

        if (booking is null)
            return (false, "Booking not found or already handled.");

        booking.Status = BookingStatus.Declined;
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool Success, string Error)> CancelBookingAsync(int bookingId, int userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return (false, "A cancellation reason is required.");

        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking is null)
            return (false, "Booking not found.");

        if (booking.CustomerId != userId && booking.TechnicianId != userId)
            return (false, "You are not authorized to cancel this booking.");

        if (booking.Status is not (BookingStatus.Pending or BookingStatus.Accepted))
            return (false, "This booking can no longer be cancelled.");

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
        return (true, string.Empty);
    }
}