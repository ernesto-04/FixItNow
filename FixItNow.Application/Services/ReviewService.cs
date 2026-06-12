using FixItNow.Domain.Models.BookingRequest.DTOs.Reviews;
using FixItNow.Domain.Models.BookingRequest.Reviews;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IReviewService
{
    Task<(bool Success, string Error)> CreateReviewAsync(int customerId, CreateReviewRequest request);
    Task<List<ReviewDto>> GetReviewsByTechnicianAsync(int technicianUserId);
    Task<bool> HasReviewedAsync(int ticketId);
}

public class ReviewService : IReviewService
{
    private readonly FixItNowDataContext _context;

    public ReviewService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Error)> CreateReviewAsync(int customerId, CreateReviewRequest request)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t =>
                t.Id == request.TicketId &&
                t.CustomerId == customerId &&
                t.Status == TicketStatus.Completed);

        if (ticket is null)
            return (false, "Ticket not found or not completed.");

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.TicketId == request.TicketId);

        if (alreadyReviewed)
            return (false, "This ticket has already been reviewed.");

        _context.Reviews.Add(new Review
        {
            TicketId = request.TicketId,
            CustomerId = customerId,
            TechnicianId = request.TechnicianId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<List<ReviewDto>> GetReviewsByTechnicianAsync(int technicianUserId)
    {
        return await _context.Reviews
            .Where(r => r.TechnicianId == technicianUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                TicketId = r.TicketId,
                CustomerName = r.Customer.FullName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> HasReviewedAsync(int ticketId)
        => await _context.Reviews.AnyAsync(r => r.TicketId == ticketId);
}