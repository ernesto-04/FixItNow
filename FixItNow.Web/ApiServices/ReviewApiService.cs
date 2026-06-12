using FixItNow.Domain.Models.BookingRequest.DTOs.Reviews;

namespace FixItNow.Web.ApiServices;

public class ReviewApiService
{
    private readonly AuthenticatedHttpService _authService;

    public ReviewApiService(AuthenticatedHttpService authService)
    {
        _authService = authService;
    }

    public async Task<bool> CreateReviewAsync(CreateReviewRequest request)
    {
        var response = await _authService.PostAsync("api/reviews", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ReviewDto>> GetReviewsByTechnicianAsync(int technicianUserId)
    {
        var response = await _authService.GetAsync($"api/reviews/{technicianUserId}");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<ReviewDto>>() ?? [];
    }

    public async Task<bool> HasReviewedAsync(int ticketId)
    {
        var response = await _authService.GetAsync($"api/reviews/has-reviewed/{ticketId}");
        if (!response.IsSuccessStatusCode) return false;
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}