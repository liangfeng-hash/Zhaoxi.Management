using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.JwtService;
using static System.Net.Mime.MediaTypeNames;

namespace Zhaoxi.Manage.MentApi.Utility.AuthorizationExt
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthorizationRegister
    {
        /// <summary>
        /// 支持授权的扩展方法
        /// </summary>
        /// <param name="builder"></param>
        public static void AuthorizationExt(this WebApplicationBuilder builder)
        {
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>  //这里是配置的鉴权的逻辑
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = tokenOptions.Audience,//
                    ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
                    ClockSkew = TimeSpan.FromSeconds(0),//设置token过期后多久失效，默认过期后300秒内仍有效
                    ////AudienceValidator = (m, n, z) =>  //可以添加一些自定义的动作
                    //{
                    //    return true;
                    //},
                    //LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    //{
                    //    return true;
                    //}
                };

                options.Events = new JwtBearerEvents
                {
                    //此处为权限验证失败后触发的事件，涵盖的场景：没有token的，token错误的
                    OnChallenge = context =>
                    {
                        //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                        context.HandleResponse();
                        //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                        var payload = JsonConvert.SerializeObject(new ApiDataResult<int>()
                        {
                            Success = false,
                            Message = "对不起没有授权，没有Token",
                            Data = 0,
                            OValue = 401
                        }, new JsonSerializerSettings
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                        });
                        //自定义返回的数据类型
                        context.Response.ContentType = "application/json";
                        //自定义返回状态码，默认为401 我这里改成 200
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //输出Json数据结果
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    },
                    OnForbidden = context =>
                    {
                        //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                        var payload = JsonConvert.SerializeObject(new ApiDataResult<int>()
                        {
                            Success = false,
                            Message = "对不起，您不具备访问该功能的权限",
                            Data = 1,
                            OValue = 403
                        }, new JsonSerializerSettings
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                        });


                        //自定义返回的数据类型
                        context.Response.ContentType = "application/json";
                        //自定义返回状态码，默认为403 我这里改成 200
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        //context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        //输出Json数据结果
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    },
                    //OnTokenValidated = context =>
                    //{
                    //    Console.WriteLine("Token解析成功了");
                    //    return Task.FromResult(0);
                    //},
                    //OnAuthenticationFailed = context =>
                    //{
                    //    Console.WriteLine("Token解析成功了");
                    //    return Task.FromResult(0);
                    //},
                    //OnMessageReceived = context =>
                    //{
                    //    return Task.FromResult(0);
                    //}
                };
            });


            //增加权限判定策略
            //需要结合业务来完成验证
            //1. 添加策略
            //2. 需要自定义策略
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("btnPolicy", policyBuilder => policyBuilder
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .AddRequirements(new MenuAuthorizeRequirement()));
            });
            builder.Services.AddTransient<IAuthorizationHandler, MenuAuthorizeHandler>();
        }
    }
}
