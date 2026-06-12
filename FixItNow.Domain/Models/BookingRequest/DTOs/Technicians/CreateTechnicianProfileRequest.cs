namespace FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;

public class CreateTechnicianProfileRequest
{
    public string Skills { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Bio { get; set; }
}