using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PlayMate.Aop;
using PlayMate.Common.DB;
using PlayMate.Common.Helper;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using PlayMate.Auth;
using PlayMate.AutoMapper;
using PlayMate.Common.Cache;

namespace PlayMate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            services.AddSingleton(new AppSettingsHelper(Env.ContentRootPath));

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            services.AddScoped<Common.Cache.IMemoryCache, Common.Cache.MemoryCache>();
            services.AddSingleton<Microsoft.Extensions.Caching.Memory.IMemoryCache>(
                factory => new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions())
            );

            services.AddSingleton<IRedisCache, RedisCache>();

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = "PlayMate API",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "PlayMate", Email = "laoziwubo@xxx.com", Url = "www.baidu.com" }
                });
                var xmlPath = Path.Combine(basePath, "PlayMate.xml");
                c.IncludeXmlComments(xmlPath, true);

                var security = new Dictionary<string, IEnumerable<string>> { { "PlayMate", new string[] { } }, };
                c.AddSecurityRequirement(security);
                c.AddSecurityDefinition("PlayMate", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
            });

            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            //var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            services.AddAuthentication("Bearer").AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = true,
                        ValidIssuer = audienceConfig["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = audienceConfig["Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true,
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // 如果过期，则把<是否过期>添加到，返回头信息中
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddScoped<SqlSugar.ISqlSugarClient>(o => new SqlSugar.SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = DataBaseConfig.ConnectionString,//必填, 数据库连接字符串
                DbType = (SqlSugar.DbType)DataBaseConfig.DbType,//必填, 数据库类型
                IsAutoCloseConnection = true,//默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = SqlSugar.InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
            }));

            services.AddMiniProfiler(options => options.RouteBasePath = "/profiler");

            services.AddCors(c =>
            {
                c.AddPolicy("all", policy =>
                {
                    policy
                        .AllowAnyOrigin()//允许任何源
                        .AllowAnyMethod()//允许任何方式
                        .AllowAnyHeader()//允许任何头
                        .AllowCredentials();//允许cookie
                });
                
                c.AddPolicy("limit", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:1818", "http://localhost:8080") //支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            


            //Autofac容器注入的内容
            //Autofac容器注入的内容
            //Autofac容器注入的内容
            var builder = new ContainerBuilder();

            builder.RegisterType<CacheAop>();
            builder.RegisterType<RedisAop>();
            builder.RegisterType<LogAop>();

            var servicesAssembly = Path.Combine(basePath, "PlayMate.Services.dll");
            builder.RegisterAssemblyTypes(Assembly.LoadFile(servicesAssembly)).AsImplementedInterfaces()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(RedisAop));

            var repositoryAssembly = Path.Combine(basePath, "PlayMate.Repository.dll");
            builder.RegisterAssemblyTypes(Assembly.LoadFrom(repositoryAssembly)).AsImplementedInterfaces();

            builder.Populate(services);
            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                c.RoutePrefix = "";
            });

            app.UseMiniProfiler();

            app.UseCors("limit");

            //app.UseJwt();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}