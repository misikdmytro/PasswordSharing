using System.Security.Cryptography;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface IRsaParametersBuilder
	{
		RSAParameters Build(PublicKey publicKey, PrivateKey privateKey);
		RSAParameters Build(PublicKey publicKey);
		RSAParameters Build(string str);
	}
}