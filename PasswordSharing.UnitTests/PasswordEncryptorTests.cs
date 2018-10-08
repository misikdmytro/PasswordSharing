using System.Security.Cryptography;
using Moq;
using PasswordSharing.Algorithms;
using PasswordSharing.Contracts;
using PasswordSharing.Models;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class PasswordEncryptorTests
	{
		private readonly Mock<IEncryptService> _encryptServiceMock;
		private readonly Mock<IRsaParametersBuilder> _builderMock;
		private readonly PasswordEncryptor _encryptor;

		public PasswordEncryptorTests()
		{
			_encryptServiceMock = new Mock<IEncryptService>();
			_builderMock = new Mock<IRsaParametersBuilder>();
			_encryptor = new PasswordEncryptor(_encryptServiceMock.Object, _builderMock.Object);
		}

		[Fact]
		public void EncodeShouldDoIt()
		{
			// Arrange
			const string str = "helloworld";

			// Act
			var password = _encryptor.Encode(str, new PublicKey());

			// Assert
			_encryptServiceMock.Verify(x => x.Encode(str, It.IsAny<RSAParameters>()), Times.Once);
			_builderMock.Verify(x => x.Build(It.IsAny<PublicKey>()), Times.Once);
		}


		[Fact]
		public void DecodeShouldDoIt()
		{
			// Arrange
			const string str = "helloworld";

			var publickKeyStr = new PublicKey().ToString();

			// Act
			_encryptor.Decode(new Password { Encoded = str, PublicKey = publickKeyStr }, new PrivateKey());

			// Assert
			_encryptServiceMock.Verify(x => x.Decode(str, It.IsAny<RSAParameters>()), Times.Once);
			_builderMock.Verify(x => x.Build(It.IsAny<PublicKey>(), It.IsAny<PrivateKey>()), Times.Once);
		}
	}
}
