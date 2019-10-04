using System.Security.Cryptography;

namespace PasswordSharing.Interfaces
{
	public interface IEncryptService
	{
		string Decode(string str, RSAParameters privateKey);
		string Encode(string str, RSAParameters publicKey);
	}
}