using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services
{
    public class UserService
    {
        private readonly FixItNowDataContext _context;

        public UserService(FixItNowDataContext context)
        {
            _context = context;
        }
        public void BecomeTechnician(int userId, CreateTechnicianProfileRequest request)
        {
            var exists = _context.TechnicianProfiles
                .Any(tp => tp.UserId == userId);

            if (exists)
                throw new Exception("User is already a technician");

            var profile = new TechnicianProfile
            {
                UserId = userId,
                Skills = request.Skills,
                Location = request.Location,
                Bio = request.Bio,
            };

            _context.TechnicianProfiles.Add(profile);
            _context.SaveChanges();
        }
    }
}
