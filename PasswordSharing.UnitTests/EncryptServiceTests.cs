using System;
using System.Security.Cryptography;
using PasswordSharing.Algorithms;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class EncryptServiceTests
	{
		[Fact]
		public void EncryptionShouldWorkCorrect()
		{
			// Arrange
			var csp = new RSACryptoServiceProvider(2048);
			var pubKey = csp.ExportParameters(false);
			var privKey = csp.ExportParameters(true);

			var service = new EncryptService(pubKey, privKey);

			var str = "helloworld";

			// Act
			var encoded = service.Encrypt(str);
			var decoded = service.Decrypt(encoded);

			// Assert
			decoded.ShouldBe(str);
		}

		[Fact]
		public void EncryptionShouldbeDeniedIfWrongKeyProvided()
		{
			// Arrange
			var csp = new RSACryptoServiceProvider(2048);
			var pubKey = csp.ExportParameters(false);

			csp = new RSACryptoServiceProvider(2048);
			var privKey = csp.ExportParameters(true);

			var service = new EncryptService(pubKey, privKey);

			var str = "helloworld";

			// Act
			var encoded = service.Encrypt(str);
			Action decodedAction = () => service.Decrypt(encoded);

			// Assert
			decodedAction.ShouldThrow<Exception>();
		}
	}
}
