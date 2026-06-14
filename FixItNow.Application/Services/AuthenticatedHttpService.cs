using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class AuthenticatedHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;

    public AuthenticatedHttpService(HttpClient httpClient, ProtectedLocalStorage localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task<bool> AttachTokenAsync()
    {
        var tokenResult = await _localStorage.GetAsync<string>("token");
        if (!tokenResult.Success || string.IsNullOrWhiteSpace(tokenResult.Value))
            return false;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        return true;
    }

    public async Task<HttpClient?> GetClientAsync()
    {
        var attached = await AttachTokenAsync();
        return attached ? _httpClient : null;
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        await AttachTokenAsync();
        return await _httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
    {
        await AttachTokenAsync();
        return await _httpClient.PostAsJsonAsync(url, body);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T body)
    {
        await AttachTokenAsync();
        return await _httpClient.PutAsJsonAsync(url, body);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        await AttachTokenAsync();
        return await _httpClient.DeleteAsync(url);
    }

    public async Task<HttpResponseMessage> PatchAsync<T>(string url, T body)
    {
        await AttachTokenAsync();
        return await _httpClient.PatchAsJsonAsync(url, body);
    }
}