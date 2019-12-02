using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using PlayMate.Common.Attribute;

namespace PlayMate.Aop
{
    public class CacheAop : IInterceptor
    {
        private readonly Common.Cache.IMemoryCache _cache;
        public CacheAop(Common.Cache.IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) is CachingAttribute)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);
                //根据key获取相应的缓存值
                var cacheValue = _cache.Get(cacheKey);
                if (cacheValue != null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    invocation.ReturnValue = cacheValue;
                    return;
                }
                //去执行当前的方法
                invocation.Proceed();
                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _cache.Set(cacheKey, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }

        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            //var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();

            string key = $"{typeName}:{methodName}:";
            //foreach (var param in methodArguments)
            //{
            //    key = $"{key}:{param}";
            //}
            return key.TrimEnd(':');
        }
    }
}
