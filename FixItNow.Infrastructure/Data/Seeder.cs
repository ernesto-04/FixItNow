using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Infrastructure.Models.Commons;

namespace FixItNow.Infrastructure.Data
{
    public static class Seeder
    {
        public static void Seed(FixItNowDataContext context)
        {
            // Check if the database is already seeded
            if (context.Users.Any()) return;

            // Seed Users
            var tech1 = new User
            {
                Email = "tech1@mail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "Technician"
            };

            var tech2 = new User
            {
                Email = "tech2@mail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "Technician"
            };
            context.Users.AddRange(tech1, tech2);
            context.SaveChanges();

            var technicians = new List<Technician>
            {
                new Technician { UserId = tech1.Id, SkillTypes = "Electrical", AssignedZone  = "Building A", Status = "Available" },
                new Technician { UserId = tech2.Id, SkillTypes = "Plumbing", AssignedZone  = "Building B", Status = "Available" },
            };
            context.Technicians.AddRange(technicians);
            context.SaveChanges();
        }
    }
}
