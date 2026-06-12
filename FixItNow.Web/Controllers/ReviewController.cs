using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs.Reviews;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/reviews")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateReviewRequest request,
        [FromServices] IValidator<CreateReviewRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var (success, error) = await _reviewService.CreateReviewAsync(GetUserId(), request);
        return success ? Ok() : BadRequest(new { message = error });
    }

    [HttpGet("{technicianUserId:int}")]
    public async Task<IActionResult> GetByTechnician(int technicianUserId)
    {
        return Ok(await _reviewService.GetReviewsByTechnicianAsync(technicianUserId));
    }

    [HttpGet("has-reviewed/{ticketId:int}")]
    [Authorize]
    public async Task<IActionResult> HasReviewed(int ticketId)
    {
        return Ok(await _reviewService.HasReviewedAsync(ticketId));
    }

    private int GetUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException();
}