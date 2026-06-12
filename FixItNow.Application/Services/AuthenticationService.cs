using System.Net;
using System.Net.Http.Json;
using FixItNow.Domain.Models.Authentications;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace FixItNow.Application.Services;

public interface IAuthenticationService
{
    Task<AuthResponse> LoginAsync(AuthRequest model);
    Task RegisterAsync(RegisterRequest model);
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

    public async Task<AuthResponse> LoginAsync(AuthRequest model)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", model);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Failed login attempt for {Email}", model.Email);
            throw new InvalidOperationException("Invalid email or password.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Login failed. Status: {Status}, Error: {Error}",
                response.StatusCode, error);
            throw new InvalidOperationException("Login failed.");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Authentication failed.");

        await _localStorage.SetAsync("token", result.Token);
        await _localStorage.SetAsync("mode", "customer");

        _logger.LogInformation("User logged in: {Email}", model.Email);
        return result;
    }

    public async Task RegisterAsync(RegisterRequest model)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", model);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Registration failed for {Email}. Error: {Error}",
                model.Email, error);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(error) ? "Registration failed." : error);
        }

        _logger.LogInformation("User registered: {Email}", model.Email);
    }
}