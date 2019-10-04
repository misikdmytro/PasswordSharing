using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PasswordSharing.Interfaces;
using PasswordSharing.Models;
using StackExchange.Redis;

namespace PasswordSharing.Redis
{
    public class RedisClient : IRedisClient
    {
        private readonly IDatabaseAsync _database;
        private readonly TimeSpan _defaultExpiration;

        public RedisClient(IDatabaseAsync database, TimeSpan defaultExpiration)
        {
            _database = database;
            _defaultExpiration = defaultExpiration;
        }

        public async Task<TValue> GetAsync<TKey, TValue>(TKey key)
            where TKey : ICacheKey
        {
            string value = await _database.StringGetAsync(key.ExtractKey());
            return !string.IsNullOrEmpty(value)
                ? JsonConvert.DeserializeObject<TValue>(value)
                : default(TValue);
        }

        public Task SetAsync<TKey, TValue>(TKey key, TValue value, TimeSpan? expiration)
            where TKey : ICacheKey
        {
            return _database.StringSetAsync(key.ExtractKey(), JsonConvert.SerializeObject(value), 
                expiration ?? _defaultExpiration);
        }

        public Task RenameAsync<TKey>(TKey oldKey, TKey newKey) where TKey : ICacheKey
        {
            return _database.KeyRenameAsync(oldKey.ExtractKey(), newKey.ExtractKey());
        }

        public Task DeleteAsync<TKey>(TKey key) where TKey : ICacheKey
        {
            return _database.KeyDeleteAsync(key.ExtractKey());
        }

        public void Dispose()
        {
        }
    }
}
