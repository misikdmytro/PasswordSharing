using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Moq;
using PasswordSharing.Constants;
using PasswordSharing.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Services;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class PasswordEncryptorTests
	{
		private readonly Mock<IEncryptService> _encryptServiceMock;
		private readonly IPasswordBuilder _encryptor;

		public PasswordEncryptorTests()
		{
			_encryptServiceMock = new Mock<IEncryptService>();
			_encryptor = new PasswordBuilder(_encryptServiceMock.Object);
		}

		[Fact]
		public void EncodeShouldDoIt()
		{
			// Arrange
			const string str = "helloworld";
		    var expiration = TimeSpan.FromHours(2);

		    // Act
		    var password = _encryptor.Encode(str, expiration);

			// Assert
			_encryptServiceMock.Verify(x => x.Encode(str, It.IsAny<RSAParameters>()), Times.Once);
			password.Key.ShouldNotBeNullOrEmpty();
            (password.ExpiresAt - DateTime.Now - expiration).ShouldBeLessThan(TimeSpan.FromMilliseconds(100));
		}

		[Fact]
		public void EncodeShouldGenerateDifferentKeys()
		{
			// Arrange
			const string str = "helloworld";

			// Act
			var password1 = _encryptor.Encode(str, TimeSpan.FromSeconds(1));
			var password2 = _encryptor.Encode(str, TimeSpan.FromSeconds(1));

			// Assert
			password1.Key.ShouldNotBe(password2.Key);
		}

		[Fact]
		public void DecodeShouldDoIt()
		{
			// Arrange
			const string str = "helloworld";
			using (var csp = new RSACryptoServiceProvider(AlgorithmConstants.KeySize))
			{
				var key = csp.ExportParameters(true);

				using (var sw = new StringWriter())
				{
					var xs = new XmlSerializer(typeof(RSAParameters));
					xs.Serialize(sw, key);

					var keyStr = sw.ToString();

					// Act
					_encryptor.Decode(new Password { Encoded = str, Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyStr)) });

					// Assert
					_encryptServiceMock.Verify(x => x.Decode(str, It.IsAny<RSAParameters>()), Times.Once);
				}
			}
		}
	}
}
