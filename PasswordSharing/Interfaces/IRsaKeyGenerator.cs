using System.Security.Cryptography;

namespace PasswordSharing.Interfaces
{
    public interface IRsaKeyGenerator
    {
        RSAParameters GenerateKey();
        string ToString(RSAParameters parameters);
        RSAParameters FromString(string key);
    }
}
