using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FixItNow.Domain.Models.BookingRequest.Tickets;
using FixItNow.Domain.Models.DTOs.Technicians;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IAdminService
{
    Task<List<TechnicianProfileDto>> GetPendingTechniciansAsync();
    Task<(bool Success, string Error)> ApproveTechnicianAsync(int userId);
    Task<(bool Success, string Error)> RejectTechnicianAsync(int userId, string reason);
    Task<List<TechnicianProfileDto>> GetRejectedTechniciansAsync();
}

public class AdminService : IAdminService
{
    private readonly FixItNowDataContext _context;

    public AdminService(FixItNowDataContext context)
    {
        _context = context;
    }

    public async Task<List<TechnicianProfileDto>> GetPendingTechniciansAsync()
    {
        return await _context.TechnicianProfiles
            .Where(tp => !tp.IsApproved && !tp.IsRejected)
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
                CompletedJobs = 0,
                AverageRating = 0,
                HourlyRate = tp.HourlyRate,
                CallOutFee = tp.CallOutFee,
                IsOnline = tp.IsOnline,
                PhoneNumber = tp.PhoneNumber ?? string.Empty,
                IsApproved = tp.IsApproved
            })
            .ToListAsync();
    }

    public async Task<List<TechnicianProfileDto>> GetRejectedTechniciansAsync()
    {
        return await _context.TechnicianProfiles
            .Where(tp => tp.IsRejected)
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
                CompletedJobs = 0,
                AverageRating = 0,
                HourlyRate = tp.HourlyRate,
                CallOutFee = tp.CallOutFee,
                IsOnline = false,
                PhoneNumber = tp.PhoneNumber ?? string.Empty,
                IsApproved = false,
                IsRejected = tp.IsRejected,
                RejectionReason = tp.RejectionReason
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Error)> ApproveTechnicianAsync(int userId)
    {
        var profile = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(tp => tp.UserId == userId);
        if (profile is null)
            return (false, "Technician not found.");
        if (profile.IsApproved)
            return (false, "Technician is already approved.");
        profile.IsApproved = true;
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool Success, string Error)> RejectTechnicianAsync(int userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return (false, "A rejection reason is required.");
        var profile = await _context.TechnicianProfiles
            .FirstOrDefaultAsync(tp => tp.UserId == userId);
        if (profile is null)
            return (false, "Technician not found.");
        if (profile.IsApproved)
            return (false, "Cannot reject an already approved technician.");
        profile.IsRejected = true;
        profile.RejectionReason = reason;
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }
}