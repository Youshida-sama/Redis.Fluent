using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Redis.Fluent.Serializarion
{
    public class RedisSerializationResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (RedisContext.Entities.ContainsKey(property.DeclaringType))
            {
                var entity = RedisContext.Entities[property.DeclaringType];
                if (((Dictionary<string, RedisProperty>)entity.GetType().GetProperty("Properties").GetValue(entity))
                    .TryGetValue(property.PropertyName, out var prop))
                {
                    property.Required = prop.Required;
                    property.Ignored = prop.Ignored;
                    if (prop.Name is not null)
                    {
                        property.PropertyName = prop.Name;
                    }
                }
            }

            return property;
        }
    }
}
