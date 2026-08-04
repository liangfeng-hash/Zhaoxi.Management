using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Reflection;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.IdentityApi.Utility.InitDatabaseExt
{
    /// <summary>
    /// 是否初始化数据库
    /// </summary>
    public static class InitDatabaseExt
    {
        /// <summary>
        /// 是否初始化数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="Exception"></exception>
        public static void InitDatabase(this WebApplicationBuilder builder)
        {
            List<Sys_Menu> MenuList = new List<Sys_Menu>();
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(ControllerBase)
                    .IsAssignableFrom(type));

            foreach (var controller in controlleractionlist)
            {
                if (controller.IsDefined(typeof(FunctionAttribute), true))
                {
                    FunctionAttribute? attribute = controller.GetCustomAttribute<FunctionAttribute>();
                    Guid guid = Guid.NewGuid();
                    Sys_Menu sysMenu = new Sys_Menu()
                    {
                        Id = guid,
                        ParentId = default,
                        MenuText = attribute?.GetDescription(),
                        MenuType = attribute is null ? (int)MenuTypeEnum.Menu : (int)attribute.GetMuType()
                    };

                    MenuList.Add(sysMenu);
                    var mehtodlist = controller.GetMethods()
                        .Where(m => m.IsDefined(typeof(FunctionAttribute), true));
                    foreach (var method in mehtodlist)
                    {
                        FunctionAttribute? childAttribute = method.GetCustomAttribute<FunctionAttribute>();
                        Sys_Menu childMenu = new Sys_Menu()
                        {
                            Id = Guid.NewGuid(),
                            ParentId = guid, 
                            MenuText = childAttribute?.GetDescription(),
                            MenuType = childAttribute is null ? (int)MenuTypeEnum.Button : (int)childAttribute.GetMuType()
                        };
                        MenuList.Add(childMenu);
                    }

                }
            }
            string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("请配置数据库链接字符串~");
            }

            ConnectionConfig connection = new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                ConnectionString = connectionString
            };
            using (SqlSugarClient client = new SqlSugarClient(connection))
            {
                client.DbMaintenance.CreateDatabase();
                if (client.DbMaintenance.IsAnyTable("Sys_Menu", false))
                {
                    client.DbMaintenance.DropTable("Sys_Menu");
                }

                Assembly assembly = Assembly.LoadFile(Path.Combine(AppContext.BaseDirectory, "Zhaoxi.Manage.Models.dll"));
                Type[] typeArray = assembly.GetTypes()
                    .Where(t => !t.Name.Equals("Sys_BaseModel") && t.Namespace.Equals("Zhaoxi.Manage.Models.Entity"))
                    .ToArray();

                client.CodeFirst.InitTables(typeArray);
                client.Insertable(MenuList).ExecuteCommand();
            }

        }
    }
}
