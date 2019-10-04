using System;
using System.Security.Cryptography;
using System.Text;
using PasswordSharing.Constants;
using PasswordSharing.Exceptions;
using PasswordSharing.Interfaces;

namespace PasswordSharing.Algorithms
{
	public class EncryptService : IEncryptService
	{
		public string Encode(string str, RSAParameters publicKey)
		{
			using (var csp = new RSACryptoServiceProvider(AlgorithmConstants.KeySize))
			{
				try
				{
					csp.ImportParameters(publicKey);
					var bytes = Encoding.Unicode.GetBytes(str);
					if (bytes.Length > AlgorithmConstants.MaxMessageSize)
					{
						throw new BadLengthException($"Expected max message size is {AlgorithmConstants.MaxMessageSize} but was {bytes.Length}");
					}

					var encoded = csp.Encrypt(bytes, false);
					return Convert.ToBase64String(encoded);
				}
				finally
				{
					csp.PersistKeyInCsp = false;
					csp.Clear();
				}
			}
		}

		public string Decode(string str, RSAParameters privateKey)
		{
			using (var csp = new RSACryptoServiceProvider(AlgorithmConstants.KeySize))
			{
				try
				{
					csp.ImportParameters(privateKey);
					var bytes = Convert.FromBase64String(str);
					var decoded = csp.Decrypt(bytes, false);
					return Encoding.Unicode.GetString(decoded);
				}
				finally
				{
					csp.PersistKeyInCsp = false;
					csp.Clear();
				}
			}
		}
	}
}
