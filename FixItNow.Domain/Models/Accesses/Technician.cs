using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixItNow.Domain.Models.Accesses
{
    public class Technician
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SkillTypes { get; set; }
        public string AssignedZone { get; set; }
        public string Status { get; set; }
    }
}
