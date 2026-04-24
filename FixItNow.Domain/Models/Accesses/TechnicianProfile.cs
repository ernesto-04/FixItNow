namespace FixItNow.Domain.Models.Accesses
{
    public class TechnicianProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Skills { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}
