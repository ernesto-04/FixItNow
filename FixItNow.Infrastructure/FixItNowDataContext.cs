using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.Chat;
using FixItNow.Domain.Models.BookingRequest.Reviews;
using FixItNow.Domain.Models.BookingRequest.Tickets;
using FixItNow.Domain.Models.Bookings;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Infrastructure
{
    public class FixItNowDataContext : DbContext
    {
        public FixItNowDataContext(DbContextOptions<FixItNowDataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TechnicianProfile> TechnicianProfiles { get; set; }
        public DbSet<TicketImage> TicketImages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BookingRequest> BookingRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.PasswordHash).IsRequired();

                entity.Property(u => u.IsAdmin)
                    .IsRequired()
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<TechnicianProfile>(entity =>
            {
                entity.HasKey(tp => tp.Id);

                entity.Property(tp => tp.Bio).HasMaxLength(1000);
                entity.Property(tp => tp.Skills).IsRequired();
                entity.Property(tp => tp.Location).IsRequired().HasMaxLength(255);
                entity.Property(tp => tp.YearsExperience).IsRequired().HasDefaultValue(0);
                entity.Property(tp => tp.ProfileImageUrl).HasMaxLength(500);
                entity.Property(tp => tp.IsApproved)
                    .IsRequired()
                    .HasDefaultValue(false);
                entity.Property(tp => tp.IsRejected)
                    .IsRequired()
                    .HasDefaultValue(false);
                entity.Property(tp => tp.RejectionReason)
                    .HasMaxLength(500);

                entity.HasOne(tp => tp.User)
                    .WithOne(u => u.TechnicianProfile)
                    .HasForeignKey<TechnicianProfile>(tp => tp.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title).IsRequired().HasMaxLength(255);
                entity.Property(t => t.Description).IsRequired();
                entity.Property(t => t.Category).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Location).IsRequired().HasMaxLength(255);
                entity.Property(t => t.Status).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();

                entity.HasOne(t => t.Customer)
                    .WithMany(u => u.CreatedTickets)
                    .HasForeignKey(t => t.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AssignedTechnician)
                    .WithMany()
                    .HasForeignKey(t => t.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(t => t.Images)
                    .WithOne(i => i.Ticket)
                    .HasForeignKey(i => i.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TicketImage>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Message).IsRequired().HasMaxLength(2000);
                entity.Property(x => x.CreatedAt).IsRequired();
                entity.Property(x => x.IsRead).HasDefaultValue(false);

                entity.HasOne(x => x.Sender)
                    .WithMany(x => x.SentMessages)
                    .HasForeignKey(x => x.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Receiver)
                    .WithMany(x => x.ReceivedMessages)
                    .HasForeignKey(x => x.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Ticket)
                    .WithMany(x => x.ChatMessages)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.TicketId, x.CreatedAt });
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.Comment).HasMaxLength(1000);
                entity.Property(r => r.CreatedAt).IsRequired();

                entity.HasIndex(r => r.TicketId).IsUnique();

                entity.HasOne(r => r.Ticket)
                    .WithOne(t => t.Review)
                    .HasForeignKey<Review>(r => r.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Customer)
                    .WithMany()
                    .HasForeignKey(r => r.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Technician)
                    .WithMany()
                    .HasForeignKey(r => r.TechnicianId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookingRequest>(entity =>
            {
                entity.ToTable("booking_requests");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title).IsRequired().HasMaxLength(255);
                entity.Property(b => b.Description).IsRequired();
                entity.Property(b => b.Category).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Location).IsRequired().HasMaxLength(255);
                entity.Property(b => b.Status).IsRequired().HasConversion<string>();
                entity.Property(b => b.CreatedAt).IsRequired();

                entity.HasOne(b => b.Customer)
                    .WithMany()
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Technician)
                    .WithMany()
                    .HasForeignKey(b => b.TechnicianId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
