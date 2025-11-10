using productHub.Application.DTOs.Auth;
using productHub.Application.Interfaces;
using productHub.Application.Security;
using productHub.Domain.Entities;
using productHub.Domain.Interfaces;

namespace productHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered.");

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var user = new User(
            username: request.Username,
            email: request.Email,
            password: hashedPassword,
            role: request.Role
        );

        await _userRepository.AddAsync(user);

        var (token, expires) = _jwtProvider.GenerateToken(user.Id, user.Username, user.Email, user.Role);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expires,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new InvalidOperationException("Invalid credentials.");

        var passwordValid = _passwordHasher.Verify(request.Password, user.Password);
        if (!passwordValid)
            throw new InvalidOperationException("Invalid credentials.");

        var (token, expires) = _jwtProvider.GenerateToken(user.Id, user.Username, user.Email, user.Role);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expires,
            Username = user.Username,
            Role = user.Role
        };
    }
}
