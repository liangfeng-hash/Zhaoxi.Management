
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zhaoxi.Manage.Common.ModelDTO.User;

namespace Zhaoxi.Manage.Common.JwtService
{

    /// <summary>
    /// 对称可逆加密实现
    /// </summary>
    public class CustomHSJWTService : CustomJWTService
    {
        private readonly JWTTokenOptions _JWTTokenOptions;
        private readonly IMemoryCache _IMemoryCache;
        public CustomHSJWTService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions, IMemoryCache iMemoryCache)
        {
            _JWTTokenOptions = jwtTokenOptions.CurrentValue;
            _IMemoryCache = iMemoryCache;
        }

        /// <summary>
        /// 创建token
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="refreshToken">刷新token</param>
        /// <returns>AccessToken</returns>
        public override string CreateToken(SysUserInfo user, out string refreshToken)
        {
            //准备有效载荷
            List<Claim> claimsArray = base.UserToClaim(user);
            //准备加密key
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey));
            //Sha256 加密方式
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken JwtAccessToken = new JwtSecurityToken(
                  issuer: _JWTTokenOptions.Issuer,
                  audience: _JWTTokenOptions.Audience,
                  claims: claimsArray.ToArray(),
                  expires: DateTime.Now.AddMinutes(5),//5分钟有效期
                  signingCredentials: creds);

            string accesstoken = new JwtSecurityTokenHandler().WriteToken(JwtAccessToken);

            //生成refreshToken
            {
                string refreshtokenGuid = Guid.NewGuid().ToString();
                Claim[] claimslist = new Claim[]
                {
                    new Claim("refreshtokenGuid", refreshtokenGuid)
                };
                JwtSecurityToken JwtRefreshToken = new JwtSecurityToken(
                     issuer: _JWTTokenOptions.Issuer,
                     audience: _JWTTokenOptions.Audience,
                     claims: claimslist,
                     expires: DateTime.Now.AddMinutes(5),//5分钟有效期
                     signingCredentials: creds);
                refreshToken = new JwtSecurityTokenHandler().WriteToken(JwtRefreshToken);
                _IMemoryCache.Set<SysUserInfo>(refreshtokenGuid, user, DateTime.Now.AddMinutes(5));
            }
            return accesstoken;

        }


    }
}
