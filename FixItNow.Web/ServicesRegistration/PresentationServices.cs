using FixItNow.Application.Services;
using FixItNow.Web.Components.API_Services;
using FixItNow.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor.Services;

namespace FixItNow.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ProtectedLocalStorage>();
            services.AddScoped<AppState>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<TicketApiService>();
            services.AddScoped(sp =>
            new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7008/")
            });

            services.AddMudServices();


            return services;
        }
    }
}
