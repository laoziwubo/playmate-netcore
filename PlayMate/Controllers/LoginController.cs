using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayMate.Auth;
using PlayMate.Common.Attribute;
using PlayMate.Common.Cache;
using PlayMate.Model.Common;
using PlayMate.Model.User;

namespace PlayMate.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IRedisCache _redis;
        public LoginController(IRedisCache redis)
        {
            _redis = redis;
        }

        [HttpPost]
        public object Post([FromBody]JwtModel model)
        {
            if (model.Name == "admin" && model.Pwd == "admin")
            {
                model.Role = "Admin";
                if (_redis.Get(model.CacheKey))
                {
                    return Ok(new JsonModel<string>
                    {
                        Code = 1,
                        Msg = "登陆成功！",
                        Payload = _redis.Get<string>(model.CacheKey)
                    });
                }
                var token = JwtHelper.IssueJwt(model);
                _redis.Set(model.CacheKey, token, TimeSpan.FromHours(2));
                return Ok(new JsonModel<string>
                {
                    Code = 1,
                    Msg = "登陆成功",
                    Payload = token
                });
            }
            else
            {
                return Ok(new JsonModel()
                {
                    Code = 0,
                    Msg = "用户名密码错误！"
                });
            }
        }
    }
}