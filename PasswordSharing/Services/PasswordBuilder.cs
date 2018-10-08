using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using PasswordSharing.Constants;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Services
{
	public class PasswordBuilder : IPasswordBuilder
	{
		private readonly IEncryptService _encryptService;

		public PasswordBuilder(IEncryptService encryptService)
		{
			_encryptService = encryptService;
		}

		public Password Encode(string password, TimeSpan expiration)
		{
			using (var csp = new RSACryptoServiceProvider(AlgorithmConstants.KeySize))
			{
				try
				{
					var key = csp.ExportParameters(true);
					var encoded = _encryptService.Encode(password, key);

					using (var sw = new StringWriter())
					{
						var xs = new XmlSerializer(typeof(RSAParameters));
						xs.Serialize(sw, key);

						var keyStr = sw.ToString();

						return new Password
						{
							Encoded = encoded,
							Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyStr)),
                            Status = PasswordStatus.Valid,
                            ExpiresAt = DateTime.Now.Add(expiration)
						};
					}
				}
				finally
				{
					csp.PersistKeyInCsp = false;
					csp.Clear();
				}
			}
		}

		public string Decode(Password password)
		{
			var keyStr = Encoding.UTF8.GetString(Convert.FromBase64String(password.Key));

			using (var sr = new StringReader(keyStr))
			{
				var xs = new XmlSerializer(typeof(RSAParameters));
				var key = (RSAParameters)xs.Deserialize(sr);

				return _encryptService.Decode(password.Encoded, key);
			}
		}
	}
}
