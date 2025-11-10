namespace productHub.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; init; } = default!;
    public DateTime ExpiresAtUtc { get; init; }
    public string Username { get; init; } = default!;
    public string Role { get; init; } = default!;
}