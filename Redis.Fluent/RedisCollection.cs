using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Threading.Tasks;
using System;

namespace Redis.Fluent
{
    public class RedisCollection<T>
    {
        private RedisEntity<T> _metadata;
        private IRedisClient _client;
        public RedisCollection(IRedisClient client)
        {
            _client = client;
            if (RedisContext.Entities.TryGetValue(typeof(T), out var value))
            {
                _metadata = (RedisEntity<T>)value;
            }
        }

        public T Get(object key)
        {
            var task = Task.Run(async () =>
            {
                var keyStr = "";

                if (_metadata.Prefix is not null)
                    keyStr += $"{_metadata.Prefix}:";

                var guid = await _client.Db0.GetAsync<Guid>($"{keyStr}meta:{key}");
                var result = await _client.Db0.GetAsync<T>($"{keyStr}{guid}");

                return result;
            });
            task.Wait();
            return task.Result;
        }

        public Guid Set(T value, TimeSpan? ttl = null)
        {
            var task = Task.Run(async () =>
            {
                Guid guid = Guid.NewGuid();
                var key = "";

                if (_metadata.Prefix is not null)
                    key += $"{_metadata.Prefix}:";

                if (_metadata.Keys.Count != 0)
                {
                    foreach (var keyDef in _metadata.Keys)
                    {
                        await Add($"{key}meta:{keyDef.Invoke(value)}", guid, ttl);
                    }
                }

                key += $"{guid}";


                await Add(key, value, ttl);
                return guid;
            });
            task.Wait();
            return task.Result;
        }

        public T Get(object key, Func<T> resultGet, TimeSpan? ttl = null)
        {
            var task = Task.Run(async () =>
            {
                var cached = Get(key);

                if (cached is not null)
                    return cached;

                var value = resultGet();
                var guid = Set(value, ttl);

                var prefix = "";

                if (_metadata.Prefix is not null)
                    prefix += $"{_metadata.Prefix}:";

                await Add($"{prefix}meta:{key}", guid, ttl);

                return value;
            });
            task.Wait();
            return task.Result;
        }

        private async Task Add<TVal>(string key, TVal value, TimeSpan? ttl = null)
        {
            if (ttl is null)
            {
                await _client.Db0.AddAsync(key, value);
            }
            else
            {
                await _client.Db0.AddAsync(key, value, ttl.Value);
            }
        }
    }
}
