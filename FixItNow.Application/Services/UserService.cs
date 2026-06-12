using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IUserService
{
    Task BecomeTechnicianAsync(int userId, CreateTechnicianProfileRequest request);
    Task<bool> IsTechnicianAsync(int userId);
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
        var exists = await _context.TechnicianProfiles
            .AnyAsync(tp => tp.UserId == userId);

        if (exists)
            throw new InvalidOperationException("User is already a technician.");

        await _context.TechnicianProfiles.AddAsync(new TechnicianProfile
        {
            UserId = userId,
            Skills = request.Skills,
            Location = request.Location,
            Bio = request.Bio
        });

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTechnicianAsync(int userId)
        => await _context.TechnicianProfiles.AnyAsync(tp => tp.UserId == userId);
}