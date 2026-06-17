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
    Task<bool> SetOnlineStatusAsync(int userId, bool isOnline);
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
            .Where(tp => tp.UserId != currentUserId && tp.IsApproved && !tp.IsRejected)
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
                CallOutFee = tp.CallOutFee,
                IsOnline = tp.IsOnline,
                PhoneNumber = tp.PhoneNumber ?? string.Empty,
                IsApproved = tp.IsApproved
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
                CallOutFee = tp.CallOutFee,
                IsOnline = tp.IsOnline,
                PhoneNumber = tp.PhoneNumber ?? string.Empty,
                IsApproved = tp.IsApproved
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
        profile.PhoneNumber = dto.PhoneNumber;
        await _context.SaveChangesAsync();
        return await GetTechnicianByIdAsync(userId);
    }

    public async Task<bool> SetOnlineStatusAsync(int userId, bool isOnline)
    {
        var profile = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (profile is null) return false;

        profile.IsOnline = isOnline;
        await _context.SaveChangesAsync();
        return true;
    }
}