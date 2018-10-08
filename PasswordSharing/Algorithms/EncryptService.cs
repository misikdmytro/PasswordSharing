using System;
using System.Security.Cryptography;
using System.Text;
using PasswordSharing.Contracts;

namespace PasswordSharing.Algorithms
{
	public class EncryptService : IEncryptService
	{
		private readonly IRSAAlgoParameters _parameters;

		public EncryptService(IRSAAlgoParameters parameters)
		{
			_parameters = parameters;
		}

		public string Encrypt(string str)
		{
			using (var csp = new RSACryptoServiceProvider(2048))
			{
				csp.ImportParameters(_parameters.PublicKey);
				var bytes = Encoding.Unicode.GetBytes(str);
				var encoded = csp.Encrypt(bytes, false);
				return Convert.ToBase64String(encoded);
			}
		}

		public string Decrypt(string str)
		{
			using (var csp = new RSACryptoServiceProvider(2048))
			{
				csp.ImportParameters(_parameters.PrivateKey);
				var bytes = Convert.FromBase64String(str);
				var decoded = csp.Decrypt(bytes, false);
				return Encoding.Unicode.GetString(decoded);
			}
		}
	}
}
