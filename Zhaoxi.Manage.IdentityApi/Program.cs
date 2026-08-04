using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.BusinessInterface.Config;
using Zhaoxi.Manage.BusinessService;
using Zhaoxi.Manage.Common.JwtService;
using Zhaoxi.Manage.IdentityApi.Utility;
using Zhaoxi.Manage.IdentityApi.Utility.InitDatabaseExt;


namespace Zhaoxi.Manage.IdentityApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.SwaggerExt(); //Swagger配置

            builder.CrosDomainsPolicy(); //配置跨域

            builder.Services.AddTransient<IUserManagerService, UserManagerService>();
            //builder.Services.AddTransient<ICustomJWTService, CustomRSSJWTervice>();//非对称
            builder.Services.AddTransient<CustomJWTService, CustomHSJWTService>();//对称
            builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));


            builder.Services.AddAutoMapper(typeof(AutoMapperConfigs));   //Automapper映射

            // Add services to the container.
            builder.InitSqlSugar();

            //Nuget: Microsoft.Extensions.Caching.Memory
            builder.Services.AddMemoryCache();


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
                    ClockSkew = TimeSpan.FromSeconds(0)
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
                            Data = 0
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
                            Data = 1
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
                    }
                };
            });



            WebApplication app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwaggerExt();
            //}

            app.UseSwaggerExt();

            app.UseCrosDomainsPolicy();//使用跨域策略

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.LoginApi();//登录

            app.Run();
        }
    }
}