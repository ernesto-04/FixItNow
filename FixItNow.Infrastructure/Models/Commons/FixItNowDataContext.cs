using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Infrastructure.Models.Commons
{
    public class FixItNowDataContext : DbContext
    {
        public FixItNowDataContext(DbContextOptions<FixItNowDataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Technician> Technicians { get; set; }

    }
}
