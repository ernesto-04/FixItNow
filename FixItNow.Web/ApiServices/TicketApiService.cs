using FixItNow.Domain.Models.BookingRequest.DTOs.Bookings;
using FixItNow.Domain.Models.BookingRequest.DTOs.Tickets;
using FixItNow.Domain.Models.Tickets;
namespace FixItNow.Web.ApiServices
{
    public class TicketApiService
    {
        private readonly AuthenticatedHttpService _authService;

        public TicketApiService(AuthenticatedHttpService authService)
        {
            _authService = authService;
        }

        public async Task<List<CustomerTicketResponse>> GetCustomerTicketsAsync()
        {
            var response = await _authService.GetAsync("api/tickets/customer-tickets");
            if (!response.IsSuccessStatusCode) return [];
            return await response.Content.ReadFromJsonAsync<List<CustomerTicketResponse>>() ?? [];
        }

        public async Task<CustomerTicketResponse?> GetCustomerTicketAsync(int ticketId)
        {
            var response = await _authService.GetAsync($"api/tickets/{ticketId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CustomerTicketResponse>();
        }

        public async Task<List<TechnicianTicketResponse>> GetTechnicianTicketsAsync()
        {
            var response = await _authService.GetAsync("api/tickets/technician-tickets");
            if (!response.IsSuccessStatusCode) return [];
            return await response.Content.ReadFromJsonAsync<List<TechnicianTicketResponse>>() ?? [];
        }

        public async Task<TechnicianTicketResponse?> GetTechnicianTicketDetailAsync(int ticketId)
        {
            var response = await _authService.GetAsync($"api/tickets/{ticketId}/technician-detail");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TechnicianTicketResponse>();
        }

        public async Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status)
        {
            var response = await _authService.PutAsync<object>(
                $"api/tickets/{ticketId}/status?status={status}", null!);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AcceptTicketAsync(int ticketId)
        {
            var response = await _authService.PostAsync<object>($"api/tickets/{ticketId}/accept", null!);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetActiveTicketsCountAsync()
        {
            var tickets = await GetTechnicianTicketsAsync();
            return tickets.Count(t => t.Status is TicketStatus.Assigned or TicketStatus.InProgress);
        }

        public async Task<bool> CancelTicketAsync(int ticketId, string reason)
        {
            var response = await _authService.PostAsync($"api/tickets/{ticketId}/cancel", new CancelBookingRequestDto { Reason = reason });
            return response.IsSuccessStatusCode;
        }
    }
}
