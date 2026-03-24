namespace FixItNow.Domain.Models.Accesses
{
    public class Technician
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string SkillTypes { get; set; }
        public string AssignedZone { get; set; }
        public string Status { get; set; }
    }
}
