namespace FixItNow.Domain.Models.Accesses;
public class TechnicianProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string? Bio { get; set; }
    public string Skills { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int YearsExperience { get; set; }
    public string? ProfileImageUrl { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? CallOutFee { get; set; }
    public bool IsOnline { get; set; } = false;
    public string? PhoneNumber { get; set; }
    public bool IsApproved { get; set; } = false;
    public bool IsRejected { get; set; } = false;
    public string? RejectionReason { get; set; }
}