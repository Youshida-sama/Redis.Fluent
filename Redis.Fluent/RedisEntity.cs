using System.Collections.Generic;
using System;

namespace Redis.Fluent
{
    public class RedisEntity<T>
    {
        public List<Func<T, object>> Keys = new List<Func<T, object>>();
        public Dictionary<string, RedisProperty> Properties { get; set; } = new Dictionary<string, RedisProperty>();
        public string Prefix { get; private set; } = null;

        public RedisProperty Property(string propName)
        {
            if (Properties.ContainsKey(propName))
            {
                return Properties[propName];
            }

            var prop = new RedisProperty(propName);
            Properties.Add(propName, prop);
            return prop;
        }

        public void HasKey(Func<T, object> propertySelector) => Keys.Add(propertySelector);
        public void HasPrefix(string prefix) => Prefix = prefix;
    }
}
