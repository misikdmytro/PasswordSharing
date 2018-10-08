using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Algorithms
{
	public class PasswordEncryptor
	{
		private readonly IEncryptService _encryptService;

		public PasswordEncryptor(IEncryptService encryptService)
		{
			_encryptService = encryptService;
		}

		public Password Encode(string password, RSAParameters publicKey)
		{
			using (var sw = new StringWriter())
			{
				var xs = new XmlSerializer(typeof(RSAParameters));
				xs.Serialize(sw, publicKey);
				var pubKeyString = sw.ToString();

				return new Password
				{
					PublicKey = pubKeyString,
					Encoded = _encryptService.Encode(password, publicKey)
				};
			}
		}

		public string Decode(Password password, RSAParameters privateKey)
		{
			using (var sr = new StringReader(password.PublicKey))
			{
				var xs = new XmlSerializer(typeof(RSAParameters));
				var pubKey = (RSAParameters)xs.Deserialize(sr);

				privateKey.Modulus = pubKey.Modulus;
				privateKey.Exponent = pubKey.Exponent;

				return _encryptService.Decode(password.Encoded, privateKey);
			}
		}
	}
}
