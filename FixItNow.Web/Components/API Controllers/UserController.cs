using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.DTOs;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FixItNowDataContext _context;
        public UserController(UserService userService, FixItNowDataContext context)
        {
            _userService = userService;
            _context = context;
        }
        [HttpPost("become-technician")]
        public IActionResult BecomeTechnician(CreateTechnicianProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _userService.BecomeTechnician(userId, request);

            return Ok(new { message = "You are now a technician" });
        }
        [HttpGet("is-technician")]
        public IActionResult IsTechnician()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isTechnician = _context.TechnicianProfiles
                .Any(tp => tp.UserId == userId);

            return Ok(isTechnician);
        }
    }
}
