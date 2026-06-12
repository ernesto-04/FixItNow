namespace FixItNow.Domain.Models.BookingRequest.DTOs.Technicians
{
    public class UpdateTechnicianProfileDto
    {
        public string Bio { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int YearsExperience { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public decimal? HourlyRate { get; set; }
        public decimal? CallOutFee { get; set; }
    }
}