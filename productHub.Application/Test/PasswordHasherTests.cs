using Xunit;
using productHub.Application.Security;

// Fake para no depender de Infrastructure
file sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string plainText) => plainText + "_hashed";
    public bool Verify(string plainText, string hashed) => hashed == Hash(plainText);
}

namespace productHub.Application.Test
{
    public sealed class PasswordHasherTests
    {
        [Fact]
        public void Hash_ProducesDifferentValue_ThanPlain()
        {
            // Arrange
            IPasswordHasher hasher = new FakePasswordHasher();

            // Act
            var hash = hasher.Hash("123456");

            // Assert
            Assert.NotEqual("123456", hash);
        }

        [Fact]
        public void Verify_ReturnsTrue_WhenMatches()
        {
            // Arrange
            IPasswordHasher hasher = new FakePasswordHasher();
            var hash = hasher.Hash("123456");

            // Act
            var ok = hasher.Verify("123456", hash);

            // Assert
            Assert.True(ok);
        }
    }
}