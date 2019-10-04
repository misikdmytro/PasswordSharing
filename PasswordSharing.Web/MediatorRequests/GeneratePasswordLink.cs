using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordSharing.Exceptions;
using PasswordSharing.Interfaces;
using PasswordSharing.Models;
using PasswordSharing.Web.Exceptions;
using PasswordSharing.Web.Models;
using Serilog;

namespace PasswordSharing.Web.MediatorRequests
{
    public class GeneratePasswordLinkRequest : IRequest<GenerateLinkModel>
    {
        public string[] Passwords { get; }
        public int ExpiresIn { get; }

        public GeneratePasswordLinkRequest(string[] passwords, int expiresIn)
        {
            Passwords = passwords;
            ExpiresIn = expiresIn;

        }
    }

    public class GeneratePasswordLinkHandler : IRequestHandler<GeneratePasswordLinkRequest, GenerateLinkModel>
    {
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IRedisClientFactory _redisClientFactory;
        private readonly IRsaKeyGenerator _keyGenerator;
        private readonly ILogger _logger = Log.ForContext<GeneratePasswordLinkHandler>();

        public GeneratePasswordLinkHandler(IPasswordEncryptor passwordEncryptor, IRsaKeyGenerator keyGenerator, IRedisClientFactory redisClientFactory)
        {
            _passwordEncryptor = passwordEncryptor;
            _keyGenerator = keyGenerator;
            _redisClientFactory = redisClientFactory;
        }

        public async Task<GenerateLinkModel> Handle(GeneratePasswordLinkRequest request, CancellationToken cancellationToken)
        {
            var key = _keyGenerator.GenerateKey();

            var passwordGroup = new PasswordGroup
            {
                Id = Guid.NewGuid(),
                Passwords = request.Passwords.Select(password => Handle(password, key)).ToArray(),
            };

            var expiration = TimeSpan.FromSeconds(request.ExpiresIn);

            _logger.Information("Create new group {@group} for {@expiration}", passwordGroup, expiration);

            var redisClient = _redisClientFactory.GetClient();
            await redisClient.SetAsync(new PasswordGroupKey(passwordGroup.Id), passwordGroup, expiration);

            return new GenerateLinkModel
            {
                Key = _keyGenerator.ToString(key),
                PasswordGroupId = passwordGroup.Id
            };
        }

        private Password Handle(string passwordStr, RSAParameters key)
        {
            try
            {
                var password = _passwordEncryptor.Encode(passwordStr, key);
                
                _logger.Debug("Password model {@password}", password);

                return password;
            }
            catch (BadLengthException ble)
            {
                const string message = "One of passwords is too long";
                _logger.Error(ble.Message);

                throw new HttpResponseException(HttpStatusCode.BadRequest, message);
            }
        }
    }
}
