using System.Net;
using System.Net.Http.Json;
using FixItNow.Domain.Models.Authentications;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            HttpClient http,
            ProtectedLocalStorage localStorage,
            ILogger<AuthenticationService> logger)
        {
            _http = http;
            _localStorage = localStorage;
            _logger = logger;
        }

        public async Task<AuthResponse> Login(AuthRequest model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Password))
                {
                    throw new InvalidOperationException(
                        "Email or Password is empty.");
                }

                var response = await _http.PostAsJsonAsync(
                    "/api/auth/login",
                    model);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning(
                        "Failed login attempt for {Email}",
                        model.Email);

                    throw new InvalidOperationException(
                        "Email or Password is invalid.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    _logger.LogError(
                        "Login API failed. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode,
                        error);

                    throw new InvalidOperationException(
                        string.IsNullOrWhiteSpace(error)
                            ? "Login failed."
                            : error);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<AuthResponse>();

                if (result == null)
                {
                    _logger.LogError(
                        "Login returned null response for {Email}",
                        model.Email);

                    throw new InvalidOperationException(
                        "Authentication failed.");
                }

                await _localStorage.SetAsync(
                    "authToken",
                    result.Token);

                await _localStorage.SetAsync(
                    "mode",
                    "customer");

                _logger.LogInformation(
                    "User logged in successfully: {Email}",
                    model.Email);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred during login for {Email}",
                    model.Email);

                throw;
            }
        }

        public async Task Register(RegisterRequest model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Password))
                {
                    throw new InvalidOperationException(
                        "Email or Password is empty.");
                }

                var response = await _http.PostAsJsonAsync(
                    "/api/auth/register",
                    model);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "Registration failed for {Email}. Error: {Error}",
                        model.Email,
                        error);

                    throw new InvalidOperationException(
                        string.IsNullOrWhiteSpace(error)
                            ? "Registration failed."
                            : error);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    _logger.LogError(
                        "Registration API failed. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode,
                        error);

                    throw new InvalidOperationException(
                        "Something went wrong while registering.");
                }

                _logger.LogInformation(
                    "User registered successfully: {Email}",
                    model.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred during registration for {Email}",
                    model.Email);

                throw;
            }
        }
    }
}