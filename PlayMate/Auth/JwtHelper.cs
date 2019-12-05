using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using PlayMate.Common.Helper;

namespace PlayMate.Auth
{
    public class JwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="jwtModel"></param>
        /// <returns></returns>
        public static string IssueJwt(JwtModel jwtModel)
        {
            string iss = AppSettingsHelper.Config("JWT", "Issuer");
            string aud = AppSettingsHelper.Config("JWT", "Audience");
            string secret = AppSettingsHelper.Config("JWT", "Secret");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jwtModel.CacheKey),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                new Claim (JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddHours(2)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Iss,iss),
                new Claim(JwtRegisteredClaimNames.Aud,aud)
            };
            claims.AddRange(jwtModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: iss,
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static JwtModel SerializeJwt(string jwtStr)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtStr);
            object role;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return new JwtModel
            {
                Role = role != null ? role as string : "",
            };
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class JwtModel
    {
        public string Role { get; set; }
        
        public string Name { get; set; }
        
        public string Pwd { get; set; }

        public string CacheKey => $"{Name.Trim()}::{Pwd.Trim()}";
    }
}
