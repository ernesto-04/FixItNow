using System.Net.Http.Json;
using FixItNow.Domain.Models.Authentications;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FixItNow.Application
{
    public interface IAuthenticationService
    {
        Task<AuthResponse> Login(AuthRequest model);
        Task Register(RegisterRequest model);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _http;
        private readonly ProtectedLocalStorage _localStorage;

        public AuthenticationService(HttpClient http, ProtectedLocalStorage localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<AuthResponse> Login(AuthRequest model)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7008/api/auth/login", model);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            return result;
        }

        public async Task Register(RegisterRequest model)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7008/api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

    }
}
