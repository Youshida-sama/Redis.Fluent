using System.Collections.Generic;
using System;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Redis.Fluent
{
    public class RedisContext
    {
        public static Dictionary<Type, object> Entities { get; set; } = new Dictionary<Type, object>();
        protected RedisContext(IRedisClient client)
        {
            OnCreating(new RedisContextBuilder());
            FillContext(client);
        }

        private void FillContext(IRedisClient client)
        {
            var type = GetType();
            foreach (var prop in type.GetProperties())
            {
                if (prop.PropertyType.Name == "RedisCollection`1")
                {
                    prop.SetValue(this, Activator.CreateInstance(prop.PropertyType, client));
                }
            }
        }

        protected virtual void OnCreating(RedisContextBuilder builder)
        {

        }
    }
}
