using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixItNow.Domain.Models.Accesses
{
    public class UserRole
    {
        public int UserId { get; set; }
        public string RoleName { get; set; } // "Customer", "Technician"

        public User User { get; set; }
    }
}
