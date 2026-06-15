using Azure.Storage.Blobs;
using FixItNow.Web.ApiServices;
using FixItNow.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

namespace FixItNow.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<AppState>();
        services.AddScoped(sp =>
            new HttpClient
            {
                BaseAddress = new Uri(
                    sp.GetRequiredService<NavigationManager>().BaseUri)
            });
        services.AddMudServices();
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new BlobServiceClient(config["AzureStorage:ConnectionString"]);
        });

        // Api Services
        services.AddScoped<TicketApiService>();
        services.AddScoped<TechnicianApiService>();
        services.AddScoped<UserApiService>();
        services.AddScoped<ReviewApiService>();
        services.AddScoped<BookingApiService>();
        services.AddScoped<NotificationApiService>();

        return services;
    }
}