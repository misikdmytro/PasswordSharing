using System;
using System.Threading.Tasks;
using PasswordSharing.Models;

namespace PasswordSharing.Interfaces
{
    public interface IRedisClient : IDisposable
    {
        Task<TValue> GetAsync<TKey, TValue>(TKey key)
            where TKey : ICacheKey;

        Task SetAsync<TKey, TValue>(TKey key, TValue value, TimeSpan? expiration = null)
            where TKey : ICacheKey;

        Task RenameAsync<TKey>(TKey oldKey, TKey newKey)
            where TKey : ICacheKey;

        Task DeleteAsync<TKey>(TKey key)
            where TKey : ICacheKey;
    }
}