using System.Security.Cryptography;

namespace PasswordSharing.Interfaces
{
    public interface IKeyGenerator
    {
        RSAParameters GenerateKey();
        string ToString(RSAParameters parameters);
        RSAParameters FromString(string key);
    }
}
