using productHub.Application.Security;

namespace productHub.Infrastructure.Security;

// BCrypt para hash seguro
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainText)
        => BCrypt.Net.BCrypt.HashPassword(plainText);

    public bool Verify(string plainText, string hashed)
        => BCrypt.Net.BCrypt.Verify(plainText, hashed);
}