using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs;
using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FixItNow.Web.Hubs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FixItNow.Web.Controllers;

[Route("api/bookings")]
[ApiController]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IHubContext<ChatHub> _hubContext;

    public BookingController(IBookingService bookingService, IHubContext<ChatHub> hubContext)
    {
        _bookingService = bookingService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingRequestDto dto,
        [FromServices] IValidator<CreateBookingRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var (success, error, notification) = await _bookingService.CreateBookingAsync(GetUserId(), dto);
        if (!success) return BadRequest(new { message = error });
        await PushNotification(notification);
        return Ok();
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
        var (success, error, notification) = await _bookingService.AcceptBookingAsync(bookingId, GetUserId());
        if (!success) return BadRequest(new { message = error });
        await PushNotification(notification);
        return Ok();
    }

    [HttpPost("{bookingId:int}/decline")]
    public async Task<IActionResult> Decline(int bookingId)
    {
        var (success, error, notification) = await _bookingService.DeclineBookingAsync(bookingId, GetUserId());
        if (!success) return BadRequest(new { message = error });
        await PushNotification(notification);
        return Ok();
    }

    [HttpPost("{bookingId:int}/cancel")]
    public async Task<IActionResult> Cancel(int bookingId, [FromBody] CancelBookingRequestDto dto)
    {
        var (success, error, notification) = await _bookingService.CancelBookingAsync(bookingId, GetUserId(), dto.Reason);
        if (!success) return BadRequest(new { message = error });
        await PushNotification(notification);
        return Ok();
    }

    private async Task PushNotification(NotificationDto? notification)
    {
        if (notification is null) return;
        await _hubContext.Clients
            .User(notification.UserId.ToString())
            .SendAsync("ReceiveNotification", notification);
    }

    private int GetUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException();
}