using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixItNow.Domain.Models.DTOs
{
    public class CreateTechnicianProfileRequest
    {
        public string Skills { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
    }
}
