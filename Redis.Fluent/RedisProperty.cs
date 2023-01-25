using Newtonsoft.Json;

namespace Redis.Fluent
{
    public class RedisProperty
    {
        public string PropertyName { get; set; }
        public Required Required { get; set; } = Required.Default;
        public bool Ignored { get; set; } = false;
        public long MaxLength { get; set; } = long.MaxValue;
        public string Name { get; set; } = null;

        public RedisProperty(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
