using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using productHub.Application.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace productHub.Infrastructure.Security;

public class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _config;

    public JwtProvider(IConfiguration config)
    {
        _config = config;
    }

    public (string token, DateTime expiresAtUtc) GenerateToken(Guid userId, string username, string email, string role)
    {
        var issuer       = _config["Jwt:Issuer"];
        var audience     = _config["Jwt:Audience"];
        var key          = _config["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");
        var expiresMins  = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 120;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("username", username),
            new(ClaimTypes.Role, role),
        };

        var signingKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires     = DateTime.UtcNow.AddMinutes(expiresMins);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expires);
    }
}