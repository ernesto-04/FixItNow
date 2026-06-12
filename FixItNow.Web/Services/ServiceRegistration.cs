using Newtonsoft.Json;

namespace FixItNow.Web.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        services.AddAuthentication();
        services.AddAuthorization();
        services.AddDistributedMemoryCache();
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        return services;
    }
}