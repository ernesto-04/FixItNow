using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class AuthenticatedHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;

    public AuthenticatedHttpService(
        HttpClient httpClient,
        ProtectedLocalStorage localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<HttpClient?> GetClientAsync()
    {
        var tokenResult = await _localStorage.GetAsync<string>("token");

        if (!tokenResult.Success ||
            string.IsNullOrWhiteSpace(tokenResult.Value))
        {
            return null;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResult.Value);

        return _httpClient;
    }
}