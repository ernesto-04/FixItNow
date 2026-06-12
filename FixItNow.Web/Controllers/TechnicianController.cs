using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FixItNow.Application.Services;
using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web.Controllers;

[Route("api/technicians")]
[ApiController]
public class TechnicianController : ControllerBase
{
    private readonly ITechnicianService _technicianService;
    private readonly IWebHostEnvironment _env;
    private readonly string _baseUrl;

    private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public TechnicianController(
        ITechnicianService technicianService,
        IWebHostEnvironment env,
        IConfiguration config)
    {
        _technicianService = technicianService;
        _env = env;
        _baseUrl = config["AppSettings:BaseUrl"] ?? string.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var currentUserId = int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        return Ok(await _technicianService.GetAllTechniciansAsync(currentUserId));
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetById(int userId)
    {
        var profile = await _technicianService.GetTechnicianByIdAsync(userId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateTechnicianProfileDto dto,
        [FromServices] IValidator<UpdateTechnicianProfileDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var updated = await _technicianService.UpdateProfileAsync(GetUserId(), dto);
        return updated is null ? NotFound() : Ok(updated);
    }


    [HttpPost("upload-profile-image")]
    [Authorize]
    public async Task<IActionResult> UploadProfileImage(
        IFormFile file,
        BlobServiceClient blobServiceClient,
        IConfiguration config)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Only jpg, png and webp are allowed." });

        var containerName = config["AzureStorage:ContainerName"]!;
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var blobClient = containerClient.GetBlobClient(fileName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        });

        return Ok(new { imageUrl = blobClient.Uri.ToString() });
    }
    private int GetUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException();
}