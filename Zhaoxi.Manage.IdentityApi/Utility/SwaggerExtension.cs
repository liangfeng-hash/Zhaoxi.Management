using Microsoft.OpenApi.Models;

namespace Zhaoxi.Manage.IdentityApi.Utility
{
    public static class SwaggerExtension
    {

        public static void SwaggerExt(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "朝夕教育 MinimalApi", Version = "v1" });
                options.SwaggerDoc("v2", new() { Title = "朝夕教育 MinimalApi", Version = "v2" });
                options.OperationFilter<SwaggerFileUploadFilter>();

                #region Swagger配置支持Token参数传递 
                //添加安全定义
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token,格式为 Bearer xxxxxxxx（注意中间必须有空格）",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"  //JwtBearerDefaults.AuthenticationScheme
                });

                //添加安全要求
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
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

                #endregion
            });
        }


        /// <summary>
        /// swagger生效
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerExt(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.EnableTryItOutByDefault();
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $" 朝夕教育 MinimalApi-v1版本  v1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", $" 朝夕教育 MinimalApi-v2版本  v2");
            });
        }
    }
}
