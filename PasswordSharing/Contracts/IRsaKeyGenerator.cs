using System.Security.Cryptography;

namespace PasswordSharing.Contracts
{
    public interface IRsaKeyGenerator
    {
        RSAParameters GenerateKey();
        string ToString(RSAParameters parameters);
        RSAParameters FromString(string key);
    }
}
