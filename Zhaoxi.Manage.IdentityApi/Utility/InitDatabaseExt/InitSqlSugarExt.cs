using Microsoft.Extensions.Configuration;
using SqlSugar;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.IdentityApi.Utility.InitDatabaseExt
{

    /// <summary>
    /// 初始化SqlSugar
    /// </summary>
    public static class InitSqlSugarExt
    {
        /// <summary>
        /// 初始化SqlSugar
        /// </summary>
        /// <param name="builder"></param>
        public static void InitSqlSugar(this WebApplicationBuilder builder)
        {
            CustomConnectionConfig customConnectionConfig = new CustomConnectionConfig();
            builder.Configuration.Bind("ConnectionStrings", customConnectionConfig);

            builder.Services.AddScoped<ISqlSugarClient>(s =>
            {
                ConnectionConfig connection = new ConnectionConfig()
                {
                    ConnectionString = customConnectionConfig.ConnectionString,
                    DbType = DbType.SqlServer,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute,
                    SlaveConnectionConfigs = customConnectionConfig.SlaveConnectionConfigs.Select(c => new SlaveConnectionConfig() { ConnectionString = c.ConnectionString, HitRate = c.CustomHitRate }).ToList()
                };
                SqlSugarClient client = new SqlSugarClient(connection);
                client.Aop.OnLogExecuting = (s, p) =>
                {
                    Console.WriteLine($"OnLogExecuting:输出Sql语句:{s} || 参数为：{string.Join(",", p.Select(p => p.Value))}");
                };
                client.Aop.OnExecutingChangeSql = (s, p) =>
                {
                    Console.WriteLine($"OnLogExecuting:输出Sql语句:{s} || 参数为：{string.Join(",", p.Select(p => p.Value))}");
                    return new KeyValuePair<string, SugarParameter[]>(s, p);
                };
                client.Aop.OnLogExecuted = (s, p) =>
                {
                    Console.WriteLine($"OnLogExecuted:输出Sql语句:{s} || 参数为：{string.Join(",", p.Select(p => p.Value))}");
                };
                client.Aop.OnError = e =>
                {
                    Console.WriteLine($"OnError:Sql语句执行异常:{e.Message}");
                };

                //////查询过滤器
                //client.QueryFilter.AddTableFilter<Sys_Menu>(it => it.IsDeleted == false);
                //client.QueryFilter.AddTableFilter<Sys_Role>(it => it.IsDeleted == false);
                //client.QueryFilter.AddTableFilter<Sys_RoleMenuMap>(it => it.IsDeleted == false);
                //client.QueryFilter.AddTableFilter<Sys_User>(it => it.IsDeleted == false);
                //client.QueryFilter.AddTableFilter<Sys_UserRoleMap>(it => it.IsDeleted == false);


                client.QueryFilter.AddTableFilter<Sys_Menu>(it => it.Status !=(int)StatusEnum.Deleted);
                client.QueryFilter.AddTableFilter<Sys_Role>(it => it.Status != (int)StatusEnum.Deleted);
                client.QueryFilter.AddTableFilter<Sys_RoleMenuMap>(it => it.Status != (int)StatusEnum.Deleted);
                client.QueryFilter.AddTableFilter<Sys_User>(it => it.Status != (int)StatusEnum.Deleted);
                client.QueryFilter.AddTableFilter<Sys_UserRoleMap>(it => it.Status != (int)StatusEnum.Deleted);

                return client;
            });
        }
    }
}
