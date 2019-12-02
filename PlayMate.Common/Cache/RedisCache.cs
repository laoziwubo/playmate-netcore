using System;
using System.Collections.Generic;
using System.Text;
using PlayMate.Common.Helper;
using StackExchange.Redis;

namespace PlayMate.Common.Cache
{
    public class RedisCache : IRedisCache
    {
        private readonly string _redisConnenctionString;

        public volatile ConnectionMultiplexer RedisConnection;

        private static readonly object ConnectionLock = new object();

        public RedisCache()
        {
            var redisConfiguration = AppSettingsHelper.Config("Redis", "ConnectionString");
            if (string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config is empty", nameof(redisConfiguration));
            }
            this._redisConnenctionString = redisConfiguration;
            this.RedisConnection = GetRedisConnection();
        }

        /// <summary>
        /// 核心代码，获取连接实例
        /// 通过双if 夹lock的方式，实现单例模式
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this.RedisConnection != null && this.RedisConnection.IsConnected)
            {
                return this.RedisConnection;
            }

            //加锁，防止异步编程中，出现单例无效的问题
            lock (ConnectionLock)
            {
                //释放redis连接
                RedisConnection?.Dispose();
                try
                {
                    this.RedisConnection = ConnectionMultiplexer.Connect(_redisConnenctionString);
                }
                catch (Exception)
                {
                    //throw new Exception("Redis服务未启用，请开启该服务，并且请注意端口号。");
                }
            }
            return this.RedisConnection;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    RedisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Get(string key)
        {
            return RedisConnection.GetDatabase().KeyExists(key);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return RedisConnection.GetDatabase().StringGet(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Get<TEntity>(string key)
        {
            var value = RedisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            RedisConnection.GetDatabase().KeyDelete(key);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        public void Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                RedisConnection.GetDatabase().StringSet(key, SerializeHelper.Serialize(value), cacheTime);
            }
        }

        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, byte[] value)
        {
            return RedisConnection.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(120));
        }
    }
}
