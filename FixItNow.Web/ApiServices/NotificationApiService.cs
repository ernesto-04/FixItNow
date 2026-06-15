using FixItNow.Domain.Models.BookingRequest.DTOs;

namespace FixItNow.Web.ApiServices
{
    public class NotificationApiService
    {
        private readonly AuthenticatedHttpService _authService;

        public NotificationApiService(AuthenticatedHttpService authService)
        {
            _authService = authService;
        }

        public async Task<List<NotificationDto>> GetUnreadAsync()
        {
            var response = await _authService.GetAsync("api/notifications/unread");
            if (!response.IsSuccessStatusCode) return new();
            return await response.Content.ReadFromJsonAsync<List<NotificationDto>>() ?? new();
        }

        public async Task MarkAllReadAsync() =>
            await _authService.PatchAsync<object>("api/notifications/mark-all-read", new());
    }
}
