using System;
using System.Security.Cryptography;
using System.Text;
using PasswordSharing.Contracts;

namespace PasswordSharing.Algorithms
{
	public class EncryptService : IEncryptService
	{
		private readonly RSAParameters _publicKey;
		private readonly RSAParameters _privateKey;

		public EncryptService(RSAParameters publicKey, RSAParameters privateKey)
		{
			_publicKey = publicKey;
			_privateKey = privateKey;
		}

		public string Encrypt(string str)
		{
			using (var csp = new RSACryptoServiceProvider(2048))
			{
				csp.ImportParameters(_publicKey);
				var bytes = Encoding.Unicode.GetBytes(str);
				var encoded = csp.Encrypt(bytes, false);
				return Convert.ToBase64String(encoded);
			}
		}

		public string Decrypt(string str)
		{
			using (var csp = new RSACryptoServiceProvider(2048))
			{
				csp.ImportParameters(_privateKey);
				var bytes = Convert.FromBase64String(str);
				var decoded = csp.Decrypt(bytes, false);
				return Encoding.Unicode.GetString(decoded);
			}
		}
	}
}
