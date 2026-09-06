
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.BusinessInterface.Config;
using Zhaoxi.Manage.BusinessService;
using Zhaoxi.Manage.Common.JwtService;
using Zhaoxi.Manage.MentApi.Utility.AuthorizationExt;
using Zhaoxi.Manage.MentApi.Utility.Filters;
using Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt;
using Zhaoxi.Manage.MentApi.Utility.RegisterExt;
using Zhaoxi.Manage.MentApi.Utility.SwaggerExt;
using Zhaoxi.Manage.Models.Entity;
using Zhaoxi.Manage.MentApi.Utility.FileMiddleware;
using Zhaoxi.Manage.Common.Notice;
using System.Net.Mail;

namespace Zhaoxi.Manage.mentApi
{
    /// <summary>
    /// 项目启动入口
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
      
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


            //加载自定义格式配置文件
            builder.Configuration.AddJsonFile("baseMenu.json", true, true);

            if (builder.Configuration["IsInitDatabase"] == "1")
            {
                builder.InitDatabase();
            }

            // Add services to the container. 
            //nuget引入:  log4net
            //            Microsoft.Extensions.Logging.Log4Net.AspNetCore
            builder.Logging.AddLog4Net("CfgFile/log4net.Config");

            builder.CrosDomainsPolicy(); //配置跨域

            builder.AddSwaggerExt(); //配置Swagger

            builder.RegistControllers();  //AddControllers 相关

            builder.InitSqlSugar(); //注册SqlSugar

            builder.RegistService(); //注册抽象和具体之间的服务

            builder.Services.AddAutoMapper(typeof(AutoMapperConfigs));  //Automapper映射

            builder.AuthorizationExt(); //Jwt鉴权授权

            builder.Services.AddHealthChecks();

            WebApplication app = builder.Build();

            app.MapHealthChecks("/health").AllowAnonymous();

            app.MapHealthChecks("/health2").AllowAnonymous();

            //读取文件的中间件
            DownloadImagesExtensions.UseDownloadImages(app, Directory.GetCurrentDirectory());

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{ 
            app.UseSwaggerExt();
            //}

            app.UseCrosDomainsPolicy();//使用跨域策略

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
             
            app.Run();

             
        }
    }
}