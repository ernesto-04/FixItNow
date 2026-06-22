using System.Security.Claims;
using FixItNow.Application.Services;
using FixItNow.Domain.Models;
using FixItNow.Domain.Models.BookingRequest.DTOs;
using FixItNow.Infrastructure;
using FixItNow.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FixItNow.Web.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly FixItNowDataContext _context;

    public AdminController(
        IAdminService adminService,
        IHubContext<ChatHub> hubContext,
        FixItNowDataContext context)
    {
        _adminService = adminService;
        _hubContext = hubContext;
        _context = context;
    }

    [HttpPatch("technicians/{userId}/approve")]
    public async Task<IActionResult> ApproveTechnician(int userId)
    {
        if (!IsAdmin) return Forbid();
        var (success, error) = await _adminService.ApproveTechnicianAsync(userId);
        if (!success) return BadRequest(error);

        await SendAndSaveNotificationAsync(
            userId,
            "Your technician application has been approved! You can now accept bookings.",
            "approval");

        return Ok();
    }

    [HttpPatch("technicians/{userId}/reject")]
    public async Task<IActionResult> RejectTechnician(int userId, [FromBody] RejectTechnicianRequest request)
    {
        if (!IsAdmin) return Forbid();
        var (success, error) = await _adminService.RejectTechnicianAsync(userId, request.Reason);
        if (!success) return BadRequest(error);

        await SendAndSaveNotificationAsync(
            userId,
            $"Your technician application was rejected: {request.Reason}",
            "rejection");

        return Ok();
    }

    private async Task SendAndSaveNotificationAsync(int userId, string message, string type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", new NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                IsRead = false
            });
    }

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

    private bool IsAdmin => User.FindFirstValue("IsAdmin") == "True";
}

public record RejectTechnicianRequest(string Reason);
