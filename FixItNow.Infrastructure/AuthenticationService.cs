using System.Net.Http.Json;
using FixItNow.Domain.Models.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FixItNow.Infrastructure
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginDto model);
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

        public async Task<LoginResponse> Login(LoginDto model)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7008/api/auth/login", model);
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result;
        }

    }
}
