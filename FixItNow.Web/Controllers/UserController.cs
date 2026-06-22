using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("become-technician")]
    public async Task<IActionResult> BecomeTechnicianAsync(
        CreateTechnicianProfileRequest request,
        [FromServices] IValidator<CreateTechnicianProfileRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        await _userService.BecomeTechnicianAsync(GetUserId(), request);
        return Ok();
    }

    [HttpGet("is-technician")]
    public async Task<IActionResult> IsTechnicianAsync()
    {
        return Ok(await _userService.IsTechnicianAsync(GetUserId()));
    }

    [HttpGet("technician-status")]
    public async Task<IActionResult> GetTechnicianStatusAsync()
    {
        var profile = await _userService.GetTechnicianStatusAsync(GetUserId());
        if (profile is null) return NoContent();
        return Ok(profile);
    }

    private int GetUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException();
}