using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Zhaoxi.Manage.Common.ModelDTO.User;

namespace Zhaoxi.Manage.Common.JwtService
{
    public class CustomRSSJWTervice : CustomJWTService
    { 
        private readonly JWTTokenOptions _JWTTokenOptions;
        private readonly IMemoryCache _IMemoryCache;
        public CustomRSSJWTervice(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions, IMemoryCache iMemoryCache)
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
            string keyDir = Directory.GetCurrentDirectory();
            if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            }
            List<Claim> claimsArray = base.UserToClaim(user); 
            //准备加密key
            RsaSecurityKey key = new RsaSecurityKey(keyParams);
            //Sha256 加密方式
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            JwtSecurityToken jwtAccessToken = new JwtSecurityToken(
                  issuer: _JWTTokenOptions.Issuer,
                  audience: _JWTTokenOptions.Audience,
                  claims: claimsArray.ToArray(),
                  expires: DateTime.Now.AddMinutes(5),//accessToken 5分钟有效期
                  signingCredentials: creds);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(jwtAccessToken);

            claimsArray.Add(new Claim("refreshtokenGuid", Guid.NewGuid().ToString()));
            JwtSecurityToken jwtRefreshToken = new JwtSecurityToken(
                 issuer: _JWTTokenOptions.Issuer,
                 audience: _JWTTokenOptions.Audience,
                 claims: claimsArray.ToArray(),
                 expires: DateTime.Now.AddMinutes(20),//RefeshToken 20分钟有效期
                 signingCredentials: creds);
            refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken);
            return returnToken;
        }
         
    }
}
