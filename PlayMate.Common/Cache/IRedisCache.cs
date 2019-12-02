using System;
using System.Collections.Generic;
using System.Text;

namespace PlayMate.Common.Cache
{
    public interface IRedisCache
    {
        //获取 Reids 缓存值
        string GetValue(string key);

        //获取值，并序列化
        T Get<T>(string key);

        //保存
        void Set(string key, object value, TimeSpan cacheTime);

        bool SetValue(string key, byte[] value);

        //判断是否存在
        bool Get(string key);

        //移除某一个缓存值
        void Remove(string key);

        //全部清除
        void Clear();
    }
}
