using System.Net;
using System.Net.Http.Json;
using FixItNow.Domain.Models.Authentications;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FixItNow.Application.Services
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

        public AuthenticationService(
            HttpClient http,
            ProtectedLocalStorage localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<AuthResponse> Login(AuthRequest model)
        {
            // Validate empty fields
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                throw new InvalidOperationException(
                    "Email or Password is empty.");
            }

            var response = await _http.PostAsJsonAsync(
                "https://localhost:7008/api/auth/login",
                model);

            // Handle unauthorized login
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException(
                    "Email or Password is invalid.");
            }

            // Handle other API errors
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Login failed."
                        : error);
            }

            var result = await response.Content
                .ReadFromJsonAsync<AuthResponse>();

            // Handle null response
            if (result == null)
            {
                throw new InvalidOperationException(
                    "Authentication failed.");
            }

            // Optional: save token to local storage
            await _localStorage.SetAsync("authToken", result.Token);

            return result;
        }

        public async Task Register(RegisterRequest model)
        {
            // Validate empty fields
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                throw new InvalidOperationException(
                    "Email or Password is empty.");
            }

            var response = await _http.PostAsJsonAsync(
                "https://localhost:7008/api/auth/register",
                model);

            // Handle duplicate email/username
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Registration failed."
                        : error);
            }

            // Handle other errors
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    "Something went wrong while registering.");
            }
        }
    }
}