using FixItNow.Domain.Models.Accesses;
using FixItNow.Infrastructure.Models.Commons;
using FixItNow.Infrastructure.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FixItNowDataContext _context;

        public AuthController(FixItNowDataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }
    }
}
