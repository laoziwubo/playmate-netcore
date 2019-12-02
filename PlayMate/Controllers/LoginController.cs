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

        [Route("jwt")]
        [HttpGet]
        public async Task<object> GetJwt(string name, string pass)
        {
            var tokenModel = new JwtModel { Uid = 10086, Role = "Admin" };
            if (_redis.Get(tokenModel.Uid.ToString()))
            {
                return Ok(new JsonModel()
                {
                    Code = 1,
                    Msg = "",
                    Payload = _redis.GetValue(tokenModel.Uid.ToString())
                });
            }
            var token = await Task.Run(() => JwtHelper.IssueJwt(tokenModel));
            _redis.Set(tokenModel.Uid.ToString(), token, TimeSpan.FromSeconds(600));
            return Ok(new JsonModel()
            {
                Code = 1,
                Msg = "",
                Payload = token
            });
        }
    }
}