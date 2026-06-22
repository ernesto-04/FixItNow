using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using static System.Net.WebRequestMethods;

public class UserApiService
{
    private readonly AuthenticatedHttpService _httpService;

    public UserApiService(AuthenticatedHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<bool> IsTechnicianAsync()
    {
        var response = await _httpService.GetAsync("/api/users/is-technician");
        if (!response.IsSuccessStatusCode) return false;
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task BecomeTechnicianAsync(int userId, CreateTechnicianProfileRequest request)
    {
        await _httpService.PostAsync($"/api/users/become-technician", request);
    }

    public async Task<TechnicianProfileDto?> GetTechnicianStatusAsync()
    {
        var response = await _httpService.GetAsync("api/users/technician-status");
        if (!response.IsSuccessStatusCode) return null;
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
        return await response.Content.ReadFromJsonAsync<TechnicianProfileDto>();
    }
}