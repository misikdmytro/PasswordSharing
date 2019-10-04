using System;
using PasswordSharing.Interfaces;
using StackExchange.Redis;

namespace PasswordSharing.Redis
{
    public class RedisClientFactory : IRedisClientFactory
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly TimeSpan _defaultExpiration;

        public RedisClientFactory(ConnectionMultiplexer connectionMultiplexer, TimeSpan defaultExpiration)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            _defaultExpiration = defaultExpiration;
        }

        public IRedisClient GetClient()
        {
            return new RedisClient(_connectionMultiplexer.GetDatabase(), _defaultExpiration);
        }
    }
}
