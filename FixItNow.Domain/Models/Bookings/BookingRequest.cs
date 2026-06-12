using FixItNow.Domain.Models.Accesses;
namespace FixItNow.Domain.Models.Bookings;
public class BookingRequest
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    public int TechnicianId { get; set; }
    public User Technician { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CancellationReason { get; set; }
    public int? CancelledByUserId { get; set; }
}