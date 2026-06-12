using System.Text.Json;
using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FixItNow.Domain.Models.Bookings;

namespace FixItNow.Web.ApiServices;

public class BookingApiService
{
    private readonly AuthenticatedHttpService _authService;

    public BookingApiService(AuthenticatedHttpService authService)
    {
        _authService = authService;
    }

    public async Task<(bool Success, string Error)> CreateBookingAsync(CreateBookingRequestDto dto)
    {
        var response = await _authService.PostAsync("api/bookings", dto);
        if (response.IsSuccessStatusCode)
            return (true, string.Empty);

        var raw = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[CreateBooking] Status: {response.StatusCode}, Body: {raw}");

        if (string.IsNullOrWhiteSpace(raw))
            return (false, $"Request failed ({response.StatusCode}) with empty body.");

        try
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return (false, error?.Message ?? raw);
        }
        catch
        {
            return (false, raw);
        }
    }

    public async Task<List<BookingRequestDto>> GetCustomerBookingsAsync()
    {
        var response = await _authService.GetAsync("api/bookings/my-bookings");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<BookingRequestDto>>() ?? [];
    }

    public async Task<List<BookingRequestDto>> GetTechnicianBookingsAsync()
    {
        var response = await _authService.GetAsync("api/bookings/incoming");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<BookingRequestDto>>() ?? [];
    }

    public async Task<bool> AcceptBookingAsync(int bookingId)
    {
        var response = await _authService.PostAsync<object>($"api/bookings/{bookingId}/accept", null!);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeclineBookingAsync(int bookingId)
    {
        var response = await _authService.PostAsync<object>($"api/bookings/{bookingId}/decline", null!);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> GetPendingBookingsCountAsync()
    {
        var bookings = await GetTechnicianBookingsAsync();
        return bookings.Count(b => b.Status == BookingStatus.Pending);
    }

    public async Task<(bool Success, string Error)> CancelBookingAsync(int bookingId, string reason)
    {
        var response = await _authService.PostAsync($"api/bookings/{bookingId}/cancel", new CancelBookingRequestDto { Reason = reason });
        if (response.IsSuccessStatusCode)
            return (true, string.Empty);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        return (false, error?.Message ?? "Something went wrong.");
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}