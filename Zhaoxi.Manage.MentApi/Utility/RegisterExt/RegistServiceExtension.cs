using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.BusinessService;
using Zhaoxi.Manage.Common.JwtService;
using Zhaoxi.Manage.MentApi.Utility.Filters;
using Zhaoxi.Manage.Common.Notice;

namespace Zhaoxi.Manage.MentApi.Utility.RegisterExt
{
    /// <summary>
    /// 服务注册
    /// </summary>
    public static class RegistServiceExtension
    {
        /// <summary>
        /// 注册服务层
        /// </summary>
        /// <param name="builder"></param>
        public static void RegistService(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUserRoleMenuService, UserRoleMenuService>();
            builder.Services.AddTransient<IUserManagerService, UserManagerService>();
            builder.Services.AddTransient<IMenuManagerService, MenuManagerService>();
            builder.Services.AddTransient<IRoleManagerService, RoleManagerService>();
             
            builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
            //builder.Services.Configure<SmtpClientConfig>(builder.Configuration.GetSection("SmtpClientConfig"));
            
        }


        /// <summary>
        /// AddControllers 相关
        /// </summary>
        /// <param name="builder"></param>
        public static void RegistControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(option =>
            {
                option.Filters.Add<CustomAsyncExceptionFilterAttribute>(); //异常捕捉
                option.Filters.Add<CustomLog4ActionFilterAttribute>(); //异常捕捉 

            }).AddNewtonsoftJson(options =>
            {
                //配置返回JSON首字母问题以及格式 nuget引入：Microsoft.AspNetCore.Mvc.NewtonsoftJson
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();//设置JSON返回格式同model一致

                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();//设置JSON返回格式首字母小写


            }).AddJsonOptions(options =>
            {
                //配置解决中文乱码问题
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            }); ;




        }

    }
}
