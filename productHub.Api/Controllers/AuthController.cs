using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using productHub.Application.DTOs.Auth;
using productHub.Domain.Interfaces;

namespace productHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration config, IUserRepository userRepository)
    {
        _config = config;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Crea un usuario y devuelve un token JWT
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var exists = await _userRepository.GetByEmailAsync(request.Email);
        if (exists != null)
            return Conflict(new { message = "El email ya está registrado." });

        var newUser = new productHub.Domain.Entities.User(
            request.Username,
            request.Email,
            request.Password, // sin hash porque así está el modelo actual
            request.Role
        );

        await _userRepository.AddAsync(newUser);

        var token = GenerateJwt(newUser);

        return Ok(new
        {
            user = newUser.Username,
            email = newUser.Email,
            role = newUser.Role,
            token
        });
    }

    /// <summary>
    /// Valida usuario y devuelve un token JWT
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || user.Password != request.Password)
            return Unauthorized(new { message = "Credenciales inválidas." });

        var token = GenerateJwt(user);

        return Ok(new
        {
            user = user.Username,
            email = user.Email,
            role = user.Role,
            token
        });
    }

    // ----------------- Helper para crear el JWT -----------------
    private string GenerateJwt(productHub.Domain.Entities.User user)
    {
        var key = _config["Jwt:Key"]!;
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.UtcNow.AddMinutes(120),
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
