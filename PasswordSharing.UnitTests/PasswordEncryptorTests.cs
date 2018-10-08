using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Moq;
using PasswordSharing.Algorithms;
using PasswordSharing.Contracts;
using PasswordSharing.Models;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class PasswordEncryptorTests
	{
		private readonly Mock<IEncryptService> _encryptServiceMock;
		private readonly PasswordEncryptor _encryptor;

		public PasswordEncryptorTests()
		{
			_encryptServiceMock = new Mock<IEncryptService>();
			_encryptor = new PasswordEncryptor(_encryptServiceMock.Object);
		}

		[Fact]
		public void EncodeShouldDoIt()
		{
			// Arrange
			using (var csp = new RSACryptoServiceProvider(2048))
			{
				const string str = "helloworld";

				var pubKey = csp.ExportParameters(false);

				// Act
				var password = _encryptor.Encode(str, pubKey);

				// Assert
				password.PublicKey.ShouldNotBeNull();
				_encryptServiceMock.Verify(x => x.Encode(str, pubKey), Times.Once);
			}
		}


		[Fact]
		public void DecodeShouldDoIt()
		{
			// Arrange
			using (var csp = new RSACryptoServiceProvider(2048))
			{
				const string str = "helloworld";

				var pubKey = csp.ExportParameters(false);
				var privKey = csp.ExportParameters(true);

				string pubKeyString;
				using (var sw = new StringWriter())
				{
					var xs = new XmlSerializer(typeof(RSAParameters));
					xs.Serialize(sw, pubKey);
					pubKeyString = sw.ToString();
				}

				// Act
					_encryptor.Decode(new Password { Encoded = str, PublicKey = pubKeyString }, privKey);

				// Assert
				_encryptServiceMock.Verify(x => x.Decode(str, It.IsAny<RSAParameters>()), Times.Once);
			}
		}
	}
}
