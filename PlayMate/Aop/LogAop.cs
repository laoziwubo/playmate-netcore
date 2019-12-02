using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using StackExchange.Profiling;
using System;

namespace PlayMate.Aop
{
    public class LogAop : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                MiniProfiler.Current.Step($"执行Service方法：{invocation.Method.Name}() -> ");
                invocation.Proceed();
                MiniProfiler.Current.Step($"被拦截方法执行完毕，返回结果：{invocation.ReturnValue}");
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}
