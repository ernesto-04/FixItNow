using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixItNow.Domain.Models.DTOs
{
    public class LoginResponse
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
