using Xunit;
using Moq;
using productHub.Application.Services;
using productHub.Application.DTOs.Auth;
using productHub.Application.Security;
using productHub.Domain.Interfaces;
using productHub.Domain.Entities;

namespace productHub.Application.UnitTests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ReturnsToken_WhenUserIsCreated()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtProvider>();

        repo.Setup(r => r.GetByEmailAsync("test@mail.com"))
            .ReturnsAsync((User?)null);

        hasher.Setup(h => h.Hash("123456")).Returns("hashed");
        jwt.Setup(j => j.GenerateToken(
                It.IsAny<Guid>(),
                "sneider",
                "test@mail.com",
                "Admin"))
            .Returns(("fakeToken", DateTime.UtcNow.AddHours(1)));

        var svc = new AuthService(repo.Object, hasher.Object, jwt.Object);

        var request = new RegisterRequest
        {
            Username = "sneider",
            Email = "test@mail.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var resp = await svc.RegisterAsync(request);

        // Assert
        Assert.Equal("fakeToken", resp.Token);
    }
}