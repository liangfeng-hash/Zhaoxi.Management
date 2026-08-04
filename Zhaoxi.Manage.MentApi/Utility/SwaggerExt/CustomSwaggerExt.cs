using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Zhaoxi.Manage.MentApi.Utility.SwaggerExt
{
    /// <summary>
    /// Swagger扩展
    /// </summary>
    public static class CustomSwaggerExt
    {
        /// <summary>
        /// 配置Swagger
        /// </summary>
        /// <param name="builder"></param>
        public static void AddSwaggerExt(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {

                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    option.SwaggerDoc(version, new OpenApiInfo()
                    {
                        Title = $"朝夕教育高级班项目实战Api文档",
                        Version = version,
                        Description = $"通用版本的CoreApi版本{version}"
                    });
                });

                // xml文档绝对路径 
                var file = Path.Combine(AppContext.BaseDirectory, "Zhaoxi.Manage.MentApi.xml");
                // true : 显示控制器层注释
                option.IncludeXmlComments(file, true);
                // 对action的名称进行排序，如果有多个，就可以看见效果了。
                option.OrderActionsBy(o => o.RelativePath);

                //添加安全定义
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token,格式为 Bearer xxxxxxxx（注意中间必须有空格）",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //添加安全要求
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {
                        new OpenApiSecurityScheme
                        {
                            Reference =new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id ="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
            });  
        }

        /// <summary>
        /// 中间件生效
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerExt(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (string version in typeof(ApiVersions).GetEnumNames())
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"高级班实战Api第【{version}】版本");
                }
            });
        }
    }
}
