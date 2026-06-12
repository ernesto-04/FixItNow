using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using Microsoft.AspNetCore.Components.Forms;

namespace FixItNow.Web.ApiServices;

public class TechnicianApiService
{
    private readonly AuthenticatedHttpService _authService;
    private readonly HttpClient _httpClient;

    public TechnicianApiService(AuthenticatedHttpService authService, HttpClient httpClient)
    {
        _authService = authService;
        _httpClient = httpClient;
    }

    // Public endpoints — no auth needed
    public async Task<List<TechnicianProfileDto>> GetAllTechniciansAsync()
    {
        var response = await _httpClient.GetAsync("api/technicians");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<TechnicianProfileDto>>() ?? [];
    }

    public async Task<TechnicianProfileDto?> GetTechnicianByIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"api/technicians/{userId}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TechnicianProfileDto>();
    }

    // Authenticated endpoints
    public async Task<TechnicianProfileDto?> UpdateProfileAsync(UpdateTechnicianProfileDto dto)
    {
        var response = await _authService.PutAsync("api/technicians/profile", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TechnicianProfileDto>();
    }

    public async Task<string?> UploadProfileImageAsync(IBrowserFile file)
    {
        var client = await _authService.GetClientAsync();
        if (client is null) return null;

        using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await client.PostAsync("api/technicians/upload-profile-image", content);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<UploadImageResponse>();
        return result?.ImageUrl;
    }
}

public class UploadImageResponse
{
    public string ImageUrl { get; set; } = string.Empty;
}