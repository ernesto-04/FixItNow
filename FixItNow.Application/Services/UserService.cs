using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IUserService
{
    Task BecomeTechnicianAsync(int userId, CreateTechnicianProfileRequest request);
    Task<bool> IsTechnicianAsync(int userId);
    Task<TechnicianProfile?> GetTechnicianStatusAsync(int userId);
}

public class UserService : IUserService
{
    private readonly FixItNowDataContext _context;

    public UserService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task BecomeTechnicianAsync(int userId, CreateTechnicianProfileRequest request)
    {
        var existing = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(tp => tp.UserId == userId);

        if (existing is not null && existing.IsApproved)
            throw new InvalidOperationException("User is already an approved technician.");

        if (existing is not null && existing.IsRejected)
        {
            // Reset for re-application
            existing.Skills = request.Skills;
            existing.Location = request.Location;
            existing.Bio = request.Bio;
            existing.IsRejected = false;
            existing.RejectionReason = null;
        }
        else if (existing is null)
        {
            await _context.TechnicianProfiles.AddAsync(new TechnicianProfile
            {
                UserId = userId,
                Skills = request.Skills,
                Location = request.Location,
                Bio = request.Bio
            });
        }
        else
        {
            throw new InvalidOperationException("Application already pending.");
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTechnicianAsync(int userId)
        => await _context.TechnicianProfiles.AnyAsync(tp => tp.UserId == userId);

    public async Task<TechnicianProfile?> GetTechnicianStatusAsync(int userId)
        => await _context.TechnicianProfiles.FirstOrDefaultAsync(tp => tp.UserId == userId);
}