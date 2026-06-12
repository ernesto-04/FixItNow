using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.Authentications;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public interface IAccountService
{
    Task<(bool Success, string Error)> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(AuthRequest request);
}

public class AccountService : IAccountService
{
    private readonly FixItNowDataContext _context;
    private readonly IJwtService _jwtService;

    public AccountService(FixItNowDataContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<(bool Success, string Error)> RegisterAsync(RegisterRequest request)
    {
        var exists = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (exists)
            return (false, "Username already taken.");

        await _context.Users.AddAsync(new User
        {
            FullName = request.FullName,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        });

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<AuthResponse?> LoginAsync(AuthRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new AuthResponse
        {
            Email = user.Email,
            Token = _jwtService.GenerateToken(user)
        };
    }
}