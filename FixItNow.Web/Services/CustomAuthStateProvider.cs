using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FixItNow.Web.Services
{
    public class CustomAuthStateProvider(ProtectedLocalStorage protectedLocalStorage,
    IOptions<CommonOptions> commonOptions,
    IOptions<TokenOptions> tokenOptions,
    IDistributedCache cache)
    : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage = protectedLocalStorage;
        private readonly CommonOptions _commonOptions = commonOptions.Value;
        private readonly TokenOptions _tokenOptions = tokenOptions.Value;
        private readonly IDistributedCache _cache = cache;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var cacheKey = await _protectedLocalStorage.GetAsync<string>(_commonOptions.LocalStorageKey);
                if (!cacheKey.Success)
                {
                    return await ClearSession();
                }

                var jwtToken = new JwtSecurityToken(cacheKey.Value);
                if (jwtToken.ValidTo < DateTime.Now.ToUniversalTime())
                {
                    return await ClearSession();
                }

                // forward JWT Claims to AuthenticationState
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "GlobalLoginAuth"));
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task StartSession(string token)
        {
            await _protectedLocalStorage.SetAsync(_commonOptions.LocalStorageKey, token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task EndSession()
        {
            var state = await ClearSession();

            await _protectedLocalStorage.DeleteAsync("mode");

            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        private async Task<AuthenticationState> ClearSession()
        {
            await _protectedLocalStorage.DeleteAsync(_commonOptions.LocalStorageKey);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}

