using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Algorithms
{
	public class PasswordEncryptor
	{
		private readonly IEncryptService _encryptService;
		private readonly IRsaParametersBuilder _rsaParametersBuilder;

		public PasswordEncryptor(IEncryptService encryptService, IRsaParametersBuilder rsaParametersBuilder)
		{
			_encryptService = encryptService;
			_rsaParametersBuilder = rsaParametersBuilder;
		}

		public Password Encode(string password, PublicKey publicKey)
		{
			var pubKeyString = publicKey.ToString();
			var @params = _rsaParametersBuilder.Build(publicKey);

			return new Password
			{
				PublicKey = pubKeyString,
				Encoded = _encryptService.Encode(password, @params)
			};
		}

		public string Decode(Password password, PrivateKey privateKey)
		{
			var publicKey = PublicKey.FromString(password.PublicKey);
			var key = _rsaParametersBuilder.Build(publicKey, privateKey);

			return _encryptService.Decode(password.Encoded, key);
		}
	}
}
