namespace FixItNow.Domain.Models.BookingRequest.DTOs.Technicians
{
    public class TechnicianProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int YearsExperience { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int CompletedJobs { get; set; }
        public double AverageRating { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? CallOutFee { get; set; }
        public bool IsOnline { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string? RejectionReason { get; set; }
    }
}