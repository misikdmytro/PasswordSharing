using System.Security.Cryptography;
using Moq;
using PasswordSharing.Constants;
using PasswordSharing.Interfaces;
using PasswordSharing.Models;
using PasswordSharing.Services;
using Xunit;

namespace PasswordSharing.UnitTests
{
    public class PasswordEncryptorTests
    {
        private readonly Mock<IEncryptService> _encryptServiceMock;
        private readonly IPasswordEncryptor _encryptor;
        private readonly RSAParameters _parameters;

        public PasswordEncryptorTests()
        {
            _encryptServiceMock = new Mock<IEncryptService>();
            _encryptor = new PasswordEncryptor(_encryptServiceMock.Object);

            using (var csp = new RSACryptoServiceProvider(AlgorithmConstants.KeySize))
            {
                _parameters = csp.ExportParameters(true);
            }
        }

        [Fact]
        public void EncodeShouldDoIt()
        {
            // Arrange
            const string str = "helloworld";

            // Act
            var password = _encryptor.Encode(str, _parameters);

            // Assert
            _encryptServiceMock.Verify(x => x.Encode(str, It.IsAny<RSAParameters>()), Times.Once);
        }

        [Fact]
        public void DecodeShouldDoIt()
        {
            // Arrange
            const string str = "helloworld";

            // Act
            _encryptor.Decode(new Password { Encoded = str }, _parameters);

            // Assert
            _encryptServiceMock.Verify(x => x.Decode(str, It.IsAny<RSAParameters>()), Times.Once);
        }
    }
}
