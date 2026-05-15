using FixItNow.Domain.Models.DTOs;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Web.Models;

namespace FixItNow.Web.Components.API_Services
{
    public class TicketApiService
    {
        private readonly AuthenticatedHttpService _authService;

        public TicketApiService(
            AuthenticatedHttpService authService)
        {
            _authService = authService;
        }

        public async Task<List<AvailableTicketResponse>> GetAvailableTicketsAsync()
        {
            var client = await _authService.GetClientAsync();

            if (client is null)
                return [];

            var response = await client.GetAsync(
                "api/tickets/available-tickets");

            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content
                .ReadFromJsonAsync<List<AvailableTicketResponse>>()
                ?? [];
        }
        public async Task<CreateTicketResponse?> CreateTicketAsync(
            CreateTicketRequest ticket,
            List<UploadedImage> uploadedImages)
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return null;

            using var content = new MultipartFormDataContent();

            content.Add(
                new StringContent(ticket.Title ?? string.Empty),
                nameof(ticket.Title));

            content.Add(
                new StringContent(ticket.Description ?? string.Empty),
                nameof(ticket.Description));

            content.Add(
                new StringContent(ticket.Category ?? string.Empty),
                nameof(ticket.Category));

            content.Add(
                new StringContent(ticket.Location ?? string.Empty),
                nameof(ticket.Location));

            foreach (var image in uploadedImages)
            {
                var imageContent =
                    new ByteArrayContent(image.Data);

                content.Add(
                    imageContent,
                    "Images",
                    image.Name);
            }

            var response = await client.PostAsync(
                "api/tickets/create-ticket",
                content);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CreateTicketResponse>();
        }
        public async Task<List<CustomerTicketResponse>> GetCustomerTicketsAsync()
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return [];

            var response = await client.GetAsync(
                "api/tickets/customer-tickets");

            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content
                .ReadFromJsonAsync<List<CustomerTicketResponse>>()
                ?? [];
        }
        public async Task<CustomerTicketResponse?> GetCustomerTicketAsync(int ticketId)
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return null;

            var response =
                await client.GetAsync($"api/tickets/{ticketId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CustomerTicketResponse>();
        }
        public async Task<bool> UpdateTicketStatusAsync(
            int ticketId,
            TicketStatus status)
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return false;

            var response = await client.PutAsync(
                $"api/tickets/{ticketId}/status?status={status}",
                null);

            return response.IsSuccessStatusCode;
        }
        public async Task<List<TechnicianTicketResponse>> GetTechnicianTicketsAsync()
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return [];

            var response = await client.GetAsync(
                "api/tickets/technician-tickets");

            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content
                .ReadFromJsonAsync<List<TechnicianTicketResponse>>()
                ?? [];
        }
        public async Task<AvailableTicketResponse?> GetTechnicianTicketByIdAsync(int ticketId)
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return null;

            var response = await client.GetAsync(
                $"api/tickets/{ticketId}/technician");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<AvailableTicketResponse>();
        }
        public async Task<bool> AcceptTicketAsync(int ticketId)
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return false;

            var response = await client.PostAsync(
                $"api/tickets/{ticketId}/accept",
                null);

            return response.IsSuccessStatusCode;
        }
        public async Task<TechnicianTicketResponse?>
    GetTechnicianTicketDetailAsync(int ticketId)
        {
            var client =
                await _authService.GetClientAsync();

            if (client is null)
                return null;

            var response = await client.GetAsync(
                $"api/tickets/{ticketId}/technician-detail");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<TechnicianTicketResponse>();
        }
    }
}
