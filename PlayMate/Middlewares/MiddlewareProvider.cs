using Microsoft.AspNetCore.Builder;

namespace PlayMate.Middlewares
{
    public static class MiddlewareProvider
    {
        public static IApplicationBuilder UseJwt(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtMidd>();
        }
    }
}
