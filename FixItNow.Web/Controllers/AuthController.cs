using FixItNow.Application.Services;
using FixItNow.Domain.Models.Authentications;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService _authService;

    public AuthController(IAccountService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        [FromServices] IValidator<RegisterRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var (success, error) = await _authService.RegisterAsync(request);
        if (!success)
            return BadRequest(new { message = error });

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        AuthRequest request,
        [FromServices] IValidator<AuthRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var response = await _authService.LoginAsync(request);
        if (response is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(response);
    }
}