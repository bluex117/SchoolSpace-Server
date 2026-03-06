using backend.app.configurations.resources;
using backend.app.services.interfaces;

using StackExchange.Redis;

namespace backend.app.services.implementations
{
    /// <summary>
    /// Delegates to the current cache (NoOp when Redis is down, Redis when connected).
    /// When Redis reconnects in the background, all calls automatically use Redis.
    /// </summary>
    public sealed class ReconnectableCacheService : ICacheService
    {
        private readonly RedisReconnectState _state;

        public ReconnectableCacheService(RedisReconnectState state)
        {
            _state = state;
        }

        private ICacheService Inner => _state.Current;

        public Task<bool> SetValueAsync(string key, string value, TimeSpan? expiry = null) =>
            Inner.SetValueAsync(key, value, expiry);

        public Task<string?> GetValueAsync(string key) =>
            Inner.GetValueAsync(key);

        public Task<long> IncrementAsync(string key, long value = 1) =>
            Inner.IncrementAsync(key, value);

        public Task<long> DecrementAsync(string key, long value = 1) =>
            Inner.DecrementAsync(key, value);

        public Task<bool> HashSetAsync(string key, string field, string value) =>
            Inner.HashSetAsync(key, field, value);

        public Task<string?> HashGetAsync(string key, string field) =>
            Inner.HashGetAsync(key, field);

        public Task<Dictionary<string, string>> HashGetAllAsync(string key) =>
            Inner.HashGetAllAsync(key);

        public Task<bool> HashDeleteAsync(string key, string field) =>
            Inner.HashDeleteAsync(key, field);

        public Task<bool> SetAddAsync(string key, string value) =>
            Inner.SetAddAsync(key, value);

        public Task<bool> SetRemoveAsync(string key, string value) =>
            Inner.SetRemoveAsync(key, value);

        public Task<string[]> SetMembersAsync(string key) =>
            Inner.SetMembersAsync(key);

        public Task<long> ListLeftPushAsync(string key, string value) =>
            Inner.ListLeftPushAsync(key, value);

        public Task<long> ListRightPushAsync(string key, string value) =>
            Inner.ListRightPushAsync(key, value);

        public Task<string?> ListLeftPopAsync(string key) =>
            Inner.ListLeftPopAsync(key);

        public Task<string?> ListRightPopAsync(string key) =>
            Inner.ListRightPopAsync(key);

        public Task<bool> DeleteKeyAsync(string key) =>
            Inner.DeleteKeyAsync(key);

        public Task<bool> KeyExistsAsync(string key) =>
            Inner.KeyExistsAsync(key);

        public Task<TimeSpan?> GetTTLAsync(string key) =>
            Inner.GetTTLAsync(key);

        public Task<bool> SetExpiryAsync(string key, TimeSpan expiry) =>
            Inner.SetExpiryAsync(key, expiry);

        public Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry) =>
            Inner.AcquireLockAsync(key, value, expiry);

        public Task<bool> ReleaseLockAsync(string key, string value) =>
            Inner.ReleaseLockAsync(key, value);

        public Task<Dictionary<string, string?>> GetManyAsync(IEnumerable<string> keys) =>
            Inner.GetManyAsync(keys);

        public IConnectionMultiplexer GetMultiplexer() =>
            Inner.GetMultiplexer();

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None) =>
            Inner.PingAsync(flags);

        public Task<RedisResult> EvalAsync(string script, RedisKey[] keys, RedisValue[] values) =>
            Inner.EvalAsync(script, keys, values);

        public Task<bool> SetValueNXAsync(string key, string value, TimeSpan? expiry = null) =>
            Inner.SetValueNXAsync(key, value, expiry);

        public Task<bool> SetValueXXAsync(string key, string value, TimeSpan? expiry = null) =>
            Inner.SetValueXXAsync(key, value, expiry);

        public Task<string?> GetSetAsync(string key, string value) =>
            Inner.GetSetAsync(key, value);

        public Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None) =>
            Inner.PublishAsync(channel, message, flags);

        public Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler) =>
            Inner.SubscribeAsync(channel, handler);

        public Task UnsubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler) =>
            Inner.UnsubscribeAsync(channel, handler);

        public Task<string> GetOrSetAsync(string key, Func<Task<string>> factory, TimeSpan expiry) =>
            Inner.GetOrSetAsync(key, factory, expiry);
    }
}
