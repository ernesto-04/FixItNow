using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FixItNow.Web.Services;

public class CustomAuthStateProvider(ProtectedLocalStorage localStorage)
    : AuthenticationStateProvider
{
    private const string TokenKey = "token";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await localStorage.GetAsync<string>(TokenKey);
            if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
                return AnonymousState();

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();
            var jwtToken = handler.ReadJwtToken(result.Value);

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await localStorage.DeleteAsync(TokenKey);
                return AnonymousState();
            }

            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(jwtToken.Claims, "GlobalLoginAuth")));
        }
        catch
        {
            return AnonymousState();
        }
    }

    public async Task StartSession(string token)
    {
        await localStorage.SetAsync(TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task EndSession()
    {
        await localStorage.DeleteAsync(TokenKey);
        await localStorage.DeleteAsync("mode");
        NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState()));
    }

    private static AuthenticationState AnonymousState() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}