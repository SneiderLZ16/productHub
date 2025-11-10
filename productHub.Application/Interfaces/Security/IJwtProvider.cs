namespace productHub.Application.Security;

public interface IJwtProvider
{
    (string token, DateTime expiresAtUtc) GenerateToken(Guid userId, string username, string email, string role);
}