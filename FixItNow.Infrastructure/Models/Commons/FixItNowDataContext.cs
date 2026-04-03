using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Infrastructure.Models.Commons
{
    public class FixItNowDataContext : DbContext
    {
        public FixItNowDataContext(DbContextOptions<FixItNowDataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(u => u.PasswordHash)
                    .IsRequired();
            });
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleName });
                entity.Property(ur => ur.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.Roles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(t => t.Description)
                    .IsRequired();
                entity.Property(t => t.Category)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(t => t.Location)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(t => t.Status)
                    .IsRequired();
                entity.Property(t => t.CreatedAt)
                    .IsRequired();
                entity.HasOne(t => t.Customer)
                    .WithMany(u => u.CreatedTickets)
                    .HasForeignKey(t => t.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(t => t.AssignedTechnician)
                    .WithMany(u => u.AssignedTickets)
                    .HasForeignKey(t => t.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
