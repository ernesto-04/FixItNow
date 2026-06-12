using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface ITechnicianService
{
    Task<List<TechnicianProfileDto>> GetAllTechniciansAsync(int currentUserId);
    Task<TechnicianProfileDto?> GetTechnicianByIdAsync(int userId);
    Task<TechnicianProfileDto?> UpdateProfileAsync(int userId, UpdateTechnicianProfileDto dto);
}

public class TechnicianService : ITechnicianService
{
    private readonly FixItNowDataContext _context;

    public TechnicianService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task<List<TechnicianProfileDto>> GetAllTechniciansAsync(int currentUserId)
    {
        return await _context.TechnicianProfiles
            .Where(tp => tp.UserId != currentUserId)
            .Select(tp => new TechnicianProfileDto
            {
                Id = tp.Id,
                UserId = tp.UserId,
                FullName = tp.User.FullName,
                Bio = tp.Bio ?? string.Empty,
                Skills = tp.Skills,
                Location = tp.Location,
                YearsExperience = tp.YearsExperience,
                ProfileImageUrl = tp.ProfileImageUrl ?? string.Empty,
                CompletedJobs = _context.Tickets.Count(t =>
                    t.AssignedTechnicianId == tp.UserId &&
                    t.Status == TicketStatus.Completed),
                AverageRating = _context.Reviews
                    .Where(r => r.TechnicianId == tp.UserId)
                    .Average(r => (double?)r.Rating) ?? 0,
                HourlyRate = tp.HourlyRate,
                CallOutFee = tp.CallOutFee
            })
            .ToListAsync();
    }

    public async Task<TechnicianProfileDto?> GetTechnicianByIdAsync(int userId)
    {
        return await _context.TechnicianProfiles
            .Where(tp => tp.UserId == userId)
            .Select(tp => new TechnicianProfileDto
            {
                Id = tp.Id,
                UserId = tp.UserId,
                FullName = tp.User.FullName,
                Bio = tp.Bio ?? string.Empty,
                Skills = tp.Skills,
                Location = tp.Location,
                YearsExperience = tp.YearsExperience,
                ProfileImageUrl = tp.ProfileImageUrl ?? string.Empty,
                CompletedJobs = _context.Tickets.Count(t =>
                    t.AssignedTechnicianId == tp.UserId &&
                    t.Status == TicketStatus.Completed),
                AverageRating = _context.Reviews
                    .Where(r => r.TechnicianId == tp.UserId)
                    .Average(r => (double?)r.Rating) ?? 0,
                HourlyRate = tp.HourlyRate,
                CallOutFee = tp.CallOutFee
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TechnicianProfileDto?> UpdateProfileAsync(int userId, UpdateTechnicianProfileDto dto)
    {
        var profile = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(tp => tp.UserId == userId);
        if (profile is null) return null;
        profile.Bio = dto.Bio;
        profile.Skills = dto.Skills;
        profile.Location = dto.Location;
        profile.YearsExperience = dto.YearsExperience;
        profile.ProfileImageUrl = dto.ProfileImageUrl;
        profile.HourlyRate = dto.HourlyRate;
        profile.CallOutFee = dto.CallOutFee;
        await _context.SaveChangesAsync();
        return await GetTechnicianByIdAsync(userId);
    }
}