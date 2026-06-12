using FixItNow.Application.Services;
using FixItNow.Application.Validators;
using FluentValidation;

namespace FixItNow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITechnicianService, TechnicianService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<AuthenticatedHttpService>();

        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        return services;
    }
}