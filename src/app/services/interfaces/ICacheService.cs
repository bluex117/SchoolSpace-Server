using StackExchange.Redis;

namespace backend.app.services.interfaces
{
    public interface ICacheService
    {
        Task<bool> SetValueAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetValueAsync(string key);

        Task<long> IncrementAsync(string key, long value = 1);
        Task<long> DecrementAsync(string key, long value = 1);

        Task<bool> HashSetAsync(string key, string field, string value);
        Task<string?> HashGetAsync(string key, string field);
        Task<Dictionary<string, string>> HashGetAllAsync(string key);
        Task<bool> HashDeleteAsync(string key, string field);

        Task<bool> SetAddAsync(string key, string value);
        Task<bool> SetRemoveAsync(string key, string value);
        Task<string[]> SetMembersAsync(string key);

        Task<long> ListLeftPushAsync(string key, string value);
        Task<long> ListRightPushAsync(string key, string value);
        Task<string?> ListLeftPopAsync(string key);
        Task<string?> ListRightPopAsync(string key);

        Task<bool> DeleteKeyAsync(string key);
        Task<bool> KeyExistsAsync(string key);
        Task<TimeSpan?> GetTTLAsync(string key);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);

        Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry);
        Task<bool> ReleaseLockAsync(string key, string value);

        Task<Dictionary<string, string?>> GetManyAsync(IEnumerable<string> keys);

        // Extensions
        IConnectionMultiplexer GetMultiplexer();
        Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None);
        Task<RedisResult> EvalAsync(string script, RedisKey[] keys, RedisValue[] values);

        // Optional extras
        Task<bool> SetValueNXAsync(string key, string value, TimeSpan? expiry = null);
        Task<bool> SetValueXXAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetSetAsync(string key, string value);
        Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None);
        Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler);
        Task UnsubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler);
        Task<string> GetOrSetAsync(string key, Func<Task<string>> factory, TimeSpan expiry);
    }
}