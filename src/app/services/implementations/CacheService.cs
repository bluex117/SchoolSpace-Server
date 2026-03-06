using backend.app.services.interfaces;
using backend.app.configurations.resources;

using StackExchange.Redis;

namespace backend.app.services.implementations
{
    public sealed class CacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;
        private static readonly LuaScript ReleaseLockScript = LuaScript.Prepare(@"
            if redis.call('GET', KEYS[1]) == ARGV[1] then
                return redis.call('DEL', KEYS[1])
            else
                return 0
            end
        ");

        public CacheService(RedisResource redisResource)
        {
            _db = redisResource.Database;
            _redis = redisResource.Multiplexer;
        }

        public Task<bool> SetValueAsync(string key, string value, TimeSpan? expiry = null) =>
            _db.StringSetAsync(key, value, expiry);

        public Task<bool> SetValueNXAsync(string key, string value, TimeSpan? expiry = null) =>
            _db.StringSetAsync(key, value, expiry, When.NotExists);

        public Task<bool> SetValueXXAsync(string key, string value, TimeSpan? expiry = null) =>
            _db.StringSetAsync(key, value, expiry, When.Exists);

        public async Task<string?> GetValueAsync(string key)
        {
            var v = await _db.StringGetAsync(key).ConfigureAwait(false);
            return v.HasValue ? v.ToString() : null;
        }

        public async Task<string?> GetSetAsync(string key, string value)
        {
            var v = await _db.StringGetSetAsync(key, value).ConfigureAwait(false);
            return v.HasValue ? v.ToString() : null;
        }

        public Task<bool> DeleteKeyAsync(string key) =>
            _db.KeyDeleteAsync(key);

        public Task<bool> KeyExistsAsync(string key) =>
            _db.KeyExistsAsync(key);

        public Task<TimeSpan?> GetTTLAsync(string key) =>
            _db.KeyTimeToLiveAsync(key);

        public Task<bool> SetExpiryAsync(string key, TimeSpan expiry) =>
            _db.KeyExpireAsync(key, expiry);
        public Task<long> IncrementAsync(string key, long value = 1) =>
            _db.StringIncrementAsync(key, value);

        public Task<long> DecrementAsync(string key, long value = 1) =>
            _db.StringDecrementAsync(key, value);
        public Task<bool> HashSetAsync(string key, string field, string value) =>
            _db.HashSetAsync(key, field, value);

        public async Task<string?> HashGetAsync(string key, string field)
        {
            var v = await _db.HashGetAsync(key, field).ConfigureAwait(false);
            return v.HasValue ? v.ToString() : null;
        }

        public async Task<Dictionary<string, string>> HashGetAllAsync(string key)
        {
            var entries = await _db.HashGetAllAsync(key).ConfigureAwait(false);
            return entries.ToDictionary(
                e => e.Name.ToString(),
                e => e.Value.ToString()
            );
        }

        public Task<bool> HashDeleteAsync(string key, string field) =>
            _db.HashDeleteAsync(key, field);

        // ---------------------------
        // Set
        // ---------------------------
        public Task<bool> SetAddAsync(string key, string value) =>
            _db.SetAddAsync(key, value);

        public Task<bool> SetRemoveAsync(string key, string value) =>
            _db.SetRemoveAsync(key, value);

        public async Task<string[]> SetMembersAsync(string key)
        {
            var members = await _db.SetMembersAsync(key).ConfigureAwait(false);
            return members.Select(m => m.ToString()).ToArray();
        }

        // ---------------------------
        // List
        // ---------------------------
        public Task<long> ListLeftPushAsync(string key, string value) =>
            _db.ListLeftPushAsync(key, value);

        public Task<long> ListRightPushAsync(string key, string value) =>
            _db.ListRightPushAsync(key, value);

        public async Task<string?> ListLeftPopAsync(string key)
        {
            var v = await _db.ListLeftPopAsync(key).ConfigureAwait(false);
            return v.HasValue ? v.ToString() : null;
        }

        public async Task<string?> ListRightPopAsync(string key)
        {
            var v = await _db.ListRightPopAsync(key).ConfigureAwait(false);
            return v.HasValue ? v.ToString() : null;
        }

        // ---------------------------
        // Locks
        // ---------------------------
        public Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry) =>
            _db.StringSetAsync(key, value, expiry, When.NotExists);

        public async Task<bool> ReleaseLockAsync(string key, string value)
        {
            var result = await _db.ScriptEvaluateAsync(
                ReleaseLockScript.OriginalScript,
                new RedisKey[] { key },
                new RedisValue[] { value }
            ).ConfigureAwait(false);

            return (int)result == 1;
        }

        public async Task<Dictionary<string, string?>> GetManyAsync(IEnumerable<string> keys)
        {
            var keyArr = keys.Select(k => (RedisKey)k).ToArray();

            if (keyArr.Length == 0)
                return new Dictionary<string, string?>();

            var values = await _db.StringGetAsync(keyArr).ConfigureAwait(false);

            var result = new Dictionary<string, string?>(keyArr.Length);
            for (int i = 0; i < keyArr.Length; i++)
            {
                var k = keyArr[i].ToString();
                result[k] = values[i].HasValue ? values[i].ToString() : null;
            }

            return result;
        }


        public IConnectionMultiplexer GetMultiplexer() => _redis;

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None) =>
            _db.PingAsync(flags);

        public Task<RedisResult> EvalAsync(string script, RedisKey[] keys, RedisValue[] values) =>
            _db.ScriptEvaluateAsync(script, keys, values);

        public Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _redis.GetSubscriber();
            return sub.PublishAsync(channel, message, flags);
        }

        public Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler)
        {
            var sub = _redis.GetSubscriber();
            return sub.SubscribeAsync(channel, handler);
        }

        public Task UnsubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler)
        {
            var sub = _redis.GetSubscriber();
            return sub.UnsubscribeAsync(channel, handler);
        }

        public async Task<string> GetOrSetAsync(string key, Func<Task<string>> factory, TimeSpan expiry)
        {
            var existing = await GetValueAsync(key).ConfigureAwait(false);
            if (existing is not null) return existing;

            var created = await factory().ConfigureAwait(false);
            await SetValueAsync(key, created, expiry).ConfigureAwait(false);
            return created;
        }
    }
}