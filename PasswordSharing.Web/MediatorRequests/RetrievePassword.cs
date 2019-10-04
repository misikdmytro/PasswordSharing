using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PasswordSharing.Interfaces;
using PasswordSharing.Models;
using PasswordSharing.Web.Exceptions;
using Serilog;
using StackExchange.Redis;

namespace PasswordSharing.Web.MediatorRequests
{
    public class RetrievePasswordRequest : IRequest<string[]>
    {
        public Guid PasswordGroupId { get; }
        public string Key { get; }

        public RetrievePasswordRequest(string key, Guid passwordGroupId)
        {
            Key = key;
            PasswordGroupId = passwordGroupId;
        }
    }

    public class RetrievePasswordHandler : IRequestHandler<RetrievePasswordRequest, string[]>
    {
        private readonly IRedisClientFactory _redisClientFactory;
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly ILogger _logger = Log.ForContext<RetrievePasswordHandler>();
        private readonly IKeyGenerator _keyGenerator;

        public RetrievePasswordHandler(IPasswordEncryptor passwordEncryptor,
            IKeyGenerator keyGenerator, IRedisClientFactory redisClientFactory)
        {
            _passwordEncryptor = passwordEncryptor;
            _keyGenerator = keyGenerator;
            _redisClientFactory = redisClientFactory;
        }

        public async Task<string[]> Handle(RetrievePasswordRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("Get group with id {passwordGroupId}", request.PasswordGroupId);

            var redisClient = _redisClientFactory.GetClient();
            var oldCacheKey = new PasswordGroupKey(request.PasswordGroupId);
            var newCacheKey = new PasswordGroupKey(Guid.NewGuid());

            _logger.Information("Renaming key {oldKey} to {newKey}", oldCacheKey, newCacheKey);

            try
            {
                await redisClient.RenameAsync(oldCacheKey, newCacheKey);
            }
            catch (RedisServerException rse)
            {
                const string message = "Link not exists";
                _logger.Error(rse.Message);

                throw new HttpResponseException(HttpStatusCode.BadRequest, message);
            }

            var passwordGroup = await redisClient.GetAsync<PasswordGroupKey, PasswordGroup>(newCacheKey);
            await redisClient.DeleteAsync(newCacheKey);

            if (passwordGroup == null)
            {
                const string message = "Link not exists";
                _logger.Error(message);

                throw new HttpResponseException(HttpStatusCode.BadRequest, message);
            }

            var key = _keyGenerator.FromString(request.Key);

            var result = passwordGroup.Passwords
                .Select(password => Handle(password, key))
                .ToArray();

            return result;
        }

        private string Handle(Password password, RSAParameters key)
        {
            try
            {
                _logger.Debug("Trying to retrieve password {@password}", password);

                var result = _passwordEncryptor.Decode(password, key);

                return result;
            }
            catch (Exception e)
            {
                const string message = "Incorrect link";
                _logger.Error(e.Message);

                throw new HttpResponseException(HttpStatusCode.BadRequest, message);
            }
        }
    }
}
