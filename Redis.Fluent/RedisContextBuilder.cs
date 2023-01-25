namespace Redis.Fluent
{
    public class RedisContextBuilder
    {
        public RedisEntity<T> Entity<T>()
        {
            if (RedisContext.Entities.ContainsKey(typeof(T)))
            {
                return (RedisEntity<T>)RedisContext.Entities[typeof(T)];
            }
            else
            {
                var entity = new RedisEntity<T>();
                RedisContext.Entities.Add(typeof(T), entity);
                return entity;
            }
        }
    }
}
