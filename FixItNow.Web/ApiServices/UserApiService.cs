using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;

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
}