using FixItNow.Domain.Models;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Infrastructure.Models.Commons
{
    public class FixItNowDataContext : DbContext
    {
        public FixItNowDataContext(DbContextOptions<FixItNowDataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TechnicianProfile> TechnicianProfiles { get; set; }
        public DbSet<TicketImage> TicketImages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.HasIndex(u => u.Email)
                    .IsUnique();
                entity.Property(u => u.PasswordHash)
                    .IsRequired();
                entity.HasOne(u => u.TechnicianProfile)
                    .WithOne(tp => tp.User)
                    .HasForeignKey<TechnicianProfile>(tp => tp.UserId);
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
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.AssignedTechnician)
                    .WithMany(u => u.AssignedTickets)
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

                entity.Property(i => i.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(i => i.Ticket)
                    .WithMany(t => t.Images)
                    .HasForeignKey(i => i.TicketId);
            });
            modelBuilder.Entity<TechnicianProfile>(entity =>
            {
                entity.HasKey(tp => tp.Id);
                entity.Property(tp => tp.Skills)
                    .IsRequired();
                entity.Property(tp => tp.Location)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(tp => tp.Bio)
                    .HasMaxLength(1000);
                entity.HasOne(tp => tp.User)
                    .WithOne(u => u.TechnicianProfile)
                    .HasForeignKey<TechnicianProfile>(tp => tp.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.IsRead)
                    .HasDefaultValue(false);

                // Sender relationship
                entity.HasOne(x => x.Sender)
                    .WithMany(x => x.SentMessages)
                    .HasForeignKey(x => x.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Receiver relationship
                entity.HasOne(x => x.Receiver)
                    .WithMany(x => x.ReceivedMessages)
                    .HasForeignKey(x => x.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ticket relationship
                entity.HasOne(x => x.Ticket)
                    .WithMany(x => x.ChatMessages)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(x => x.TicketId);

                entity.HasIndex(x => new
                {
                    x.TicketId,
                    x.CreatedAt
                });
            });
        }
    }
}
