using FixItNow.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FixItNow.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    private bool IsAdmin => User.FindFirstValue("IsAdmin") == "True";

    [HttpGet("technicians/pending")]
    public async Task<IActionResult> GetPendingTechnicians()
    {
        if (!IsAdmin) return Forbid();
        var result = await _adminService.GetPendingTechniciansAsync();
        return Ok(result);
    }

    [HttpGet("technicians/rejected")]
    public async Task<IActionResult> GetRejectedTechnicians()
    {
        if (!IsAdmin) return Forbid();
        var result = await _adminService.GetRejectedTechniciansAsync();
        return Ok(result);
    }

    [HttpPatch("technicians/{userId}/approve")]
    public async Task<IActionResult> ApproveTechnician(int userId)
    {
        if (!IsAdmin) return Forbid();
        var (success, error) = await _adminService.ApproveTechnicianAsync(userId);
        if (!success) return BadRequest(new { error });
        return Ok();
    }

    [HttpPatch("technicians/{userId}/reject")]
    public async Task<IActionResult> RejectTechnician(int userId, [FromBody] RejectRequest request)
    {
        if (!IsAdmin) return Forbid();
        var (success, error) = await _adminService.RejectTechnicianAsync(userId, request.Reason);
        if (!success) return BadRequest(new { error });
        return Ok();
    }
}

public record RejectRequest(string Reason);