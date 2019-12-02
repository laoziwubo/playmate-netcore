using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlayMate.Auth;

namespace PlayMate.Middlewares
{
    public class JwtMidd
    {
        private readonly RequestDelegate _next;
        public JwtMidd(RequestDelegate next)
        {
            _next = next;
        }

        private void PreProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke preproceed");
            //...
        }
        private void PostProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke postproceed");
            //....
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            PreProceed(httpContext);

            //检测是否包含'Authorization'请求头
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                PostProceed(httpContext);
                return _next(httpContext);
            }

            //var headerToken = httpContext.Request.Headers["Authorization"].ToString();
            var headerToken = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                if (headerToken.Length >= 128)
                {
                    var tm = JwtHelper.SerializeJwt(headerToken);

                    //授权
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role, tm.Role)
                    };
                    var identity = new ClaimsIdentity(claims);
                    var principal = new ClaimsPrincipal(identity);
                    httpContext.User = principal;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} middleware wrong:{e.Message}");
            }
            PostProceed(httpContext);
            return _next(httpContext);
        }
    }
}
