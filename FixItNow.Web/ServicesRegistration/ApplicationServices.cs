using FixItNow.Application.Services;

namespace FixItNow.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<TicketService>();
            services.AddScoped<UserService>();
            services.AddScoped<JwtService>();
            return services;
        }
    }
}