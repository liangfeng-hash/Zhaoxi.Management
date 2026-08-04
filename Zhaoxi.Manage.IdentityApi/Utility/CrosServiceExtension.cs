namespace Zhaoxi.Manage.IdentityApi.Utility
{
    /// <summary>
    /// 跨域相关
    /// </summary>
    public static class CrosServiceExtension
    {
        /// <summary>
        /// 配置跨域策略
        /// </summary>
        /// <param name="builder"></param>
        public static void CrosDomainsPolicy(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("AllCrosDomainsPolicy", builder =>
                { 
                    builder.AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowAnyOrigin(); 
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void UseCrosDomainsPolicy(this WebApplication app)
        {
            app.UseCors("AllCrosDomainsPolicy");
        }

    }
}
