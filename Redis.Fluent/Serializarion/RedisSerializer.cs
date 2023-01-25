using StackExchange.Redis.Extensions.Core;
using System;
using Newtonsoft.Json;
using System.Text;

namespace Redis.Fluent.Serializarion
{
    public class RedisSerializer : ISerializer
    {
        /// <summary>
        /// Encoding to use to convert string to byte[] and the other way around.
        /// </summary>
        /// <remarks>
        /// StackExchange.Redis uses Encoding.UTF8 to convert strings to bytes,
        /// hence we do same here.
        /// </remarks>
        private static readonly Encoding encoding = Encoding.UTF8;

        private readonly JsonSerializerSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisSerializer"/> class.
        /// </summary>
        public RedisSerializer()
            : this(null)
        {
            settings = new JsonSerializerSettings()
            {
                ContractResolver = new RedisSerializationResolver()
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisSerializer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public RedisSerializer(JsonSerializerSettings? settings)
        {
            this.settings = settings;
            this.settings = new JsonSerializerSettings()
            {
                ContractResolver = new RedisSerializationResolver()
            };
        }

        /// <inheritdoc/>
        public byte[] Serialize<T>(T? item)
        {
            if (item == null)
                return Array.Empty<byte>();

            var type = item?.GetType();
            var jsonString = JsonConvert.SerializeObject(item, type, settings);
            return encoding.GetBytes(jsonString);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] serializedObject)
        {
            var jsonString = encoding.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString, settings)!;
        }
    }
}
