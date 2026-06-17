using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FixItNow.Domain.Models.Accesses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FixItNow.Application.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

public class JwtService : IJwtService
{
    private readonly SymmetricSecurityKey _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        var jwt = configuration.GetSection("JwtSettings");
        _secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
        _issuer = jwt["Issuer"]!;
        _audience = jwt["Audience"]!;
    }

    public string GenerateToken(User user)
    {
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim("IsAdmin", user.IsAdmin.ToString())
            ],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}