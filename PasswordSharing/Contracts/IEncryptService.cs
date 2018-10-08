using System.Security.Cryptography;

namespace PasswordSharing.Contracts
{
	public interface IEncryptService
	{
		string Decode(string str, RSAParameters privateKey);
		string Encode(string str, RSAParameters publicKey);
	}
}