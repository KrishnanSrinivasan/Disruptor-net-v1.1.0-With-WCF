using System;
using ServiceStack.Redis;

namespace Consumer
{
    /// <summary>
    /// Wrapper over the <see cref="ServiceStack.Redis.RedisClient"/>
    /// </summary>
    internal class RedisDB : IDisposable
    {
        private readonly RedisClient _redisClient;

        internal static RedisDB Instance { get; private set; }

        private  RedisDB() 
        {
            _redisClient = new RedisClient();
            _redisClient.Ping();
        }

        internal static void Initialize() 
        {
            Instance = new RedisDB();
        }

        internal void Write(String key, Byte[] data)
        {
            _redisClient.Add<Byte[]>(key, data);
        }

        public void Dispose()
        {
            _redisClient.Dispose();
        }
    }
}

