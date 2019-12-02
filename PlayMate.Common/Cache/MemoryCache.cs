using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using mc = Microsoft.Extensions.Caching.Memory.IMemoryCache;

namespace PlayMate.Common.Cache
{
    public class MemoryCache : IMemoryCache
    {
        private readonly mc _cache;

        public MemoryCache(mc cache)
        {
            _cache = cache;
        }

        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }
}
