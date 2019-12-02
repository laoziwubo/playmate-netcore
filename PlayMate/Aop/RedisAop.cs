using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using PlayMate.Common.Attribute;
using PlayMate.Common.Cache;

namespace PlayMate.Aop
{
    public class RedisAop: IInterceptor
    {
        private readonly IRedisCache _redis;
        public RedisAop(IRedisCache redis)
        {
            _redis = redis;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var cacheAttr = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            if (cacheAttr != null)
            {
                var cacheKey = CustomCacheKey(invocation);
                var cacheValue = _redis.GetValue(cacheKey);
                if (cacheValue != null)
                {
                    var type = invocation.Method.ReturnType;
                    var resultTypes = type.GenericTypeArguments;
                    if (type.FullName == "System.Void")
                    {
                        return;
                    }
                    object response;
                    if (typeof(Task).IsAssignableFrom(type))
                    {
                        //核心2：返回异步对象Task<T>
                        if (resultTypes.Any())
                        {
                            var resultType = resultTypes.FirstOrDefault();
                            // 核心3，直接序列化成 dynamic 类型，之前我一直纠结特定的实体
                            dynamic temp = Newtonsoft.Json.JsonConvert.DeserializeObject(cacheValue, resultType);
                            response = Task.FromResult(temp);
                        }
                        else
                        {
                            //Task 无返回方法 指定时间内不允许重新运行
                            response = Task.Yield();
                        }
                    }
                    else
                    {
                        // 核心4，要进行 ChangeType
                        response = System.Convert.ChangeType(_redis.Get<object>(cacheKey), type);
                    }

                    invocation.ReturnValue = response;
                    return;
                }

                invocation.Proceed();
                
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    object response;
                    
                    var type = invocation.Method.ReturnType;
                    if (typeof(Task).IsAssignableFrom(type))
                    {
                        var resultProperty = type.GetProperty("Result");
                        response = resultProperty.GetValue(invocation.ReturnValue);
                    }
                    else
                    {
                        response = invocation.ReturnValue;
                    }
                    if (response == null) response = string.Empty;
                    // 核心5：将获取到指定的response 和特性的缓存时间，进行set操作
                    _redis.Set(cacheKey, response, TimeSpan.FromMinutes(cacheAttr.AbsoluteExpiration));
                }
            }
            else
            {
                invocation.Proceed();
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
