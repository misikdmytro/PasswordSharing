using System.Security.Cryptography;

namespace PasswordSharing.Contracts
{
	public interface IRSAAlgoParameters
	{
		RSAParameters PublicKey { get; }
		RSAParameters PrivateKey { get; }
	}
}
