using FixItNow.Application.Services;
using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.Authentications;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Web
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FixItNowDataContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(FixItNowDataContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task Register(RegisterRequest request)
        {
            // check username uniqueness
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (existingUser != null)
                throw new Exception("Username already taken");

            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        [HttpPost("login")]
        public IActionResult Login(AuthRequest dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized();
            }
            var token = _jwtService.GenerateToken(user);
            return Ok(new { Email = user.Email, Token = token });
        }
    }
}
