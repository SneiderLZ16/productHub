namespace productHub.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string Role { get; init; } = default!; // "Admin" | "Seller" | "Buyer"
}