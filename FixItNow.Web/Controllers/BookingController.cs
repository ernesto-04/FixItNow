using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/bookings")]
[ApiController]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingRequestDto dto,
        [FromServices] IValidator<CreateBookingRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var (success, error) = await _bookingService.CreateBookingAsync(GetUserId(), dto);
        return success ? Ok() : BadRequest(new { message = error });
    }

    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetCustomerBookings()
    {
        return Ok(await _bookingService.GetCustomerBookingsAsync(GetUserId()));
    }

    [HttpGet("incoming")]
    public async Task<IActionResult> GetTechnicianBookings()
    {
        return Ok(await _bookingService.GetTechnicianBookingsAsync(GetUserId()));
    }

    [HttpPost("{bookingId:int}/accept")]
    public async Task<IActionResult> Accept(int bookingId)
    {
        var (success, error) = await _bookingService.AcceptBookingAsync(bookingId, GetUserId());
        return success ? Ok() : BadRequest(new { message = error });
    }

    [HttpPost("{bookingId:int}/decline")]
    public async Task<IActionResult> Decline(int bookingId)
    {
        var (success, error) = await _bookingService.DeclineBookingAsync(bookingId, GetUserId());
        return success ? Ok() : BadRequest(new { message = error });
    }

    [HttpPost("{bookingId:int}/cancel")]
    public async Task<IActionResult> Cancel(int bookingId, [FromBody] CancelBookingRequestDto dto)
    {
        var (success, error) = await _bookingService.CancelBookingAsync(bookingId, GetUserId(), dto.Reason);
        return success ? Ok() : BadRequest(new { message = error });
    }

    private int GetUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException();
}