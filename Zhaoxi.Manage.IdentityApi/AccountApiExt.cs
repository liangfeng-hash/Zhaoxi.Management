using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar.Extensions;
using System.Runtime.CompilerServices;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.JwtService;
using Zhaoxi.Manage.Common.ModelDTO.User;

namespace Zhaoxi.Manage.IdentityApi
{
    public static class AccountApiExt
    {
        /// <summary>
        /// 登录颁发Token
        /// </summary>
        /// <param name="app"></param>
        public static void LoginApi(this WebApplication app)
        {
            //登录使用用户名和密码获取token
            app.MapPost("auth/Account", ([FromServices] IUserManagerService userManagerService, [FromServices] CustomJWTService _iJWTService, [FromServices] IMemoryCache iMemoryCache, User user) =>
            {
                SysUserInfo? userInfo = userManagerService.Login(user.Name, user.Password);
                if (userInfo == null)
                {
                    return new ApiResult()
                    {
                        Success = false,
                        Message = "用户名或密码错误",
                    };
                }
                string accesstoken = _iJWTService.CreateToken(userInfo, out string refreshToken);
                return new ApiDataResult<object>()
                {
                    Success = true,
                    Message = "登录成功",
                    Data = new
                    {
                        UserName = userInfo.Name,
                        Imageurl = userInfo.Imageurl
                    },
                    OValue = new
                    {
                        RefreshToken = refreshToken,
                        Accesstoken = accesstoken
                    }
                };
            }).WithGroupName("v1").WithDescription("登录-使用用户名密码获取token");


            ////刷新Token
            app.MapGet("auth/Account",
             [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            ([FromServices] CustomJWTService _iJWTService, [FromServices] IMemoryCache _IMemoryCache, HttpContext context) =>
            {
                string refreshtokenGuid = context?.User?.FindFirst("refreshtokenGuid")?.Value;
                if (string.IsNullOrWhiteSpace(refreshtokenGuid))
                {
                    return new ApiResult()
                    {
                        Success = false,
                        Message = "已过期需要重新登录",
                    }; 
                }
                SysUserInfo userInfo = _IMemoryCache.Get<SysUserInfo>(refreshtokenGuid);
                if (userInfo == null)
                {
                    return new ApiResult()
                    {
                        Success = false,
                        Message = "已过期需要重新登录",
                    };
                }
                string token = _iJWTService.CreateToken(userInfo, out string refreshToken);
                _IMemoryCache.Remove(refreshtokenGuid);
                return new ApiDataResult<string>()
                {
                    Success = true,
                    Message = "Token刷新成功",
                    Data = token,
                    OValue = refreshToken
                };
            }).WithGroupName("v1").WithDescription("刷新Token,使用RreshToken换新Token"); ;
        }

    }
}
