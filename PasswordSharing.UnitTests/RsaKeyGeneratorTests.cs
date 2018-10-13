using PasswordSharing.Contracts;
using PasswordSharing.Services;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
    public class RsaKeyGeneratorTests
    {
        private readonly IRsaKeyGenerator _keyGenerator;

        public RsaKeyGeneratorTests()
        {
            _keyGenerator = new RsaKeyGenerator();
        }

        [Fact]
        public void GenerateKeyShouldDoIt()
        {
            // Arrange
            // Act
            var key = _keyGenerator.GenerateKey();

            // Assert
            key.ShouldNotBeNull();
        }

        [Fact]
        public void ConvertToStringShouldWorkTwice()
        {
            // Arrange
            var key = _keyGenerator.GenerateKey();

            // Act
            var str1 = _keyGenerator.ToString(key);
            var str2 = _keyGenerator.ToString(key);

            // Assert
            str1.ShouldBe(str2);
        }

        [Fact]
        public void ConvertFromStringShouldWorkCorrectly()
        {
            // Arrange
            var key = _keyGenerator.GenerateKey();
            var str = _keyGenerator.ToString(key);

            // Act
            var result = _keyGenerator.FromString(str);

            // Assert
            result.ShouldNotBeNull();
        }
    }
}
