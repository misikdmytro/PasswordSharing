namespace PasswordSharing.Interfaces
{
    public interface IRedisClientFactory
    {
        IRedisClient GetClient();
    }
}