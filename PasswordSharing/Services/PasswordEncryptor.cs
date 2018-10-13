using System.Security.Cryptography;
using PasswordSharing.Contracts;
using PasswordSharing.Models;

namespace PasswordSharing.Services
{
    public class PasswordEncryptor : IPasswordEncryptor
    {
        private readonly IEncryptService _encryptService;

        public PasswordEncryptor(IEncryptService encryptService)
        {
            _encryptService = encryptService;
        }

        public Password Encode(string password, RSAParameters key)
        {
            var encoded = _encryptService.Encode(password, key);

            return new Password
            {
                Encoded = encoded
            };
        }

        public string Decode(Password password, RSAParameters key)
        {
            return _encryptService.Decode(password.Encoded, key);
        }
    }
}
