using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services
{
    public interface IUserService
    {
        Task BecomeTechnician(
            int userId,
            CreateTechnicianProfileRequest request);
    }

    public class UserService : IUserService
    {
        private readonly FixItNowDataContext _context;

        public UserService(FixItNowDataContext context)
        {
            _context = context;
        }

        public async Task BecomeTechnician(
            int userId,
            CreateTechnicianProfileRequest request)
        {
            var exists = await _context.TechnicianProfiles
                .AnyAsync(tp => tp.UserId == userId);

            if (exists)
                throw new Exception("User is already a technician");

            var profile = new TechnicianProfile
            {
                UserId = userId,
                Skills = request.Skills,
                Location = request.Location,
                Bio = request.Bio,
            };

            await _context.TechnicianProfiles.AddAsync(profile);

            await _context.SaveChangesAsync();
        }
    }
}