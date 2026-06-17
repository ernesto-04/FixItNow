using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;

namespace FixItNow.Web.ApiServices;

public class AdminApiService
{
    private readonly AuthenticatedHttpService _http;

    public AdminApiService(AuthenticatedHttpService http)
    {
        _http = http;
    }

    public async Task<List<TechnicianProfileDto>> GetPendingTechniciansAsync()
    {
        var response = await _http.GetAsync("api/admin/technicians/pending");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<TechnicianProfileDto>>() ?? [];
    }

    public async Task<List<TechnicianProfileDto>> GetRejectedTechniciansAsync()
    {
        var response = await _http.GetAsync("api/admin/technicians/rejected");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<TechnicianProfileDto>>() ?? [];
    }

    public async Task<bool> ApproveTechnicianAsync(int userId)
    {
        var response = await _http.PatchAsync($"api/admin/technicians/{userId}/approve", new { });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RejectTechnicianAsync(int userId, string reason)
    {
        var response = await _http.PatchAsync($"api/admin/technicians/{userId}/reject", new { Reason = reason });
        return response.IsSuccessStatusCode;
    }
}