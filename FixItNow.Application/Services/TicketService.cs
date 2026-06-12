using FixItNow.Domain.Models.BookingRequest.DTOs.Tickets;
using FixItNow.Domain.Models.Bookings;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FixItNow.Application.Services;

public interface ITicketService
{
    Task<List<CustomerTicketResponse>> GetCustomerTicketsAsync(int userId);
    Task<List<TechnicianTicketResponse>> GetTechnicianTicketsAsync(int userId);
    Task AcceptTicketAsync(int ticketId, int technicianUserId);
    Task UpdateStatusAsync(int ticketId, int userId, TicketStatus newStatus);
    Task<TicketChatResponse?> GetTicketChatAsync(int ticketId, int userId);
    Task<TechnicianTicketResponse?> GetTechnicianTicketDetailAsync(int ticketId, int userId);
    Task<CustomerTicketResponse?> GetCustomerTicketDetailAsync(int ticketId, int userId);
    Task CancelTicketAsync(int ticketId, int userId, string reason);
}

public class TicketService : ITicketService
{
    private readonly FixItNowDataContext _context;
    private readonly string _baseUrl;

    private static readonly Dictionary<TicketStatus, List<TicketStatus>> _validTransitions = new()
    {
        { TicketStatus.Unassigned, [TicketStatus.Assigned] },
        { TicketStatus.Assigned,   [TicketStatus.InProgress, TicketStatus.Cancelled] },
        { TicketStatus.InProgress, [TicketStatus.Completed, TicketStatus.Cancelled] },
        { TicketStatus.Completed,  [] }
    };

    public TicketService(FixItNowDataContext context, IConfiguration config)
    {
        _context = context;
        _baseUrl = config["AppSettings:BaseUrl"] ?? string.Empty;
    }

    public async Task<List<CustomerTicketResponse>> GetCustomerTicketsAsync(int userId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Images)
            .Where(t => t.CustomerId == userId)
            .ToListAsync();

        return tickets.Select(t => new CustomerTicketResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Category = t.Category,
            Location = t.Location,
            Status = t.Status,
            TechnicianName = t.AssignedTechnician?.Username ?? "Not assigned",
            AssignedTechnicianId = t.AssignedTechnicianId,
            CustomerId = t.CustomerId,
            ImageUrls = t.Images.Select(i => $"{_baseUrl}{i.ImageUrl}").ToList()
        }).ToList();
    }

    public async Task<List<TechnicianTicketResponse>> GetTechnicianTicketsAsync(int userId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.Images)
            .Where(t => t.AssignedTechnicianId == userId)
            .ToListAsync();

        return tickets.Select(t => new TechnicianTicketResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Category = t.Category,
            Location = t.Location,
            Status = t.Status,
            CustomerName = t.Customer.Username,
            CustomerId = t.CustomerId,
            AssignedTechnicianId = t.AssignedTechnicianId,
            ImageUrls = t.Images.Select(i => $"{_baseUrl}{i.ImageUrl}").ToList(),
            CreatedAt = t.CreatedAt
        }).ToList();
    }

    public async Task<CustomerTicketResponse?> GetCustomerTicketDetailAsync(int ticketId, int userId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Images)
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == ticketId &&
                (t.CustomerId == userId || t.AssignedTechnicianId == userId));

        if (ticket is null) return null;

        return new CustomerTicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Category = ticket.Category,
            Location = ticket.Location,
            Status = ticket.Status,
            TechnicianName = ticket.AssignedTechnician?.Username,
            AssignedTechnicianId = ticket.AssignedTechnicianId,
            CustomerId = ticket.CustomerId,
            CreatedAt = ticket.CreatedAt,
            ImageUrls = ticket.Images.Select(i => $"{_baseUrl}{i.ImageUrl}").ToList()
        };
    }

    public async Task<TechnicianTicketResponse?> GetTechnicianTicketDetailAsync(int ticketId, int userId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Images)
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.AssignedTechnicianId == userId);

        if (ticket is null) return null;

        return new TechnicianTicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Category = ticket.Category,
            Location = ticket.Location,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            CustomerName = ticket.Customer.Username,
            ImageUrls = ticket.Images.Select(i => i.ImageUrl).ToList()
        };
    }

    public async Task AcceptTicketAsync(int ticketId, int technicianUserId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId)
            ?? throw new KeyNotFoundException("Ticket not found.");

        if (ticket.Status != TicketStatus.Unassigned)
            throw new InvalidOperationException("Ticket is already taken.");

        var isTechnician = await _context.TechnicianProfiles
            .AnyAsync(tp => tp.UserId == technicianUserId);

        if (!isTechnician)
            throw new InvalidOperationException("User is not a technician.");

        ticket.AssignedTechnicianId = technicianUserId;
        ticket.Status = TicketStatus.Assigned;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int ticketId, int userId, TicketStatus newStatus)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId)
            ?? throw new KeyNotFoundException("Ticket not found.");

        if (ticket.AssignedTechnicianId != userId)
            throw new InvalidOperationException("You are not assigned to this ticket.");

        if (!_validTransitions.TryGetValue(ticket.Status, out var allowed) || !allowed.Contains(newStatus))
            throw new InvalidOperationException($"Invalid status transition: {ticket.Status} → {newStatus}.");

        ticket.Status = newStatus;
        await _context.SaveChangesAsync();
    }

    public async Task<TicketChatResponse?> GetTicketChatAsync(int ticketId, int userId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(t => t.Id == ticketId &&
                (t.CustomerId == userId || t.AssignedTechnicianId == userId));

        if (ticket is null) return null;

        return new TicketChatResponse
        {
            Id = ticket.Id,
            CustomerId = ticket.CustomerId,
            AssignedTechnicianId = ticket.AssignedTechnicianId,
            TechnicianName = ticket.AssignedTechnician?.Username,
            CustomerName = ticket.Customer.Username,
            Status = ticket.Status,
            Title = ticket.Title,
            ReceiverId = ticket.CustomerId == userId
                ? ticket.AssignedTechnicianId ?? 0
                : ticket.CustomerId
        };
    }

    public async Task CancelTicketAsync(int ticketId, int userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("A cancellation reason is required.");

        var ticket = await _context.Tickets.FindAsync(ticketId)
            ?? throw new KeyNotFoundException("Ticket not found.");

        if (ticket.AssignedTechnicianId != userId)
            throw new InvalidOperationException("You are not assigned to this ticket.");

        if (!_validTransitions.TryGetValue(ticket.Status, out var allowed) || !allowed.Contains(TicketStatus.Cancelled))
            throw new InvalidOperationException($"Cannot cancel a ticket with status {ticket.Status}.");

        ticket.Status = TicketStatus.Cancelled;

        var booking = await _context.BookingRequests
            .FirstOrDefaultAsync(b =>
                b.CustomerId == ticket.CustomerId &&
                b.TechnicianId == ticket.AssignedTechnicianId &&
                b.Status == BookingStatus.Accepted);

        if (booking is not null)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = reason;
            booking.CancelledByUserId = userId;
        }

        await _context.SaveChangesAsync();
    }
}